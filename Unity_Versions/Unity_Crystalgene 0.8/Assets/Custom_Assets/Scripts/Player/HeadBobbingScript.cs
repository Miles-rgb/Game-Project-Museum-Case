using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobbingScript : MonoBehaviour
{
    public Camera sideCamera;
    private CharacterController controller;

    // Start is called before the first frame update
    private void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        
        if (new Vector2(horizontal, vertical).magnitude != 0f && controller.isGrounded)
        {
            StartBobbing();
        }
        else
        {
            StopBobbing();
        }
    }

    private void StartBobbing()
    {
        sideCamera.GetComponent<Animator>().Play("Headbob");
    }

    private void StopBobbing()
    {
        if (sideCamera.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime >1)
        {
            sideCamera.GetComponent<Animator>().Play("Default");
        }
    }
}
