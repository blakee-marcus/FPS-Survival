using System.Collections;
using UnityEngine;
using TMPro;

public class Wingman : MonoBehaviour
{
  [Header("References")]
  public PlayerStats stats;

  private float nextTimeToFire = 0f;
  private int currentAmmo;
  private bool isReloading = false;

  public Camera fpsCam;
  public TextMeshProUGUI currentAmmoText;
  public TextMeshProUGUI maxAmmoText;
  [SerializeField] ParticleSystem muzzleFlash;

  private void Start()
  {
    currentAmmo = stats.weaponMaxAmmo;
    UpdateAmmoUI();
  }

  private void OnEnable()
  {
    isReloading = false;
  }

  private void Update()
  {
    HandleInput();
  }

  private void HandleInput()
  {
    if (isReloading) { return; }

    if (currentAmmo <= 0)
    {
      StartCoroutine(Reload());
    }

    if (Input.GetKeyDown(KeyCode.R))
    {
      StartCoroutine(Reload());
    }

    if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
    {
      nextTimeToFire = Time.time + (1f / stats.weaponFireRate);
      Shoot();
    }
  }

  private IEnumerator Reload()
  {
    isReloading = true;
    yield return new WaitForSeconds(stats.weaponReloadTime);
    currentAmmo = stats.weaponMaxAmmo;
    isReloading = false;
    Debug.Log("Reloaded");
    UpdateAmmoUI();
  }

  private void Shoot()
  {
    muzzleFlash.Play();
    currentAmmo--;
    UpdateAmmoUI();
    RaycastHit hit;

    Debug.Log("Shooting");
    Debug.Log(currentAmmo);

    if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit))
    {
      Enemy enemy = hit.transform.GetComponent<Enemy>();

      if (enemy == null)
      {
        enemy = hit.transform.GetComponentInParent<Enemy>();
      }

      if (enemy != null)
      {
        if (hit.collider.CompareTag("Head"))
        {
          int headshotDamage = Mathf.RoundToInt(stats.weaponDamage * stats.headshotMultiplier);
          DamagePopup.Create(hit.point, headshotDamage, true);
          enemy.TakeDamage(headshotDamage);
        }
        else if (hit.collider.CompareTag("Limb"))
        {
          int limbDamage = Mathf.RoundToInt(stats.weaponDamage * stats.limbMultiplier);
          DamagePopup.Create(hit.point, limbDamage, false);
          enemy.TakeDamage(limbDamage);
        }
        else
        {
          DamagePopup.Create(hit.point, stats.weaponDamage, false);
          enemy.TakeDamage(stats.weaponDamage);
        }
      }
    }
  }

  private void UpdateAmmoUI()
  {
    currentAmmoText.text = currentAmmo.ToString("D3");
    maxAmmoText.text = stats.weaponMaxAmmo.ToString("D3");
  }
}
