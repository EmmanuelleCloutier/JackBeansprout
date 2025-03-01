using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpe : MonoBehaviour
{
    [SerializeField] private int _ptsVictoire = default;
    [SerializeField] public AudioSource _cri = default;

    private GestionUINiv2 _gestionUINiv2;
    private GestionUIJeu _gestionUIJeu;
    private GestionSon _gestionSon;

    // Start is called before the first frame update
    void Start()
    {
        _gestionUINiv2 = FindObjectOfType<GestionUINiv2>().GetComponent<GestionUINiv2>(); //Trouve l'objet avec le script qui génèrera une animation quand la harpe aura été trouvée
        _gestionUIJeu = FindObjectOfType<GestionUIJeu>().GetComponent<GestionUIJeu>(); //Trouve l'objet avec le script qui conserve le score des deux joueurs
        _gestionSon = FindObjectOfType<GestionSon>().GetComponent<GestionSon>(); //Pour éventuellement mettre la musique sur pause lorsque la harpe va crier
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Player1")) //Si c'est le joueur 1 qui a trouvé la harpe
        {
            _gestionUIJeu.AjouterScore(_ptsVictoire, 1); //On ajoute les points au joueur 1
            StartCoroutine(_gestionUINiv2.FinNiveau2()); //On génère la coroutine de la fin du niveau 2

            _gestionSon.ArreterMusique(4); //On arrête la musique
            _cri.PlayOneShot(_cri.clip, 0.4f); //Pour que la harpe cri
        }
        else if (other.gameObject.tag.Equals("Player2")) //Si c'est le joueur 2 qui a trouvé la harpe
        {
            _gestionUIJeu.AjouterScore(_ptsVictoire, 2);//On ajoute les points au joueur 2
            StartCoroutine(_gestionUINiv2.FinNiveau2()); //On génère la coroutine de la fin du niveau 2

            _gestionSon.ArreterMusique(4); //On arrête la musique
            _cri.PlayOneShot(_cri.clip, 0.4f); //Pour que la harpe cri
        }


    }
}
