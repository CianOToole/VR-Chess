using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Holding : XRGrabInteractable
{
    GameObject tile;

    protected override void Awake()
    {
        base.Awake();
       tile = GameObject.Find(this.name);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
       
        if (isSelected)
        {
            ChessBoard.currentlyDragging = this.gameObject;
            //Gets list of where I can go and highlights the tiles.
            ChessBoard.availableMoves = ChessBoard.currentlyDragging.GetComponent<ChessPiece>().GetAvailableMoves(ref ChessBoard.chessPieces, ChessBoard.TILE_COUNT_X, ChessBoard.TILE_COUNT_Y);
            ChessBoard.HighlightTiles();
        }
    }


    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (!isSelected)
        {
            ChessBoard.findTileHit(this.gameObject);
            // Debug.Log(someGameObject + "isn't Selected");
            ChessBoard.RemoveHighlightTiles();
        }
    }


}
