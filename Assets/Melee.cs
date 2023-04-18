using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Melee : MonoBehaviour
{
    public int damage = 45;
    public float fireRate = 2.6f;
    private float nextTimeToFire = 0f;
    public float range = 4f;

    public int maxAmmo = 0;
    private int currentAmmo;

    public Camera fpsCam;
    public TextMeshProUGUI currentAmmoText;
    public TextMeshProUGUI maxAmmoText;


    private void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    private void Update()
    {
        HandleInput();
        UpdateAmmoUI();
    }

    private void HandleInput()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }

    }

    private void UpdateAmmoUI()
    {
        if (currentAmmo != int.Parse(currentAmmoText.text))
        {
            currentAmmoText.text = currentAmmo.ToString("D2");
        }

        if (maxAmmo != int.Parse(maxAmmoText.text))
        {
            maxAmmoText.text = maxAmmo.ToString("D3");
        }
    }

    private void Shoot()
    {
        RaycastHit hit;


        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy == null) 
            { 
                enemy = hit.transform.GetComponentInParent<Enemy>();
            }
            
            if (enemy != null)
            {
                    DamagePopup.Create(hit.point, damage, false);
                    enemy.TakeDamage(damage);
            }
        }
    }
}
