using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCWander : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3.5f;   // ความเร็วเดินปกติ
    public float fleeSpeed = 6f;     // ความเร็วเมื่อหนี
    public float acceleration = 8f;  // อัตราเร่งของ NavMeshAgent

    [Header("Wander Settings")]
    public float wanderRadius = 10f;
    public float wanderTime = 3f;

    [Header("Flee Settings")]
    public float fleeDistance = 5f;
    public Transform player;

    private NavMeshAgent agent;
    private float timer;
    private bool isFleeing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = walkSpeed; // ตั้งค่าความเร็วเริ่มต้น
        agent.acceleration = acceleration;

        timer = wanderTime;

        if (!agent.isOnNavMesh)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
            {
                transform.position = hit.position;
                agent.Warp(hit.position);
            }
            else
            {
                Debug.LogError("NPC ไม่อยู่บน NavMesh! กำลังลบตัวเอง...");
                Destroy(gameObject);
            }
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }
    }

    void Update()
    {
        if (player == null || !agent.isOnNavMesh) return;

        timer += Time.deltaTime;
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < fleeDistance)
        {
            if (!isFleeing)
            {
                isFleeing = true;
                agent.speed = fleeSpeed;  // ✅ เพิ่มความเร็วเมื่อหนี
                FleeFromPlayer();
            }
        }
        else
        {
            if (isFleeing)
            {
                isFleeing = false;
                agent.speed = walkSpeed; // ✅ กลับมาใช้ความเร็วปกติ
            }

            if (timer >= wanderTime)
            {
                Wander();
                timer = 0;
            }
        }
    }

    void Wander()
    {
        if (!agent.isOnNavMesh) return;

        Vector3 randomDirection = Random.insideUnitSphere * wanderRadius;
        randomDirection += transform.position;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomDirection, out hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    void FleeFromPlayer()
    {
        if (!agent.isOnNavMesh) return;

        Vector3 fleeDirection = (transform.position - player.position).normalized * wanderRadius;
        float randomAngle = Random.Range(-45f, 45f);
        fleeDirection = Quaternion.Euler(0, randomAngle, 0) * fleeDirection;

        Vector3 fleePosition = transform.position + fleeDirection;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleePosition, out hit, wanderRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, wanderRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, fleeDistance);
    }
}
