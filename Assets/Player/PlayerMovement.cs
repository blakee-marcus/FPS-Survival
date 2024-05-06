using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
  // References
  [Header("References")]
  public CharacterController controller;
  public ParticleSystem dust;
  public PlayerStats stats;


  // Movement speed
  [Header("Movement Speed")]
  float speed;
  
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
  [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
  [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
  [SerializeField] private float crouchHeight = 0.5f;
  [SerializeField] private float standingHeight = 1.9f;
  [SerializeField] private float timeToCrouch = 0.25f;
  private bool isCrouching;

  bool isGrounded;
  bool isSprinting;

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
    standingHeight = transform.localScale.y;
    slideSpeedDecrease = slideSpeedIncrease * 1.2f;
  }

  private void Update()
  {
    // Set Grounded State
    isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
    // Reset Velocity
    if (isGrounded && velocity.y < 0)
    {
      velocity.y = -5f;
    }
    // Apply gravity
    velocity.y += gravity * Time.deltaTime;
    controller.Move(velocity * Time.deltaTime);

    if (isGrounded && !isSliding)
    {
      SpeedHandler();
    }
    if (isSliding)
    {
      HandleSlide();
    }
    HandleInput();
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

    // Hold Crouch
    if (Input.GetKeyDown(KeyCode.C))
    {
      Crouch();
    }
    if (Input.GetKeyUp(KeyCode.C))
    {
      ExitCrouch();
    }

    // Toggle Crouch
    if (Input.GetKeyDown(KeyCode.LeftControl) && !isCrouching)
    {
      Crouch();
    }
    else if (Input.GetKeyDown(KeyCode.LeftControl) && isCrouching)
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
    if (speed > stats.runSpeed)
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
    controller.height = (standingHeight * 2);
    controller.center = standingCenter;
    transform.localScale = new Vector3(transform.localScale.x, standingHeight, transform.localScale.z);
    isCrouching = false;
    isSliding = false;
  }

  void HandleSlide()
  {
    DecreaseSpeed(slideSpeedDecrease);
    slideDuration -= 1f * Time.deltaTime;
    CreateDust();
    if (slideDuration <= 0)
    {
      isSliding = false;
    }
  }

  void SpeedHandler()
  {
    speed = isCrouching ? stats.crouchSpeed : isSprinting ? stats.sprintSpeed : stats.runSpeed;
  }

  void CreateDust()
  {
    dust.Play();
  }
}
