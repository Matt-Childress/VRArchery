using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Bow : XRGrabInteractable
{
    //the middle of the bowstring
    public Collider nockPoint;

    //string Hand for positioning/rotating
    [HideInInspector]
    public Transform stringHand;

    //the bow's collider
    private Collider bowCollider;

    //for rotating the bow relative to both hands
    private Quaternion attachIntialRotation;

    //for bowstring handling after shot
    private Vector3 originalNockPosition;
    private Transform originalNockParent;

    //arrow position is tracked if there is an arrow
    private Transform arrow;

    //position arrow should snap to when nocked
    private Vector3 shelfedArrowPosition = new Vector3(-0.44f, 0f, 0f);

    //constant base arrow power (negative for velocity direction)
    private const float baseArrowPower = -3000f;

    //constant distance that arrow should remain from the nock
    private const float nockedArrowDistanceOffset = 0.65f;
    
    //track the distance the bow is pulled back
    private float drawLength;

    private void Start()
    {
        //grab bow collider reference
        bowCollider = GetComponent<Collider>();

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
            drawLength = Vector3.Distance(nockPoint.transform.position, transform.position);
            arrow.localPosition = new Vector3(drawLength - nockedArrowDistanceOffset, 0f, 0f);
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
        //disable the bow collider so it doesn't interfere with arrow shots
        bowCollider.enabled = false;

        //when bow is picked up, set the initial rotation offset of the attach object
        attachIntialRotation = (args.interactorObject as XRBaseInteractor).attachTransform.localRotation;
        base.OnSelectEntered(args);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        //re enable the bow collider so it can be picked up
        bowCollider.enabled = true;

        //drop the bow
        ReleaseString();

        //reenable the nock point so arrows can be nocked
        nockPoint.enabled = true;

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
        XRGrabInteractable arrowGI = arrowHand.heldObject;

        //set the interactable tracking off so that we can manually position and rotate the arrow respective to the bow
        arrowGI.trackRotation = false;
        arrowGI.trackPosition = false;

        //parent the arrow to the bow and reset its transform to follow the bow
        arrow = arrowGI.transform;
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

        //calculate the shot force based on draw length
        float shotPower = drawLength * baseArrowPower;

        //zero out arrow's forces for a stable shot
        arrowRB.velocity = Vector3.zero;
        arrowRB.angularVelocity = Vector3.zero;

        //set arrow not kinematic so shot physics work
        arrowRB.isKinematic = false;

        //add the force in the direction to shoot the arrow
        arrowRB.AddForce(arrowRB.transform.right * shotPower);

        //reenable the nock point so arrows can be nocked
        nockPoint.enabled = true;
    }

    private void ReleaseString()
    {
        //if there's an arrow on the string, unset the arrow local tracking variables and seperate the arrow from the bow
        if (arrow)
        {
            XRGrabInteractable arrowGI = arrow.GetComponent<XRGrabInteractable>();
            arrowGI.transform.parent = null;
            arrowGI.trackRotation = true;
            arrowGI.trackPosition = true;
        }

        //child the nockPoint back to the bow object
        nockPoint.transform.SetParent(originalNockParent);
        nockPoint.transform.localPosition = originalNockPosition;

        //stringHand tracker
        stringHand = null;
        arrow = null;
    }
}
