using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitReset : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("o"))
        {
            Application.Quit();
        }    
        if (Input.GetKey("p"))
        {            
            Loader.Load(Loader.Scene.Level_00);
        }  
    }
}
