using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    //we are using an invisible object to keep track of the current checkpoint's location
    private GameObject lastCheckPointPos;

    private void Start()
    {
        lastCheckPointPos = GameObject.FindGameObjectWithTag("GM");
    }
    //when an object with a tak "Player" enters our trigger collider, position of the last checkpoint gets updated
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            lastCheckPointPos.transform.position = transform.position;
            lastCheckPointPos.transform.rotation = transform.rotation;
        }
    }
}
