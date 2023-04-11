using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpNuke : MonoBehaviour
{
    private GameObject[] enemies;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceToPlayer <= 3f)
        {
            Nuke();
        }
    }

    void Nuke()
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Enemy enemyScript = enemy.GetComponent<Enemy>();
            if (enemyScript != null)
            {
                enemyScript.Die();
            }
        }
        Destroy(gameObject);
    }
}
