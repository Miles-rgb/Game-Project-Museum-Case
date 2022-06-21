using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterScript : MonoBehaviour
{    
    private CharacterController controller;
    private Camera playerCamera;
    private CameraPov cameraPov;
    private TimeManager timeManager;
    //settings for field of view while flying 
    private const float NORMAL_FOV = 70f;
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
    private float movementSpeed;
    private Vector3 controllerVelocity;
    private Vector3 lastMotion;
    //settings for gravitational force and jumping height
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravityForce;
    //smoothing jumping mechanic
    private float hangTime = 0.1f;
    private float hangTimeCounter;
    private float jumpBufferLenght = 0.2f;
    private float jumpBufferCounter;
    //a drag force slowing players momentum down after flying
    private float momentumDrag;
    //setting for the grapplinghook
    public LayerMask hookPoints;
    [SerializeField] private float maxHookDistance;
    [SerializeField] private float maxHookRadius;
    private float hookshotSpeed;
    //position hit by the grapplinghook
    [SerializeField] private Transform debugHitPointTransform;
    public bool isAiming;
    private Vector3 hookshotPosition;
    private float hookshotSize;
    [SerializeField] private Transform PlayerEquipment;
    //we want the player to be able to take his equipment rather than giving it to them
    private bool canHook;
    [SerializeField] private Transform HookshotTransform;    
    [SerializeField] private Transform beamParticleEffect;
    //invisible object that updates last coordinates to checkpoints
    [SerializeField] private Transform lastCheckpointPos;
    //torus animation
    [SerializeField] private Transform[] torusObjects;
    private Animator torusAnimator01;
    private Animator torusAnimator02;
    //We work with states so that we have more accurate control of the player's actions
    private State currentState;    
    private enum State
    {
        Normal,
        HookshotThrown,
        HookshotFlyingPlayer,        
    }

    [SerializeField] private AudioSource grapplingFX;
    
    private void Awake()
    {        
        controller = GetComponent<CharacterController>();
        playerCamera = transform.Find("Main Camera").GetComponent<Camera>();
        cameraPov = playerCamera.GetComponent<CameraPov>();  
        windParticleSystem = transform.Find("Main Camera").Find("Particle Wind").GetComponent<ParticleSystem>();
        windParticleSystem.Stop();
        isAiming = false;
        //we turn off players ability at the start, waiting for them to pick up their equipment
        PlayerEquipment.gameObject.SetActive(false);
        canHook = false;
        HookshotTransform.gameObject.SetActive(false);
        beamParticleEffect.gameObject.SetActive(false);
        timeManager = PlayerEquipment.GetComponent<TimeManager>();
        torusAnimator01 = torusObjects[0].GetComponent<Animator>();
        torusAnimator02 = torusObjects[1].GetComponent<Animator>();
    }

    private void Update()
    {   
        FallingOff();
        /*if (controller.collisionFlags == CollisionFlags.Sides)
        {
            Debug.Log("Wallrun");
        }*/
        //simple state machine
        switch (currentState)
        {
            default:
            case State.Normal:
                ControllerView();
                ControllerMovement();
                HookshotStart();               
                break;
            case State.HookshotThrown:
                ControllerView();
                ControllerMovement();
                HookshotThrown();
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
            gameObject.transform.rotation = lastCheckpointPos.rotation;
            cameraVerticalAngle = 0f;
            playerCamera.transform.localRotation = Quaternion.Euler(cameraVerticalAngle, 0f, 0f);
        }
    }
    //keeps track for if player collides with the HandWatch to unlock his abilities
    private void OnTriggerStay(Collider other)
    {           
        if (Input.GetKey("e"))
        {
            if (other.CompareTag("HandWatch"))
            {
                PlayerEquipment.gameObject.SetActive(true);
                canHook = true;
            }
        }        

        if (other.CompareTag("Chargers"))
        {
            timeManager.TimeSlowDuration += 120f;
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
        controllerVelocity = Vector3.zero;        
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        float sprint = Input.GetAxisRaw("Fire3");
        movementSpeed = 8f;
        //handle jumping buffer
        if (TestInputJump())
        {
            jumpBufferCounter = jumpBufferLenght;
        }
        else
        {
            jumpBufferCounter -= Time.unscaledDeltaTime;
        }
        //when player is grounded they have an ability to dash     
        if (controller.isGrounded && sprint > 0.1f)
        {
            //dash
        }   
        //direction speed change
        //walking backwards
        if (vertical < 0f)
        {
            movementSpeed = movementSpeed * 0.5f;
        } 
        //walking sideways
        else if (horizontal != 0f)
        {
            movementSpeed = movementSpeed * 0.75f;
        } 
        //calculate the direction and speed of player 
        if(controller.isGrounded)
        {
            controllerVelocity = (transform.right * horizontal + transform.forward * vertical).normalized * movementSpeed;
        } 
        else
        {
            controllerVelocity = (transform.right * horizontal + transform.forward * vertical).normalized * movementSpeed * 0.55f;
        }
     
        //edge jump
        if (controller.isGrounded)
        {
            hangTimeCounter = hangTime;
            controllerVelocityY = gravityForce;                      
        }
        else
        {
            hangTimeCounter -= Time.unscaledDeltaTime;
        }  
        //jumping
        if (jumpBufferCounter > 0f && hangTimeCounter > 0f)
        {         
            jumpBufferCounter = 0f;       
            hangTimeCounter = 0f;
            controllerVelocityY = Mathf.Sqrt(jumpHeight * -1.5f * gravityForce);
        }   
        else
        {
            jumpBufferCounter -= Time.unscaledDeltaTime;
        }   
        if(!controller.isGrounded)
        {

            controllerVelocity.x += Mathf.Clamp(lastMotion.x, -movementSpeed - 1.2f, movementSpeed + 1.2f);
            controllerVelocity.z += Mathf.Clamp(lastMotion.z, -movementSpeed - 1.2f, movementSpeed + 1.2f);
        }
        
        //calculate gravity 
        controllerVelocityY += gravityForce * 1.35f * Time.unscaledDeltaTime;
        //apply gravity to player
        controllerVelocity.y = controllerVelocityY;
        //Apply momentum
        controllerVelocity += controllerVelocityMomentum;

        //Move Player
        controller.Move(controllerVelocity * Time.unscaledDeltaTime);
        
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
            controllerVelocityMomentum -= controllerVelocityMomentum * momentumDrag * Time.unscaledDeltaTime;
            if (controllerVelocityMomentum.magnitude < .0f)
            {
                controllerVelocityMomentum = Vector3.zero;
                controllerVelocity = Vector3.zero;
                
            }
        }
        if (controller.isGrounded)
        {
            lastMotion = controllerVelocity * 0.8f;
        }        
    }
    //makes sure to reset gravitational force
    private void ResetGravityEffect()
    {
        controllerVelocityY = -1f;
    }
    //trigger to start hooking process
    private void HookshotStart()
    {
        if (Physics.SphereCast(playerCamera.transform.position, maxHookRadius, playerCamera.transform.forward, out RaycastHit zero, maxHookDistance, hookPoints) && canHook != false)
        {
            TorusExpand();
            isAiming = true;
        }
        else if(canHook != false)
        {
            TorusRetract();
            isAiming = false;
        }
        if (TestInputFire1() && canHook != false)
        {           
            //we raycast a beam from players camera view, if we hit a hookable platform we begin hooking process
            if (Physics.SphereCast(playerCamera.transform.position, maxHookRadius, playerCamera.transform.forward, out RaycastHit raycastHit, maxHookDistance, hookPoints))
            {
                debugHitPointTransform.position = raycastHit.point;
                hookshotPosition = raycastHit.point;
                hookshotSize = 0f;
                HookshotTransform.gameObject.SetActive(true);
                beamParticleEffect.gameObject.SetActive(true);
                grapplingFX.Play();
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
        hookshotSize += hookshotThrownSpeed * Time.unscaledDeltaTime;
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
        TorusStay();
        
        Vector3 hookshotPositionOffset = (hookshotPosition - new Vector3(0f, 3f, 0f));
        Vector3 hookshotDir = (hookshotPositionOffset - transform.position).normalized;

        float hookshotSpeedMin = 10f;
        float hookshotSpeedMax = 30f;
        hookshotSpeed = Mathf.Clamp(Vector3.Distance(transform.position, hookshotPosition), hookshotSpeedMin, hookshotSpeedMax);
        float hookshotSpeedMultiplier = 4f;
    
        controller.Move(hookshotDir * hookshotSpeed * hookshotSpeedMultiplier * Time.unscaledDeltaTime);

        //end of the hook
        float reachedHookshotPositionDistance = 4f;
        if (Vector3.Distance(transform.position, hookshotPosition) < reachedHookshotPositionDistance)
        {
            float momentumExtraSpeed = 5f;
            controllerVelocityMomentum = hookshotDir * hookshotSpeed * momentumExtraSpeed;
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
            controllerVelocityMomentum += Vector3.up * (jumpHeight / 2f);
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
        lastMotion = Vector3.zero;
        cameraPov.SetCameraFov(NORMAL_FOV);
        TorusDefault();
    }   

    private void TorusDefault()
    {
        torusAnimator01.Play("Default");
        torusAnimator02.Play("Default");
    }

    private void TorusExpand()
    {
        torusAnimator01.Play("StartExpanding");
        torusAnimator02.Play("StartExpanding");
    }

    private void TorusStay()
    {
        torusAnimator01.Play("Torus1Stay");
        torusAnimator02.Play("Torus2Stay");
    }

    private void TorusRetract()
    {
        torusAnimator01.Play("StartRetracting");
        torusAnimator02.Play("StartRetracting");
    }
}
