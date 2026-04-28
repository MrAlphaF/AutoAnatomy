using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Walking speed in meters per second")]
    public float walkSpeed = 4f;

    [Tooltip("Flying speed when in fly mode (faster than walking)")]
    public float flySpeed = 8f;

    [Tooltip("How strongly gravity pulls the player down")]
    public float gravity = -20f;

    [Tooltip("Initial upward velocity when jumping")]
    public float jumpForce = 7f;

    [Header("Look Settings")]
    [Tooltip("Mouse sensitivity for looking around")]
    public float mouseSensitivity = 2f;

    [Tooltip("Maximum angle the camera can look up/down (degrees)")]
    public float maxLookAngle = 85f;

    [Header("Modes")]
    [Tooltip("Fly mode disables gravity and allows free vertical movement")]
    public bool flyMode = false;

    // Component references
    private CharacterController controller;
    private Camera playerCamera;

    // State
    private float verticalVelocity = 0f;
    private float cameraPitch = 0f;  // up/down rotation tracking

    void Start()
    {
        // Get references to attached/child components
        controller = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();

        // Lock and hide the mouse cursor for FPS feel
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // === Toggle fly mode with F key ===
        if (Input.GetKeyDown(KeyCode.F))
        {
            flyMode = !flyMode;
            verticalVelocity = 0f;  // reset gravity when toggling
        }

        // === Toggle cursor lock with Escape (useful for editor testing) ===
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        HandleMouseLook();
        HandleMovement();
    }

    private void HandleMouseLook()
    {
        // Read mouse input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        // Rotate the BODY left/right (around Y axis) — TRANSFORMATION
        transform.Rotate(0f, mouseX, 0f);

        // Rotate the CAMERA up/down (around X axis), clamped so neck doesn't break
        cameraPitch -= mouseY;
        cameraPitch = Mathf.Clamp(cameraPitch, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localEulerAngles = new Vector3(cameraPitch, 0f, 0f);
    }

    private void HandleMovement()
    {
        // Read WASD input
        float horizontalInput = Input.GetAxis("Horizontal"); // A/D
        float verticalInput = Input.GetAxis("Vertical");     // W/S

        // Build movement vector relative to player's facing direction
        Vector3 moveDirection = transform.right * horizontalInput + transform.forward * verticalInput;

        if (flyMode)
        {
            // Fly mode: vertical movement via Space (up) and Left Ctrl (down)
            float verticalFly = 0f;
            if (Input.GetKey(KeyCode.Space)) verticalFly = 1f;
            if (Input.GetKey(KeyCode.LeftControl)) verticalFly = -1f;

            moveDirection.y = verticalFly;
            controller.Move(moveDirection * flySpeed * Time.deltaTime);
        }
        else
        {
            // Walking mode: apply gravity
            if (controller.isGrounded)
            {
                verticalVelocity = -1f;  // small downward force to keep grounded

                // Jump
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    verticalVelocity = jumpForce;
                }
            }
            else
            {
                // Falling: accumulate gravity
                verticalVelocity += gravity * Time.deltaTime;
            }

            moveDirection.y = verticalVelocity;
            // PHYSICS — CharacterController.Move handles collision against the world
            controller.Move(moveDirection * walkSpeed * Time.deltaTime);
        }
    }
}