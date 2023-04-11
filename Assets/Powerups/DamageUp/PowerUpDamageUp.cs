using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpDamageUp : MonoBehaviour
{
    [SerializeField] private GameObject pickupEffect;
    public float duration = 30f;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            
           StartCoroutine(Pickup(other));
        }
    }

    IEnumerator Pickup(Collider player)
    {
        Instantiate(pickupEffect, transform.position, transform.rotation);

        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Collider>().enabled = false;

        Debug.Log(player);
        
        // player.GetComponent<Wingman>().damage *= 10;

        yield return new WaitForSeconds(duration);

        // player.GetComponent<Wingman>().damage /= 10;

        Destroy(gameObject);
    }
}
