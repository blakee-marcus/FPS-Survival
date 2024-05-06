using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpDamageUp : MonoBehaviour
{
    [SerializeField] private GameObject pickupEffect;
    public float duration = 30f;
    private GameObject instantiatedEffect;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            
           StartCoroutine(Pickup(other));
        }
    }

    IEnumerator Pickup(Collider player)
    {
        instantiatedEffect = Instantiate(pickupEffect, transform.position, transform.rotation);

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        player.gameObject.transform.GetComponent<PlayerStats>().weaponDamage *= 10;

        yield return new WaitForSeconds(duration);

        player.gameObject.transform.GetComponent<PlayerStats>().weaponDamage /= 10;
        
        Destroy(instantiatedEffect);
        Destroy(gameObject);
    }
}
