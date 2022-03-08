using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Bow : XRGrabInteractable
{
    //the middle of the bowstring
    public Transform nockPoint;

    //string Hand for positioning/rotating
    [HideInInspector]
    public Transform stringHand;

    //for rotating the bow relative to both hands
    private Quaternion attachIntialRotation;

    //for bowstring handling after shot
    private Vector3 originalNockPosition;
    private Transform originalNockParent;

    //arrow position is tracked if there is an arrow
    private Transform arrow;

    //position arrow should snap to when nocked
    private Vector3 shelfedArrowPosition = new Vector3(-0.44f, 0f, 0f);

    private void Start()
    {
        //grab resting bowstring position
        originalNockPosition = nockPoint.transform.localPosition;
        originalNockParent = nockPoint.transform.parent;
    }

    private void Update()
    {
        //after an arrow is nocked and before a shot
        if(stringHand)
        {
            //pull the string
            nockPoint.transform.position = stringHand.transform.position;

            //update arrow position based on the string pull distance
            float distance = Vector3.Distance(nockPoint.transform.position, transform.position);
            arrow.localPosition = new Vector3(distance - 0.65f, 0f, 0f);
        }
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {   
        //handle bow rotating based on position of both hands
        if(stringHand && interactorsSelecting.Count > 0)
        {
            GetInteractor().attachTransform.rotation = GetTwoHandRotation();
        }
        base.ProcessInteractable(updatePhase);
    }

    private Quaternion GetTwoHandRotation()
    {
        //get quaternion based on position of both hands
        if (interactorsSelecting.Count > 0)
        {
            return Quaternion.LookRotation(GetInteractor().attachTransform.position - stringHand.position, GetInteractor().transform.up);
        }
        return new Quaternion();
    }
    
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        //when bow is picked up, set the initial rotation offset of the attach object
        attachIntialRotation = (args.interactorObject as XRBaseInteractor).attachTransform.localRotation;
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        //drop the bow
        ReleaseString();

        //set attach object back to original rotation
        (args.interactorObject as XRBaseInteractor).attachTransform.localRotation = attachIntialRotation;
        base.OnSelectExited(args);
    }

    private XRBaseInteractor GetInteractor()
    {
        //get the first hand that grabbed the bow
        return interactorsSelecting.Count > 0 ? interactorsSelecting[0] as XRBaseInteractor : null;
    }

    public void Nock(Hand arrowHand)
    {
        //reference to grab interactable
        XRGrabInteractable arrowInteractable = arrowHand.heldObject;

        //set the interactable tracking off so that we can manually position and rotate the arrow respective to the bow
        arrowInteractable.trackRotation = false;
        arrowInteractable.trackPosition = false;

        //parent the arrow to the bow and reset its transform to follow the bow
        arrow = arrowInteractable.transform;
        arrow.parent = transform;
        arrow.localPosition = shelfedArrowPosition;
        arrow.localRotation = Quaternion.identity;

        //child the nockPoint to the hand that will pull the string
        nockPoint.transform.SetParent(arrowHand.transform);
        nockPoint.transform.localPosition = Vector3.zero;

        //stringHand tracker
        stringHand = arrowHand.transform;
    }

    public void Shoot(Rigidbody arrowRB)
    {
        //release the string
        ReleaseString();

        //unset the arrow local tracking variables
        XRGrabInteractable arrow = arrowRB.GetComponent<XRGrabInteractable>();
        arrow.transform.parent = null;
        arrow.trackRotation = true;
        arrow.trackPosition = true;

        //add the force that shoots the arrow
        arrowRB.AddForce(arrowRB.transform.right * -1500);
    }

    private void ReleaseString()
    {
        //child the nockPoint back to the bow object
        nockPoint.transform.SetParent(originalNockParent);
        nockPoint.transform.localPosition = originalNockPosition;

        //stringHand tracker
        stringHand = null;
        arrow = null;
    }
}
