using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallingTree : MonoBehaviour
{

    public float finishedFallingHeight = -35f;
    public GameObject _poussiere;
    private Animator anim;

    private GestionScenes _gestionScene;

    private void Start()
    {
        _gestionScene = FindObjectOfType<GestionScenes>().GetComponent<GestionScenes>(); //Trouve l'objet avec le script permettant de changer de niveau
        anim = _poussiere.GetComponent<Animator>();
    }

    public void fall()
    {
        this.GetComponent<BoxCollider2D>().isTrigger = true;
        StartCoroutine(explosion());
    }

    IEnumerator explosion()
    {
        yield return new WaitForSecondsRealtime(8f);
        anim.SetBool("chutevigne", true);
        yield return new WaitForSecondsRealtime(2f);

        StartCoroutine(_gestionScene.ChangerScene());
    }
    
}
