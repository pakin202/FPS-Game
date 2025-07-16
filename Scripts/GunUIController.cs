using UnityEngine;
using TMPro;  // สำหรับ TextMeshPro

public class GunUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject ammoPanel;   // อ้างอิงถึง Panel
    [SerializeField] private TMP_Text ammoText;      // สำหรับ Text UI

    private void Start()
    {
        UpdateAmmoDisplay(0, 0);     // ค่าเริ่มต้น
        ShowAmmoPanel(true);         // แสดง Panel ตอนเริ่มเกม
    }

    // เมทอดสำหรับอัปเดตจำนวนกระสุน
    public void UpdateAmmoDisplay(int currentAmmo, int maxAmmo)
    {
        ammoText.text = $"{currentAmmo}/{maxAmmo}";
    }

    // เมทอดสำหรับเปิด/ปิด Panel
    public void ShowAmmoPanel(bool show)
    {
        ammoPanel.SetActive(show);
    }
}
