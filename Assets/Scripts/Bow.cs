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
   
    private void Start()
    {
        //grab resting bowstring position
        originalNockPosition = nockPoint.transform.localPosition;
        originalNockParent = nockPoint.transform.parent;
    }

    private void Update()
    {
        //after an arrow is nocked and before a shot, pull the string
        if(stringHand)
        {
            nockPoint.transform.position = stringHand.transform.position;
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

    public void Nock(Transform newParent)
    {
        //child the nockPoint to the hand that will pull the string
        nockPoint.transform.SetParent(newParent);
        nockPoint.transform.localPosition = Vector3.zero;

        //stringHand tracker
        stringHand = newParent;
    }

    public void Shoot(Rigidbody arrowRB)
    {
        //release the string and add the force that shoots the arrow
        ReleaseString();
        arrowRB.AddForce(arrowRB.transform.right * -1000);
    }

    private void ReleaseString()
    {
        //child the nockPoint back to the bow object
        nockPoint.transform.SetParent(originalNockParent);
        nockPoint.transform.localPosition = originalNockPosition;

        //stringHand tracker
        stringHand = null;
    }
}
