using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevel : MonoBehaviour
{
    [SerializeField] private GameObject interact;
    
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
            Loader.Load(Loader.Scene.Level_01);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        interact.SetActive(false);
    }
}
