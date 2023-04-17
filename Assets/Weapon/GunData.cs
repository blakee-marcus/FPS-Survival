using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Gun", menuName = "Weapon/Gun")]
public class GunData : ScriptableObject
{
    [Header("Info")]
    public string gunName;

    [Header("Shooting")]
    public float damage;

    [Header("Reloading")]
    public int currentAmmo;
    public int maxAmmo;
    public float fireRate;
    public float reloadTime;
    [HideInInspector]
    public bool isReloading;
}
