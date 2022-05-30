using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUI : MonoBehaviour
{
    private Transform Player;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Player);
    }
}
