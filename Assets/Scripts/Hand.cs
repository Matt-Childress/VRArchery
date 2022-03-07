using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Hand : MonoBehaviour
{
    //left or right hand
    public HandSide side;

    //the object held by this hand
    public HeldObject heldObject;

    //reference to the xr controller
    private XRBaseInteractor xrController;
    //reference to the xr pointer
    private XRInteractorLineVisual lineVisual;

    private void Start()
    {
        //set references to xr components
        xrController = GetComponent<XRBaseInteractor>();
        lineVisual = GetComponent<XRInteractorLineVisual>();
    }

    public void GrabbedObject()
    {
        //handle an object being grabbed
        heldObject = GetHeldObject();
        lineVisual.enabled = false; //turn off the pointer line if an object is in hand
    }

    public void DroppedObject()
    {
        //handle an object being dropped
        heldObject = null;
        lineVisual.enabled = true; //if the hand has no object, turn on the pointer line
    }

    private HeldObject GetHeldObject()
    {
        //retrieve the object being held or null if no object is held
        List<IXRSelectInteractable> interactables = xrController.interactablesSelected;
        if(interactables.Count > 0)
        {
            return interactables[0].transform.GetComponent<HeldObject>();
        }

        return null;
    }
}
