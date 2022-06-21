using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monumentis : MonoBehaviour
{
    [SerializeField] private GameObject interact;
    [SerializeField] private GameObject portal;
    
    private void Start()
    {
        interact.SetActive(false);
        portal.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        interact.SetActive(true);
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKey("e"))
        {
            portal.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        interact.SetActive(false);
    }
}
