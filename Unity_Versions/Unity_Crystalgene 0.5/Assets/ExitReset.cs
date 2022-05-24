using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitReset : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }      
        if (Input.GetKey("p"))
        {            
            Loader.Load(Loader.Scene.Level_00);
        }
    }
}
