using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public enum SpecialMove
{
    None = 0,
    EnPassant,
    Castling,
    Promotion
}

public class ChessBoard : MonoBehaviour
{

    [Header("Art stuff")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private static float tileSize = 0.054f;
    [SerializeField] private static float yOffset = 1f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;
    [SerializeField] private GameObject leftHand;

    [Header("Prefabs & Materials")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] private Material[] teamMaterials;

    [Header("UI")]
    [SerializeField] public GameObject[] winLossUI;

    //LOGIC
    public static ChessPiece[,] chessPieces;
    public static GameObject currentlyDragging;
    public static int winLoss = 3;
    public static List<Vector2Int> availableMoves = new List<Vector2Int>();
    private static List<ChessPiece> deadWhites = new List<ChessPiece>();
    private static List<ChessPiece> deadBlacks = new List<ChessPiece>();
    
    public const int TILE_COUNT_X = 8;
    public const int TILE_COUNT_Y = 8;
    private static GameObject[,] tiles;
    private Camera currentCamera;
    public static Vector2Int currentHover;
    private static Vector3 bounds;
    private static Dictionary<string, string> gridSetter;
    public static bool myTurn;
    public static string currentAImove;
    public static string allMoves;
    public static SpecialMove specialMove;
    public static List<Vector2Int[]> moveList = new List<Vector2Int[]>();
    


    private void Awake()
    {
        myTurn = false;
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPieces();
        setPieceLocation();
        ChessAI.GetBestMove1();
        
        

    }

    private void Update()
    {
        if (!currentCamera) 
        {
            currentCamera = Camera.main;
          
            return;
        }

        if (myTurn)
        {
           
            AImove(currentAImove);
            myTurn = false;
        }

        if (winLoss == 0)
        {
            winLossUI[0].SetActive(true);
        }

        if (winLoss == 1)
        {
            winLossUI[1].SetActive(true);
        }
    }

    //Generating the Board
    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY)
    {
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX / 2) * tileSize, 0, (tileCountX / 2) * tileSize) + boardCenter;


        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
            for (int y = 0; y < tileCountY; y++)
                tiles[x, y] = GenerateSingleTile(tileSize, x, y);
        
    }
    private GameObject GenerateSingleTile(float tileSize, int x, int y)
    {
        GameObject tileObject = new GameObject(string.Format("X{0} Y{1}", x,y));
        tileObject.transform.parent = transform;
        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x * tileSize, yOffset, y * tileSize) - bounds;
        vertices[1] = new Vector3(x * tileSize, yOffset, (y+1) * tileSize) - bounds;
        vertices[2] = new Vector3((x+1) * tileSize, yOffset, y * tileSize) - bounds;
        vertices[3] = new Vector3((x + 1) * tileSize, yOffset, (y + 1) * tileSize) - bounds;

        int[] tris = new int[] { 0, 1, 2, 1, 3, 2 };

        mesh.vertices = vertices;
        mesh.triangles = tris;

        mesh.RecalculateNormals();

        
        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>().size = new Vector3(0.01f, 1f, 0.01f); ;
        tileObject.GetComponent<BoxCollider>().isTrigger = true;
        tileObject.AddComponent<colTest>();
        //tileObject.AddComponent<XRSocketInteractor>();
        

        return tileObject;
    }

    // Spawning of the pieces
    private void SpawnAllPieces()
    {
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

        int whiteTeam = 0, blackTeam = 1;

        //White
        chessPieces[0, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);
        chessPieces[1, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
        chessPieces[2, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[3, 0] = SpawnSinglePiece(ChessPieceType.Queen, whiteTeam);
        chessPieces[4, 0] = SpawnSinglePiece(ChessPieceType.King, whiteTeam);
        chessPieces[5, 0] = SpawnSinglePiece(ChessPieceType.Bishop, whiteTeam);
        chessPieces[6, 0] = SpawnSinglePiece(ChessPieceType.Knight, whiteTeam);
       chessPieces[7, 0] = SpawnSinglePiece(ChessPieceType.Rook, whiteTeam);

        for (int i = 0; i < TILE_COUNT_X; i++)
            chessPieces[i, 1] = SpawnSinglePiece(ChessPieceType.Pawn, whiteTeam);

        //Black
        chessPieces[0, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);
        chessPieces[1, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[2, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[3, 7] = SpawnSinglePiece(ChessPieceType.Queen, blackTeam);
        chessPieces[4, 7] = SpawnSinglePiece(ChessPieceType.King, blackTeam);
        chessPieces[5, 7] = SpawnSinglePiece(ChessPieceType.Bishop, blackTeam);
        chessPieces[6, 7] = SpawnSinglePiece(ChessPieceType.Knight, blackTeam);
        chessPieces[7, 7] = SpawnSinglePiece(ChessPieceType.Rook, blackTeam);

        for (int i = 0; i < TILE_COUNT_X; i++)
            chessPieces[i, 6] = SpawnSinglePiece(ChessPieceType.Pawn, blackTeam);
        


    }
    private ChessPiece SpawnSinglePiece(ChessPieceType type, int team)
    {
        ChessPiece cp = Instantiate(prefabs[(int)type - 1], transform).GetComponent<ChessPiece>();

        cp.type = type;
        cp.team = team;
        cp.GetComponent<MeshRenderer>().material = teamMaterials[team];
        
        Physics.IgnoreLayerCollision(10, 10);
        return cp;
    }

    // Positioning
    private void PositionAllPieces()
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (chessPieces[x, y] != null)
                    PositionSinglePiece(x, y, true);
    }

    private static void PositionSinglePiece(int x, int y, bool force = false)
    {
        try
        {
            chessPieces[x, y].currentX = x;
            chessPieces[x, y].currentY = y;
            chessPieces[x, y].transform.position = GetTileCenter(x, y);
            chessPieces[x, y].transform.rotation = Quaternion.Euler(-90, 0, 0);
        } catch (Exception e)
        {
            Debug.Log(e);
        }

        

    }
    private static Vector3 GetTileCenter(int x, int y)
    {
        return new Vector3(x * tileSize, yOffset, y * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2);
    }

    //Operations
    public static Vector2Int lookupTileIndex(GameObject hitInfo)
    {
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (tiles[x, y] == hitInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one;
            
        
    }
    
     private static bool MoveTo(ChessPiece cp, int x, int y, bool NotaiMove)
    {
        if (NotaiMove) { 
            if (!ContainsValidMove(ref availableMoves, new Vector2Int(x, y)))
            {
                return false;
            }
        }
        
        Vector2Int previousPosition = new Vector2Int(cp.currentX, cp.currentY);

        if (chessPieces[x, y] != null)
        {
            
            ChessPiece ocp = chessPieces[x, y];
            if (cp.team == ocp.team)
            {
                return false;
            }

            if(ocp.team == 0)
            {
                //if (ocp.type == ChessPieceType.King)
                    //CheckMate(1);

                deadWhites.Add(ocp);
                ocp.transform.position =(new Vector3(8 * tileSize, yOffset, -1 * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize/2) + (Vector3.forward * 0.03f) * deadWhites.Count);
                //ocp.transform.localScale = Vector3.Lerp(ocp.transform.localScale, Vector3.one * 0.3f, Time.deltaTime * 1);
            }
            else
            {
                //if (ocp.type == ChessPieceType.King)
                    //CheckMate(0);

                deadBlacks.Add(ocp);
                ocp.transform.position = (new Vector3(-1 * tileSize, yOffset, 8 * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2) + (Vector3.back * 0.03f) * deadBlacks.Count);
            }
        }

        chessPieces[x, y] = cp;
        if (NotaiMove)
        {
            string pastPlace = cp.gridSpot;
            cp.gridSpot = GetKeyFromValue(x.ToString() + "," + y.ToString());
            allMoves += pastPlace + cp.gridSpot + " ";
            ChessAI.SendLine("position startpos move " + allMoves);
            ChessAI.SendLine("go movetime 5000");

        }
        cp.gridSpot = GetKeyFromValue(x.ToString() + "," + y.ToString());
        chessPieces[previousPosition.x, previousPosition.y] = null;
        PositionSinglePiece(x, y);
        moveList.Add(new Vector2Int[] { previousPosition, new Vector2Int(x, y) });

        ProcessSpecialMove();
        if (CheckForCheckmate())
        {
            CheckMate(cp.team);
        }

        return true;
    }
    public static bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos)
    {
        for (int i = 0; i < moves.Count; i++)
            if (moves[i].x == pos.x && moves[i].y == pos.y)
                return true;

        return false;
    }
    
    //used when a user moves a chess piece to call the MoveTo function
    public static void findTileHit(GameObject piece)
 {
        
     currentlyDragging = piece;
     ChessPiece currentPiece = currentlyDragging.GetComponent<ChessPiece>();
     Vector2Int previousPosition = new Vector2Int(currentPiece.currentX, currentPiece.currentY);
     bool validMove = MoveTo(currentPiece, currentHover.x, currentHover.y, true);
     if (!validMove)
     {
         currentlyDragging.transform.position = GetTileCenter(previousPosition.x, previousPosition.y);
         currentlyDragging.transform.rotation = Quaternion.Euler(-90, 0, 0);
         currentlyDragging = null;
     }

 }

    public static void AImove(string move)
    {
        string chessPieceToMove = move.Substring(3, 2);
        string chessPieceAt = move.Substring(1, 2);
        //Debug.Log(move);
        if (gridSetter.ContainsKey(chessPieceToMove))
        {

            string passThis = gridSetter[chessPieceToMove];

            string getThis = gridSetter[chessPieceAt];

            //location the piece has to go
            string xS = passThis.Substring(0, 1);
            string yS = passThis.Substring(2, 1);

            //The piece we want to move
            string xSt = getThis.Substring(0, 1);
            string ySt = getThis.Substring(2, 1);


            int x, y, b, f;
            //location the piece has to go
            x = Int32.Parse(xS);
            y = Int32.Parse(yS);

            //The piece we want to move
            b = Int32.Parse(xSt);
            f = Int32.Parse(ySt);


            ChessPiece currentPieceGet = chessPieces[b, f];
            GameObject currentTileGet = tiles[x, y];
            currentlyDragging = currentPieceGet.gameObject;
            availableMoves = currentlyDragging.GetComponent<ChessPiece>().GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
            specialMove = currentlyDragging.GetComponent<ChessPiece>().GetSpecialMoves(ref chessPieces, ref moveList, ref availableMoves);
            Vector2Int tile = lookupTileIndex(currentTileGet.gameObject);
            bool validMove = MoveTo(currentPieceGet, tile.x, tile.y, false);
            //Debug.Log(validMove + "the move should work");

        }
    }

    public static void setPieceLocation()
    {
        gridSetter = ChessPieceGridLocation.giveGrid();
        string xString;
        string yString;
        string xyTogether;
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if (chessPieces[x, y] != null) { 
                    xString = chessPieces[x, y].currentX.ToString();
                    yString = chessPieces[x, y].currentY.ToString();
                    xyTogether = xString + "," + yString;
                    //Debug.Log(gridSetter.ContainsValue(""));
                    if (gridSetter.ContainsValue(xyTogether))
                    {
                        chessPieces[x, y].gridSpot = GetKeyFromValue(xyTogether);
                       // Debug.Log(chessPieces[x, y].gridSpot + chessPieces[x, y].type);
                    }
                }
            }


        

    }

    public static string GetKeyFromValue(string valueVar)
    {
        foreach (string keyVar in gridSetter.Keys)
        {
            if (gridSetter[keyVar] == valueVar)
            {
                return keyVar;
            }
        }
        return null;
    }

    //Highlight Tiles
    public static void HighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
        
    }

    public static void RemoveHighlightTiles()
    {
        for (int i = 0; i < availableMoves.Count; i++)
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");

        availableMoves.Clear();
    }

    //this 
    private static void CheckMate(int team)
    {
        DisplayVictory(team);
    }

    //this 
    private static void DisplayVictory(int winningTeam)
    {
        
        if (winningTeam == 0)
        {
            //GameObject win = GameObject.Find("ChessWin");
            //winLossUI[0].SetActive(true);
            //win.transform.position = new Vector3(-0.749f, -1.101f, -25.367f);
            winLoss = 0;
        }

        if (winningTeam == 1)
        {
            //GameObject loss = GameObject.Find("ChessLoss");
            //winLossUI[1].SetActive(true);
            
            winLoss = 1;
            //loss.transform.position = new Vector3(-0.749f, -1.101f, -25.367f);
        }
    }

    //this 
    private static void ProcessSpecialMove()
    {
        var test = new ChessBoard();
        if(specialMove == SpecialMove.EnPassant)
        {
            
            var newMove = moveList[moveList.Count - 1];
            ChessPiece myPawn = chessPieces[newMove[1].x, newMove[1].y];
            var targetPawnPosition = moveList[moveList.Count - 2];
            ChessPiece enemyPawn = chessPieces[targetPawnPosition[1].x, targetPawnPosition[1].y];

            if (myPawn.currentX == enemyPawn.currentX)
            {
                if(myPawn.currentY == enemyPawn.currentY - 1 || myPawn.currentY == enemyPawn.currentY + 1)
                {
                    if(enemyPawn.team == 0)
                    {
                        deadWhites.Add(enemyPawn);
                        enemyPawn.transform.position = (new Vector3(8 * tileSize, yOffset, -1 * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2) + (Vector3.forward * 0.03f) * deadWhites.Count);
                    }
                    else
                    {
                        deadBlacks.Add(enemyPawn);
                        enemyPawn.transform.position = (new Vector3(-1 * tileSize, yOffset, 8 * tileSize) - bounds + new Vector3(tileSize / 2, 0, tileSize / 2) + (Vector3.back * 0.03f) * deadBlacks.Count);
                    }
                    chessPieces[enemyPawn.currentX, enemyPawn.currentY] = null;
                }
            }
        }

        if(specialMove == SpecialMove.Promotion)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];
            ChessPiece targetPawn = chessPieces[lastMove[1].x, lastMove[1].y];

            if(targetPawn.type == ChessPieceType.Pawn)
            {
                if(targetPawn.team == 0 && lastMove[1].y == 7)
                {
                    ChessPiece newQueen = test.SpawnSinglePiece(ChessPieceType.Queen, 0);
                    Destroy(chessPieces[lastMove[1].x, lastMove[1].y].gameObject);
                    chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
                    PositionSinglePiece(lastMove[1].x, lastMove[1].y);
                }

                if (targetPawn.team == 1 && lastMove[1].y == 0)
                {
                    ChessPiece newQueen = test.SpawnSinglePiece(ChessPieceType.Queen, 1);
                    Destroy(chessPieces[lastMove[1].x, lastMove[1].y].gameObject);
                    chessPieces[lastMove[1].x, lastMove[1].y] = newQueen;
                    PositionSinglePiece(lastMove[1].x, lastMove[1].y);
                }
            }
        }

        if(specialMove == SpecialMove.Castling)
        {
            Vector2Int[] lastMove = moveList[moveList.Count - 1];

            //Left Rook
            if(lastMove[1].x == 2)
            {
                if(lastMove[1].y == 0) // White side
                {
                    ChessPiece rook = chessPieces[0, 0];
                    chessPieces[3, 0] = rook;
                    PositionSinglePiece(3, 0);
                    chessPieces[0, 0] = null;
                }
                else if(lastMove[1].y == 7) //Black side
                {
                    ChessPiece rook = chessPieces[0, 7];
                    chessPieces[3, 7] = rook;
                    PositionSinglePiece(3, 7);
                    chessPieces[0, 7] = null;
                }
            }

            //Right Rook
            else if (lastMove[1].x == 6)
            {
                if (lastMove[1].y == 0) // White side
                {
                    ChessPiece rook = chessPieces[7, 0];
                    chessPieces[5, 0] = rook;
                    PositionSinglePiece(5, 0);
                    chessPieces[7, 0] = null;
                }
                else if (lastMove[1].y == 7) //Black side
                {
                    ChessPiece rook = chessPieces[7, 7];
                    chessPieces[5, 7] = rook;
                    PositionSinglePiece(5, 7);
                    chessPieces[7, 7] = null;
                }
            }
        }
    }

    //this 
    public static void PreventCheck()
    {
        ChessPiece targetKing = null;
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (chessPieces[x, y] != null)
                    if (chessPieces[x, y].type == ChessPieceType.King)
                        if (chessPieces[x, y].team == currentlyDragging.GetComponent<ChessPiece>().team)
                            targetKing = chessPieces[x, y];

        //Removing move that will put the user in check
        SimulateMoveForSinglePiece(currentlyDragging.GetComponent<ChessPiece>(), ref availableMoves, targetKing);
    }

    //this 
    private static void SimulateMoveForSinglePiece(ChessPiece cp, ref List<Vector2Int> moves, ChessPiece targetKing)
    {
        // Save the current values, to reset after the function call
        int actualX = cp.currentX;
        int actualY = cp.currentY;
        List<Vector2Int> movesToRemove = new List<Vector2Int>();

        // Going through all the moves, simualte them and check if we're in check
        for (int i = 0; i < moves.Count; i++)
        {
            int simX = moves[i].x;
            int simY = moves[i].y;

            Vector2Int kingPositionThisSim = new Vector2Int(targetKing.currentX, targetKing.currentY);
            //Did we simulate the king's move
            if (cp.type == ChessPieceType.King)
                kingPositionThisSim = new Vector2Int(simX, simY);

            ChessPiece[,] simulation = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];
            List<ChessPiece> simAttackingPieces = new List<ChessPiece>();
            for (int x = 0; x < TILE_COUNT_X; x++)
            {
                for (int y = 0; y < TILE_COUNT_Y; y++)
                {
                    if (chessPieces[x, y] != null)
                    {
                        simulation[x, y] = chessPieces[x, y];
                        if (simulation[x, y].team != cp.team)
                            simAttackingPieces.Add(simulation[x, y]);
                    }

                }
            }

            //Simulate that move
            simulation[actualX, actualY] = null;
            cp.currentX = simX;
            cp.currentY = simY;
            simulation[simX, simY] = cp;

            //Did one of the piece got taken down during our simulation
            var deadPiece = simAttackingPieces.Find(c => c.currentX == simX && c.currentY == simY);
            if (deadPiece != null)
                simAttackingPieces.Remove(deadPiece);

            //Get all the simulated attacking pieces moves
            List<Vector2Int> simMoves = new List<Vector2Int>();
            for (int a = 0; a < simAttackingPieces.Count; a++)
            {
                var pieceMoves = simAttackingPieces[a].GetAvailableMoves(ref simulation, TILE_COUNT_X, TILE_COUNT_Y);
                for (int b = 0; b < pieceMoves.Count; b++)
                    simMoves.Add(pieceMoves[b]);
            }

            //Is the king in trouble? if so, remove the move
            if(ContainsValidMove(ref simMoves, kingPositionThisSim))
            {
                movesToRemove.Add(moves[i]);
            }

            //Restore the actual CP data
            cp.currentX = actualX;
            cp.currentY = actualY;
        }


            // Remove from the current available move list
            for (int i = 0; i < movesToRemove.Count; i++)
            {
                moves.Remove(movesToRemove[i]);
            }
        

    }

    //this 
    private static bool CheckForCheckmate()
    {
        var lastMove = moveList[moveList.Count - 1];
        int targetTeam = (chessPieces[lastMove[1].x, lastMove[1].y].team == 0) ? 1 : 0;

        List<ChessPiece> attackingPieces = new List<ChessPiece>();
        List<ChessPiece> defendingPieces = new List<ChessPiece>();
        ChessPiece targetKing = null;
        for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if (chessPieces[x, y] != null)
                {
                    if (chessPieces[x, y].team == targetTeam)
                    {
                        defendingPieces.Add(chessPieces[x, y]);
                        if (chessPieces[x, y].type == ChessPieceType.King)
                            targetKing = chessPieces[x, y];
                    }
                    else
                    {
                        attackingPieces.Add(chessPieces[x, y]);
                    }
                }

        // Is the king attacked right now
        List<Vector2Int> currentAvailableMoves = new List<Vector2Int>();
        for (int i = 0; i < attackingPieces.Count; i++)
        {
            var pieceMoves = attackingPieces[i].GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
            for (int b = 0; b < pieceMoves.Count; b++)
                currentAvailableMoves.Add(pieceMoves[b]);
        }

        //Are we in check
        if(ContainsValidMove(ref currentAvailableMoves, new Vector2Int(targetKing.currentX, targetKing.currentY)))
        {
            //king is under attack, can a piece be moved to help
            for (int i = 0; i < defendingPieces.Count; i++)
            {
                List<Vector2Int> defendingMoves = defendingPieces[i].GetAvailableMoves(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                //Since we're sending ref availableMoves, we will be deleting moves that are putting us in check
                SimulateMoveForSinglePiece(defendingPieces[i], ref defendingMoves, targetKing);

                if (defendingMoves.Count != 0)
                    return false;
            }

            return true; //Checkmate exit
        }



        return false;
    }
}
