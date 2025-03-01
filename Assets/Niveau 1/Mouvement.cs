using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouvement : MonoBehaviour
{


    [SerializeField] private GameObject _character;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _otherPlayer;


    [HideInInspector] public bool play = false;
    private int n_joueur;
    private Vector2 playerInput;
    private bool jumpPressed = false;
    private bool wantJump = false;
    private int canJump = 0;
    private Vector2 lasttouchPosition;
    private bool waiting = false;
    private bool jumping = false;
    private Animator anim;

    private GameObject _canvasScore;

    private const int pointParPlateforme = 100;
    private const int heightMilestone = 10;
    private int heightMilestoneAchieved = 5;
    private const float respawn_interval = 1f;
    private const float _moveSpeed = 40f;
    private const float _jumpSpeed = 15f;


    private void Start()
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
        lasttouchPosition = this.GetComponent<Transform>().position;
    }


    private void Update()
    {
        if(play)
        {   
            if(n_joueur == 1)
            {
                playerInput = new Vector2(Input.GetAxis("Horizontal_P1"), 0f);
                jumpPressed = Input.GetKeyDown(KeyCode.W);
            }

            if (n_joueur == 2)
            {
                playerInput = new Vector2(Input.GetAxis("Horizontal_P2"), 0f);
                jumpPressed = Input.GetKeyDown(KeyCode.UpArrow);
            }



            if (canJump > 0 && jumpPressed)
            {
                jumpPressed = false;
                canJump--;
                wantJump = true;
            }

        }
    
    }

    //même vitesse que toute la physique
    private void FixedUpdate()
    {
        if (!waiting)
        {


            // move

            if (jumping)
            {
                _rb.AddForce(playerInput * _moveSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
                if (playerInput.x > 0)
                {
                    //droite
                    anim.SetBool("isjumpingG", false);
                    anim.SetBool("isjumpingD", true);
                }
                else
                {
                    //gauche
                    anim.SetBool("isjumpingD", false);
                    anim.SetBool("isjumpingG", true);
                }

            }
            else if (playerInput.x == 0)
            {
                anim.SetBool("iswalkingD", false);
                anim.SetBool("iswalkingG", false);
            }
            else
            {
                _rb.AddForce(playerInput * _moveSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);

                if (playerInput.x > 0)
                {
                    //droite
                    anim.SetBool("iswalkingD", true);
                }
                else
                {
                    //gauche
                    anim.SetBool("iswalkingG", true);
                }
            }

            // jump
            if (wantJump)
            {
                jumping = true;
                _rb.AddForce(Vector2.up * _jumpSpeed, ForceMode2D.Impulse);


                //anim jump
                if (playerInput.x > 0)
                {
                    //droite
                    anim.SetBool("isjumpingD", true);
                }
                else if (playerInput.x < 0)
                {
                    //gauche
                    anim.SetBool("isjumpingG", true);
                }




                wantJump = false;
            }
            

            if(this.GetComponent<Transform>().position.y < lasttouchPosition.y - 2f)
            {
                StartCoroutine(respawn());
                
            }
        }
    }

    IEnumerator respawn()
    {
        waiting = true;//stoper le mouvement durant le respawn

        this.GetComponent<CapsuleCollider2D>().isTrigger = true;//deactiver collider

        //add animation respawn 
        yield return new WaitForSecondsRealtime(respawn_interval);
        
        this.GetComponent<CapsuleCollider2D>().isTrigger = false;//activer collider

        this.GetComponent<Transform>().position = new Vector2(lasttouchPosition.x, lasttouchPosition.y + 0.5f);
        waiting = false;
    }

    /*
    * Rôle : actions quand touche plateforme
    * Entrée : aucune 
    * Sortie : aucune 
    */
    void OnTriggerEnter2D(Collider2D other)
    {

        // toucher une plateforme -> toucher
        if (other.gameObject.tag.Equals("Plateforme") && other.gameObject.GetComponent<Transform>().position.y >= lasttouchPosition.y - 2f)
        {
            //Changer la plateforme de réaparition
            lasttouchPosition = other.gameObject.GetComponent<Transform>().position;
            _camera.GetComponent<FollowPlayer>().minHeight = lasttouchPosition.y;

            //ajouter score
            if(heightMilestoneAchieved <= other.GetComponent<Transform>().position.y)
            {
                _canvasScore.GetComponent<GestionUIJeu>().AjouterScore(pointParPlateforme, n_joueur);
                heightMilestoneAchieved += heightMilestone;
            }
        }
        //reseter les sauts
        jumping = false;
        anim.SetBool("isjumpingG", false);
        anim.SetBool("isjumpingD", false);
        canJump = 2;
    }

    /*
     * Rôle : un double jump uniquement
     * Entrée : aucune 
     * Sortie : aucune 
     */
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Plateforme") || other.gameObject.tag.Equals("HighestPlateforme"))
        {
            canJump = 1;
        }
    } 

}

