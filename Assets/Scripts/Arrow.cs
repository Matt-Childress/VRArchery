using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Arrow : XRGrabInteractable
{
    [HideInInspector]
    public Rigidbody rB;
    private Collider arrowCollider;

    private void Start()
    {
        //grab rigidbody and collider references
        rB = GetComponent<Rigidbody>();
        arrowCollider = GetComponent<Collider>();
    }

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
