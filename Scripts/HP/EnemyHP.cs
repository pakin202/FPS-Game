using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour, IDamageable
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    // อีเวนต์สำหรับแจ้งเตือน UI เมื่อค่า HP เปลี่ยน
    public event Action<float, float> OnHealthChanged;

    private void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth); // อัปเดต UI ครั้งแรก
    }

    public void TakeDamage(float damage, Vector3 hitPoint)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"Enemy took {damage} damage at {hitPoint}. Remaining health: {currentHealth}");

        // เรียกอีเวนต์ให้ UI อัปเดต
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Enemy died.");
        Destroy(gameObject);
    }

    // ฟังก์ชันสำหรับให้ UI ดึงค่าพลังชีวิตไปแสดง
    public float GetHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
}
