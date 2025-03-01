using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GestionUIJeu : MonoBehaviour
{
    //public static GestionUIJeu instance;
    [SerializeField] private TextMeshProUGUI _txtScoreJ1 = default;
    [SerializeField] private TextMeshProUGUI _txtScoreJ2 = default;

    int _scoreJ1, _scoreJ2;


    void Start()
    {
        _scoreJ1 = 0;
        _scoreJ2 = 0;
        DontDestroyOnLoad(this.gameObject); //Fait en sorte que les points s'accumulent d'une scène à l'autre
    }

    /*
     * Role : Augmenter le score d'un des joueurs
     * Entree : Un int correspondant au nombre de points a ajouter, un int correspondant au numéro du joueur
     * Sortie : Aucune
     */
    public void AjouterScore(int p_points, int p_numJoueur)
    {
        //Si c'est le joueur 1
        if (p_numJoueur == 1)
        {
            _scoreJ1 += p_points; //Modifie les points du joueur 1
            _txtScoreJ1.text = "Points : " + _scoreJ1.ToString(); //Met à jour l'affichage des points du joueur 1
        }
        else if (p_numJoueur == 2) //Si c'est le joueur 2
        {
            _scoreJ2 += p_points; //Modifie les points du joueur 2
            _txtScoreJ2.text = "Points : " + _scoreJ2.ToString(); //Met à jour l'affichage des points du joueur 2
        }

    }

    /*
     * Role : Comparer les scores des joueurs pour déterminer le gagnant
     * Entree : Aucune
     * Sortie : Un int qui représente le numéro du joueur gagnant ou 0 en cas d'égalité
     */
    public int ComparerScores()
    {
        if (_scoreJ1 < _scoreJ2) //Si le joueur 2 a un pointage plus élevé
        {
            return 2; //Indique que joueur #2 a gagné
        }
        else if (_scoreJ2 < _scoreJ1) //Si le joueur 1 a un pointage plus élevé
        {
            return 1; //Indique que joueur #1 a gagné
        }
        else //Si aucun joueur n'a un pointage plus élevé
        {
            return 0; //Indique qu'il n'y a aucun gagnant
        }
    }

}

