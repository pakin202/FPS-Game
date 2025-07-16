using UnityEngine;
using StarterAssets;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 3.0f;
    public float sprintSpeed = 6.0f;
    public float crouchSpeed = 1.5f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;
    public float rotationSpeed = 10.0f;
    public int maxJumps = 2;

    [Header("Crouch Settings")]
    public float crouchHeight = 1.0f;
    public Vector3 crouchCameraOffset = new Vector3(0, -0.5f, 0);

    [Header("Fly Settings")]
    public float flySpeed = 5.0f;

    [Header("References")]
    [SerializeField] private StarterAssetsInputs starterInputs;
    public Transform cameraTransform;

    private CharacterController controller;
    public Vector3 playerVelocity;
    private bool groundedPlayer;
    private int jumpCount;

    // Crouch & Fly States
    private float originalHeight;
    private Vector3 originalCameraPosition;
    private bool isCrouching = false;
    private bool isFlying = false;

    // Camera Rotation
    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;
    private const float _threshold = 0.01f;
    private const float BottomClamp = -90f;
    private const float TopClamp = 30f;
    private bool LockCameraPosition = false;

    private void Start()
    {
        controller = GetComponent<CharacterController>();

        if (starterInputs == null)
        {
            starterInputs = GetComponent<StarterAssetsInputs>();
        }

        originalHeight = controller.height;
        originalCameraPosition = cameraTransform.localPosition;

        jumpCount = maxJumps;

        // เชื่อมต่อ Event
        PlayerController.jumpInput += HandleJump;
        PlayerController.sprintInput += HandleMovement;
        PlayerController.crouchInput += HandleCrouch;
        PlayerController.flyInput += HandleFly;
        PlayerController.moveInput += HandleMovement;
    }

    private void OnDisable()
    {
        PlayerController.jumpInput -= HandleJump;
        PlayerController.sprintInput -= HandleMovement;
        PlayerController.crouchInput -= HandleCrouch;
        PlayerController.flyInput -= HandleFly;
        PlayerController.moveInput -= HandleMovement;
    }

    private void Update()
    {
        GroundCheck();
        CameraRotation();
        HandleMovement();

        if (starterInputs.crouch)
        {
            HandleCrouch();
        }

        if (starterInputs.fly) // ตรวจสอบว่ากดปุ่ม Fly หรือไม่
        {
            HandleFly(); // เรียกฟังก์ชันสำหรับการบิน
        }

        // Gravity
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }
    private void GroundCheck()
    {
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer)
        {
            jumpCount = maxJumps;
            if (playerVelocity.y < 0)
            {
                playerVelocity.y = -2.0f;
            }
        }
    }

    private void HandleMovement()
    {
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -2.0f;
        }

        Vector2 inputDirection = starterInputs.move;

        Vector3 move = new Vector3(inputDirection.x, 0, inputDirection.y);
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0f;

        float speed = isCrouching
            ? crouchSpeed
            : (starterInputs.sprint ? sprintSpeed : walkSpeed);

        if (isCrouching && starterInputs.sprint)
        {
            speed = crouchSpeed;
        }

        controller.Move(move * speed * Time.deltaTime);
    }

    private void HandleJump()
    {
        if (jumpCount > 0 && groundedPlayer)
        {
            jumpCount--;
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    private void HandleCrouch()
    {
        if (!isCrouching)
        {
            controller.height = crouchHeight;
            cameraTransform.localPosition += crouchCameraOffset;
            isCrouching = true;
        }
        else
        {
            controller.height = originalHeight;
            cameraTransform.localPosition = originalCameraPosition;
            isCrouching = false;
        }
    }

    private void HandleFly()
    {
        if (starterInputs.fly)  // เมื่อกดปุ่ม Fly
        {
            playerVelocity.y = 10f; // เพิ่มความเร็วในการบินขึ้น
        }
        else  // เมื่อปล่อยปุ่ม Fly
        {
            playerVelocity.y = 0f;  // หยุดการบิน, ค่าจะถูกแทนที่ด้วยแรงโน้มถ่วง (gravity)
        }
    }


    private void CameraRotation()
    {
        if (starterInputs.look.sqrMagnitude >= _threshold && !LockCameraPosition)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse() ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += starterInputs.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += starterInputs.look.y * deltaTimeMultiplier;

            _cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch, BottomClamp, TopClamp);


            cameraTransform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
        }
    }

    private bool IsCurrentDeviceMouse()
    {
        return Mouse.current != null;
    }
}


