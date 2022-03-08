using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Hand : MonoBehaviour
{
    //the object held by this hand
    [HideInInspector]
    public XRGrabInteractable heldObject;

    //reference to the xr controller
    [HideInInspector]
    public XRBaseInteractor xrController;
    //reference to the xr pointer
    private XRInteractorLineVisual lineVisual;

    private GameManager gm;

    private void Start()
    {
        //set references to xr components
        xrController = GetComponent<XRBaseInteractor>();
        lineVisual = GetComponent<XRInteractorLineVisual>();

        //local ref to GM
        gm = GameManager.instance;
    }

    public void GrabbedObject()
    {
        //handle an object being grabbed
        heldObject = GetHeldObject();
        lineVisual.enabled = false; //turn off the pointer line if an object is in hand
    }

    public void DroppedObject()
    {
        //see if a shot should be performed
        if(!(heldObject is Bow))
        {
            gm.TryShoot(this);
        }

        //handle an object being dropped
        heldObject = null;
        lineVisual.enabled = true; //if the hand has no object, turn on the pointer line
    }

    private XRGrabInteractable GetHeldObject()
    {
        //retrieve the object being held or null if no object is held
        List<IXRSelectInteractable> interactables = xrController.interactablesSelected;
        if(interactables.Count > 0)
        {
            return interactables[0] as XRGrabInteractable;
        }

        return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        //handling when an arrow is placed on the string
        if (other.tag.Equals("NockPoint") && heldObject != null && !(heldObject is Bow))
        {
            gm.TryNock(this);
        }
    }
}
