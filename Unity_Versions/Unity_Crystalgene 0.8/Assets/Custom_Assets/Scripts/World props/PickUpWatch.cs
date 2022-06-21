using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpWatch : MonoBehaviour
{
    public GameObject crossHair;
    [SerializeField] private GameObject interact;
    //when the player enters our trigger collider it removes place equipment to create an illusion of picking it up

    private void Start()
    {
        interact.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        interact.SetActive(true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKey("e"))
        {            
            Destroy(gameObject);
            crossHair.gameObject.SetActive(true);
            interact.SetActive(false);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        interact.SetActive(false);
    }
}
