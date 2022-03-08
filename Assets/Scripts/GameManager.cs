using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public Hand leftHand;
    public Hand rightHand;

    private void Awake()
    {
        //set reference to GM instance for other scripts
        instance = this;
    }

    public void TryNock(Hand hand)
    {
        //check if the bow is in one hand, and the arrow is in the other. if so then the arrow can be put on the string

        //if the arrow is in the left hand and the right hand is holding the bow
        if (hand.side == HandSide.Left && rightHand.heldObject != null && rightHand.heldObject is Bow)
        {
            (rightHand.heldObject as Bow).Nock(hand.heldObject.attachTransform); //nock
        }
        //if the arrow is in the right hand and the left hand is holding the bow
        else if (hand.side == HandSide.Right && leftHand.heldObject != null && leftHand.heldObject is Bow)
        {
            (leftHand.heldObject as Bow).Nock(hand.heldObject.attachTransform); //nock
        }
    }

    public void TryShoot(Hand hand)
    {
        //checking bow hand
        Bow bow = hand.side == HandSide.Left ? rightHand.heldObject as Bow : leftHand.heldObject as Bow;
        //get arrow rigidbody for shot force application
        Rigidbody arrow = hand.heldObject != null ? hand.heldObject.GetComponent<Rigidbody>() : null;

        //check that bow and arrow are both held correctly for a shot
        if (bow != null && arrow != null && bow.stringHand)
        {
            bow.Shoot(arrow); //shoot
        }
    }
}
