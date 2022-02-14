using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colTest : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hand")
        {
            //ChessBoard.findTileHit();
            GameObject someGameObject = GameObject.Find(this.name);
            someGameObject.layer = LayerMask.NameToLayer("Hover");
        }
            
    }

    private void OnTriggerExit(Collider other)
    {
        GameObject someGameObject = GameObject.Find(this.name);
        someGameObject.layer = LayerMask.NameToLayer("Tile");
    }
}
