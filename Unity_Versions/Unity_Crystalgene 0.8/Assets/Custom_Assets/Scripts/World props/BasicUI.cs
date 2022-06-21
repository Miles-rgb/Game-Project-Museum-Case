using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUI : MonoBehaviour
{
    public GameObject UIholder;
    private Transform Player;
    private float distance;
    private float minDistance = 15f;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player").GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Player);
        distance = Vector3.Distance(transform.position, Player.position);
        if (distance < minDistance)
        {
            UIholder.SetActive(true);
        }
        else
        {
            UIholder.SetActive(false);
        }
    }
}
