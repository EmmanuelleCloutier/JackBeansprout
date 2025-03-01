using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GestionInstruct : MonoBehaviour
{
    [SerializeField] private GameObject _imageReadyJ1 = default;
    [SerializeField] private GameObject _imageReadyJ2 = default;
    [SerializeField] private GameObject _cestParti = default;
    [SerializeField] private TextMeshProUGUI _txtPretJ1 = default;
    [SerializeField] private TextMeshProUGUI _txtPretJ2 = default;

    private bool joueur1Pret = false;
    private bool joueur2Pret = false;
    private GestionScenes _gestionScene;

    // Start is called before the first frame update
    void Start()
    {
        _cestParti.SetActive(false); //Cache le texte qui dit que la partie va commencer
        _imageReadyJ1.GetComponent<Image>().color = Color.red; //Met en rouge les images accompagnant les textes "Prêt?" pour indiquer que le joueur n'est pas prêt
        _imageReadyJ2.GetComponent<Image>().color = Color.red;
        _txtPretJ1.text = "Pret?"; //Écrit Prêt? avec un ? pour demander si le joueur est prêt
        _txtPretJ2.text = "Pret?";
        _gestionScene = FindObjectOfType<GestionScenes>().GetComponent<GestionScenes>(); //Trouver le script GestionScenes pour pouvoir changer de scène lorsque les joueurs seront prêts
    }

    // Update is called once per frame
    void Update()
    {
        //Si le joueur 1 appuit sur son bouton
         if (Input.GetKeyDown("Horizontal_P1") || Input.GetKeyDown("Vertical_P1") || Input.GetKeyDown("space"))
         {
             joueur1Pret = true; //le joueur 1 est prêt
             _imageReadyJ1.GetComponent<Image>().color = Color.green; //Met en vert l'images accompagnant le texte "Prêt" pour indiquer que le joueur 1 est prêt
            _txtPretJ1.text = "Pret!"; //Écrit Prêt! avec un ! pour indiquer que le joueur 1 est prêt
        }
        //Si le joueur 2 appuit sur son bouton
        if (Input.GetKeyDown("Horizontal_P2") || Input.GetKeyDown("Vertical_P2") || Input.GetKeyDown("escape"))
         {
             joueur2Pret = true; //le joueur 2 est prêt
            _imageReadyJ2.GetComponent<Image>().color = Color.green;  //Met en vert l'images accompagnant le texte "Prêt" pour indiquer que le joueur 2 est prêt
            _txtPretJ2.text = "Pret!"; //Écrit Prêt! avec un ! pour indiquer que le joueur 2 est prêt
        }
        //Si les deux joueurs sont prêts
         if(joueur1Pret && joueur2Pret)
         {
            StartCoroutine(cestParti());
        }

    }

    /*
     * Rôle : Créer la séquence de transition entre les instructions et le commencement du niveau
     * Entrée : Aucune
     */
    IEnumerator cestParti()
    {
        yield return new WaitForSeconds(0.5f);
        _cestParti.SetActive(true); //Affiche du texte pour indiquer que la partie va commencer
        yield return new WaitForSeconds(2f); 
        StartCoroutine(_gestionScene.ChangerScene()); //Passer à la scène du niveau
       
    }
}
