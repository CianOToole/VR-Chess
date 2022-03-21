using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessPieceGridLocation : MonoBehaviour
{
   public static Dictionary<string, string> chessGrid = new Dictionary<string, string>();


   public static Dictionary<string, string> giveGrid()
    {
        chessGrid.Add("a1", "0,0");
        chessGrid.Add("a2", "0,1");
        chessGrid.Add("a3", "0,2");
        chessGrid.Add("a4", "0,3");
        chessGrid.Add("a5", "0,4");
        chessGrid.Add("a6", "0,5");
        chessGrid.Add("a7", "0,6");
        chessGrid.Add("a8", "0,7");

        chessGrid.Add("b1", "1,0");
        chessGrid.Add("b2", "1,1");
        chessGrid.Add("b3", "1,2");
        chessGrid.Add("b4", "1,3");
        chessGrid.Add("b5", "1,4");
        chessGrid.Add("b6", "1,5");
        chessGrid.Add("b7", "1,6");
        chessGrid.Add("b8", "1,7");

        chessGrid.Add("c1", "2,0");
        chessGrid.Add("c2", "2,1");
        chessGrid.Add("c3", "2,2");
        chessGrid.Add("c4", "2,3");
        chessGrid.Add("c5", "2,4");
        chessGrid.Add("c6", "2,5");
        chessGrid.Add("c7", "2,6");
        chessGrid.Add("c8", "2,7");

        chessGrid.Add("d1", "3,0");
        chessGrid.Add("d2", "3,1");
        chessGrid.Add("d3", "3,2");
        chessGrid.Add("d4", "3,3");
        chessGrid.Add("d5", "3,4");
        chessGrid.Add("d6", "3,5");
        chessGrid.Add("d7", "3,6");
        chessGrid.Add("d8", "3,7");

        chessGrid.Add("e1", "4,0");
        chessGrid.Add("e2", "4,1");
        chessGrid.Add("e3", "4,2");
        chessGrid.Add("e4", "4,3");
        chessGrid.Add("e5", "4,4");
        chessGrid.Add("e6", "4,5");
        chessGrid.Add("e7", "4,6");
        chessGrid.Add("e8", "4,7");

        chessGrid.Add("f1", "5,0");
        chessGrid.Add("f2", "5,1");
        chessGrid.Add("f3", "5,2");
        chessGrid.Add("f4", "5,3");
        chessGrid.Add("f5", "5,4");
        chessGrid.Add("f6", "5,5");
        chessGrid.Add("f7", "5,6");
        chessGrid.Add("f8", "5,7");

        chessGrid.Add("g1", "6,0");
        chessGrid.Add("g2", "6,1");
        chessGrid.Add("g3", "6,2");
        chessGrid.Add("g4", "6,3");
        chessGrid.Add("g5", "6,4");
        chessGrid.Add("g6", "6,5");
        chessGrid.Add("g7", "6,6");
        chessGrid.Add("g8", "6,7");

        chessGrid.Add("h1", "7,0");
        chessGrid.Add("h2", "7,1");
        chessGrid.Add("h3", "7,2");
        chessGrid.Add("h4", "7,3");
        chessGrid.Add("h5", "7,4");
        chessGrid.Add("h6", "7,5");
        chessGrid.Add("h7", "7,6");
        chessGrid.Add("h8", "7,7");

        return chessGrid;
    }
}
