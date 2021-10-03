using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Anchor : MonoBehaviour
{
    Rigidbody2D body;
    public Rigidbody2D Body => body;

    List<Anchor> connections = new List<Anchor>();

    public float breakForce = 200;

    public bool isUsable = true;

    public ConnectionRenderer connectionRendererPrefab;
    Dictionary<SpringJoint2D, ConnectionRenderer> connectionMap = new Dictionary<SpringJoint2D, ConnectionRenderer>();

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
    }

    public void AddConnection(Anchor a)
    {
        //var joint = gameObject.AddComponent<HingeJoint2D>();
        //joint.connectedBody = a.Body;
        //joint.breakForce = breakForce;
        //joint.autoConfigureConnectedAnchor = false;

        var joint = gameObject.AddComponent<SpringJoint2D>();
        joint.connectedBody = a.Body;
        joint.breakForce = breakForce;
        joint.frequency = 6;
        joint.dampingRatio = .2f;

        AddConnectionRenderer(joint, a);

        //var direction = a.transform.position - transform.position;
        //a.Body.AddForce(direction.normalized * -2, ForceMode2D.Impulse);

        connections.Add(a);
    }

    private void Update()
    {
        foreach (var joint in connectionMap.Keys)
        {
            if (joint == null)
                continue;

            if(joint.connectedBody != null)
            {
                var distance = (transform.position - joint.connectedBody.transform.position).magnitude;
                if (distance > 6)
                    joint.breakForce = 0;
            }

            connectionMap[joint].BreakFactor = joint.reactionForce.magnitude / joint.breakForce;
        }
    }

    internal void AddConnectionRenderer(SpringJoint2D joint, Anchor anchor)
    {
        var connection = Instantiate(connectionRendererPrefab, transform);
        connection.AnchorA = this;
        connection.AnchorB = anchor;
        connectionMap.Add(joint, connection);

    }

    private void OnMouseOver()
    {
        //if (Input.GetMouseButtonUp(1))
        //{
        //    if (Graph.Instance.IsRootAnchor(this))
        //        return;

        //    Graph.Instance.UnregisterAnchor(this);
        //    Destroy(gameObject);
        //}

        //if (Input.GetMouseButtonUp(2))
        //    Graph.Instance.CalculatePath(this, null);
    }

    private void OnDrawGizmosSelected()
    {
        //if (Graph.Instance == null)
        //    return;
      
        //var node = Graph.Instance.GetNodeForAnchor(this);
        //var p1 = transform.position;
        //Gizmos.color = Colors.CornflowerBlue;
        //foreach (var con in node.Connections)
        //{
        //    var p2 = con.to.Anchor.transform.position;
        //    Gizmos.DrawLine(p1, p2);
        //    //Gizmos.DrawSphere(p2, .4f);
        //}
    }

    public void RemoveConnection(Anchor anchor)
    {
        foreach (var kv in connectionMap)
        {
            if(kv.Key != null)
            {
                if(kv.Key.attachedRigidbody.gameObject == anchor.gameObject)
                {
                    if(kv.Value != null)
                        Destroy(kv.Value.gameObject);
                    connectionMap.Remove(kv.Key);
                    return;
                }
            }
        }
    }
}
