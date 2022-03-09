using UnityEngine;
using UnityEngine.UI;

public class Target : MonoBehaviour
{
    //the text to display hits on
    public Text scoreText;

    //track the number of hits
    private int hits;

    private void OnCollisionEnter(Collision collision)
    {
        GameObject gO = collision.gameObject;
        if(gO.tag.Equals("Arrow"))
        {
            //when an arrow collides with the target, freeze it by setting the arrow to kinematic
            gO.GetComponent<Arrow>().rB.isKinematic = true;

            //increment hit count
            hits++;
            UpdateScoreText();
        }
    }

    private void UpdateScoreText()
    {
        //display hits
        scoreText.text = "Hits: " + hits.ToString();
    }
}
