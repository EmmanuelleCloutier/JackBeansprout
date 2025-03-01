using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class directionAffichage : MonoBehaviour
{
    [SerializeField] private GameObject[] directions;
    int lastDirection = 0;


    public void changeDirection(int direction)
    {
        directions[lastDirection].SetActive(false);
        if (direction != 5) 
        { 
            directions[direction].SetActive(true);
            lastDirection = direction;
        }
    }

    public void blinkAllDirection(bool on)
    {
        if (on)
        {
            foreach(GameObject x in directions)
            {
                x.SetActive(true);
            }
        }
        else
        {
            foreach(GameObject x in directions)
            {
                x.SetActive(false);
            }
        }
    }


}
