using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyWanderAI : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3.5f;  // ความเร็วเดินปกติ
    public float chaseSpeed = 5f;   // ความเร็วไล่ล่า
    public float fleeSpeed = 6f;    // ความเร็วหนี
    public float acceleration = 8f; // อัตราเร่งของ NavMeshAgent

    [Header("Wander Settings")]
    public float wanderRadius = 10f;
    public float wanderInterval = 5f;
    public float stopDuration = 2f;

    [Header("Game Settings")]
    public GameSettings gameSettings;

    [Header("Chase & Flee Settings")]
    public Transform player;
    public float chaseRange = 15f;
    public float attackRange = 2f;
    public float damagePerSecond = 10f;
    public float fleeThreshold = 30f; // HP ต่ำกว่าค่านี้ให้หนี
    public float fleeDuration = 5f;   // ระยะเวลาที่หนีก่อนกลับมาไล่ล่า

    private NavMeshAgent agent;
    private bool isChasing;
    private bool isFleeing;
    private Coroutine attackCoroutine;
    private EnemyHP enemyHP;

    private void Start()
    {
        // ตั้งค่าความเสียหายและ attack range ตามโหมดความยาก
        if (gameSettings != null)
        {
            damagePerSecond = gameSettings.GetEnemyDamage();
            attackRange = gameSettings.GetEnemyAttackRange();
        }
        else
        {
            Debug.LogError("GameSettings not assigned!");
        }

        agent = GetComponent<NavMeshAgent>();
        enemyHP = GetComponent<EnemyHP>();

        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player")?.transform;
        }

        if (agent == null || !agent.isOnNavMesh)
        {
            Debug.LogError("NavMeshAgent ไม่พร้อมใช้งาน!");
            enabled = false;
            return;
        }

        agent.speed = walkSpeed;
        agent.acceleration = acceleration;
        StartCoroutine(WanderRoutine());
    }

    private void Update()
    {
        if (player == null || enemyHP == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        float currentHealth = enemyHP.GetHealth(); // ✅ ใช้ GetHealth() แทน

        if (currentHealth <= fleeThreshold && !isFleeing) 
        {
            StartCoroutine(StartFleeing());
        }
        else if (!isFleeing) 
        {
            if (distanceToPlayer <= attackRange)
            {
                if (attackCoroutine == null)
                {
                    attackCoroutine = StartCoroutine(AttackPlayer());
                }
            }
            else if (distanceToPlayer <= chaseRange)
            {
                StartChasing();
            }
            else
            {
                StopChasing();
            }
        }
    }

    private IEnumerator WanderRoutine()
    {
        while (true)
        {
            if (!isChasing && !isFleeing)
            {
                SetRandomWanderTarget();
                yield return new WaitForSeconds(wanderInterval);

                if (!isChasing && !isFleeing)
                {
                    agent.isStopped = true;
                    yield return new WaitForSeconds(stopDuration);
                    agent.isStopped = false;
                }
            }
            yield return null;
        }
    }

    private void SetRandomWanderTarget()
    {
        if (isChasing || isFleeing) return;

        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit navHit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(navHit.position);
        }
    }

    private void StartChasing()
    {
        if (isFleeing) return;

        isChasing = true;
        agent.speed = chaseSpeed;
        agent.SetDestination(player.position);
        Debug.Log("[Enemy] กำลังไล่ผู้เล่น!");
    }

    private IEnumerator StartFleeing()
    {
        isFleeing = true;
        isChasing = false;
        agent.speed = fleeSpeed;

        Vector3 fleeDirection = (transform.position - player.position).normalized * wanderRadius;
        Vector3 fleeTarget = transform.position + fleeDirection;

        if (NavMesh.SamplePosition(fleeTarget, out NavMeshHit navHit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(navHit.position);
            Debug.Log("[Enemy] กำลังหนีจากผู้เล่น!");
        }
        else
        {
            Debug.Log("[Enemy] หาทางหนีไม่ได้!");
        }

        yield return new WaitForSeconds(fleeDuration);

        isFleeing = false;
        agent.speed = walkSpeed; // กลับไปใช้ความเร็วเดินปกติ
        Debug.Log("[Enemy] หยุดหนี กลับมาไล่ล่า!");
    }

    private void StopChasing()
    {
        if (isChasing)
        {
            isChasing = false;
            agent.speed = walkSpeed;
            SetRandomWanderTarget();
            Debug.Log("[Enemy] หยุดไล่ล่า!");
        }
    }

    private IEnumerator AttackPlayer()
    {
        while (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            Debug.Log("Enemy is attacking the player!");

            if (player.TryGetComponent<PlayerHealth>(out var health))
            {
                Vector3 hitPoint = player.position;
                int damage = Mathf.RoundToInt(damagePerSecond * Time.deltaTime);
                health.TakeDamage(damage, hitPoint);
            }

            yield return null;
        }

        attackCoroutine = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
