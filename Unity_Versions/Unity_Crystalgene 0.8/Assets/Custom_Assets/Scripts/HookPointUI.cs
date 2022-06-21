using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookPointUI : MonoBehaviour
{
    private Transform Player;

    void Start()
    {
        Player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    void Update()
    {
        transform.LookAt(Player);
    }
}
