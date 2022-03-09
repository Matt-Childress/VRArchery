using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    //the text to display hits on
    public Text scoreText;

    //track the number of hits
    private int hits;

    //transforms for judging if player is too close
    public Transform playerTransform;
    public Transform minDistanceMarkerTransform;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag.Equals("Arrow"))
        {
            //when an arrow collides with the target, freeze it by setting the arrow to kinematic
            collision.gameObject.GetComponent<Arrow>().rB.isKinematic = true;

            //if the player is behind the distance marker
            if (Vector3.Distance(transform.position, playerTransform.position) > Vector3.Distance(transform.position, minDistanceMarkerTransform.position))
            {
                //increment hit count
                hits++;
                UpdateScoreText();
            }
            else //if the player is in front of the distance marker
            {
                DisplayTooCloseMessage();
            }
        }
    }

    private void UpdateScoreText()
    {
        //display hits
        scoreText.text = "Hits: " + hits.ToString();
        scoreText.color = Color.green;
    }

    private void DisplayTooCloseMessage()
    {
        //display that the player is too close to the target
        scoreText.text = "TOO CLOSE!";
        scoreText.color = Color.red;
    }
}
