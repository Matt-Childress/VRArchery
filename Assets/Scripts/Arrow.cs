using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Arrow : XRGrabInteractable
{
    //public variables to set in the editor for arrow prefab
    public Rigidbody rB;
    public Collider arrowCollider;

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        //when the arrow is dropped it should be affected by physics again
        rB.isKinematic = false;
        arrowCollider.enabled = true;

        base.OnSelectExited(args);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        //when the arrow is picked up, turn off the collider
        arrowCollider.enabled = false;

        base.OnSelectEntered(args);
    }
}
