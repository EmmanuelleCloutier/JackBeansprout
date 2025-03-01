using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloudNextLevel : MonoBehaviour
{

    private const int pointBonusFin = 300;

    private GestionScenes _gestionScene;
    private GameObject _canvasScore;

    private bool ended = false;

    private void Start()
    {
        _gestionScene = FindObjectOfType<GestionScenes>().GetComponent<GestionScenes>(); //Trouve l'objet avec le script permettant de changer de niveau
        _canvasScore = GameObject.Find("CanvasJeu");
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (!ended)
        {
            ended = true;
            int n_joueur = 0;
            if (other.gameObject.tag.Equals("Player1"))
            {
                n_joueur = 1;
            }
            if (other.gameObject.tag.Equals("Player2"))
            {
                n_joueur = 2;
            }

            // bonus fin
            _canvasScore.GetComponent<GestionUIJeu>().AjouterScore(pointBonusFin, n_joueur);
            StartCoroutine(_gestionScene.ChangerScene());
        }
    }

}
