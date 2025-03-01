using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform player;

    public float minHeight = 0;

    private float height;


    void Update()
    {
        if (minHeight >= player.transform.position.y)
        {
            height = minHeight;
        }
        else
        {
            height = player.transform.position.y;
        }
        transform.position = new Vector3(transform.position.x, height+2f, transform.position.z);
    }

}