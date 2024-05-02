using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementManager : MonoBehaviour
{
    private InputController inputActions;
    private CharacterController character;
    public Animator animator;

    [Header("Movement Setting")]
    public float currentSpeed = 3.0f;
    public float turnSmoothTime = 0.1f;
    public float walkSpeed = 3.0f;
    public float runSpeed = 5.0f;

    public Transform mainCameraTransform;

    [Space]
    [Header("Jump Setting")]
    public float gravity = 9.81f;
    [SerializeField] private float raycastDistance = 0.1f;
    [SerializeField] private float raycastOffset = 0.05f;
    [SerializeField] private float maxSlopeAngle = 45f;
    public LayerMask groundMask;
    public Transform groundCheck;

    public bool isGrounded;
    public bool isOnSlop;

    public float jumpHeight;
    public float jumpButtonGracePeriod;

    private float ySpeed;
    private Vector3 direction;
    private float speed = 3f;
    private float originalStepOffset;
    private float? lastGroundTime;
    private float? jumpButtonPressedTime;

    [Space]
    [Header("Boolians")]
    public bool isWalk;
    public bool isRun;
    public bool isJump;
    public bool isGround;

    private float turnSmoothVelocity;
    private float magnitude;

    [SerializeField] private Transform footBone;
    public float raycastDistanceBone;

    void Awake()
    {
        inputActions = new InputController();
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        originalStepOffset = character.stepOffset;
    }


    void Update()
    {
        HandlingCharacterMovement();
        HandlingSprint();
        HandlingJump();

        // Ground Check
        CheckGround();
        CheckSlope();

        //Cursor Unlock
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           Cursor.lockState = CursorLockMode.None;
        }

        CheckBone();

    }

    private void CheckGround()
    {
        isGrounded = false;

        // Cast rays around the character's feet to check for ground
        Vector3 raycastOrigin = transform.position;
        RaycastHit hit;

        if (Physics.Raycast(raycastOrigin + Vector3.up * raycastOffset, Vector3.down, out hit, raycastDistance, groundMask))
        {
            isGrounded = true;
        }
        else if (Physics.Raycast(raycastOrigin - Vector3.up * raycastOffset, Vector3.down, out hit, raycastDistance, groundMask))
        {
            isGrounded = true;
        }
    }

    private void CheckSlope()
    {
        isOnSlop = false;

        // Cast a ray to check for slope
        RaycastHit hit;
        Vector3 downDirection = transform.TransformDirection(Vector3.down);

        if (Physics.Raycast(transform.position, downDirection, out hit, raycastDistance, groundMask))
        {
            // Calculate slope angle
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            if (slopeAngle > maxSlopeAngle)
            {
                isOnSlop = true;
            }
        }
    }

    // Public method to check if the character is grounded
    public bool IsGrounded()
    {
        return isGrounded;
    }

    // Public method to check if the character is on a slope
    public bool IsOnSlope()
    {
        return isOnSlop;
    }

    private void HandlingCharacterMovement()
    {
        Vector2 inputMovement = inputActions.Player.Movement.ReadValue<Vector2>().normalized;
        float horizontal = inputMovement.x;
        float verticle = inputMovement.y;
        direction = new Vector3(horizontal, 0f, verticle).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // Mouse Look Setting
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + mainCameraTransform.eulerAngles.y;
            float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            character.Move(moveDir.normalized * currentSpeed * Time.deltaTime);

            // Movement Animation
            animator.SetFloat("speed", direction.magnitude);

        }
        else animator.SetFloat("speed", 0f);

        // Apply Gravity
        /*if (!isGrounded) character.Move(Vector3.down * gravity * Time.deltaTime); */

        ySpeed += Physics.gravity.y * Time.deltaTime;
    }

    private void HandlingSprint()
    {
        // get movement input
        Vector2 inputVector = inputActions.Player.Movement.ReadValue<Vector2>();

        // Set sprint bool
        if (inputActions.Player.Sprint.triggered) isRun = !isRun;
        if (!isRun) isWalk = true;
        else isWalk = false;

        if (inputVector == Vector2.zero) isRun = false;

        // Handle the speed
        if (isRun) currentSpeed = runSpeed;
        else if (isWalk) currentSpeed = walkSpeed;

        if (isRun) animator.SetBool("isRun", true);
        else animator.SetBool("isRun", false);
    }

    // Foot bone check ground
    private void CheckBone()
    {
        isGround = false;

        // Cast a ray from the foot bone downwards to check for ground
        RaycastHit hit;
        if (Physics.Raycast(footBone.position, Vector3.down, out hit, raycastDistanceBone, groundMask))
        {
            isGround = true;
        }
    }

    // Public method to check if the foot is grounded
    public bool IsGround()
    {
        return isGround;
    }
    private void HandlingJump()
    {
        magnitude = Mathf.Clamp01(direction.magnitude) * speed * Time.deltaTime;

        if (isGround)
        {
            lastGroundTime = Time.time;
        }
        if (inputActions.Player.Jump.IsPressed())
        {
            jumpButtonPressedTime = Time.time;
        }
        if (Time.time - lastGroundTime <= jumpButtonGracePeriod)
        {
            character.stepOffset = originalStepOffset;
            ySpeed = -0.5f;
            animator.SetBool("isGround", true);
            animator.SetBool("isFall", false);
            
            if (Time.time - jumpButtonPressedTime <= jumpButtonGracePeriod)
            {
                ySpeed = jumpHeight;
                animator.SetBool("isJump", true);
                isJump = true;
                jumpButtonPressedTime = null;
                lastGroundTime = null;
            }
        }
        else
        {
            character.stepOffset = 0f;
            animator.SetBool("isMove", false);

            if ((isJump && ySpeed < 0) || ySpeed < -2)
            {
                animator.SetBool("isJump", false);
                animator.SetBool("isFall", true);
            }
        }
     
        Vector3 velocity = direction * magnitude;
        velocity.y = ySpeed;
        character.Move(velocity * Time.deltaTime);
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
