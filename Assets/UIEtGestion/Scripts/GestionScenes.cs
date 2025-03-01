using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GestionScenes : MonoBehaviour
{
    public Animator fonduAuNoir;

    /*
     * R�le : Charger la scene suivante
     * Entr�e : Aucune
     */
    public IEnumerator ChangerScene()
    {
        //Trouve l'objet responsable du son
        GestionSon _gestionSon = FindObjectOfType<GestionSon>().GetComponent<GestionSon>();
        
        int indexProchaineScene = SceneManager.GetActiveScene().buildIndex + 1; //Trouve le num�ro de la sc�ne en cours
        _gestionSon.ArreterMusique(indexProchaineScene - 1); //Arrete la musique en cours


        fonduAuNoir.SetTrigger("Start"); //G�n�re l'animation pour rendre les transitions plus smooth
        yield return new WaitForSeconds(1.4f); //Attends la fin de l'animation

        _gestionSon.JouerMusique(indexProchaineScene); //Fait jouer la musique de la sc�ne suivante
        SceneManager.LoadScene(indexProchaineScene); //Charge la sc�ne avec le num�ro juste apr�s celui de la sc�ne en cours

        
    }

    /*
     * R�le : Aller au premier niveau
     * Entr�e : Aucune
     * Sortie : Aucune
     */
    public IEnumerator ChargerSceneDepart()
    {
        //Trouve l'objet responsable du son

        Destroy(GameObject.Find("GestionSon"));

        fonduAuNoir.SetTrigger("Start"); //G�n�re l'animation pour rendre les transitions plus smooth
        yield return new WaitForSeconds(1.4f); //Attends la fin de l'animation
        SceneManager.LoadScene(0); //Charge la sc�ne #1 (Sc�ne des instructions du niveau 1)
    }

    public IEnumerator ChargerSceneInstruction1()
    {
        //Trouve l'objet responsable du son
        GestionSon _gestionSon = FindObjectOfType<GestionSon>().GetComponent<GestionSon>();

        _gestionSon.ArreterMusique(_gestionSon._soundtrack.Length - 1); //Arrete la musique en cours
        fonduAuNoir.SetTrigger("Start"); //G�n�re l'animation pour rendre les transitions plus smooth
        yield return new WaitForSeconds(1.4f); //Attends la fin de l'animation
        _gestionSon.JouerMusique(1); //Fait jouer la musique de la sc�ne 1
        SceneManager.LoadScene(1); //Charge la sc�ne #1 (Sc�ne des instructions du niveau 1)
    }

    /*
     * R�le : Quitter le jeu
     * Entr�e : Aucune
     * Sortie : Aucune
     */
    public void Quitter()
    {
        Application.Quit();
    }

}
