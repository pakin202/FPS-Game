using System;
using System.Collections;
using UnityEngine;
using StarterAssets;

public class Gun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GunData gunData;
    [SerializeField] private Transform cam;
    [SerializeField] private GunUIController gunUIController;
    [SerializeField] private StarterAssetsInputs starterInputs;
    [SerializeField] private AudioSource gunAudioSource;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip reloadSound;

    [Header("Player References")]
    [SerializeField] private GameObject player;
    private CharacterController playerController;
    private CharacterMovement characterMovement;

    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletPoint;
    [SerializeField] private float bulletSpeed = 100f;
    [SerializeField] private float bulletLifetime = 5f;

    private Camera mainCamera;
    private float timeSinceLastShot;
    private bool isShooting;
    private bool isZooming;
    private float currentAccuracy;

    private void Start()
    {
        mainCamera = Camera.main;

        if (player != null)
        {
            playerController = player.GetComponent<CharacterController>();
            player.TryGetComponent(out characterMovement);
        }
        else
        {
            Debug.LogError("Player reference is not assigned in Gun script!");
        }

        if (gunData == null)
        {
            Debug.LogError("GunData is not assigned in Gun script!");
        }

        PlayerShoot.shootInputDown += StartShooting;
        PlayerShoot.shootInputUp += StopShooting;
        PlayerShoot.reloadInput += StartReload;
        PlayerShoot.zoomInputDown += StartZoom;
        PlayerShoot.zoomInputUp += StopZoom;

        UpdateAmmoUI();
    }

    private void OnDisable()
    {
        gunData.reloading = false;
        StopShooting();
        StopZoom();
    }

    private void Update()
    {
        timeSinceLastShot += Time.deltaTime;

        HandleZoom();
        HandleAccuracy();

        if (isShooting && !gunData.reloading && CanShoot())
        {
            Shoot();
        }
    }

    private void HandleAccuracy()
    {
        if (starterInputs.crouch)
        {
            currentAccuracy = gunData.crouchAccuracy;
        }
        else if (playerController != null && !playerController.isGrounded)
        {
            currentAccuracy = gunData.jumpAccuracy;
        }
        else if (starterInputs.move != Vector2.zero)
        {
            if (characterMovement != null && characterMovement.playerVelocity.magnitude > 0f)
            {
                currentAccuracy = starterInputs.sprint &&
                                  characterMovement.playerVelocity.magnitude > characterMovement.sprintSpeed * 0.9f
                    ? gunData.runAccuracy
                    : gunData.walkAccuracy;
            }
            else
            {
                currentAccuracy = gunData.walkAccuracy;
            }
        }
        else
        {
            currentAccuracy = gunData.baseAccuracy;
        }
    }

    private Vector3 CalculateAccuracy(Vector3 shootDirection)
    {
        shootDirection.x += UnityEngine.Random.Range(-1f, 1f) * (1 - currentAccuracy);
        shootDirection.y += UnityEngine.Random.Range(-1f, 1f) * (1 - currentAccuracy);
        return shootDirection.normalized;
    }

    private bool CanShoot() => timeSinceLastShot > 1f / (gunData.fireRate / 60f);

    private void StartShooting() => isShooting = true;
    private void StopShooting() => isShooting = false;

    private void Shoot()
    {
        if (gunData.currentAmmo > 0 && CanShoot())
        {
            switch (gunData.fireMode)
            {
                case GunData.FireMode.Single:
                    ShootSingleBullet();
                    isShooting = false; // เพิ่มบรรทัดนี้เพื่อหยุดการยิงทันทีหลังยิงครั้งเดียว
                    break;
                case GunData.FireMode.Burst:
                    StartCoroutine(ShootBurst());
                    break;
                case GunData.FireMode.Auto:
                    ShootSingleBullet();
                    break;
            }
        }
    }

    private void ShootSingleBullet()
    {
        if (!CanShoot()) return; // เพิ่มการตรวจสอบอีกครั้งเพื่อความปลอดภัย

        FireBullet();
        gunData.currentAmmo--;
        timeSinceLastShot = 0;
        StartCoroutine(CameraShake(gunData.shakeMagnitude, gunData.shakeDuration, gunData.shakeSmoothness));
        UpdateAmmoUI();
        PlayShootSound();
    }

    private IEnumerator ShootBurst()
    {
        int shotsFired = 0;
        while (shotsFired < gunData.burstCount && gunData.currentAmmo > 0)
        {
            FireBullet();
            gunData.currentAmmo--;
            shotsFired++;
            timeSinceLastShot = 0;
            StartCoroutine(CameraShake(gunData.shakeMagnitude, gunData.shakeDuration, gunData.shakeSmoothness)); // เรียก Camera Shake
            UpdateAmmoUI();
            PlayShootSound();
            yield return new WaitForSeconds(gunData.burstDelay);
        }
    }

    private void FireBullet()
    {
        if (cam == null)
        {
            Debug.LogError("Camera reference is null in Gun script!");
            return;
        }

        if (bulletPrefab == null || bulletPoint == null)
        {
            Debug.LogError("BulletPrefab or BulletPoint is not assigned in Gun script!");
            return;
        }

        Vector3 shootDirection = cam.forward;
        shootDirection = CalculateAccuracy(shootDirection);

        if (Physics.Raycast(cam.position, shootDirection, out RaycastHit hitInfo, gunData.maxDistance))
        {
            if (hitInfo.transform.TryGetComponent<EnemyHP>(out var target))
            {
                target.TakeDamage(gunData.damage, hitInfo.point); // ใช้เวอร์ชันที่มี hitPoint
            }
            else
            {
                Debug.LogWarning("Hit object does not have IDamageable: " + hitInfo.transform.name);
            }
        }

        GameObject bullet = Instantiate(bulletPrefab, bulletPoint.position, Quaternion.LookRotation(shootDirection));
        if (bullet.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.AddForce(shootDirection * bulletSpeed, ForceMode.Impulse);
        }

        Destroy(bullet, bulletLifetime);
        DrawDebugLine();
    }

    private void DrawDebugLine() => Debug.DrawRay(cam.position, cam.forward * gunData.maxDistance, Color.red);

    private IEnumerator CameraShake(float magnitude, float duration, float smoothness)
    {
        Vector3 originalPosition = mainCamera.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // กำหนดทิศทางเฉพาะแกน x+ และ y+
            float x = UnityEngine.Random.Range(0f, 1f) * magnitude; // x+ เท่านั้น
            float y = UnityEngine.Random.Range(0f, 1f) * magnitude; // y+ เท่านั้น

            mainCamera.transform.localPosition = new Vector3(x, y, originalPosition.z);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // ค่อยๆ คืนกล้องกลับไปที่ตำแหน่งเดิม
        while (Vector3.Distance(mainCamera.transform.localPosition, originalPosition) > 0.01f)
        {
            mainCamera.transform.localPosition = Vector3.Lerp(
                mainCamera.transform.localPosition,
                originalPosition,
                smoothness * Time.deltaTime
            );
            yield return null;
        }

        mainCamera.transform.localPosition = originalPosition; // Reset กล้องกลับไปที่ตำแหน่งเดิม
    }

    private void StartZoom() => isZooming = true;
    private void StopZoom() => isZooming = false;

    private void HandleZoom()
    {
        if (mainCamera != null)
        {
            mainCamera.fieldOfView = Mathf.Lerp(
                mainCamera.fieldOfView,
                isZooming && gunData.canZoom ? gunData.zoomFOV : 60f,
                gunData.zoomSpeed * Time.deltaTime
            );
        }
    }

    private void PlayShootSound() => gunAudioSource?.PlayOneShot(shootSound);
    private void PlayReloadSound() => gunAudioSource?.PlayOneShot(reloadSound);

    private void UpdateAmmoUI() => gunUIController?.UpdateAmmoDisplay(gunData.currentAmmo, gunData.magSize);

    public void StartReload()
    {
        if (!gunData.reloading && gameObject.activeSelf)
            StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        gunData.reloading = true;
        yield return new WaitForSeconds(gunData.reloadTime);
        gunData.currentAmmo = gunData.magSize;
        gunData.reloading = false;
        PlayReloadSound();
        UpdateAmmoUI();
    }

    public int GetCurrentAmmo()
    {
        if (gunData == null)
        {
            Debug.LogError("Gun: gunData ยังไม่ได้ถูกกำหนดค่า");
            return 0;
        }
        return gunData.currentAmmo;
    }

    public int GetMaxAmmo()
    {
        if (gunData == null)
        {
            Debug.LogError("Gun: gunData ยังไม่ได้ถูกกำหนดค่า");
            return 0;
        }
        return gunData.magSize;
    }
}