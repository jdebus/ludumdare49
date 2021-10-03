using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;

public class ConnectionVisualizer : MonoBehaviour
{
    public bool showForces = false;

    private void OnDrawGizmos()
    {
        var joints = FindObjectsOfType<SpringJoint2D>();
        foreach (var joint in joints)
        {
            if (joint.connectedBody is null)
                continue;

            var reactionMag = joint.reactionForce.magnitude;
            var breakFactor = reactionMag / joint.breakForce;
            Gizmos.color = Color.Lerp(Colors.White, Colors.OrangeRed, breakFactor) * 0.75f;
            var p1 = (Vector2)joint.transform.position;
            var p2 = joint.connectedBody.position;
            Gizmos.DrawLine(p1, p2);


            //if (showForces)
            //{
            //    var center = p1 + ((p2 - p1) / 2);
            //    Handles.Label(center, $"{reactionMag}"); 
            //}
            

        }
    }
}
