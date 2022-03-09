using UnityEngine;

public class KillingFloor : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag.Equals("Arrow"))
        {
            //destroy dropped arrows
            Destroy(collision.gameObject);
        }
        else
        {
            //place dropped bow at starting position
            collision.transform.position = Vector3.up;
        }
    }
}
