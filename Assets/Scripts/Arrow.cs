using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Arrow : XRGrabInteractable
{
    [HideInInspector]
    public Rigidbody rB;

    private void Start()
    {
        //grab rigidbody reference
        rB = GetComponent<Rigidbody>();
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        //when the arrow is dropped it should be affected by physics again
        rB.isKinematic = false;

        base.OnSelectExited(args);
    }
}
