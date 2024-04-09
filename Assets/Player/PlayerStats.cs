using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
  [Header("Health")]
  public int maxHealth = 100;
  public int currentHealth;

  [Header("Weapon")]
  public int weaponDamage = 10;
  public float weaponReloadTime = 1f;

  [Header("Movement")]
  public int maxExtraJumpCharges;
  public float runSpeed = 8f;
  public float sprintSpeed = 12f;
  public float crouchSpeed = 4f;
  public float gravity = -19.62f;
  public float jumpHeight = 3f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
