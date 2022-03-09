using UnityEngine;

public class Target : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        GameObject gO = collision.gameObject;
        if(gO.tag.Equals("Arrow"))
        {
            //when an arrow collides with the target, freeze it by setting the arrow to kinematic
            gO.GetComponent<Arrow>().rB.isKinematic = true;
        }
    }
}
