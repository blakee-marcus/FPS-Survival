using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // References
    [Header("References")]
    public CharacterController controller;
    public Animator anim;


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
    float slideSpeed = 15f;
    public float slideDuration = 1.5f;
    float slideHeight = 0.6f;
    Vector3 slidingCenter = new Vector3(0, 0.3f, 0);


    // Climbing
    [Header("Climbing")]
    bool isClimbing;
    public float climbSpeed = 5f;
    public float detectionLength;
    public float sphereCastRadius;
    public float wallLookAngle;
    private float maxWallLookAngle;
    public float climbTimer;
    public float maxClimbTime;

    private RaycastHit frontWallHit;
    private bool wallFront;
    public LayerMask wallMask;

    // Extra jump charges
    [Header("Extra Jump Charges")]
    public float totalExtraJumpCharges;
    float extraJumpCharges;

    private void Start()
    {
        startHeight = transform.localScale.y;
        anim = GetComponentInChildren<Animator>();
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
        CheckClimbing();
        ClimbingStateMachine();
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
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && !isCrouching)
        {
            isSprinting = true;
            Sprint();
        }
        if (Input.GetKeyUp(KeyCode.LeftShift) && isSprinting)
        {
            isSprinting = false;
            Idle();
        }

        // Automatically trigger slide if player is crouching while sprinting
        if (isGrounded && isSprinting && isCrouching)
        {
            Slide();
        }
        
        if (isClimbing) ClimbingMovement();

        if (!isSliding)
        {
            SpeedHandler();
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
            extraJumpCharges = totalExtraJumpCharges;
        }

        // Automatically trigger slide if player is crouching while sprinting
        if (isGrounded && isCrouching)
        {
            Slide();
        }
    }


    void Crouch()
{
    if (isGrounded)
    {
        // controller.height = crouchHeight;
        // smoothly transition to crouch height over 1 second
        controller.height = Mathf.Lerp(controller.height, crouchHeight, 1.5f * Time.deltaTime);
        // controller.center = crouchingCenter;
        // smoothly transition to crouching center over 1 second
        controller.center = Vector3.Lerp(controller.center, crouchingCenter, 1.5f * Time.deltaTime);
        transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
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
        slideDuration = 1.5f;
        slideSpeed = 15f;
    }

    void Slide()
    {
        isSliding = true;
        speed = slideSpeed;
        // have slide speed decrease to crouch speed within the duration of the slide
        slideSpeed = Mathf.Lerp(slideSpeed, crouchSpeed, slideDuration * Time.deltaTime);
        controller.height = slideHeight;
        controller.center = slidingCenter;
        transform.localScale = new Vector3(transform.localScale.x, slideHeight, transform.localScale.z);
        // after duration of slide, reset to crouch
        if (slideDuration <= 0)
        {
            isSliding = false;
            Crouch();
            slideDuration = 1.5f;
        }
        else
        {
            slideDuration -= Time.deltaTime;
        }
    }

    void CheckClimbing()
    {
        wallFront = Physics.SphereCast(transform.position, sphereCastRadius, transform.forward, out frontWallHit, detectionLength, wallMask);
        wallLookAngle = Vector3.Angle(transform.forward, -frontWallHit.normal);
        if (isGrounded)
        {
            climbTimer = maxClimbTime;
        }
    }
    void StartClimbing()
    {
        isClimbing = true;
    }

    void ClimbingMovement()
    {
        velocity = new Vector3(velocity.x, climbSpeed, velocity.z);
    }

    void StopClimbing()
    {
        isClimbing = false;
    }

    void ClimbingStateMachine()
    {
        if(wallFront && Input.GetKey(KeyCode.W) && wallLookAngle < maxWallLookAngle)
        {
            if (!isClimbing && climbTimer > 0) StartClimbing();
            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer < 0) StopClimbing();
            
        }
        else
        {
            if(isClimbing) StopClimbing();
        }
    }
    void SpeedHandler()
    {
        speed = isSprinting ? sprintSpeed : isCrouching ? crouchSpeed : runSpeed;
        // Set Animation States
        if (move != Vector3.zero && !isSprinting)
        {
            Walk();
        }
        if (move == Vector3.zero)
        {
            Idle();
        }
    }

    // Animation Settings
    private void Idle()
    {
        anim.SetFloat("Speed", 0f, 0.1f, Time.deltaTime);
    }
    private void Walk()
    {
        anim.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
    }
    private void Sprint()
    {
        anim.SetFloat("Speed", 1f, 0.1f, Time.deltaTime);
    }
}
