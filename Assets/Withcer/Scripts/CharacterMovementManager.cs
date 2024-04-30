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
    public float groundCheckDistance = 0.5f;
    public LayerMask groundMask;
    public Transform groundCheck;
    public bool isGrounded;
    public float jumpHeight;
    private Vector3 velocity = Vector3.zero;

    [Space]
    [Header("Boolians")]
    public bool isWalk;
    public bool isRun;
    public bool isJump;

    private float turnSmoothVelocity;
    public float raycastDistance;

    void Awake()
    {
        inputActions = new InputController();
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        HandlingCharacterMovement();
        HandlingSprint();
        HandlingJump();

        // Ground Check
        isGrounded = Physics.Raycast(groundCheck.position, Vector3.down, groundCheckDistance, groundMask);

        //Cursor Unlock
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           Cursor.lockState = CursorLockMode.None;
        }

    }

    private void HandlingCharacterMovement()
    {
        Vector2 inputMovement = inputActions.Player.Movement.ReadValue<Vector2>().normalized;
        float horizontal = inputMovement.x;
        float verticle = inputMovement.y;
        Vector3 direction = new Vector3(horizontal, 0f, verticle).normalized;

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
        if (!isGrounded) character.Move(Vector3.down * gravity * Time.deltaTime);    
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

    private void HandlingJump()
    {
        if (inputActions.Player.Jump.triggered && isGrounded)
        {
            velocity.y = Mathf.Sqrt(2f * jumpHeight * gravity);
            isJump = true;
        }
        velocity.y -= gravity * Time.deltaTime;
        character.Move(velocity * Time.deltaTime);
        JumpAnimation();
    }

    private void JumpAnimation()
    {
        if (inputActions.Player.Jump.triggered && isJump == true)
        {
            animator.SetBool("isJump", true);
        }
        if (isGrounded == false)
        {
            animator.SetBool("isFall", true);
            animator.SetBool("isJump", false);
            isJump = false;
        }

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, groundMask))
        {
            animator.SetBool("isLand", true);
            animator.SetBool("isFall", false);
        }
        
    }

    public void GroundDetect()
    {
       animator.SetBool("isLand", false);
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
