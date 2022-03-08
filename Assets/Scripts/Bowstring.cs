using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bowstring : MonoBehaviour
{
    public Transform nockPoint;

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (lineRenderer.positionCount > 1)
        {
            lineRenderer.SetPosition(1, transform.InverseTransformPoint(nockPoint.position));
        }
    }
}
