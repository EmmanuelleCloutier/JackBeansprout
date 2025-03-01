using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    [SerializeField] private CréationLabyrinte _creationLabyrinthe = default;
    [SerializeField] private PathNode _pathNode = default;
    [SerializeField] private GameObject _pathNodeContainer = default;

    //Dimensions de la grille de noeuds
    private float _longueur = 100;
    private float _hauteur = 100; 
    private int _tailleGrille = 10; //Nombre de noeuds par rangé

    //Coordonnées du premier noeud de la grille
    private float _coordonneDepartX = 0;
    private float _coordonneDepartY = 0;

    //Taille d'une case contenant un noeud
    private float _tailleCaseX;
    private float _tailleCaseY;

    private PathNode[,] allNodes = default; //Array qui contient tous les noeuds
    private List<PathNode> openList = default; //Liste des noeuds à analyser
    private List<PathNode> closedList = default; //Liste des noeuds qui ont été analysés

    private PathNode _endNode = default; //Noeud à la fin du chemin
    private PathNode _startNode = default; //Noeud au début du chemin

    void Start()
    {
        getMazeComponents();
        genererGrille();
        
    }

    /*
     * Rôle : Copier les composantes qui ont servies à créer le labyrinthe
     * Entrée : Aucune
     * Sortie : Aucune
     */
    private void getMazeComponents()
    {
        _longueur = _creationLabyrinthe._longueur;
        _hauteur = _creationLabyrinthe._largeur;
        _tailleGrille = (int)_creationLabyrinthe._tailleGrille;
        _coordonneDepartX = _creationLabyrinthe._CoordonneDepartX;
        _coordonneDepartY = _creationLabyrinthe._CoordonneDepartY;
    }

    /*
     * Rôle : Générer la grille de noeud
     * Entrée : Aucune
     * Sortie : Aucune
     */
    private void genererGrille()
    {
        float posX = 0;
        float posY = 0;

        //On détermine la taille d'un noeud
        _tailleCaseX = _longueur / _tailleGrille; 
        _tailleCaseY = _hauteur / _tailleGrille;

        //On ajuste les valeurs des coordonnées de départ, car elles sont en fonction du coin du labyrinthe
        _coordonneDepartX += _tailleCaseX / 2;
        _coordonneDepartY += _tailleCaseY / 2;

        //À partir du nombre de noeuds, on ajuste la taille du tableau contenant tous les noeuds
        allNodes = new PathNode[_tailleGrille, _tailleGrille];

        //Tant qu'on n'a pas dépassé la taille horizontale de la grille
        while (posX < _longueur)
        {
            //Tant qu'on n'a pas dépassé la taille verticale de la grille
            while (posY < _hauteur)
            {
                //On détermine la position dans la grille où on est rendu
                Vector3 position = new Vector3(posX + _coordonneDepartX, posY + _coordonneDepartY, 0f);

                PathNode newNode = Instantiate(_pathNode, position, Quaternion.identity); //On crée un nouveau noeud
                allNodes[(int)(posX / _tailleCaseX), (int)(posY / _tailleCaseY)] = newNode; //On ajoute ce noeud dans le tableau de tous les noeuds
                newNode.setPosition(posX, posY, _tailleCaseX, _tailleCaseY); //On met le noeud à la position où on est rendu
                newNode.transform.parent = _pathNodeContainer.transform; //Dans la hierarchie, on met ce noeud dans un container de noeuds

                posY += _tailleCaseY;//On va à la position verticale suivante
            }
            posX += _tailleCaseX;//On va à la position horizontale suivante
            posY = 0;//On remet la position verticale à 0
        }
    }

    /*
     * Rôle : Calculer le coût lié à la distance entre deux noeuds
     * Entrée : 2 PathNode qui sont les noeuds dont on doit calculer la distance
     * Sortie : 1 entier qui est le coût lié à la distance
     */
    private int CalculateDistanceCost(PathNode p_a, PathNode p_b)
    {
        int distanceX = Mathf.Abs(p_a.x - p_b.x); //Calcul de la distance en x
        int distanceY = Mathf.Abs(p_a.y - p_b.y);//Calcul de la distance en y
        return distanceX + distanceY; //On retourne la somme des distances en x et en y
    }

    /*
     * Rôle : Déterminer quel noeud de la liste ouverte possède le coût f le plus faible
     * Entrée : Aucune
     * Sortie : 1 PathNode qui est le noeud avec le coût f le plus faible
     */
    private PathNode GetLowestFCostNode()
    {
        PathNode lowestFCostNode = openList[0]; //On commence avec le premier noeud de la liste

        foreach (PathNode currentNode in openList)//On regarde tous les autres noeuds
        {
            if (currentNode.fCost < lowestFCostNode.fCost) //Si le noeud a un coût f plus petit que le plus petit coût f précédent
            {
                lowestFCostNode = currentNode; //Ce noeud devient celui avec le coût f le faible
            }
        }

        return lowestFCostNode; 
    }

    /*
     * Rôle : Retracer le chemin parcouru
     * Entrée : 1 PathNode qui est le noeud final
     * Sortie : Une liste de PathNode qui est le chemin parcouru
     */
    private List<PathNode> CalculatePath(PathNode p_endNode)
    {
        //On crée une liste de PathNode qui commence par le noeud final
        List<PathNode> path = new List<PathNode>();
        path.Add(p_endNode);

        //En commençant par le noeud final
        PathNode currentNode = p_endNode;

        //Tant qu'on n'est pas arrivé au noeud initial
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode); //On retrace par quel noeud on est arrivé
            currentNode = currentNode.cameFromNode; //On passe au noeud précédent
        }

        path.Reverse(); //On inverse la liste pour que le noeud initial soit au début
        return path;
    }

    /*
     * Rôle : Déterminer les noeuds adjacents à un noeud
     * Entrée : 1 PathNode qui est le noeud dont on veut regarder les voisins
     * Sortie : 1 liste de PathNode qui contient tous les noeuds adjacents
     */
    private List<PathNode> GetNeighbourList(PathNode p_currentNode)
    {
        List<PathNode> neighbourList = new List<PathNode>();

        if (p_currentNode.DetectWallSide(-_tailleCaseX)) //Regarde s'il y a un mur à gauche
        {
            neighbourList.Add(allNodes[p_currentNode.x - 1, p_currentNode.y]); //Si la voie est libre, on ajoute le noeud qui se trouve à gauche
        }
        if (p_currentNode.DetectWallSide(_tailleCaseX)) //Regarde s'il y a un mur à droite
        {
            neighbourList.Add(allNodes[p_currentNode.x + 1, p_currentNode.y]); //Si la voie est libre, on ajoute le noeud qui se trouve à droite
        }
        if (p_currentNode.DetectWallUp(-_tailleCaseY)) //Regarde en bas
        {
            neighbourList.Add(allNodes[p_currentNode.x, p_currentNode.y - 1]); //Si la voie est libre, on ajoute le noeud qui se trouve en bas
        }
        if (p_currentNode.DetectWallUp(_tailleCaseY)) //Regarde en haut
        {
            neighbourList.Add(allNodes[p_currentNode.x, p_currentNode.y + 1]); //Si la voie est libre, on ajoute le noeud qui se trouve en haut
        }
        return neighbourList;
    }

    /*
     * Rôle : Trouver le noeud qui se trouve à une position spécifique
     * Entrée : 1 Vector2 qui la position du noeud cherché
     * Sortie : 1 PathNode qui est le noeud à la position spécifique
     */
    private PathNode GetNodeAtPosition(Vector2 p_position)
    {
        Collider2D[] nodesAtPosition = Physics2D.OverlapCircleAll(p_position, 1f);//On crée une liste de tous les objets se trouvant à la position cherchée

        foreach(Collider2D node in nodesAtPosition) //On regarde tous les objects à cette position
        {
            if (node.tag == "PathNode") //Si c'est un noeud
            {
                return node.GetComponent<PathNode>(); //On indique que c'est le noeud qu'on cherche
            }
            
        }

        //Si aucun noeud ne se trouve à cette position, on retourne le noeud final
        return null;
    }

    /*
     * Rôle : Trouver le chemin le plus court entre deux noeuds
     * Entrée : 2 float qui sont les coordonnées en x et en y du noeud de départ
     * Sortie : 1 liste de PathNode qui correspond au chemin le plus court
     */
    public List<PathNode> FindPath(float p_startX, float p_startY)
    { 
        ResetAllNodes(); //On s'assure que les coûts de tous les noeuds soient initialisés à 0

        //On détermine un noeud final et un noeud initial
        _endNode = allNodes[0, 0];

        _startNode = GetNodeAtPosition(new Vector3(p_startX, p_startY));
        

        //Initialisation des listes ouvertes et fermées
        openList = new List<PathNode> { _startNode };
        closedList = new List<PathNode>();

        //Calcule des coûts liés au noeud de départ
        _startNode.hCost = CalculateDistanceCost(_startNode, _endNode);
        _startNode.CalculateFCost();

        //Tant qu'il reste des noeuds à analyser
        while (openList.Count > 0)
        {
            //Détermine quel noeud parmi ceux à analyser à le coût f le plus petit
            PathNode currentNode = GetLowestFCostNode();

            //Si ce noeud est le noeud final
            if (currentNode == _endNode)
            {
                return CalculatePath(_endNode); //On retrace le chemin qui a mené au noeud final
            }

            //Détermine les noeuds adjacents à celui qu'on est en train d'analyser
            List<PathNode> neighbourList = GetNeighbourList(currentNode);

            foreach (PathNode neighbourNode in neighbourList) //Pour chaque noeud adjacent
            {
                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode); //Calcul du coût f

                if (closedList.Contains(neighbourNode)) //Si le noeud a déjà été analysé
                {
                    if (tentativeGCost < neighbourNode.gCost) //Si le nouveau coût f est plus petit que le précédent
                    {
                        //Changement du prédécesseur
                        neighbourNode.cameFromNode = currentNode;
                        
                        //Recalcul des coûts du noeud
                        neighbourNode.gCost = tentativeGCost;
                        neighbourNode.CalculateFCost();
                    }
                }
                else //Si c'est la première fois qu'on regarde ce noeud
                {
                    openList.Add(neighbourNode); //Ajout de ce noeud à la liste des noeuds à analyser

                    //Calcul des coûts du noeud
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, _endNode);
                    neighbourNode.CalculateFCost();

                    //Indique le prédécesseur
                    neighbourNode.cameFromNode = currentNode;
                    
                }

                openList.Remove(currentNode);//Ce noeud n'est plus à analyser
                closedList.Add(currentNode);//Indique que le noeud à déjà été analysé


            }
        }

        //Si on n'arrive pas au noeud final, on indique qu'il n'y a aucun chemin
        return null;
    }

    /*
     * Rôle : Réinitialiser les coûts des noeuds à 0
     * Entrée : 1 PathNode qui est le noeud dont on veut regarder les voisins
     * Sortie : 1 liste de PathNode qui contient tous les noeuds adjacents
     */
    public void ResetAllNodes()
    {
        foreach(PathNode node in allNodes)
        {
            node.gCost = 0;
            node.hCost = 0;
            node.fCost = 0;
            node.cameFromNode = null;
        }
    }
}

