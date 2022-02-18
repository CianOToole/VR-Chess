using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hand")
        {
            
            GameObject someGameObject = GameObject.Find(this.name);
            ChessBoard.currentHover = ChessBoard.lookupTileIndex(someGameObject);
            someGameObject.layer = LayerMask.NameToLayer("Hover");
        }
            
    }

    private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.tag == "Hand")
        {
            GameObject someGameObject = GameObject.Find(this.name);
           
            someGameObject.layer = LayerMask.NameToLayer("Tile");
        }
            
    }
}
