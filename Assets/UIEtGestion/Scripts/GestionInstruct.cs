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
        _imageReadyJ1.GetComponent<Image>().color = Color.red; //Met en rouge les images accompagnant les textes "Pr�t?" pour indiquer que le joueur n'est pas pr�t
        _imageReadyJ2.GetComponent<Image>().color = Color.red;
        _txtPretJ1.text = "Pret?"; //�crit Pr�t? avec un ? pour demander si le joueur est pr�t
        _txtPretJ2.text = "Pret?";
        _gestionScene = FindObjectOfType<GestionScenes>().GetComponent<GestionScenes>(); //Trouver le script GestionScenes pour pouvoir changer de sc�ne lorsque les joueurs seront pr�ts
    }

    // Update is called once per frame
    void Update()
    {
        //Si le joueur 1 appuit sur son bouton
         if (Input.GetKeyDown("space"))
         {
             joueur1Pret = true; //le joueur 1 est pr�t
             _imageReadyJ1.GetComponent<Image>().color = Color.green; //Met en vert l'images accompagnant le texte "Pr�t" pour indiquer que le joueur 1 est pr�t
            _txtPretJ1.text = "Pret!"; //�crit Pr�t! avec un ! pour indiquer que le joueur 1 est pr�t
        }
        //Si le joueur 2 appuit sur son bouton
        if (Input.GetKeyDown("escape"))
         {
             joueur2Pret = true; //le joueur 2 est pr�t
            _imageReadyJ2.GetComponent<Image>().color = Color.green;  //Met en vert l'images accompagnant le texte "Pr�t" pour indiquer que le joueur 2 est pr�t
            _txtPretJ2.text = "Pret!"; //�crit Pr�t! avec un ! pour indiquer que le joueur 2 est pr�t
        }
        //Si les deux joueurs sont pr�ts
         if(joueur1Pret && joueur2Pret)
         {
            StartCoroutine(cestParti());
        }

    }

    /*
     * R�le : Cr�er la s�quence de transition entre les instructions et le commencement du niveau
     * Entr�e : Aucune
     */
    IEnumerator cestParti()
    {
        yield return new WaitForSeconds(0.5f);
        _cestParti.SetActive(true); //Affiche du texte pour indiquer que la partie va commencer
        yield return new WaitForSeconds(2f); 
        StartCoroutine(_gestionScene.ChangerScene()); //Passer � la sc�ne du niveau
       
    }
}
