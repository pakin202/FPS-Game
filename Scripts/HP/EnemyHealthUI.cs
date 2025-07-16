using UnityEngine;
using TMPro;

public class EnemyHealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private EnemyHP enemyHealth;

    private void Start()
    {
        if (enemyHealth != null)
        {
            // ผูกฟังก์ชันกับอีเวนต์ของ EnemyHP
            enemyHealth.OnHealthChanged += UpdateHealthUI;
            UpdateHealthUI(enemyHealth.GetHealth(), enemyHealth.GetMaxHealth()); // อัปเดตครั้งแรก
        }
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
        {
            enemyHealth.OnHealthChanged -= UpdateHealthUI;
        }
    }

    private void UpdateHealthUI(float currentHealth, float maxHealth)
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealth} / {maxHealth}";
        }
    }
}
