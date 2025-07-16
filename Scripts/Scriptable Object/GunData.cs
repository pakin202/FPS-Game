using UnityEngine;

[CreateAssetMenu(fileName = "New Gun Data", menuName = "Gun Data")]
public class GunData : ScriptableObject
{
    public enum FireMode { Single, Burst, Auto }

    [Header("Ammo Settings")]
    [Tooltip("ขนาดแม็กกาซีน")] 
    public int magSize;
    
    [Tooltip("กระสุนปัจจุบันในแม็ก")]
    public int currentAmmo;
    
    [Tooltip("เวลารีโลด")] 
    public float reloadTime;
    
    [Tooltip("กำลังรีโลดอยู่หรือไม่")] 
    public bool reloading;

    [Header("Fire Settings")]
    [Tooltip("โหมดการยิง")] 
    public FireMode fireMode;
    
    [Tooltip("อัตราการยิง (นัดต่อนาที)")] 
    public float fireRate;
    
    [Tooltip("จำนวนนัดต่อ Burst")] 
    public int burstCount;
    
    [Tooltip("ความล่าช้าระหว่าง Burst")] 
    public float burstDelay;
    
    [Tooltip("ความเสียหายต่อนัด")] 
    public float damage;
    
    [Tooltip("ระยะยิงสูงสุด")] 
    public float maxDistance;

    [Header("Zoom Settings")]
    [Tooltip("สามารถซูมได้หรือไม่")] 
    public bool canZoom;
    
    [Tooltip("มุมมองเมื่อซูม")] 
    public float zoomFOV;
    
    [Tooltip("ความเร็วในการซูม")] 
    public float zoomSpeed;

    [Header("Accuracy Settings")]
    [Tooltip("ความแม่นยำพื้นฐาน")] 
    public float baseAccuracy = 0.98f;
    
    [Tooltip("ความแม่นยำเมื่อย่อตัว")] 
    public float crouchAccuracy = 1.0f;
    
    [Tooltip("ความแม่นยำเมื่อเดิน")] 
    public float walkAccuracy = 0.85f;
    
    [Tooltip("ความแม่นยำเมื่อวิ่ง")] 
    public float runAccuracy = 0.6f;
    
    [Tooltip("ความแม่นยำเมื่อกระโดด")] 
    public float jumpAccuracy = 0.4f;

    [Header("Camera Shake Settings")]
    [Tooltip("ความแรงของการสั่นของกล้อง")] 
    public float shakeMagnitude = 0.1f;
    
    [Tooltip("ระยะเวลาการสั่นของกล้อง")] 
    public float shakeDuration = 0.1f;
    
    [Tooltip("ความเร็วในการคืนสู่ตำแหน่งปกติ")] 
    public float shakeSmoothness = 5.0f;
}