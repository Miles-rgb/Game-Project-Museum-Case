using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    //we are trying to create fade effect for death transition by changing the alpha of an image
    [SerializeField] private Transform Player;
    private Image blackout;

    private void Awake()
    {
        blackout = GetComponent<Image>();        
        var tempColor = blackout.color;
        tempColor.a = 0f;
        blackout.color = tempColor;
    }

    private void Update()
    {
        // when player start to lose consciousness
        if (Player.position.y <= -30f)
        {
            StartBlackout();
        }
        else
        {
            ResetBlackout();
        }
    }

    private void StartBlackout()
    {
        //with a loop we increase alpha over time
        for (var i = 0f; i <= 1f; i+=0.1f)
        {
            var tempColor = blackout.color;
            if (tempColor.a < 1f)
            {
                tempColor.a +=  Time.unscaledDeltaTime/3;
                blackout.color = tempColor;
            }
        }
    }
    private void ResetBlackout()
    {
        var tempColor = blackout.color;
        if (tempColor.a > 0f)
        {
            tempColor.a -=  Time.unscaledDeltaTime/2;
            blackout.color = tempColor;
        }
        
    }
}
