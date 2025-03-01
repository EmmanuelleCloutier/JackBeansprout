using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GestionEcranAccueil : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _txtDebuter = default;

    private GestionScenes _gestionScene;
    private bool _continueClignoter = true;
   
    void Start()
    {
        StartCoroutine(ClignotementTextDepart());
        _gestionScene = FindObjectOfType<GestionScenes>().GetComponent<GestionScenes>(); //Trouve l'objet avec le script permettant de changer de niveau
    }

    void Update()
    {
        //Attendre qu'un des joueurs appuie sur un bouton avant de passer à la prochaine scène
        if (Input.anyKeyDown)
        {
            StartCoroutine(_gestionScene.ChangerScene());
            _continueClignoter = false;
        }
    }

    /*
     * Rôle : Faire clignoter le text qui dit d'appuyer sur un bouton pour commencer à jouer
     * Entrée : Aucune
     */
    IEnumerator ClignotementTextDepart()
    {
        while (_continueClignoter)
        {
            _txtDebuter.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.6f);
            _txtDebuter.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.4f);
        }
    }
}
