using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class GestionUINiv1 : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _txtDonneDepart = default;
    [SerializeField] private TextMeshProUGUI _txtPlayer1 = default;
    [SerializeField] private TextMeshProUGUI _txtPlayer2 = default;
    [SerializeField] private GameObject _splitBorder = default;
    //[SerializeField] private Camera _camJ1 = default;
    //[SerializeField] private Camera _camJ2 = default;
    [SerializeField] private GameObject joueur1, joueur2;

    private GestionScenes _gestionScene;
    private int scoreJ1 = 0;
    private int scoreJ2 = 0;



    // Start is called before the first frame update
    void Start()
    {
       
        _splitBorder.SetActive(true);
        StartCoroutine(DonnerDepart());


        scoreJ1 = 0;
        scoreJ2 = 0;

        
        this.GetComponent<GestionUIJeu>().AjouterScore(scoreJ1,1);
        this.GetComponent<GestionUIJeu>().AjouterScore(scoreJ2, 0);
    }

    IEnumerator DonnerDepart()
    {
        //Attendre la fin de l'animation de fondu au noir
        _txtDonneDepart.gameObject.SetActive(false);
        yield return new WaitForSeconds(3f);

        //Décompte avant le départ
        _txtDonneDepart.text = "      3";
        _txtDonneDepart.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        _txtDonneDepart.text = "      2";
        yield return new WaitForSeconds(1f);
        _txtDonneDepart.text = "      1";
        yield return new WaitForSeconds(1f);
        _txtDonneDepart.text = "Partez!";

        joueur1.GetComponent<Mouvement>().play = true;
        joueur2.GetComponent<Mouvement>().play = true;

        yield return new WaitForSeconds(1f);
        _txtDonneDepart.gameObject.SetActive(false); 
    }

   
}

