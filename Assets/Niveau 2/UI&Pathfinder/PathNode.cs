using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    [SerializeField] private float _largeurMurs = default;

    public int x;
    public int y;

    public int gCost;
    public int hCost;
    public int fCost;

    public PathNode cameFromNode = default;

    public void setPosition(float p_posX, float p_posY, float p_tailleCaseX, float p_tailleCaseY)
    {
        x = (int)(p_posX / p_tailleCaseX);
        y = (int)(p_posY / p_tailleCaseY);
        transform.localScale = new Vector3((p_tailleCaseX - _largeurMurs), (p_tailleCaseY - _largeurMurs), 0);
    }

    public void CalculateFCost()
    {
        fCost = gCost + hCost;
    }

    /*
     * Rôle : Détecter s'il y a un mur au-dessus ou en-dessus du noeud
     * Entrée : 1 entier qui indique à quelle distance du noeud se trouverait le mur
     * Sortie : 1 booléen qui indique si le chemin est dégagé
     */
    public bool DetectWallUp(float p_dist)
    {
        bool espaceLibre = true; //Booléen qui indique si le chemin est dégagé

        //Envoie un raycast pour voir s'il y a un mur
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, p_dist, LayerMask.GetMask("DetectionMur"));

        //Si le raycast frappe quelque chose
        if (hit.collider != null)
        {
            espaceLibre = false; //On indique que le chemin ne passe pas par là
        }

        return espaceLibre;
    }

    /*
     * Rôle : Détecter s'il y a un mur à droite ou à gauche du noeud
     * Entrée : 1 entier qui indique à quelle distance du noeud se trouverait le mur
     * Sortie : 1 booléen qui indique si le chemin est dégagé
     */
    public bool DetectWallSide(float p_dist)
    {
        bool espaceLibre = true; //Booléen qui indique si le chemin est dégagé

        //Envoie un raycast pour voir s'il y a un mur
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, p_dist, LayerMask.GetMask("DetectionMur"));

        //Si le raycast frappe quelque chose
        if (hit.collider != null)
        {
            espaceLibre = false; //On indique que le chemin ne passe pas par là
        }

        return espaceLibre;
    }


}
