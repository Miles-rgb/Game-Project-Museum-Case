using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColor : MonoBehaviour
{
    private Image hookUI;
    private Transform Player;
    private float distance;
    private float minDistance = 31f;
    private Color defaultColor = new Color(0f, 0.65f, 0f, 0.8f);
    private PlayerCharacterScript changeColor;
    private bool onlyOne;
    // Start is called before the first frame update
    void Start()
    {        
        hookUI = GetComponent<Image>();
        Player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        changeColor = GameObject.FindWithTag("Player").GetComponent<PlayerCharacterScript>();
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(transform.position, Player.position);
        if (distance < minDistance && changeColor.isAiming == true)
        {                
            hookUI.color = Color.green;
        }
        else
        {
            hookUI.color = defaultColor;
        }
    }
}
