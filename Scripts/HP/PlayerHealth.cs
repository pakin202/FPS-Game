using UnityEngine;
using System;

public class PlayerHealth  : MonoBehaviour
{
    [Header("Game Settings")]
    public GameSettings gameSettings;

    [Header("Health Settings")]
    public int maxHealth = 100;
    public int currentHealth;

    public event Action<int, int> OnHealthChanged; // เปลี่ยนเป็น int

    private void Start()
    {
        maxHealth = gameSettings.GetPlayerHealth();
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth, maxHealth); // แจ้ง UI ครั้งแรก
    }

    public void TakeDamage(int damage, Vector3 hitPoint)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(currentHealth, 0); // ป้องกันค่าติดลบ

        OnHealthChanged?.Invoke(currentHealth, maxHealth); // แจ้ง UI

        if (currentHealth <= 0)
        {
            Die(hitPoint);
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth); // แจ้ง UI
    }

    private void Die(Vector3 hitPoint)
    {
        Debug.Log(gameObject.name + " has died.");
        Destroy(gameObject);
    }
}
