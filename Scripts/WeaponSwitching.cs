using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] weapons;

    [Header("Keys")]
    [SerializeField] private KeyCode[] keys;

    [Header("Settings")]
    [SerializeField] private float switchTime;

    [SerializeField] private GunUIController gunUIController; // อ้างอิงถึง GunUIController

    private int selectedWeapon = 0;
    private float timeSinceLastSwitch = 0f;

    private void Start()
    {
        SetWeapons();
        Select(selectedWeapon);
    }

    private void SetWeapons()
    {
        int weaponCount = transform.childCount;
        weapons = new Transform[weaponCount];

        for (int i = 0; i < weaponCount; i++)
        {
            weapons[i] = transform.GetChild(i);
        }

        // ตรวจสอบว่ามีคีย์เพียงพอหรือไม่
        if (keys == null || keys.Length < weaponCount)
        {
            Debug.LogWarning("WeaponSwitching: จำนวนคีย์ไม่พอกับจำนวนอาวุธ");
            keys = new KeyCode[weaponCount]; // ป้องกัน ArrayIndexOutOfBounds
        }
    }

    private void Update()
    {
        int previousSelectedWeapon = selectedWeapon;

        for (int i = 0; i < keys.Length; i++)
        {
            if (Input.GetKeyDown(keys[i]) && timeSinceLastSwitch >= switchTime)
            {
                selectedWeapon = i;
                break; // ออกจาก loop ทันทีที่พบ key ถูกกด
            }
        }

        if (previousSelectedWeapon != selectedWeapon)
        {
            Select(selectedWeapon);
        }

        timeSinceLastSwitch += Time.deltaTime;
    }

    private void Select(int weaponIndex)
    {
        if (weaponIndex < 0 || weaponIndex >= weapons.Length)
        {
            Debug.LogError("WeaponSwitching: weaponIndex อยู่เกินขอบเขตของอาร์เรย์");
            return;
        }

        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].gameObject.SetActive(i == weaponIndex);
        }

        timeSinceLastSwitch = 0f;

        OnWeaponSelected();
    }

    private void OnWeaponSelected()
    {
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        if (gunUIController == null)
        {
            Debug.LogError("WeaponSwitching: GunUIController ยังไม่ได้กำหนดค่า");
            return;
        }

        Gun currentGun = weapons[selectedWeapon].GetComponentInChildren<Gun>();

        if (currentGun != null)
        {
            Debug.Log("WeaponSwitching: กำลังอัปเดต UI ของอาวุธ " + currentGun.name);
            gunUIController.UpdateAmmoDisplay(currentGun.GetCurrentAmmo(), currentGun.GetMaxAmmo());
        }
        else
        {
            Debug.LogWarning("WeaponSwitching: ไม่มี Gun component บนอาวุธที่เลือก");
        }
    }
}
