using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraPov : MonoBehaviour
{
    private Camera playerCamera;    
    private float targetFov;
    private float fov;
    
    private void Awake()
    {
        playerCamera = GetComponent<Camera>();
        targetFov = playerCamera.fieldOfView;
        fov = targetFov;
    }

    private void Update()
    {
        float fovSpeed = 4f;
        //linearly interpolate between a and b by t
        fov = Mathf.Lerp(fov, targetFov, Time.deltaTime * fovSpeed);
        playerCamera.fieldOfView = fov;        
    }

    public void SetCameraFov(float targetFov)
    {
        this.targetFov = targetFov;
    }
}
