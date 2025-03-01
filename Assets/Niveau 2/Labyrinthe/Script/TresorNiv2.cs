using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TresorNiv2 : MonoBehaviour
{
    [SerializeField] int _pointsTresors;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player1") //Si le joueur touché est le joueur 1
        {
            if (other.GetComponent<JoueurNiv2>()._jeuDebute == true) //Si le jeu est encore en cours
            {
                GestionUIJeu UICommand = FindObjectOfType<GestionUIJeu>().GetComponent<GestionUIJeu>(); //On trouve le script où sont inscrits les points
                UICommand.AjouterScore(_pointsTresors, 1); //On ajoute les points au joueur 1
                Destroy(this.gameObject); //On retire le trésor qui a été trouvé
            }
        }
        else if (other.gameObject.tag == "Player2") //Si c'est le joueur 2
        {
            if (other.GetComponent<JoueurNiv2>()._jeuDebute == true)
            {
                GestionUIJeu UICommand = FindObjectOfType<GestionUIJeu>().GetComponent<GestionUIJeu>(); //On trouve le script où sont inscrits les points
                UICommand.AjouterScore(_pointsTresors, 2); //On ajoute les points au joueur 2
                Destroy(this.gameObject); //On retire le trésor qui a été trouvé
            }
        }
        
    }
}
