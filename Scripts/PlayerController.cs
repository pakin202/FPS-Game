using System;
using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;
public class PlayerController : MonoBehaviour
{
    public static Action jumpInput;
    public static Action crouchInput;
    public static Action sprintInput;
    public static Action moveInput;
    public static Action flyInput;  // เพิ่มการกำหนด Action สำหรับ Fly

    [Header("References")]
    [SerializeField] private StarterAssetsInputs starterInputs;

    [Header("Input Actions")]
    [SerializeField] private InputActionReference jumpAction;
    [SerializeField] private InputActionReference crouchAction;
    [SerializeField] private InputActionReference sprintAction;
    [SerializeField] private InputActionReference moveAction;
    [SerializeField] private InputActionReference flyAction;  // เพิ่มการอ้างอิง FlyAction

    void Start()
    {

    }

    private void OnEnable()
    {
        // สมัคร Event
        jumpAction.action.performed += OnJumpPressed;
        sprintAction.action.performed += OnSprintPressed;
        crouchAction.action.performed += OnCrouchPressed;
        moveAction.action.performed += OnMovePressed;
        flyAction.action.performed += OnFlyPressed;  // สมัคร Fly Action

        // เปิดใช้งาน Input Actions
        jumpAction.action.Enable();
        sprintAction.action.Enable();
        crouchAction.action.Enable();
        moveAction.action.Enable();
        flyAction.action.Enable();  // เปิดใช้งาน FlyAction
    }

    private void OnDisable()
    {
        // ยกเลิกการสมัคร Event
        jumpAction.action.performed -= OnJumpPressed;
        sprintAction.action.performed -= OnSprintPressed;
        crouchAction.action.performed -= OnCrouchPressed;
        moveAction.action.performed -= OnMovePressed;
        flyAction.action.performed -= OnFlyPressed;  // ยกเลิกการสมัคร Fly Action

        // ปิดการใช้งาน Input Actions
        jumpAction.action.Disable();
        sprintAction.action.Disable();
        crouchAction.action.Disable();
        moveAction.action.Disable();
        flyAction.action.Disable();  // ปิดการใช้งาน FlyAction
    }

    private void OnJumpPressed(InputAction.CallbackContext context)
    {
        jumpInput?.Invoke();
    }

    private void OnSprintPressed(InputAction.CallbackContext context)
    {
        sprintInput?.Invoke();
    }

    private void OnCrouchPressed(InputAction.CallbackContext context)
    {
        crouchInput?.Invoke();
    }

    private void OnMovePressed(InputAction.CallbackContext context)
    {
        Vector2 moveDirection = context.ReadValue<Vector2>();
        starterInputs.MoveInput(moveDirection); // ส่งค่า Move ให้ StarterAssetsInputs
        Debug.Log("walk");
    }

    private void OnFlyPressed(InputAction.CallbackContext context)  // ฟังก์ชันใหม่สำหรับการกด Fly
    {
        bool isFlyPressed = context.performed; // ตรวจสอบการกดปุ่ม Fly
        starterInputs.FlyInput(isFlyPressed);  // ส่งค่า Fly ไปที่ StarterAssetsInputs
        Debug.Log(isFlyPressed ? "Fly activated" : "Fly deactivated");
    }
}