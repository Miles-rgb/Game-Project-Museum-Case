using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterScript : MonoBehaviour
{    
    private CharacterController controller;
    private Camera playerCamera;
    private CameraPov cameraPov;
    //settings for field of view while flying 
    private const float NORMAL_FOV = 70f;
    private const float RUNNING_FOV = 85f;
    private const float HOOKSHOT_FOV = 100f;
    //wind particle effect
    private ParticleSystem windParticleSystem;
    //settings for mouse sensitivity; SerializeField makes a private float somewhat public, I prefer it for visual distinction
    [SerializeField] private float mouseSensitivity;
    private float cameraVerticalAngle;
    private float controllerVelocityY;
    //momentum after exiting hook state
    public Vector3 controllerVelocityMomentum;
    //settings for moving speed
    [SerializeField] private float maxWalkSpeed;
    [SerializeField] private float maxRunSpeed;
    private float movementSpeed;
    //Player gains extra movement speed while the time is slowed down
    private float timeSlowed = 1f;
    //settings for gravitational force and jumping height
    private float jumpHeight;
    private float jumpHeightNormal = 1f;
    private float jumpHeightRunning = 2f;
    [SerializeField] private float gravityForce;
    //a drag force slowing players momentum down after flying
    private float momentumDrag;
    //setting for the grapplinghook
    public LayerMask hookPoints;
    [SerializeField] private float maxHookDistance;
    private float hookshotSpeed;
    //position hit by the grapplinghook
    [SerializeField] private Transform debugHitPointTransform;
    private Vector3 hookshotPosition;
    private float hookshotSize;
    [SerializeField] private Transform PlayerEquipment;
    //we want the player to be able to take his equipment rather than giving it to them
    private bool canHook;
    [SerializeField] private Transform HookshotTransform;    
    [SerializeField] private Transform beamParticleEffect;
    //invisible object that updates last coordinates to checkpoints
    [SerializeField] private Transform lastCheckpointPos;
    //We work with states so that we have more accurate control of the player's actions
    private State currentState;    
    private enum State
    {
        Normal,
        HookshotThrown,
        HookshotFlyingPlayer,        
    }
    
    private void Awake()
    {        
        controller = GetComponent<CharacterController>();
        playerCamera = transform.Find("Main Camera").GetComponent<Camera>();
        cameraPov = playerCamera.GetComponent<CameraPov>();        
        windParticleSystem = transform.Find("Main Camera").Find("Particle Wind").GetComponent<ParticleSystem>();
        windParticleSystem.Stop();
        Cursor.lockState = CursorLockMode.Locked;
        //we turn off players ability at the start, waiting for them to pick up their equipment
        PlayerEquipment.gameObject.SetActive(false);
        canHook = false;
        HookshotTransform.gameObject.SetActive(false);
        beamParticleEffect.gameObject.SetActive(false);
    }

    private void Update()
    {      
        //a way to exit exported game file
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }       
        FallingOff();
        //simple state machine
        switch (currentState)
        {
            default:
            case State.Normal:
                ControllerView();
                ControllerMovement();
                HookshtoStart();
                break;
            case State.HookshotThrown:
                HookshotThrown();
                ControllerView();
                ControllerMovement();
                break;
            case State.HookshotFlyingPlayer:
                ControllerView();
                HookshotMovement();
                break;
        }
        
    }

    //keeps track of the players position to determent if they should reset to the last checkpoint
    private void FallingOff()
    {
        if(transform.position.y <= -50f)
        {
            gameObject.transform.position = lastCheckpointPos.position;
        }
    }
    //keeps track for if player collides with the HandWatch to unlock his abilities
    private void OnTriggerEnter(Collider other)
    {           
        if (other.CompareTag("HandWatch"))
        {
            PlayerEquipment.gameObject.SetActive(true);
            canHook = true;
        }        
    }
    //handles the player's viewpoint
    private void ControllerView()
    {
        float viewX = Input.GetAxisRaw("Mouse X");
        float viewY = Input.GetAxisRaw("Mouse Y");
        transform.Rotate(new Vector3(0f, viewX * mouseSensitivity, 0f), Space.Self);

        cameraVerticalAngle -= viewY * mouseSensitivity;
        //this lock player from rotating camera upside down
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f);
        //transform camera to face the new angle
        playerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);
    }
    //handles player's movement
    private void ControllerMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        float sprint = Input.GetAxisRaw("Fire3");
        //when player is grounded they have an ability to sprint, gaining move speed that makes them faster even in air       
        if (controller.isGrounded && sprint > 0.1f)
        {
            movementSpeed = maxRunSpeed;
            jumpHeight = jumpHeightRunning;
            cameraPov.SetCameraFov(RUNNING_FOV);
        }
        else if (controller.isGrounded)
        {
            movementSpeed = maxWalkSpeed;
            jumpHeight = jumpHeightNormal;
            cameraPov.SetCameraFov(NORMAL_FOV);
        }     
        if (vertical < 0f && controller.isGrounded)
        {
            movementSpeed = movementSpeed * 0.4f;
        } 
        else if (horizontal != 0f && controller.isGrounded)
        {
            movementSpeed = movementSpeed * 0.65f;
        } 
        //while time is slowed give player a boost to their movement
        if (Input.GetButtonDown("Fire2") && canHook != false)
        {
            timeSlowed = 1.5f;  
        }  
        if (Input.GetButtonUp("Fire2") && canHook != false)
        {
            timeSlowed = 1f;    
        }
        //calculate the direction and speed of player
        Vector3 controllerVelocity = (transform.right * horizontal + transform.forward * vertical).normalized * movementSpeed * timeSlowed;
        if (controller.isGrounded)
        {
            controllerVelocityY = gravityForce / 1.5f;
            if (TestInputJump())
            {                
                controllerVelocityY = Mathf.Sqrt(jumpHeight * -2f * gravityForce);
            }
        }        
        //calculate gravity 
        controllerVelocityY += gravityForce * Time.deltaTime;
        //apply gravity to player
        controllerVelocity.y = controllerVelocityY;
        //Apply momentum
        controllerVelocity += controllerVelocityMomentum;

        //Move Player
        controller.Move(controllerVelocity * Time.deltaTime);
        //Dampen momentum
        if (controllerVelocityMomentum.magnitude >= 0f)
        {
            if (controller.isGrounded)
            {
                momentumDrag = 5f;
            }
            else
            {
                momentumDrag = 3f;
            }
            controllerVelocityMomentum -= controllerVelocityMomentum * momentumDrag * Time.deltaTime;
            if (controllerVelocityMomentum.magnitude < .0f)
            {
                controllerVelocityMomentum = Vector3.zero;
                controllerVelocity = Vector3.zero;
            }
        }
    }
    //makes sure to reset gravitational force
    private void ResetGravityEffect()
    {
        controllerVelocityY = -1f;
    }
    //trigger to start hooking process
    private void HookshtoStart()
    {
        if (TestInputFire1() && canHook != false)
        {           
            //we raycast a beam from players camera view, if we hit a hookable platform we begin hooking process
            if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit raycastHit, maxHookDistance, hookPoints))
            {
                debugHitPointTransform.position = raycastHit.point;
                hookshotPosition = raycastHit.point;
                hookshotSize = 0f;
                HookshotTransform.gameObject.SetActive(true);
                beamParticleEffect.gameObject.SetActive(true);
                HookshotTransform.localScale = Vector3.zero;
                currentState = State.HookshotThrown; 
            }
        }
    }
    //time when the hook si being thrown
    private void HookshotThrown()
    {
        //extention from players arm shoots towards the shot position
        HookshotTransform.LookAt(hookshotPosition);
        beamParticleEffect.LookAt(hookshotPosition);

        float hookshotThrownSpeed = 70f;
        hookshotSize += hookshotThrownSpeed * Time.deltaTime;
        HookshotTransform.localScale = new Vector3(1, 1, hookshotSize);

        if (hookshotSize >= Vector3.Distance(transform.position, hookshotPosition))
        {
            currentState = State.HookshotFlyingPlayer;
            cameraPov.SetCameraFov(HOOKSHOT_FOV);
            windParticleSystem.Play();
        }
    }
    //moving along the hooked trajectory
    private void HookshotMovement()
    {
        HookshotTransform.LookAt(hookshotPosition);
        beamParticleEffect.LookAt(hookshotPosition);

        Vector3 hookshotDir = (hookshotPosition - transform.position).normalized;

        float hookshotSpeedMin = 10f;
        float hookshotSpeedMax = 30f;
        hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition), hookshotSpeedMin, hookshotSpeedMax);
        float hookshotSpeedMultiplier = 1.75f;
    
        controller.Move(hookshotDir * hookshotSpeed * hookshotSpeedMultiplier * Time.deltaTime);

        //end of the hook
        float reachedHookshotPositionDistance = 1f;
        if (Vector3.Distance(transform.position, hookshotPosition) < reachedHookshotPositionDistance)
        {
            float momentumExtraSpeed = 5f;
            controllerVelocityMomentum = hookshotDir * hookshotSpeed * momentumExtraSpeed;
            cameraPov.SetCameraFov(RUNNING_FOV);
            StopHookshot();
        }
        //Cancel Hookshot
        if (TestInputFire1())
        {
            float momentumExtraSpeed = 3f;
            controllerVelocityMomentum = hookshotDir * hookshotSpeed * momentumExtraSpeed;
            cameraPov.SetCameraFov(NORMAL_FOV);
            StopHookshot();
        }
        //Jump from Hookshot
        if (TestInputJump())
        {
            float momentumExtraSpeed = 8f;
            controllerVelocityMomentum = hookshotDir * hookshotSpeed * momentumExtraSpeed;
            controllerVelocityMomentum += Vector3.up * jumpHeight;
            cameraPov.SetCameraFov(RUNNING_FOV);
            StopHookshot();
        }
    }
    //these are made so that we don't have to repeat calling for action
    private bool TestInputFire1()
    {
        return Input.GetButtonDown("Fire1");
    }

    private bool TestInputFire2()
    {
        return Input.GetButtonDown("Fire2");
    }

    private bool TestInputJump()
    {
        return Input.GetButtonDown("Jump");
    }
    //when we stop hooking we want to reset values back to normal
    private void StopHookshot()
    {
        currentState = State.Normal;
        ResetGravityEffect();
        HookshotTransform.gameObject.SetActive(false);
        beamParticleEffect.gameObject.SetActive(false);
        windParticleSystem.Stop();
    }    
}
