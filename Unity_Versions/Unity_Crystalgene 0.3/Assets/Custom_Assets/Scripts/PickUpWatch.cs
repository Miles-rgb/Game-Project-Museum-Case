using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpWatch : MonoBehaviour
{
    public GameObject crossHair;
    //when the player enters our trigger collider it removes place equipment to create an illusion of picking it up
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {            
            Destroy(gameObject);
            crossHair.gameObject.SetActive(true);
        }
    }
}
