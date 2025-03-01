using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GestionSon : MonoBehaviour
{
    [SerializeField] public AudioSource[] _soundtrack = default;

    void Start()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Music"); //Vérifie si un autre objet identique est déjà présent

        if (objs.Length > 1) //Si un objet identique existe déjà
        {
            Destroy(this.gameObject); //L'objet est détruit afin qu'il n'y ait qu'une musique en jeu
        }

        _soundtrack[0].Play();

        DontDestroyOnLoad(this.gameObject); //Fait en sorte que la musique continue d'une scène à l'autre
    }

    /*
     * Rôle : Arrêter la musique en cours
     * Entrée : 1 entier qui indique quelle musique est en cours
     * Sortie : Aucune
     */
    public void ArreterMusique(int p_numeroMusique)
    {
        _soundtrack[p_numeroMusique].Stop();
    }

    /*
     * Rôle : Jouer de la musique
     * Entrée : 1 entier qui indique quelle musique jouer
     * Sortie : Aucune
     */
    public void JouerMusique(int p_numeroMusique)
    {
        _soundtrack[p_numeroMusique].Play();
    }

}
