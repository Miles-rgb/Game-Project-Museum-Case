using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour
{
    private Image cursor;
    public Camera playerCamera;
    public LayerMask hookPoints;
    private float maxHookDistance = 20f;
    // Start is called before the first frame update
    void Start()
    {        
        cursor = GetComponent<Image>();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit raycastHit, maxHookDistance, hookPoints))
        {                
            cursor.color = Color.green;
        }
        else
        {
            cursor.color = Color.white;
        }
    }
}
