using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementManager : MonoBehaviour
{
    private InputController inputActions;
    private CharacterController character;
    private Animator animator;

    [Header("Movement Setting")]
    public float currentSpeed = 3.0f;
    public float turnSmoothTime = 0.1f;
    public Transform mainCameraTransform;
    [Space]
    [Header("Jump Setting")]



    private float turnSmoothVelocity;

    void Awake()
    {
        inputActions = new InputController();
        character = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        HandlingCharacterMovement();
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
