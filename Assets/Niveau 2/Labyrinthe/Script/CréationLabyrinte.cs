using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.TextCore.Text;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Random = UnityEngine.Random;






public class CréationLabyrinte : MonoBehaviour
{
    [SerializeField] private Camera _camEnsemble = default;
    [SerializeField] private GameObject _background = default;

    [SerializeField] GameObject _Joueur1;
    [SerializeField] GameObject _Joueur2;
    [SerializeField] private GameObject _Geant;
    

    [SerializeField] public TresorNiv2 _Tresor;
    [SerializeField] float _probabiliteTresor;



    [SerializeField] GameObject _Harpe;

    [SerializeField] GameObject _IntelliBalle;
    [SerializeField] GameObject _YeuxBalle;

    [SerializeField] GameObject _coin1;
    [SerializeField] GameObject _coin2;
    [SerializeField] GameObject _coin3;
    [SerializeField] GameObject _coin4;

    [SerializeField] GameObject _CoinMur;
    [SerializeField] GameObject _Mur;

    [SerializeField] private GameObject _ContainerMur = default;
    [SerializeField] private GameObject _ContainerCoins = default;
    [SerializeField] private GameObject _ContainerTresors = default;

    [SerializeField] public float _longueur;
    [SerializeField] public float _largeur;

    [SerializeField] public float _CoordonneDepartX;
    [SerializeField] public float _CoordonneDepartY;

    [SerializeField] float _CoordonneSalleX;
    [SerializeField] float _CoordonneSalleY;

    [SerializeField] float _TailleSalleSpeciale;

    [SerializeField] public float _tailleGrille;



    private float[] _UniteDeDistance = new float[4];


    private float _direction;

    private bool _haut = true, _bas = true, _gauche = true, _droite = true;


    private bool _confirmationDirection = false;

    private bool _confirmationRecul;
    private bool _confirmationNEWdir;

    private bool[] _verifBackHaut = new bool[4];
    private bool[] _verifBackBas = new bool[4];
    private bool[] _verifBackGauche = new bool[4];
    private bool[] _verifBackDroite = new bool[4];


    private List<float> _MemoireACourtTerme = new List<float>();
    private List<int> _MemoireALongTerme = new List<int>();

    private float _nbCasestotales;
    private float _nbCasesExplorés = 1;
    private int _CurseurMemoire;

    private float taillexcarre;
    private float tailleycarre;


    private int _rand;
    void Start()
    {
        AjusterCam();

        taillexcarre = 0.62494f;
        tailleycarre = 0.2989262f;
        _MemoireALongTerme.Add(0);
        //placement des coins principaux selon la taille voulue
        _coin1.transform.position = new Vector2(_CoordonneDepartX, _CoordonneDepartY);
        _coin2.transform.position = new Vector2(_coin1.transform.position.x + _longueur, _coin1.transform.position.y);
        _coin3.transform.position = new Vector2(_coin1.transform.position.x, _coin1.transform.position.y + _largeur);
        _coin4.transform.position = new Vector2(_coin1.transform.position.x + _longueur, _coin1.transform.position.y + _largeur);



        //calcul de la distance verticale et horizontale entre les coins des murs
        _UniteDeDistance[0] = _longueur / _tailleGrille;
        _UniteDeDistance[1] = _largeur / _tailleGrille;


        //calcul de la distance verticale et horizontale des murs pour qu'il soient entre les coins des murs
        _UniteDeDistance[2] = (_longueur / _tailleGrille) / 2;
        _UniteDeDistance[3] = (_largeur / _tailleGrille) / 2;

        _nbCasestotales = (_tailleGrille * _tailleGrille);

        //génération des coins des murs



        for (int y = 0; y <= _tailleGrille; y++)
        {

            for (int i = 0; i <= _tailleGrille; i++)
            {
                Vector3 position = new Vector3((_UniteDeDistance[0] * i)- 0.05391f, (_UniteDeDistance[1] * y)- 0.04119f, 0);
                GameObject newCoin = Instantiate(_CoinMur, position, transform.rotation);
                newCoin.transform.parent = _ContainerCoins.transform;
                newCoin.transform.localScale = new Vector3(0.1715994f, 0.1677747f, 0);

            }
        }

        //génération des murs horizontaux
        for (int y = 0; y <= (_tailleGrille + 0.5f); y++)
        {

            for (int i = 0; i < _tailleGrille; i++)
            {


                
                Vector3 position2 = new Vector3((_UniteDeDistance[0] * (i + 1) - _UniteDeDistance[2]), _UniteDeDistance[1] * y, 0);
                GameObject MurHorizontaux = Instantiate(_Mur, position2, transform.rotation);
                MurHorizontaux.transform.localScale = new Vector3(1, 1, 0);
                MurHorizontaux.transform.localScale = new Vector3(taillexcarre, _UniteDeDistance[1] * tailleycarre, 0);
                MurHorizontaux.transform.eulerAngles = new Vector3(0, 0, 90);
                MurHorizontaux.transform.parent = _ContainerMur.transform;
            }
        }

        // génération des murs Verticaux
        for (int y = 0; y <= _tailleGrille; y++)
        {

            for (int i = 0; i < (_tailleGrille); i++)
            {



                Vector3 position3 = new Vector3(_UniteDeDistance[0] * y, (_UniteDeDistance[1] * (i + 1) - _UniteDeDistance[3]), 0);
                GameObject MurVerticaux = Instantiate(_Mur, position3, transform.rotation);
                MurVerticaux.transform.localScale = new Vector3(1, 1 , 0);
                MurVerticaux.transform.localScale = new Vector3(taillexcarre, _UniteDeDistance[1]*tailleycarre, 0);
                MurVerticaux.transform.parent = _ContainerMur.transform;
            }
        }


        //instruction pour que la balle rouge(la balle inteligente) soit dans le carré du bas à gauche du labyrinthe
        _IntelliBalle.transform.position = new Vector3(_CoordonneDepartX + _UniteDeDistance[2], _CoordonneDepartY + _UniteDeDistance[3], 0);

        generationSalle();
        GenerationTresor();
        _IntelliBalle.transform.position = new Vector3(0, 0, 0);
        _IntelliBalle.transform.position = new Vector3(_CoordonneDepartX + _UniteDeDistance[2], _CoordonneDepartY + _UniteDeDistance[3], 0);

        generationPrincipale();

        porteSalleSpeciale();

        Destroy(_IntelliBalle);
        Destroy(_YeuxBalle);


        Vector3 positionJ1 = new Vector3((_CoordonneDepartX + _UniteDeDistance[2]) - (_UniteDeDistance[2] / 2), _CoordonneDepartY + _UniteDeDistance[3], 0);
        Vector3 positionJ2 = new Vector3((_CoordonneDepartX + _UniteDeDistance[2]) + (_UniteDeDistance[2] / 2), _CoordonneDepartY + _UniteDeDistance[3], 0);

        _Joueur1.transform.position = positionJ1;
        _Joueur2.transform.position = positionJ2;

        Vector3 positionBIGBOY = new Vector3((_coin4.transform.position.x - _UniteDeDistance[2]), _coin4.transform.position.y - _UniteDeDistance[3], 0);
        GameObject Geant = Instantiate(_Geant, positionBIGBOY, transform.rotation);


        
        Geant GeantCommander = FindObjectOfType<Geant>();
        GeantCommander.PartageDonnées(_UniteDeDistance[0], 0);
        GeantCommander.PartageDonnées(_UniteDeDistance[1], 1);
        GeantCommander.PartageDonnées(_UniteDeDistance[2], 2);
        GeantCommander.PartageDonnées(_UniteDeDistance[3], 3);

        GeantCommander.InfoPositionJoueur(_Joueur1.transform.position, 0);
        GeantCommander.InfoPositionJoueur(_Joueur2.transform.position, 1);

     
    }

    /*
     * Rôle : Aligner la caméra pricipale et le fond de jeu avec le labyrinthe
     * Entrée : Aucune
     * Sortie : Aucune
     */
    private void AjusterCam()
    {
        Vector3 centreDuLabyrinthe = new Vector3((_CoordonneDepartX + _longueur / 2), (_CoordonneDepartY + _largeur / 2), 0f); //détermine la position du centre du labyrinthe

        _camEnsemble.transform.position = centreDuLabyrinthe; //Aligne la caméra au centre du labyrinthe
        _camEnsemble.orthographicSize = _largeur / 2; //Ajuste le champ de vision de la caméra selon la taille du labyrinthe

        _background.transform.position = centreDuLabyrinthe; //Aligne le fond de jeu au centre du labyrinthe
        _background.transform.localScale = new Vector3(_longueur, _largeur, 0f); //Ajuste la taille du fond de jeu pour qu'elle soit la même que celle du labyrinthe
    }

    private void porteSalleSpeciale()
    {
        int centreSalleSP = (int)_TailleSalleSpeciale / 2;

        _IntelliBalle.transform.position = new Vector3(_MemoireACourtTerme[0], _MemoireACourtTerme[1], 0);

        //for (int i = 1; i < _TailleSalleSpeciale; i++)
        //{
        //    _IntelliBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x, _IntelliBalle.transform.position.y + _UniteDeDistance[1], 0);
        //}

        for (int i = 0; i < centreSalleSP ; i++)
        {
            _IntelliBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x + _UniteDeDistance[0], _IntelliBalle.transform.position.y , 0);
        }
        RaycastHit2D porte = Physics2D.Raycast(_IntelliBalle.transform.position, Vector2.down/*Vector2.up*/, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
        Destroy(porte.collider.gameObject);

        _IntelliBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x, _IntelliBalle.transform.position.y + _UniteDeDistance[1], 0);
        Instantiate(_Harpe, _IntelliBalle.transform.position, transform.rotation);


    }

    private void generationSalle()
    {
        //Détermine que la position de la harpe est au centre de la salle spéciale
        int posXHarpe = (int)(_TailleSalleSpeciale / 2);
        int posYHarpe = posXHarpe + 1;

        Vector3 anciennePosition;
        do
        {
            _IntelliBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x + _UniteDeDistance[0], _IntelliBalle.transform.position.y, 0);

        } while (_IntelliBalle.transform.position.x < _CoordonneSalleX);

        

        do
        {
            _IntelliBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x, _IntelliBalle.transform.position.y + _UniteDeDistance[1], 0);

        } while (_IntelliBalle.transform.position.y < _CoordonneSalleY);

        _MemoireACourtTerme.Add(_IntelliBalle.transform.position.x);
        _MemoireACourtTerme.Add(_IntelliBalle.transform.position.y);

        anciennePosition = _IntelliBalle.transform.position;
        for (int x = 1; x <= _TailleSalleSpeciale; x++)
        {

            for (int i = 1; i <= _TailleSalleSpeciale; i++)
            {
              

                if (x < _TailleSalleSpeciale)
                {
                    RaycastHit2D hit2 = Physics2D.Raycast(_IntelliBalle.transform.position, Vector2.up, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));


                    Destroy(hit2.collider.gameObject);
                }



                if (_IntelliBalle.transform.position.x < _coin4.transform.position.x && i < _TailleSalleSpeciale)
                {
                    RaycastHit2D hit1 = Physics2D.Raycast(_IntelliBalle.transform.position, Vector2.right, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));

                    Destroy(hit1.collider.gameObject);
                    _IntelliBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x + _UniteDeDistance[2], _IntelliBalle.transform.position.y, 0);

                    if (x < _TailleSalleSpeciale)
                    {
                        RaycastHit2D hit3 = Physics2D.Raycast(_IntelliBalle.transform.position, Vector2.up, _UniteDeDistance[3]);
                        Destroy(hit3.collider.gameObject);
                    }
                    _IntelliBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x + _UniteDeDistance[2], _IntelliBalle.transform.position.y, 0);

                }
                _nbCasestotales--;
                
            }
            anciennePosition.y += _UniteDeDistance[1];
            _IntelliBalle.transform.position = anciennePosition;
        }
      
        
    }



    private void GenerationTresor()
    {
        float nbTresor;
        float coXTresor;
        float coYTresor;
        int taille = (int)_tailleGrille;

        nbTresor = (_probabiliteTresor * _nbCasestotales) / 100;
        if (nbTresor <= 0)
        {
            nbTresor = 1;
        }

        for (int i = 1; i <= (nbTresor + 1); i++)
        {
            coXTresor = Random.Range(1, taille);
            coYTresor = Random.Range(1, taille);

            Vector3 positionTresor = new Vector3((_CoordonneDepartX + _UniteDeDistance[2]) + (_UniteDeDistance[0] * coXTresor), (_CoordonneDepartY + _UniteDeDistance[3]) + (_UniteDeDistance[1] * coYTresor), 0);

            RaycastHit2D trUp = Physics2D.Raycast(positionTresor, Vector2.up, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
            RaycastHit2D trDown = Physics2D.Raycast(positionTresor, Vector2.down, _UniteDeDistance[3],LayerMask.GetMask("DetectionMur"));
            RaycastHit2D trLeft = Physics2D.Raycast(positionTresor, Vector2.left, _UniteDeDistance[2],LayerMask.GetMask("DetectionMur"));
            RaycastHit2D trRight = Physics2D.Raycast(positionTresor, Vector2.right, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));

            if (trUp == true && trDown == true && trRight == true && trLeft == true)
            {
                TresorNiv2 newTresor = Instantiate(_Tresor, positionTresor, transform.rotation);
                newTresor.transform.parent = _ContainerTresors.transform;
            }
            else
            {
                i--;
            }
            
        }


    }


        
   


        private void generationPrincipale()
        {
            do
            {







                //rénitialisaton des booléen qui affirme la validité de la direction
                _haut = true;
                _bas = true;
                _droite = true;
                _gauche = true;

                //ce booléen permet de confirmer que la sphère intelligente à bel et bien bouger à la case où la balle verte(la balle visionnaire) est
                _confirmationDirection = false;

                 //Yeux balle représente la balle verte(la balle visionaire) dans le code
                //ici, on s'assure que Yeux Balle est bel et bien à la même position que la balle rouge
                _YeuxBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x, _IntelliBalle.transform.position.y, 0);


                //boucle principale
                do
                {
                    //on détermine la prochaine direcion
                    _direction = Random.Range(1, 5);



                    




                    

                    if (_haut == true || _bas == true || _droite == true || _gauche == true)
                    {
                        //Switch qui performe l'action nécéssaire selon la direction
                        switch (_direction)
                        {
                            case 1:
                                //si la direction du haut est valide; fait ceci
                                if (_haut == true)
                                {
                                    //on demande à la balle verte de se placer dans la case en haut de la balle rouge
                                    _YeuxBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x, _IntelliBalle.transform.position.y + _UniteDeDistance[1], 0);

                                    //on vérifie si la balle verte est toujours dans le labyrinthe
                                    if (_YeuxBalle.transform.position.y < _coin3.transform.position.y)
                                    {
                                        //on déploie un raycast dans toute les direction pour voir si la case ne fait pas partie du chemin
                                        RaycastHit2D hitup = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.up, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
                                        RaycastHit2D hitdown = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.down, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
                                        RaycastHit2D hitleft = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.left, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));
                                        RaycastHit2D hitright = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.right, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));

                                        //si les raycasts confirment la présence des murs, alors continue
                                        if (hitup == true && hitdown == true && hitleft == true && hitright == true)
                                        {


                                            //la destruction du mur du haut pourrait se faire ici

                                            RaycastHit2D hit = Physics2D.Raycast(_IntelliBalle.transform.position, Vector2.up, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
                                            Destroy(hit.collider.gameObject);


                                            _IntelliBalle.transform.position = new Vector3(_YeuxBalle.transform.position.x, _YeuxBalle.transform.position.y, 0);
                                            _MemoireALongTerme.Add(1);
                                            _nbCasesExplorés++;
                                            _confirmationDirection = true;

                                        }
                                        else
                                        {
                                           _haut = false;
                                        }
                                    }
                                    else
                                    {
                                        _haut = false;
                                    }
                                }
                                break;


                            case 2:
                                if (_bas == true)
                                {
                                    _YeuxBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x, _IntelliBalle.transform.position.y - _UniteDeDistance[1], 0);
                                    if (_YeuxBalle.transform.position.y >_coin1.transform.position.y)
                                    {
                                        RaycastHit2D hitup = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.up, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
                                        RaycastHit2D hitdown = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.down, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
                                        RaycastHit2D hitleft = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.left, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));
                                        RaycastHit2D hitright = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.right, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));

                                        if (hitup == true && hitdown == true && hitleft == true && hitright == true)
                                        {
                                            RaycastHit2D hit = Physics2D.Raycast(_IntelliBalle.transform.position, Vector2.down, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
                                            Destroy(hit.collider.gameObject);

                                            _IntelliBalle.transform.position = new Vector3(_YeuxBalle.transform.position.x, _YeuxBalle.transform.position.y, 0);
                                            _MemoireALongTerme.Add(2);
                                            _nbCasesExplorés++;
                                            _confirmationDirection = true;
                                        }
                                        else
                                        {
                                            _bas = false;
                                        }
                                    }
                                    else
                                    {
                                        _bas = false;
                                    }

                                }
                                break;


                            case 3:
                                if (_gauche == true)
                                {
                                    _YeuxBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x - _UniteDeDistance[0], _IntelliBalle.transform.position.y, 0);
                                    if (_YeuxBalle.transform.position.x > _coin1.transform.position.x)
                                    {
                                        RaycastHit2D hitup = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.up, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
                                        RaycastHit2D hitdown = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.down,_UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
                                        RaycastHit2D hitleft = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.left, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));
                                        RaycastHit2D hitright = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.right, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));

                                        if (hitup == true && hitdown == true && hitleft == true && hitright == true)
                                        {

                                            RaycastHit2D hit = Physics2D.Raycast(_IntelliBalle.transform.position, Vector2.left, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));
                                            Destroy(hit.collider.gameObject);


                                            _IntelliBalle.transform.position = new Vector3(_YeuxBalle.transform.position.x, _YeuxBalle.transform.position.y, 0);
                                            _MemoireALongTerme.Add(3);
                                            _nbCasesExplorés++;
                                            _confirmationDirection = true;
                                        }
                                        else
                                        {

                                            _gauche = false;
                                        }
                                    }
                                    else
                                    {
                                        _gauche = false;
                                    }
                                }
                                break;


                            case 4:
                                if (_droite == true)
                                {
                                    _YeuxBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x + _UniteDeDistance[0],_IntelliBalle.transform.position.y, 0);
                                    if (_YeuxBalle.transform.position.x < _coin2.transform.position.x)
                                    {
                                        RaycastHit2D hitup = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.up, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
                                        RaycastHit2D hitdown = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.down, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
                                        RaycastHit2D hitleft = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.left, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));
                                        RaycastHit2D hitright = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.right, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));

                                        if (hitup == true && hitdown == true && hitleft == true && hitright == true)
                                        {
                                            RaycastHit2D hit = Physics2D.Raycast(_IntelliBalle.transform.position, Vector2.right, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));
                                            Destroy(hit.collider.gameObject);


                                            _IntelliBalle.transform.position = new Vector3(_YeuxBalle.transform.position.x, _YeuxBalle.transform.position.y, 0);
                                            _MemoireALongTerme.Add(4);
                                            _nbCasesExplorés++;
                                            _confirmationDirection = true;
                                        }
                                        else
                                        {
                                            _droite = false;
                                        }
                                    }
                                    else
                                    {
                                        _droite = false;
                                    }
                                }
                                break;

                        }

                    }




                    //recul
                    else if (_haut == false && _bas == false && _droite == false && _gauche == false)
                    {

                        do
                        {
                            _CurseurMemoire = (_MemoireALongTerme.Count) - 1;

                            switch (_MemoireALongTerme[_CurseurMemoire])
                            {
                                case 1:
                                    _IntelliBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x, _IntelliBalle.transform.position.y - _UniteDeDistance[1], 0);

                                    _MemoireALongTerme.RemoveAt(_CurseurMemoire);
                                    break;

                                case 2:
                                    _IntelliBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x, _IntelliBalle.transform.position.y + _UniteDeDistance[1], 0);

                                    _MemoireALongTerme.RemoveAt(_CurseurMemoire);
                                    break;

                                case 3:
                                    _IntelliBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x +_UniteDeDistance[0], _IntelliBalle.transform.position.y, 0);

                                    _MemoireALongTerme.RemoveAt(_CurseurMemoire);
                                    break;

                                case 4:
                                    _IntelliBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x - _UniteDeDistance[0], _IntelliBalle.transform.position.y, 0);

                                   _MemoireALongTerme.RemoveAt(_CurseurMemoire);
                                    break;
                            }

                            _YeuxBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x, _IntelliBalle.transform.position.y + _UniteDeDistance[1], 0);
                            _verifBackHaut[0] = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.up, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
                            _verifBackHaut[1] = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.down, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
                            _verifBackHaut[2] = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.left, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));
                            _verifBackHaut[3] = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.right, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));



                            _YeuxBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x, _IntelliBalle.transform.position.y - _UniteDeDistance[1], 0);
                            _verifBackBas[0] = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.up, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
                            _verifBackBas[1] = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.down, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
                            _verifBackBas[2] = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.left, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));
                            _verifBackBas[3] = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.right, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));

                            _YeuxBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x - _UniteDeDistance[0], _IntelliBalle.transform.position.y, 0);
                            _verifBackGauche[0] = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.up, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
                            _verifBackGauche[1] = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.down, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
                            _verifBackGauche[2] = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.left, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));
                            _verifBackGauche[3] = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.right, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));

                            _YeuxBalle.transform.position = new Vector3(_IntelliBalle.transform.position.x + _UniteDeDistance[0], _IntelliBalle.transform.position.y, 0);
                            _verifBackDroite[0] = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.up, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
                            _verifBackDroite[1] = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.down, _UniteDeDistance[3], LayerMask.GetMask("DetectionMur"));
                            _verifBackDroite[2] = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.left, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));
                            _verifBackDroite[3] = Physics2D.Raycast(_YeuxBalle.transform.position, Vector2.right, _UniteDeDistance[2], LayerMask.GetMask("DetectionMur"));


                            if (IsAllTrue(_verifBackHaut) == true || IsAllTrue(_verifBackBas) == true || IsAllTrue(_verifBackGauche) == true || IsAllTrue(_verifBackDroite) == true)
                            {

                                do
                                {

                                    _rand = Random.Range(0, 4);

                                    switch (_rand)
                                    {
                                        case 0:
                                            if (IsAllTrue(_verifBackHaut) == true)
                                            {
                                                _haut = true;
                                                _confirmationRecul = true;
                                                _confirmationNEWdir = true;
                                            }
                                            else
                                            {
                                                _confirmationNEWdir = false;
                                            }
                                            break;

                                        case 1:

                                            if (IsAllTrue(_verifBackBas) == true)
                                            {
                                                _bas = true;
                                                _confirmationRecul = true;
                                                _confirmationNEWdir = true;
                                            }
                                            else
                                            {
                                                _confirmationNEWdir = false;
                                            }
                                            break;

                                        case 2:
                                            if (IsAllTrue(_verifBackGauche) == true)
                                            {
                                                _gauche = true;
                                                _confirmationRecul = true;
                                                _confirmationNEWdir = true;
                                            }
                                            else
                                            {
                                                _confirmationNEWdir = false;
                                            }
                                            break;

                                        case 3:
                                            if (IsAllTrue(_verifBackDroite) == true)
                                            {
                                                _droite = true;
                                                _confirmationRecul = true;
                                                _confirmationNEWdir = true;
                                            }
                                            else
                                            {
                                                _confirmationNEWdir = false;
                                            }
                                            break;
                                    }

                                } while (_confirmationNEWdir != true);

                            }

                            else
                            {
                                _confirmationRecul = false;
                            }




                        } while (_confirmationRecul != true);

                    }




                } while (_confirmationDirection != true);
            } while (_nbCasesExplorés != _nbCasestotales);
          
        
        }
    
    
    

        //vérifie  que l'array est true partout
        //je l'ai copié de reddit. TOO BAD ! 
        public bool IsAllTrue(bool[] collection)
        {
            for (int i = 0; i < collection.Length; i++)
                if (!collection[i])
                {
                    return false;
                }
            return true;
        }
    




}
