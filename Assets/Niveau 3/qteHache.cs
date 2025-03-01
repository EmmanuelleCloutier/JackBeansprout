using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class qteHache : MonoBehaviour
{
    
    public Rigidbody2D _rb;

    private int n_joueur;
    public GameObject _directionsAffichage;
    public GameObject _otherCharacter;
    public float minInput = 0.5f, maxInput = 0.5f;
    public GameObject _tree;

    private bool play = false, needNull = false;

    private int nbrInput = 0;
    private Vector2 playerInput;
    private string input = "Null", directionNeeded = "Waiting";
    private Animator anim;


    private int _nbrInputGame1 = 20,_nbrInputGame2 = 30;
    private const int pointParCoupPrecis = 50;
    private const int pointParCoupMash = 50;
    private const int pointBonusFin = 300;

    private GameObject _canvasScore;

    // Start is called before the first frame update
    void Start()
    {
        if (this.CompareTag("Player1"))
        {
            n_joueur = 1;
        }
        else if (this.CompareTag("Player2"))
        {
            n_joueur = 2;
        }

        _canvasScore = GameObject.Find("CanvasJeu");
        anim = GetComponent<Animator>();
        _nbrInputGame2 += _nbrInputGame1;
        StartCoroutine(animationDepart());

    }

    // Update is called once per frame
    void Update()
    {
        if (play)
        {
            
            // Prendre l'input selon le joueur
            if (n_joueur == 1)
            {
                playerInput = new Vector2(Input.GetAxis("Horizontal_P1"), Input.GetAxis("Vertical_P1"));
              
            }
            if (n_joueur == 2)
            {
                playerInput = new Vector2(Input.GetAxis("Horizontal_P2"), Input.GetAxis("Vertical_P2"));
              
            }

            // Transformer l'input en 4 directions ( et aucune)
            if (playerInput.x > minInput && playerInput.y < maxInput && playerInput.y > -maxInput)
            {
                input = "Right";
               
            }
            else if (playerInput.x < -minInput && playerInput.y < maxInput && playerInput.y > -maxInput)
            {
                input = "Left";
               
            }
            else if (playerInput.y > minInput && playerInput.x < maxInput && playerInput.x > -maxInput)
            {
                input = "Up";
                
            }
            else if (playerInput.y < -minInput && playerInput.x < maxInput && playerInput.x > -maxInput)
            {
                input = "Down";
               
            }
            else
            {
                input = "Null";
            }

            //game1 : précision
            if (input == directionNeeded && nbrInput < _nbrInputGame1)
            {
                if (directionNeeded == "Null")
                {
                    //levé
                    directionNeeded = CreateInput();
                }
                else
                {
                    nbrInput++;

                    //points ++
                    _canvasScore.GetComponent<GestionUIJeu>().AjouterScore(pointParCoupPrecis, n_joueur);

                    _directionsAffichage.GetComponent<directionAffichage>().changeDirection(5);
                    directionNeeded = "Null";
                }
            }

            //game2 : button mash
            if (nbrInput >= _nbrInputGame1 && nbrInput < _nbrInputGame2)
            {

                if (needNull && input == "Null")
                {
                    _directionsAffichage.GetComponent<directionAffichage>().blinkAllDirection(true);
                    needNull = false;
                    
                }
                else if(!needNull && input != "Null")
                {
                    nbrInput++;

                    //points ++
                    _canvasScore.GetComponent<GestionUIJeu>().AjouterScore(pointParCoupMash, n_joueur);

                    _directionsAffichage.GetComponent<directionAffichage>().blinkAllDirection(false);
                    needNull = true;
                    

                }
            }
            if (nbrInput >= _nbrInputGame2)
            {
                _canvasScore.GetComponent<GestionUIJeu>().AjouterScore(pointBonusFin, n_joueur);

                end();
                _otherCharacter.GetComponent<qteHache>().end();
            }



        }
    }

    IEnumerator animationDepart()
    {
        anim.SetBool("landing", true);
        yield return new WaitForSecondsRealtime(1.5f);
        anim.SetBool("landing", false);

        yield return new WaitForSecondsRealtime(1.5f);
        anim.SetBool("iswalking", true);

        Vector2 animInput = new Vector2(0f, 0f);

        if (this.CompareTag("Player1"))
        {
            animInput = new Vector2(700f, 0f);
        }

        if (this.CompareTag("Player2"))
        {
            animInput = new Vector2(-700f, 0f);
        }

        _rb.AddForce(animInput * Time.fixedDeltaTime, ForceMode2D.Impulse);
        anim.SetBool("iswalking", false);

        yield return new WaitForSecondsRealtime(1f);
        

        _directionsAffichage.SetActive(true);
        directionNeeded = CreateInput();
        play = true;
        anim.SetBool("hacher", true);
    }

    string CreateInput()
    {
        int randomChoice = Random.Range(0, 3);
        _directionsAffichage.GetComponent<directionAffichage>().changeDirection(randomChoice);
        switch (randomChoice)
        {
            case 0:
                return "Up";
            case 1:
                return "Down";
            case 2:
                return "Left";
            case 3:
                return "Right";
        }
        return "Null";
    }

    void end()
    {
        play = false;
        anim.SetBool("hacher", false);
        _directionsAffichage.GetComponent<directionAffichage>().blinkAllDirection(false);
        _directionsAffichage.SetActive(false);
        _tree.GetComponent<fallingTree>().fall();

    }



}
