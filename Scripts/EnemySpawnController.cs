using UnityEngine;

public class EnemySpawnController : MonoBehaviour
{
    public GameObject enemyPrefab; // Prefab ของ Enemy
    public Transform player; // อ้างอิงตำแหน่งของ Player
    public float spawnRadius = 10f; // รัศมีที่ Enemy จะเกิดรอบ Player
    public int spawnAmount = 3; // จำนวน Enemy ที่จะเกิดในแต่ละครั้ง
    public float spawnInterval = 5f; // ระยะเวลาระหว่างการเกิด
    public int maxEnemies = 20; // จำนวนศัตรูสูงสุดที่สามารถอยู่ในฉากได้

    private int currentEnemyCount = 0; // จำนวนศัตรูที่มีอยู่ในฉากปัจจุบัน

    private void Start()
    {
        InvokeRepeating("SpawnEnemiesAroundPlayer", 0f, spawnInterval); // เรียก SpawnEnemies รอบ Player ทุก spawnInterval วินาที
    }

    private void SpawnEnemiesAroundPlayer()
    {
        if (currentEnemyCount >= maxEnemies)
        {
            Debug.Log("Enemy limit reached. No more enemies will spawn.");
            return; // หากจำนวนศัตรูถึงขีดจำกัดแล้ว ไม่ทำการเกิดใหม่
        }

        for (int i = 0; i < spawnAmount; i++)
        {
            if (currentEnemyCount >= maxEnemies)
                break; // หยุดการเกิดหากถึงจำนวนสูงสุด

            // สุ่มตำแหน่งการเกิดในรัศมี
            Vector3 randomPosition = Random.insideUnitSphere * spawnRadius;
            randomPosition.y = 0f; // เพื่อให้อยู่ในระนาบเดียวกันกับ Player
            Vector3 spawnPosition = player.position + randomPosition;

            // สร้าง Enemy
            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            currentEnemyCount++; // เพิ่มจำนวนศัตรูที่อยู่ในฉาก

            // ตรวจจับการทำลายศัตรู
            // enemy.GetComponent<Enemy>().OnEnemyDestroyed += HandleEnemyDestroyed;
        }
    }

    private void HandleEnemyDestroyed()
    {
        currentEnemyCount--; // ลดจำนวนศัตรูเมื่อถูกทำลาย
        Debug.Log("Enemy destroyed. Current enemy count: " + currentEnemyCount);
    }

    private void OnDrawGizmosSelected()
    {
        // แสดงรัศมีใน Scene View เพื่อดูว่าศัตรูจะเกิดในบริเวณใด
        if (player != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(player.position, spawnRadius);
        }
    }
}
