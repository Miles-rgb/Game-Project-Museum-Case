using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private float TimeSlowDuration;
    private float TimeSlowDepletion = 60f;
    private State currentTimeState;    
    private enum State
    {
        Normal,
        TimeBeingSlowed,
        TimeBeingRecharged,        
    }

    public Slider sliderTracker;

    private void Start()
    {
        sliderTracker.gameObject.SetActive(true);
        sliderTracker.value = TimeSlowDuration;
    }
    
    private void Update()
    {        
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
        }
    }

    private void TimeSlow()
    {    
        if (TestInputFire2() || TimeSlowDuration < 0.1f)
        {
            ReturnTimeToNormal();
            currentTimeState = State.Normal; 
        }
        else if (TimeSlowDuration > 0.1f)
        {
            Doslowmotion();
            TimeSlowDuration -= TimeSlowDepletion * 5f * Time.deltaTime;
            sliderTracker.value = TimeSlowDuration;
        }
    }

    private void TimeRecharging()
    {
        if (TimeSlowDuration < 360f)
        {
            TimeSlowDuration += TimeSlowDepletion * 0.5f * Time.deltaTime;
            sliderTracker.value = TimeSlowDuration;
        }
        else
        {
            TimeSlowDuration = 360f; 
            sliderTracker.value = TimeSlowDuration;          
        }
    }

    private bool TestInputFire2()
    {
        return Input.GetButtonDown("Fire2");
    }


    public void Doslowmotion ()
    {
        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02F * Time.timeScale;
    }

    public void ReturnTimeToNormal()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02F ;
    }
}
