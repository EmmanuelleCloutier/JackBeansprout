using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayerLvl3 : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        transform.position = new Vector3(transform.position.x, player.transform.position.y+2.5f, transform.position.z);
    }

}