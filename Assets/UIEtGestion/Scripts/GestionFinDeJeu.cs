using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GestionFinDeJeu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _txtGagnant = default;
    [SerializeField] private TextMeshProUGUI _txtRejouer = default;
    [SerializeField] private TextMeshProUGUI _txtTimer = default;
    [SerializeField] private GameObject _imagesGagnant = default;


    public GameObject _canvasJeu;
    private GestionScenes _gestionScene;
    private int _gagnant;
    private int tempsRestant;
    private bool _resultAffiches;

    void Start()
    {
        _resultAffiches = false; //Indique que les r�sultats ne sont pas encore affich�s

        //D�sactive les �l�ments UI non n�cessaires pour le moment
        _imagesGagnant.SetActive(false);
        _txtRejouer.gameObject.SetActive(false);
        _txtTimer.gameObject.SetActive(false);
        
        _gestionScene = FindObjectOfType<GestionScenes>().GetComponent<GestionScenes>();
        _canvasJeu = GameObject.Find("CanvasJeu");
        StartCoroutine(RevelationGagnant());
}

void Update()
    {
        //Une fois les r�sultats affich�s, on retourne � la sc�ne d'instruction 1 quand un joueur appuie sur un bouton (rejouer)
        if (Input.anyKeyDown && _resultAffiches)
        {
            StartCoroutine(_gestionScene.ChargerSceneInstruction1());
        }
    }

    /*
     * R�le : Cr�er un d�lai avant d'afficher le r�sultat
     * Entr�e : Aucune
     */
    IEnumerator RevelationGagnant()
    {
        _txtGagnant.text = "Et le gagnant est...";
        
        //On compare les points en faisant appel au script gestionUIJeu
        _gagnant = _canvasJeu.GetComponent<GestionUIJeu>().ComparerScores();

        yield return new WaitForSeconds(5.5f); //D�lai pour suspense

        if (_gagnant != 0) //Si ce n'est pas une �galit�
        {
            _txtGagnant.text = "Joueur " + _gagnant; //Affiche le num�ro du gagnant

        }
        else //Si �galit�
        {
            _txtGagnant.text = "Egalite"; //Indique qu'il y a �galit�
        }
        
        _imagesGagnant.SetActive(true); //G�n�rer l'image correspondant au bon gagnant
                                                  
        yield return new WaitForSeconds(3f); //On laisse les r�sultats affich�s pendant quelques secondes

        StartCoroutine(TimerAvantQuitter()); //Commencer le timer avant de quitter
        
        Destroy(_canvasJeu); //On d�truit le UI pour ne pas qu'il soit en double si le jeu recommence

    }

    /*
     * R�le : G�rer et afficher le temps qu'il reste avant que le programme se ferme et faire clignoter le texte qui indique le temps restant
     * Entr�e : Aucune
     */
    IEnumerator TimerAvantQuitter()
    {
        tempsRestant = 30;
        _txtRejouer.gameObject.SetActive(true); //On affiche le texte pour demander aux joueurs s'ils veulent rejouer
        _resultAffiches = true; //On peut maintenant recommencer le jeu

        while(tempsRestant >= 0) //Tant qu'il reste du temps
        {
            _txtTimer.text = tempsRestant.ToString(); //On affiche le temps qui reste

            //On fait clignoter le texte
            _txtTimer.gameObject.SetActive(true);
            yield return new WaitForSeconds(.6f);
            _txtTimer.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.4f);
            tempsRestant--;
        }

        //Si le temps est �coul� et que l'on n'a pas recommencer le jeu
         StartCoroutine(_gestionScene.ChargerSceneDepart()); //On revient au d�part

    }

}
