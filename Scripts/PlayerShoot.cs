using System;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    public static Action shootInputDown;
    public static Action shootInputUp;
    public static Action reloadInput;
    public static Action zoomInputDown;
    public static Action zoomInputUp;

    [Header("References")]
    [SerializeField] private StarterAssetsInputs starterInputs; // อ้างอิงถึง StarterAssetsInputs

    [Header("Input Actions")]
    [SerializeField] private InputActionReference shootAction;
    [SerializeField] private InputActionReference reloadAction;
    [SerializeField] private InputActionReference zoomAction; // เพิ่ม Input สำหรับ Zoom

    private void OnEnable()
    {
        // ยิง
        shootAction.action.performed += OnShootPressed;
        shootAction.action.canceled += OnShootReleased;

        // รีโหลด
        reloadAction.action.performed += OnReloadPressed;

        // ซูม
        zoomAction.action.performed += OnZoomPressed;
        zoomAction.action.canceled += OnZoomReleased;

        // เปิดใช้งาน Input Actions
        shootAction.action.Enable();
        reloadAction.action.Enable();
        zoomAction.action.Enable();
    }

    private void OnDisable()
    {
        // ยิง
        shootAction.action.performed -= OnShootPressed;
        shootAction.action.canceled -= OnShootReleased;

        // รีโหลด
        reloadAction.action.performed -= OnReloadPressed;

        // ซูม
        zoomAction.action.performed -= OnZoomPressed;
        zoomAction.action.canceled -= OnZoomReleased;

        // ปิดการใช้งาน Input Actions
        shootAction.action.Disable();
        reloadAction.action.Disable();
        zoomAction.action.Disable();
    }

    private void OnShootPressed(InputAction.CallbackContext context)
    {
        shootInputDown?.Invoke();
    }

    private void OnShootReleased(InputAction.CallbackContext context)
    {
        shootInputUp?.Invoke();
    }

    private void OnReloadPressed(InputAction.CallbackContext context)
    {
        reloadInput?.Invoke();
    }

    private void OnZoomPressed(InputAction.CallbackContext context)
    {
        zoomInputDown?.Invoke();
    }

    private void OnZoomReleased(InputAction.CallbackContext context)
    {
        zoomInputUp?.Invoke();
        Debug.Log("");
    }
}
