using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // References
    [Header("References")]
    public CharacterController controller;
    public ParticleSystem dust;


    // Movement speed
    [Header("Movement Speed")]
    float speed;
    public float runSpeed = 8f;
    public float sprintSpeed = 12f;
    public float crouchSpeed = 4f;

    // Gravity and jump
    [Header("Gravity and Jump")]
    public float gravity = -19.62f;
    public float jumpHeight = 3f;

    // Ground check
    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    // Movement vectors
    [Header("Movement Vectors")]
    Vector3 velocity;
    Vector3 forwardDirection;
    Vector3 move;

    // Crouching
    [Header("Crouching")]
    bool isCrouching;
    float crouchHeight = 0.5f;
    float startHeight;
    Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);

    // Standing
    [Header("Standing")]
    bool isGrounded;
    bool isSprinting;
    Vector3 standingCenter = new Vector3(0, 0.9f, 0);

    // Sliding
    [Header("Sliding")]
    bool isSliding;
    float slideSpeedIncrease = 2.5f;
    float slideSpeedDecrease;
    float slideDuration = 1.5f;
    public float maxSlideDuration = 1.5f;

    // Extra jump charges
    [Header("Extra Jump Charges")]
    public float maxExtraJumpCharges;
    float extraJumpCharges;

    void IncreaseSpeed(float speedIncrease)
    {
        speed += speedIncrease * Time.deltaTime;
    }

    void DecreaseSpeed(float speedDecrease)
    {
        speed -= speedDecrease * Time.deltaTime;
    }

    private void Start()
    {
        startHeight = transform.localScale.y;
        slideSpeedDecrease = slideSpeedIncrease * 1.2f;
    }

    private void Update()
    {
        // Set Grounded State
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        // Reset Velocity
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
        // Apply gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        HandleInput();
        if (isGrounded && !isSliding)
        {
            SpeedHandler();
        }
        if (isSliding)
        {
            HandleSlide();
        }
    }
    private void HandleInput()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        // Crouch
        if (Input.GetKeyDown(KeyCode.C))
        {
            Crouch();
        }
        if (Input.GetKeyUp(KeyCode.C))
        {
            ExitCrouch();
        }
        // Sprint
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded)
        {
            isSprinting = !isSprinting;
        }
    }


    void Jump()
    {
        // Calculate jump velocity based on current movement
        float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        if (isSliding)
        {
            jumpVelocity += velocity.magnitude;
        }

        // Perform the jump
        if (Input.GetButtonDown("Jump") && extraJumpCharges > 0)
        {
            velocity.y = jumpVelocity;
            extraJumpCharges--;
        }
        else if (Input.GetButtonDown("Jump") && extraJumpCharges == 0 && isGrounded)
        {
            velocity.y = jumpVelocity;
        }

        // Reset extra jumps to starting value
        if (isGrounded)
        {
            extraJumpCharges = maxExtraJumpCharges;
        }
    }


    void Crouch()
    {
        controller.height = Mathf.Lerp(controller.height, crouchHeight, 1.5f * Time.deltaTime);
        controller.center = Vector3.Lerp(controller.center, crouchingCenter, 1.5f * Time.deltaTime);
        transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
        if (speed > runSpeed)
        {
            isSprinting = false;
            isSliding = true;
            if (isGrounded)
            {
                IncreaseSpeed(slideSpeedIncrease);
            }
            slideDuration = maxSlideDuration;
        }
        isCrouching = true;
    }

    void ExitCrouch()
    {
        controller.height = (startHeight * 2);
        controller.center = standingCenter;
        transform.localScale = new Vector3(transform.localScale.x, startHeight, transform.localScale.z);
        isCrouching = false;
        isSliding = false;
    }

    void HandleSlide()
    {
        DecreaseSpeed(slideSpeedDecrease);
        slideDuration -= 1f * Time.deltaTime;
        if (slideDuration <= 0)
        {
            isSliding = false;
        }
    }

    void SpeedHandler()
    {
        speed = isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : runSpeed;
    }

    void CreateDust()
    {
        dust.Play();
    }
}
