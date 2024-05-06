using UnityEngine;

public class Enemy : MonoBehaviour
{
  // Ground check
  [Header("Ground Check")]
  public Transform groundCheck;
  public LayerMask groundMask;
  Vector3 velocity;
  public float groundDistance = 0.4f;
  public float gravity = -19.62f;
  bool isGrounded;

  public float maxHealth;
  public int attackDamage = 10;
  public float attackRange = 3f;
  public float moveSpeed = 1.85f;
  public float attackSpeed = 1.25f;
  public float powerupDropChance = 2f;

  public GameObject[] powerUpPrefabs;

  private float currentHealth;
  private GameObject player;
  private float attackTimer = 0f;

  void Start()
  {
    currentHealth = maxHealth;
    player = GameObject.FindGameObjectWithTag("Player");
  }

  void Update()
  {
    HandleGravity();
    if (player != null)
    {
      float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

      if (distanceToPlayer <= attackRange && attackTimer <= 0f)
      {
        // Attack the player
        player.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
        attackTimer = attackSpeed;
      }
      else if (distanceToPlayer > attackRange)
      {
        // Move towards the player
        transform.LookAt(player.transform.position);
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
      }
    }

    if (attackTimer > 0f)
    {
      attackTimer -= Time.deltaTime;
    }
  }

  void HandleGravity()
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
    transform.position += velocity * Time.deltaTime;
  }

  public void TakeDamage(float damage)
  {
    currentHealth -= damage;
    if (currentHealth <= 0f)
    {
      DropHandler();
      Die();
    }
  }

  void DropHandler()
  {
    float randomValue;
    randomValue = Random.Range(0f, 100f);
    if (randomValue <= powerupDropChance)
    {
      int randomPowerUp = Random.Range(0, powerUpPrefabs.Length);
      Instantiate(powerUpPrefabs[randomPowerUp], transform.position, Quaternion.identity);
    }
  }

  public void Die()
  {
    // Play death animation, disable enemy GameObject, etc.
    Destroy(gameObject);
  }
}
