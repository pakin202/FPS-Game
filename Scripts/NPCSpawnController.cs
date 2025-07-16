using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCSpawnController : MonoBehaviour
{
    public GameObject npcPrefab;  // Prefab ของ NPC
    public Transform player;  // อ้างอิงตำแหน่งของผู้เล่น
    public float spawnRadius = 10f;  // รัศมีที่ NPC สามารถเกิดได้
    public int maxNPCs = 5;  // จำนวนสูงสุดของ NPC ที่จะเกิด
    public float spawnInterval = 5f;  // เวลาที่ใช้ในการเกิด NPC ตัวใหม่
    public float navMeshCheckDistance = 2f; // ระยะที่ใช้ตรวจสอบ NavMesh

    private List<GameObject> spawnedNPCs = new List<GameObject>();

    void Start()
    {
        StartCoroutine(SpawnNPCs());
    }

    IEnumerator SpawnNPCs()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            // ลบ NPC ที่ถูกทำลายออกจากลิสต์
            spawnedNPCs.RemoveAll(npc => npc == null);

            if (spawnedNPCs.Count < maxNPCs)
            {
                SpawnNPC();
            }
        }
    }

    void SpawnNPC()
    {
        Vector3 randomDirection = Random.insideUnitSphere * spawnRadius;
        randomDirection.y = 0; // ป้องกันไม่ให้เกิดลอยกลางอากาศ
        Vector3 spawnPosition = player.position + randomDirection;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(spawnPosition, out hit, spawnRadius, NavMesh.AllAreas))
        {
            GameObject newNPC = Instantiate(npcPrefab, hit.position, Quaternion.identity);
            spawnedNPCs.Add(newNPC);
        }
        else
        {
            Debug.LogWarning("ตำแหน่งเกิด NPC ไม่อยู่บน NavMesh, ลองใหม่...");
        }
    }
}
