using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GestionUINiv2 : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _txtDonneDepart = default;
    [SerializeField] private GameObject _splitBorder = default;
    [SerializeField] private Camera _camEnsemble = default;
    [SerializeField] private Camera _camJ1 = default;
    [SerializeField] private Camera _camJ2 = default;

    private JoueurNiv2 _joueur1;
    private JoueurNiv2 _joueur2;
    private GestionScenes _gestionScene;


    void Start()
    {
        //Activer la caméra qui montre l'ensemble du labyrinthe
        _camJ1.enabled = false;
        _camJ2.enabled = false;
        _camEnsemble.enabled = true;

        _splitBorder.SetActive(false); //Ne pas mettre la barre pour le split screen tout de suite
        
        //Trouve le script pour chaque joueur pour pouvoir leur indiquer à quel moment ils peuvent commencer à bouger
        _joueur1 = GameObject.FindWithTag("Player1").GetComponent<JoueurNiv2>();
        _joueur2 = GameObject.FindWithTag("Player2").GetComponent<JoueurNiv2>();

        StartCoroutine(DonnerDepart());

        _gestionScene = FindObjectOfType<GestionScenes>().GetComponent<GestionScenes>(); //Trouve l'objet avec le script permettant de changer de niveau
    }

    /*
     * Rôle : Afficher un signal de départ
     * Entrée : Aucune
     */
    IEnumerator DonnerDepart()
    {
        //Attendre la fin de l'animation de fondu au noir
        _txtDonneDepart.gameObject.SetActive(false);
        yield return new WaitForSeconds(3f);

        //Décompte avant le départ
        _txtDonneDepart.text = "3";
        _txtDonneDepart.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        _txtDonneDepart.text = "2";
        yield return new WaitForSeconds(1f);
        _txtDonneDepart.text = "1";
        yield return new WaitForSeconds(1f);
        _txtDonneDepart.text = "Partez!";
        yield return new WaitForSeconds(1f);

        _txtDonneDepart.gameObject.SetActive(false); //Enlever le texte du décompte
        _splitBorder.SetActive(true); //Mettre la barre qui sépare le split screen

        //Mettre les caméras en split screen
        _camJ1.enabled = true;
        _camJ2.enabled = true;
        _camEnsemble.enabled = false;

        //Indiquer au script des joueurs que le jeu commence
        _joueur1.DebuterJeu();
        _joueur2.DebuterJeu();
    }

    /*
     * Rôle : Générer une animation à la fin du niveau
     * Entrée : Aucune
     */
    public IEnumerator FinNiveau2()
    {
        //Fait en sorte que les joueurs ne puissent pas bougés
        _joueur1._jeuDebute = false;
        _joueur2._jeuDebute = false;
        
        //Arrête l'animation de déplacement des joueurs
        _joueur1._direction = Vector2.zero;
        _joueur2._direction = Vector2.zero;

        //Activer la caméra qui montre l'ensemble du labyrinthe
        _camJ1.enabled = false;
        _camJ2.enabled = false;
        _camEnsemble.enabled = true;

        _splitBorder.SetActive(false); //Enlever la barre qui sépare le split screen

        yield return new WaitForSeconds(2f); //Attend pendant que la harpe crie

        FindObjectOfType<Geant>().GetComponent<Geant>().FinNiveau(); //Le géant se réveille et il est en colère

        yield return new WaitForSeconds(2f); //Attend pendant l'animation du géant en colère

        _joueur1.FinNiveau(); //Le joueur 1 s'enfuit du labyrinthe, le joueur 2 commencera à s'enfuir quand le joueur 1 lui dira qu'il a fini d'utiliser le pathfinder

        yield return new WaitForSeconds(18f); //Attend pendant que les joueurs s'enfuient

        StartCoroutine(_gestionScene.ChangerScene()); //Passe à la scène suivante
    }
}
