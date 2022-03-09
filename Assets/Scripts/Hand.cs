using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
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

    //hold reference to gamemanager instance
    private GameManager gm;

    //quiver hand tracking flags
    private bool handInQuiver;
    private bool quiverArrow;

    private void Start()
    {
        //set references to xr components
        xrController = GetComponent<XRBaseInteractor>();
        lineVisual = GetComponent<XRInteractorLineVisual>();

        //local ref to GM
        gm = GameManager.instance;
    }

    private void Update()
    {
        //manually start and end instantiated arrow interactions in update
        if(handInQuiver && heldObject == null)
        {
            bool pressed;
            GetComponent<XRController>().inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out pressed); //manually getting grip button

            if (pressed) //spawn an arrow and handle xr interaction
            {
                quiverArrow = true;
                IXRSelectInteractable arrowGI = Instantiate(gm.arrowPrefab);
                xrController.StartManualInteraction(arrowGI);
                heldObject = arrowGI as XRGrabInteractable;
                lineVisual.enabled = false;
            }
        }
        else if(quiverArrow) //if currently grabbing a spawned quiver arrow
        {
            bool pressed;
            GetComponent<XRController>().inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out pressed); //manually getting grip button

            if(!pressed)
            {
                //end the interaction and stop tracking the arrow
                quiverArrow = false;
                xrController.EndManualInteraction();
                heldObject = null;
                lineVisual.enabled = true;
            }
        }
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
            //disable the nock point collider so it doesn't interfere with shot physics
            other.enabled = false;

            //try to nock an arrow
            gm.TryNock(this);
        }
        else if(other.tag.Equals("Quiver"))
        {
            //if hand entering quiver then send haptic vibration to let the player know they have reached the quiver, and set handInQuiver flag for update
            handInQuiver = true;
            try
            {
                (xrController as XRBaseControllerInteractor).SendHapticImpulse(0.5f, 0.1f);
            }
            catch(Exception e)
            {
                Debug.Log("Issue sending Haptic Impulse: " + e.Message);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if hand exiting quiver, unset the flag so update isn't tracking
        if (other.tag.Equals("Quiver"))
        {
            handInQuiver = false;
        }
    }
}
