using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    //we are using an invisible object to keep track of the current checkpoint's location
    private GameObject lastCheckPointPos;
    private ParticleSystem checkPointFirework;

    private TrackCheckpoints trackCheckpoints;
    private float timeLeft;
    
    [SerializeField] private AudioSource checkpointFX;

    private void Start()
    {
        lastCheckPointPos = GameObject.FindGameObjectWithTag("GM");
        checkPointFirework = GetComponent<ParticleSystem>();
        trackCheckpoints.PlayerThroughCheckpointExit(this);
        var em = checkPointFirework.emission;
        em.enabled = false;
    }
    //when an object with a tak "Player" enters our trigger collider, position of the last checkpoint gets updated
    private void OnTriggerEnter(Collider other)
    {        
        var em = checkPointFirework.emission;
        if (other.CompareTag("Player"))
        {
            timeLeft = 300f;
            lastCheckPointPos.transform.position = transform.position;
            lastCheckPointPos.transform.rotation = transform.rotation;
            trackCheckpoints.PlayerThroughCheckpoint(this);
            checkpointFX.Play();
            em.enabled = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        timeLeft -= 60f * Time.deltaTime;
        if (timeLeft < 0f)
        {
            OnTimerStop();
            timeLeft = 300f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        OnTimerStop();
    }

    private void OnTimerStop()
    {
        var em = checkPointFirework.emission;

        trackCheckpoints.PlayerThroughCheckpointExit(this);
        em.enabled = false;
    }

    public void SetTrackCheckpoints(TrackCheckpoints trackCheckpoints)
    {
        this.trackCheckpoints = trackCheckpoints;
    }
}
