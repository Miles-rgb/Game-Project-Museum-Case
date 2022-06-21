using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackCheckpointsUI : MonoBehaviour
{
    [SerializeField] private TrackCheckpoints trackCheckpoints;

    private void Start()
    {
        trackCheckpoints.OnPlayerCheckpoint += TrackCheckpoints_OnPlayerCheckpoint;
        trackCheckpoints.OnPlayerCheckpointExit += TrackCheckpoints_OnPlayerCheckpointExit;
        Hide();
    }

    private void TrackCheckpoints_OnPlayerCheckpoint(object sender, System.EventArgs e)
    {
        Show();
    }

    private void TrackCheckpoints_OnPlayerCheckpointExit(object sender, System.EventArgs e)
    {
        Hide();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
