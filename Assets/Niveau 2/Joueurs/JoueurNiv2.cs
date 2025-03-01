using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoueurNiv2 : MonoBehaviour
{
    [SerializeField] private float _walkSpeedMax = 15f;
    [SerializeField] private float _initialSpeed = 10f;
    [SerializeField] private float _runSpeedMax = 40f;
    [SerializeField] private float _acceleration = 1f;
    [SerializeField] private float _rotationSpeed = 700f;
    [SerializeField] private Camera _cam = default;
    [SerializeField] private Pathfinder _pathfinder = default;

    private Rigidbody2D _rb;
    public Vector2 _direction = Vector2.zero;
    public bool _jeuDebute = false;
    private bool _suivreChemin = false;
    private float _posX, _posY;
    private int _distanceX, _distanceY;
    private List<PathNode> _chemin;
    private float _currentSpeed = 0f;
    private float _accelerationTime = 0f;

    public Animator _animator;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>(); //On accède au rigidbody du joueur
        _jeuDebute = false;
        _suivreChemin = false;
    }

    void FixedUpdate()
    {
        if (_jeuDebute) //Si le jeu est en cours
        {
            SetSpeed(_walkSpeedMax);
            MouvementsJoueurs();
        }
        else if (_suivreChemin) //Si le joueur doit suivre le chemin généré par l'algorithme du chemin le plus court
        {
            SetSpeed(_runSpeedMax);
            SuivreChemin(); 
        }

        RotateInDirectionOfInput();
        _cam.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, -10); //Déplace la caméra avec le joueur
    }

    /*
     * Rôle : Déterminer la vitesse du joueur
     * Entrée : 1 float qui indique la vitesse maximale du joueur
     * Sortie : Aucune
     */
    private void SetSpeed(float p_SpeedMax)
    {
        _accelerationTime += Time.deltaTime; //Augmente le temps depuis le début de l'accélération
        _currentSpeed = _initialSpeed + _accelerationTime * _acceleration; //Détermine la vitesse actuelle
        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, p_SpeedMax); //S'assure que la vitesse ne dépasse pas la vitesse maximale
    }

    /*
     * Rôle : Déplacer le joueur dans la direction voulue
     * Entrée : Aucune
     * Sortie : Aucune
     */
    private void MouvementsJoueurs()
    {
        if (_direction != Vector2.zero) //Si le joueur se déplace
        {
            Vector3 direction3D = new Vector3(_direction.x, _direction.y, 0f);
            _rb.MovePosition(transform.position + direction3D); //Déplace le joueur dans la direction voulue
        }
        else
        {
            _rb.constraints = RigidbodyConstraints2D.FreezePosition; //On bloque la position
            _accelerationTime = 0; //Le joueur perd son accélération, parce qu'il a arrêté de bouger
        }

        if (this.CompareTag("Player1")) //Si c'est le joueur 1
        {
            _posX = Input.GetAxisRaw("Horizontal_P1"); //Regarde dans quelle direction horizontale le joueur veut se déplacer
            _posY = Input.GetAxisRaw("Vertical_P1"); //Regarde dans quelle direction verticale le joueur veut se déplacer
            _direction = new Vector2(_posX, _posY) * _currentSpeed * Time.deltaTime; //Détermine le vecteur de direction
        }
        else if (this.CompareTag("Player2")) //Si c'est le joueur 2
        {
            _posX = Input.GetAxis("Horizontal_P2"); //Regarde dans quelle direction horizontale le joueur veut se déplacer
            _posY = Input.GetAxis("Vertical_P2"); //Regarde dans quelle direction verticale le joueur veut se déplacer
            _direction = new Vector2(_posX, _posY) * _currentSpeed * Time.deltaTime; //Détermine le vecteur de direction
        }
    }

    /*
     * Rôle : Faire tourner le joueur dans la direction de son déplacement
     * Entrée : Aucune
     */
    private void RotateInDirectionOfInput()
    {
        if (_direction != Vector2.zero) //Si le joueur se déplace
        {
            _rb.constraints = RigidbodyConstraints2D.None; //On permet au joueur de se déplacer
            _rb.freezeRotation = false; //On permet la rotation
            Quaternion targetRotation = Quaternion.LookRotation(transform.forward, _direction); //On détermine dans quelle direction effectuer la rotation
            Quaternion rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime); //On crée quaternion de déplacement

            _rb.MoveRotation(rotation); //On effectue la rotation
            _animator.SetBool("IsWalking", true); //Fait jouer l'animation du joueur qui se déplace
        }
        else //Si le joueur ne se déplace pas
        {
            _rb.freezeRotation = true; //On bloque la rotation
            _animator.SetBool("IsWalking", false); //Arrête l'animation du joueur qui se déplace
        }

    }

    /*
     * Rôle : Indiquer que le joueur peut commencer à se déplacer
     * Entrée : Aucune
     * Sortie : Aucune
     */
    public void DebuterJeu()
    {
        _jeuDebute = true;
    }

    /*
     * Rôle : Indique au joueur que le jeu est terminé
     * Entrée : Aucune
     * Sortie : Aucune
     */
    public void FinNiveau()
    {
        //Trouve le chemin le plus court qui permet de retourner au début du labyrinthe
        _chemin = _pathfinder.FindPath(this.transform.position.x, this.transform.position.y);

        _accelerationTime = 0; //Remet le temps d'accélération à 0, car le joueur part du repos
        _suivreChemin = true; //Pour que le joueur commence à suivre le chemin
        
        //On regarde si le joueur est le joueur 1, car seul le joueur 1 est appelé par un script extérieur pour que les deux joueurs n'utilisent pas la grille de PathNodes en même temps
        if(this.CompareTag("Player1"))
        {
            GameObject.FindWithTag("Player2").GetComponent<JoueurNiv2>().FinNiveau(); //On appelle la fonction de fin de niveau du joueur 2
        }
        
    }

    /*
     * Rôle : Faire en sorte que le joueur suive le chemin le plus court vers la sortie du labyrinthe
     * Entrée : Aucune
     * Sortie : Aucune
     */
    private void SuivreChemin()
    {
        if (_chemin.Count > 0) //Si on n'a pas atteint la fin du chemin
        {
            float distance = (_chemin[0].transform.position - transform.position).magnitude; //On calcule la distance entre la position du joueur et le prochain noeud 
            _direction = (_chemin[0].transform.position - transform.position)  / distance; //On calcule un vecteur qui indique dans quelle direction se diriger

            if(distance < 0.2f) //Si le noeud est atteint
            {
                _chemin.Remove(_chemin[0]); //On passe au prochain noeud
            }
            else //Si le noeud n'est pas atteint
            {
                //On déplace le joueur en direction du noeud
                Vector3 direction3D = new Vector3(_direction.x, _direction.y, 0f);
                transform.position = Vector3.MoveTowards(transform.position, _chemin[0].transform.position, _currentSpeed * Time.deltaTime);
            }

        }
        else //Si on est à la fin du chemin
        {
            this.gameObject.SetActive(false); //Le joueur disparaît
        }
        

    }
}
