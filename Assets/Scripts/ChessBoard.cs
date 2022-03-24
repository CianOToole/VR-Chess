using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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
    //LOGIC
    public static ChessPiece[,] chessPieces;
    public static GameObject currentlyDragging;
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
    public static int bruh;

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
            bruh = 1;
            AImove(currentAImove);
            myTurn = false;
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
            if (!ContainsValidMove(ref availableMoves, new Vector2(x, y)))
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
                Vector2Int tile = lookupTileIndex(currentTileGet.gameObject);
                bool validMove = MoveTo(currentPieceGet, tile.x, tile.y, false);
                Debug.Log(validMove + "the move should work");

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

    private void CheckMate(int team)
    {
        DisplayVictory(team);
    }

    private void DisplayVictory(int winningTeam)
    {

    }
}
