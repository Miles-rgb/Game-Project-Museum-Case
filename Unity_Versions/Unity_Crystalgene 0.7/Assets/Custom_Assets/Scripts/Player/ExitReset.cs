using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitReset : MonoBehaviour
{
    public PlayerCharacterScript playerControl;
    public HeadBobbingScript headBob;
    public Transform playerOffHand;
    public bool startInMenu;

    private void Start()
    {
        if (startInMenu == true)
        {
            playerControl.enabled = false;
            headBob.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            playerOffHand.gameObject.SetActive(true);
        }    
        else
        {
            playerOffHand.gameObject.SetActive(false);
        }    
    }
    // Update is called once per frame
    private void Update()
    {
        /*if (Input.GetKey("escape"))
        {
            Application.Quit();
        }      */
        if (Input.GetKeyDown("escape"))
        {            
            if (playerControl.enabled == true)
            {
                Cursor.lockState = CursorLockMode.None;
                playerOffHand.gameObject.SetActive(true);
                headBob.sideCamera.GetComponent<Animator>().Play("Default");
                headBob.enabled = false;
                playerControl.enabled = false;
            }
        }
        if (TestInputFire1())
        {
            Cursor.lockState = CursorLockMode.Locked;   
            playerOffHand.gameObject.SetActive(false);             
            headBob.enabled = true;
            playerControl.enabled = true;
        }        
    }

    private bool TestInputFire1()
    {
        return Input.GetButtonDown("Fire1");
    }
}
