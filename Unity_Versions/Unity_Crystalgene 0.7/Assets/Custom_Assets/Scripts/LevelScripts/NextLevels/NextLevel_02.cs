using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevel_02 : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {            
            Loader.Load(Loader.Scene.Level_03);
        }
    }
}
