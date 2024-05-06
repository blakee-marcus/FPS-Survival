using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    //public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;
  public PlayerStats stats;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = stats.maxHealth;
        healthBar.SetMaxHealth(stats.maxHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
    }
}
