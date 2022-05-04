using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System.IO;
using System;
using System.Threading;
using System.Text;

public class ChessAI : MonoBehaviour
{
    public static Process myProcess;
  public static string GetBestMove(string forsythEdwardsNotationString)
        {
            var p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "E:\\VR-Chess\\stockfish_14.1_win_x64_avx2.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardError = true;
            p.Start();
            
            
            string setupString = "position startpos move " + forsythEdwardsNotationString;
            p.StandardInput.WriteLine(setupString);
            
            UnityEngine.Debug.Log(setupString);
             // Process for 5 seconds
            string processString = "go movetime 5000";

            // Process 20 deep
            // string processString = "go depth 20";

            p.StandardInput.WriteLine(processString);

            string bestMoveInAlgebraicNotation = p.StandardOutput.ReadLine();
            UnityEngine.Debug.Log(bestMoveInAlgebraicNotation);

            p.Close();
           

        return bestMoveInAlgebraicNotation;
        }

   
    public static void GetBestMove1()
    {
        string currentDir = System.IO.Directory.GetCurrentDirectory();
        
        ProcessStartInfo si = new ProcessStartInfo()
        {
            FileName = currentDir + "\\stockfish_14.1_win_x64_avx2.exe",
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardInput = true,
            RedirectStandardOutput = true
        };

        myProcess = new Process();
        myProcess.StartInfo = si;
        try
        {
            // throws an exception on win98
            
            myProcess.PriorityClass = ProcessPriorityClass.BelowNormal;
        }
        catch { }

        myProcess.OutputDataReceived += new DataReceivedEventHandler(myProcess_OutputDataReceived);

        myProcess.Start();
        myProcess.BeginErrorReadLine();
        myProcess.BeginOutputReadLine();

        SendLine("uci");
        SendLine("isready");
        SendLine("ucinewgame");
        


        //return bestMoveInAlgebraicNotation;
    }

    public static void SendLine(string command)
    {
        myProcess.StandardInput.WriteLine(command);
        myProcess.StandardInput.Flush();
    }

    private static void myProcess_OutputDataReceived(object sender, DataReceivedEventArgs e)
    {
        string text = e.Data;
        string bestmove = "bestmove";
        if (text.Contains(bestmove))
        {
            string move = text.Substring(8, 5);
            ChessBoard.allMoves += move + " ";
            ChessBoard.currentAImove = move;
            ChessBoard.myTurn = true;
            //UnityEngine.Debug.Log(move);
        }
        
    }

}
