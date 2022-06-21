using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footsteps : MonoBehaviour
{
    private CharacterController controller;
    [SerializeField] private AudioSource walkingFX;
    [SerializeField] private AudioSource jumpingFX;
    [SerializeField] private AudioSource landingFX;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(controller.isGrounded == true && controller.velocity.magnitude > 2f && walkingFX.isPlaying == false)
        {
            walkingFX.volume = Random.Range(0.8f, 1f);
            walkingFX.pitch = Random.Range(0.8f, 1.1f);
            walkingFX.Play();
        }
        else if(controller.velocity.magnitude < 2f || controller.isGrounded == false)
        {
            walkingFX.Stop();
        }

        if(controller.isGrounded == false)
        {
            jumpingFX.Play();
        }
        else if(controller.isGrounded == true)
        {
            landingFX.Play();
        }
    }
}
