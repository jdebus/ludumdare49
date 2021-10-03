using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ConnectionRenderer : MonoBehaviour
{
    public Anchor AnchorA { get; set; }
    public Anchor AnchorB { get; set; }
    public float BreakFactor { get; set; } = 0;

    LineRenderer lr;

    public int segments = 5;

    public float opacity = 1;
    public bool colorOverride = false;
    public Color overrideColor = Colors.White;

    public bool autoDestroy = true;

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if(AnchorA == null || AnchorB == null)
        {
            if(autoDestroy)
                Destroy(gameObject);
            return;
        }

        var distance = (AnchorA.transform.position - AnchorB.transform.position).magnitude;
        if (distance > 6)
        {
            Graph.Instance.BreakConnection(AnchorA, AnchorB);
            lr.enabled = false;
            return;
        }

        lr.enabled = true;

        lr.positionCount = segments + 1;
        lr.startColor = lr.endColor = Color.Lerp(Colors.White, Colors.DarkRed, BreakFactor) * opacity;
        if (colorOverride)
        {
            lr.startColor = lr.endColor = overrideColor * opacity;
        }

        Vector3 a = AnchorA.transform.position;
        Vector3 b = AnchorB.transform.position;
        a.z = b.z = 0;

        Vector3[] points = new Vector3[lr.positionCount];
        for (int i = 0; i < lr.positionCount -1; i++)
        {
            points[i] = Vector3.Lerp(a, b, (float)i / lr.positionCount);
        }
        points[segments] = b;


        lr.SetPositions(points);
        
    }
}
