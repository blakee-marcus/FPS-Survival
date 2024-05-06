using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
  [Header("Health")]
  public int maxHealth = 100;

  [Header("Weapon")]
  public int weaponDamage = 50;
  public float weaponReloadTime = 2.1f;
  public int weaponMaxAmmo = 30;
  public float weaponFireRate = 2.6f;
  public float limbMultiplier = 0.9f;
  public float headshotMultiplier = 1.9f;

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
