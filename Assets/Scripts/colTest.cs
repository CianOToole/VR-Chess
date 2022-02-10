using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("pog1");
        if (other.gameObject.tag == "Gimp")
        {
            ChessBoard.findTileHit();
        }
            
    }
}
