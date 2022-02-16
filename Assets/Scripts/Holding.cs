using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Holding : XRGrabInteractable
{
    GameObject someGameObject;

    protected override void Awake()
    {
        base.Awake();
        someGameObject = GameObject.Find(this.name);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
       
        if (isSelected)
        {
            Debug.Log(someGameObject + "is Selected");
        }
    }


    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (!isSelected)
        {
            Debug.Log(someGameObject + "isn't Selected");
        }
    }


}
