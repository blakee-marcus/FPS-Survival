using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactables : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        
        float interactRange = 2f;
        Physics.OverlapSphere(transform.position, interactRange);
    }
}
