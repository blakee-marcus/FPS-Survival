using UnityEngine;

public class Enemy : MonoBehaviour
{
    
    public int maxHealth;
    public int attackDamage = 10;
    public float attackRange = 3f;
    public float moveSpeed = 1.85f;
    public float attackSpeed = 1.25f; // Attack speed in seconds
    public float powerupDropChance = 0.02f; // Chance of dropping a powerup (0.5 = 50%)
    public GameObject nukePowerUpPrefab; // Reference to the nuke power-up prefab

    private float currentHealth;
    private GameObject player;
    private float attackTimer = 0f; // Timer between attacks


    void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
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

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemy took " + damage + " damage. Current health: " + currentHealth);
        if (currentHealth <= 0)
        {
            if (Random.value < powerupDropChance)
            {
                Instantiate(nukePowerUpPrefab, transform.position, Quaternion.identity);
            }
            Die();
        }
    }

    public void Die()
    {
        // Play death animation, disable enemy GameObject, etc.
        Destroy(gameObject);
    }
}
