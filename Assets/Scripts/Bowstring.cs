using UnityEngine;

public class Bowstring : MonoBehaviour
{
    //this should be where the two halves of the string meet and create an angle
    public Transform nockPoint;

    //this half of the string's line renderer
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        //assign linerenderer on start
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //each frame, update the endpoint position to the nockpoint to track a string pull
        if (lineRenderer.positionCount > 1)
        {
            lineRenderer.SetPosition(1, transform.InverseTransformPoint(nockPoint.position));
        }
    }
}
