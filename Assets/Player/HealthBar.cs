using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
  public Slider currentSlider;
  public Slider damageSlider;
  float damageTimer = 3f;
  float timer = 0f;
  // Set the max health of the slider
  public void SetMaxHealth(int health)
  {
    currentSlider.maxValue = health;
    currentSlider.value = health;
    damageSlider.maxValue = health;
    damageSlider.value = health;
  }

  public void SetHealth(int health)
  {
    currentSlider.value = health;
    StartCoroutine(UpdateDamageSlider(health));
  }

  IEnumerator UpdateDamageSlider(int health)
  {
    yield return new WaitForSeconds(1.5f); // Wait for 1.5 seconds
    damageSlider.value = health;
  }
}
