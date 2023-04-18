using System.Collections;
using UnityEngine;
using TMPro;

public class Wingman : MonoBehaviour
{
    public int damage = 45;
    public float fireRate = 2.6f;
    private float nextTimeToFire = 0f;

    public int maxAmmo = 6;
    private int currentAmmo;
    public float reloadTime = 2.1f;

    public float headshotMultiplier = 2.15f;
    public float limbMultiplier = 0.9f;

    private bool isReloading = false;

    public Camera fpsCam;
    public TextMeshProUGUI currentAmmoText;
    public TextMeshProUGUI maxAmmoText;

    [SerializeField] ParticleSystem muzzleFlash;

    private void Start()
    {
        currentAmmo = maxAmmo;
        UpdateAmmoUI();
    }

    private void OnEnable()
    {
        isReloading = false;
    }

    private void Update()
    {
        HandleInput();
        UpdateAmmoUI();
    }

    private void HandleInput()
    {
        if (isReloading) { return; }

        if (currentAmmo <= 0)
        {
            StartCoroutine(Reload());
            return;
        }

        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
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

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
    }

    private void Shoot()
    {
        muzzleFlash.Play();
        RaycastHit hit;

        currentAmmo--;

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit))
        {
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy == null) 
            { 
                enemy = hit.transform.GetComponentInParent<Enemy>();
            }
            
            if (enemy != null)
            {
                Debug.Log(hit.collider.CompareTag("Head"));
                if (hit.collider.CompareTag("Head"))
                {
                    int headshotDamage = Mathf.RoundToInt(damage * headshotMultiplier);
                    DamagePopup.Create(hit.point, headshotDamage, true);
                    enemy.TakeDamage(headshotDamage);
                }
                else if (hit.collider.CompareTag("Limb"))
                {
                    int limbDamage = Mathf.RoundToInt(damage * limbMultiplier);
                    DamagePopup.Create(hit.point, limbDamage, false);
                    enemy.TakeDamage(limbDamage);
                }
                else
                {
                    DamagePopup.Create(hit.point, damage, false);
                    enemy.TakeDamage(damage);
                }
            }
        }
    }
}
