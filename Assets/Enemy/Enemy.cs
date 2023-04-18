using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int maxHealth;
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
        if (currentHealth <= 0)
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
