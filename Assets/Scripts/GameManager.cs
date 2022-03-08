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

        Bow bow = null;

        //if the arrow is in the left hand and the right hand is holding the bow
        if (hand == leftHand && rightHand.heldObject != null && rightHand.heldObject is Bow)
        {
            bow = rightHand.heldObject as Bow; //bow is in the right hand
        }
        //if the arrow is in the right hand and the left hand is holding the bow
        else if (hand == rightHand && leftHand.heldObject != null && leftHand.heldObject is Bow)
        {
            bow = leftHand.heldObject as Bow; //bow is in the left hand
        }

        bow.Nock(hand); //put arrow on bow
    }

    public void TryShoot(Hand hand)
    {
        //checking bow hand
        Bow bow = hand == leftHand ? rightHand.heldObject as Bow : leftHand.heldObject as Bow;
        //get arrow rigidbody for shot force application
        Rigidbody arrow = hand.heldObject != null ? hand.heldObject.GetComponent<Rigidbody>() : null;

        //check that bow and arrow are both held correctly for a shot
        if (bow != null && arrow != null && bow.stringHand)
        {
            bow.Shoot(arrow); //shoot
        }
    }
}
