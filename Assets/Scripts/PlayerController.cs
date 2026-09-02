using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float rotationSpeed = 10f;
    public float jumpHeight = 1.5f;
    public float gravity = -15f;

    private CharacterController controller;
    private Transform cameraTransform;
    private Animator animator;
    private float verticalVelocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        cameraTransform = Camera.main.transform;
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool isGrounded = controller.isGrounded;
        if (isGrounded && verticalVelocity < 0f)
            verticalVelocity = -2f; // keeps him snapped to the ground instead of drifting

        Vector2 input = Keyboard.current != null
            ? new Vector2(
                (Keyboard.current.dKey.isPressed ? 1 : 0) - (Keyboard.current.aKey.isPressed ? 1 : 0),
                (Keyboard.current.wKey.isPressed ? 1 : 0) - (Keyboard.current.sKey.isPressed ? 1 : 0))
            : Vector2.zero;

        bool isSprinting = Keyboard.current != null && Keyboard.current.leftShiftKey.isPressed;
        float targetSpeed = isSprinting ? runSpeed : walkSpeed;
        float currentSpeed = input.magnitude * targetSpeed;

        Vector3 moveDirection = Vector3.zero;

        if (input.sqrMagnitude > 0.001f)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            moveDirection = camForward * input.y + camRight * input.x;

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Jump
        bool jumpPressed = Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame;
        if (isGrounded && jumpPressed)
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        animator.SetBool("Jump", isGrounded && jumpPressed);

        // Gravity
        verticalVelocity += gravity * Time.deltaTime;

        // Move (horizontal + vertical combined)
        Vector3 finalMove = moveDirection * currentSpeed + Vector3.up * verticalVelocity;
        controller.Move(finalMove * Time.deltaTime);

        // Feed the Animator
        animator.SetFloat("Speed", currentSpeed);
        animator.SetFloat("MotionSpeed", Mathf.Clamp01(input.magnitude));
        animator.SetBool("Grounded", isGrounded);
        animator.SetBool("FreeFall", !isGrounded && verticalVelocity < -2f);
    }

    // These satisfy the animation events baked into the walk/land clips
    private void OnFootstep(AnimationEvent animationEvent) { }
    private void OnLand(AnimationEvent animationEvent) { }
}