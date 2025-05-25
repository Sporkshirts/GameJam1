using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float verticalMoveSpeed = 1f;
    [SerializeField] private float horizontalRotateSpeed = 600f;
    [SerializeField] private float verticalRotateSpeed = 300f;
    [SerializeField] private float acceleration = 30f;
    [SerializeField] private float deceleration = 40f;
    [SerializeField] private float maxPitchAngle = 30f;
    [SerializeField] private float maxPitchAngleNoInput = 75f;

    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject settingsMenu;
    [SerializeField] private CinemachineInputAxisController cameraControls;
    [SerializeField] private Animator animator;


    private Rigidbody rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction moveUpAction;
    private InputAction moveDownAction;
    private InputAction pauseAction;

    private float verticalInput;


    private Vector3 moveDirection;
    private Vector3 velocitySmoothDamp;
    private Transform cameraTransform;

    private Vector3 currentVelocity;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();

        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
        moveUpAction = playerInput.actions["MoveUp"];
        moveDownAction = playerInput.actions["MoveDown"];
        pauseAction = playerInput.actions["Pause"];
        cameraTransform = Camera.main.transform;
    }

    private void FixedUpdate()
    {
        MovePlayerRelativeToCamera();
        RotatePlayerHorizontally();
        RotatePlayerVertically();
    }

    private void OnEnable()
    {
        pauseAction.performed += PauseGame;
    }
    private void OnDisable()
    {
        pauseAction.performed -= PauseGame;
    }

    private void PauseGame(InputAction.CallbackContext context)
    {
        if (Time.timeScale > 0)
        {
            GameManager.Instance.PauseMenu();
        }
        else
        {
            GameManager.Instance.Resume();
        }
        
    }

    private void MovePlayerRelativeToCamera()
    {
        Vector2 move = moveAction.ReadValue<Vector2>();

        moveDirection = cameraTransform.forward * move.y + cameraTransform.right * move.x;
        moveDirection = new Vector3(moveDirection.x, 0, moveDirection.z).normalized;

        Vector3 targetVelocity = moveDirection * moveSpeed * move.magnitude;

        verticalInput = 0f;

        if (moveUpAction.IsPressed())
        {
            verticalInput += 1f;
        }
        if (moveDownAction.IsPressed())
        {
            verticalInput -= 1f;
        }

        targetVelocity.y = verticalInput * verticalMoveSpeed;
        moveDirection.y = verticalInput;

        float smoothTime;

        if (move.magnitude > 0.1f || verticalInput != 0.0f)
        {
            animator.SetBool("Walk", true);
            smoothTime = 1.0f / acceleration;
        }
        else
        {
            animator.SetBool("Walk", false);
            smoothTime = 1.0f / deceleration;
        }

        currentVelocity = Vector3.SmoothDamp(currentVelocity, targetVelocity, ref velocitySmoothDamp, smoothTime);

        Vector3 displacement = currentVelocity.magnitude * transform.forward * Time.fixedDeltaTime;

        rb.MovePosition(rb.position + displacement);
    }

    private void RotatePlayerHorizontally()
    {

        Vector3 flatDirection = new Vector3(moveDirection.x, 0f, moveDirection.z);

        if (flatDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(flatDirection);
            Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, horizontalRotateSpeed * Time.fixedDeltaTime);

            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, newRotation.eulerAngles.y, transform.eulerAngles.z);
        }
    }

    private void RotatePlayerVertically()
    {
        Vector2 move = moveAction.ReadValue<Vector2>();

        moveDirection.Normalize();

        float pitchTarget = -moveDirection.y * maxPitchAngle;
        float pitchTargetNoInput = -moveDirection.y * maxPitchAngleNoInput;
        float currentPitch = transform.eulerAngles.x;

        if (Mathf.Abs(verticalInput) < 0.01f && move.magnitude < 0.01f)
        {
            return;
        }


        if (currentPitch > 180f)
        {
            currentPitch -= 360f;
        }

        float newPitch;

        if (move.magnitude < 0.1f)
        {
            newPitch = Mathf.MoveTowards(currentPitch, pitchTargetNoInput, verticalRotateSpeed * Time.fixedDeltaTime);
        }
        else
        {
            float slowVerticalRotateSpeed = verticalRotateSpeed / 2;
            newPitch = Mathf.MoveTowards(currentPitch, pitchTarget, slowVerticalRotateSpeed * Time.fixedDeltaTime);
        }

        transform.rotation = Quaternion.Euler(newPitch, transform.eulerAngles.y, transform.eulerAngles.z);

    }
}