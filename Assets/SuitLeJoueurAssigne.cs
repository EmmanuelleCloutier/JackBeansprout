using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuitLeJoueurAssigne : MonoBehaviour
{
    [SerializeField] int numJoueur;
    GameObject _joueurAssigne;
    // Start is called before the first frame update
    void Start()
    {
        if (numJoueur == 1)
        {
            _joueurAssigne = GameObject.FindWithTag("Player1");
        }
        else
        {
            _joueurAssigne = GameObject.FindWithTag("Player2");
        }
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3(_joueurAssigne.transform.position.x, _joueurAssigne.transform.position.y, -10);
    }
}
