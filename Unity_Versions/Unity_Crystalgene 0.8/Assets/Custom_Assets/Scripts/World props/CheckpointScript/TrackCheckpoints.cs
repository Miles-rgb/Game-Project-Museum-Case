using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpoints : MonoBehaviour
{
    public event EventHandler OnPlayerCheckpoint;
    public event EventHandler OnPlayerCheckpointExit;

    private void Awake()
    {
        foreach (Transform checkpointSingleTransform in transform)
        {
            CheckPoint checkpointSingle = checkpointSingleTransform.GetComponent<CheckPoint>();
            checkpointSingle.SetTrackCheckpoints(this); 
        }
    }

    public void PlayerThroughCheckpoint(CheckPoint checkpointSingle)
    {
        OnPlayerCheckpoint?.Invoke(this, EventArgs.Empty);
    }

    public void PlayerThroughCheckpointExit(CheckPoint checkpointSingle)
    {
        OnPlayerCheckpointExit?.Invoke(this, EventArgs.Empty);
    }
}
