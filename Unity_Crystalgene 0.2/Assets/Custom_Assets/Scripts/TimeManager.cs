using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    
    private void Update()
    {
        //Debug.Log(TimeSlowDuration);
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
        }
    }

    private void TimeRecharging()
    {
        if (TimeSlowDuration < 360f)
        {
            TimeSlowDuration += TimeSlowDepletion * 0.5f * Time.deltaTime;
        }
        else
        {
            TimeSlowDuration = 360f;           
        }
    }




    private bool TestInputFire2()
    {
        return Input.GetButtonDown("Fire2");
    }

    public void Doslowmotion ()
    {
        Time.timeScale = 0.2f;
    }
    public void ReturnTimeToNormal()
    {
        Time.timeScale = 1f;
    }
}
