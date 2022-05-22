using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HourGlassAnimation : MonoBehaviour
{
    ParticleSystem timeJuice;  

    public Transform startPoint;
    public Transform endPoint;
    private float speed = 0.75f;
    private Rigidbody rb;
    private Vector3 currentPos;
    private Vector3 EulerAngleVelocity;

    private bool animateMove;

    [SerializeField] private float TimeSlowDuration;
    private float TimeSlowDepletion = 60f;
    private State currentTimeState;    
    private enum State
    {
        Normal,
        TimeBeingSlowed,
        TimeBeingRecharged,        
    }  
    // Start is called before the first frame update
    void Start()
    {
        timeJuice = GetComponent<ParticleSystem>();
        rb = GetComponent<Rigidbody>();
        EulerAngleVelocity = new Vector3(0f, 0f, 360f);
        var em = timeJuice.emission;
        em.enabled = false;
        animateMove = false;
    }

    // Update is called once per frame
    private void Update()
    {        
        Debug.Log(animateMove); 
        switch (currentTimeState)
        {
            default:
            case State.Normal:
                TimeRecharging();
                TimeBeingSlowedStart();
                break;
            case State.TimeBeingSlowed:
                TimeSlow();
                break;
        }
    }

    private void TimeBeingSlowedStart()
    {
        if (TestInputFire2() && TimeSlowDuration > 0.1f)
        {
            currentTimeState = State.TimeBeingSlowed;  
            animateMove = true;          
        }
    }

    private void TimeSlow()
    {    
        var em = timeJuice.emission;
        if (TestInputFire2() || TimeSlowDuration < 0.1f)
        {
            em.enabled = false;
            currentTimeState = State.Normal; 
        }
        else if (TimeSlowDuration > 0.1f)
        {
            if (animateMove = true)
            {
                transform.position = Vector3.MoveTowards(transform.position, endPoint.transform.position, Time.deltaTime * speed);
                
            }
            if (transform.position == endPoint.position)
            {
                animateMove = false;
            }                       
            Quaternion deltaRotation = Quaternion.Euler(EulerAngleVelocity * Time.fixedDeltaTime);
            rb.MoveRotation(rb.rotation * deltaRotation);
            em.enabled = true;
            TimeSlowDuration -= TimeSlowDepletion * 5f * Time.deltaTime;
        }
    }

    private void TimeRecharging()
    {
        if (TimeSlowDuration < 360f)
        {
            TimeSlowDuration += TimeSlowDepletion * 0.5f * Time.deltaTime;
            Quaternion deltaRotation = Quaternion.Euler(EulerAngleVelocity * Time.fixedDeltaTime * -1f);
            rb.MoveRotation(rb.rotation * deltaRotation);
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, startPoint.transform.position, Time.deltaTime * speed);
            Quaternion target = Quaternion.Euler(-90f, 0f, 0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, target,  Time.deltaTime * speed);
            TimeSlowDuration = 360f;                      
        }
    }

    private bool TestInputFire2()
    {
        return Input.GetButtonDown("Fire2");
    }
}
