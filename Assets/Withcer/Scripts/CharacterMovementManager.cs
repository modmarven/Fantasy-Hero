using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementManager : MonoBehaviour
{
    private InputController inputActions;
    private CharacterController character;
    public Animator animator;
    private Rigidbody rigidBody;

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
    public bool isGrounded;

    [Space]
    [Header("Boolians")]
    public bool isWalk;
    public bool isRun;

    private float turnSmoothVelocity;

    void Awake()
    {
        inputActions = new InputController();
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        rigidBody = GetComponent<Rigidbody>();
    }


    void Update()
    {
        HandlingCharacterMovement();
        HandlingSprint();

        // Ground Check
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, groundCheckDistance, groundMask);
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
        if (!isGrounded) character.Move(Vector3.down * gravity);    
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

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
