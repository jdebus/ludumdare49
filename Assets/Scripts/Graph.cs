using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph : MonoBehaviour
{
    public static Graph Instance { get; private set; }

    Dictionary<Anchor, Node> nodeMap = new Dictionary<Anchor, Node>();
    Dictionary<Node, Anchor> anchorMap = new Dictionary<Node, Anchor>();

    List<Anchor> rootAnchors = new List<Anchor>();

    private void Awake()
    {
        if (Instance != null)
            Destroy(Instance.gameObject);
        Instance = this;
    }

    private void Start()
    {
        rootAnchors = FindObjectsOfType<Anchor>().Where(a => a.isUsable).ToList();
        foreach (var a in rootAnchors)
            RegisterAnchor(a, null);

        foreach (var a in rootAnchors)
        {  
            var n = GetNodeForAnchor(a);
            var springs = a.GetComponents<SpringJoint2D>();
            foreach (var s in springs)
            {
                var anchor = s.connectedBody.GetComponent<Anchor>();
                var anchorNode = GetNodeForAnchor(anchor);
                n.AddConnection(anchorNode);
                anchorNode.AddConnection(n);
                a.AddConnectionRenderer(s, anchor);
            }
        }
    }

    public  void BreakConnection(Anchor anchorA, Anchor anchorB)
    {
        if(anchorA != null && anchorB != null)
        {
            var nodeA = GetNodeForAnchor(anchorA);
            var nodeB = GetNodeForAnchor(anchorB);
            nodeA.RemoveConnection(nodeB);
            nodeB.RemoveConnection(nodeA);
        }
    }

    public List<Anchor> FindAnchorsInRange(Vector2 position, float maxRange, float maxAnchors)
    {
        List<Anchor> inRange = new List<Anchor>();
        foreach (var a in nodeMap.Keys)
        {
            if (a == null)
                continue;
            var ap = (Vector2)a.transform.position;
            var distance = (ap - position).magnitude;
            if (distance <= maxRange)
                inRange.Add(a);

            if (inRange.Count >= maxAnchors)
                break;
        }

        return inRange;
    }

    public void RegisterAnchor(Anchor anchor, List<Anchor> connected)
    {
        if (anchor == null)
            return;

        if (nodeMap.ContainsKey(anchor))
            return;

        var node = new Node();
        node.Anchor = anchor;

        if(connected != null)
        {
            foreach (var con in connected)
            {
                var conNode = nodeMap[con];
                node.AddConnection(conNode);
                conNode.AddConnection(node);
            }
        }
        
        nodeMap.Add(anchor, node);
    }

    public void UnregisterAnchor(Anchor anchor)
    {
        if (anchor == null)
            return;

        if (nodeMap.ContainsKey(anchor) == false)
            return;

        var node = nodeMap[anchor];
        foreach (var connected in node.Connections)
            connected.to.RemoveConnection(node);

        nodeMap.Remove(anchor);
    }

    internal bool IsRootAnchor(Anchor anchor)
    {
        return rootAnchors.Contains(anchor);
    }

    public Node GetNodeForAnchor(Anchor anchor)
    {
        if (anchor == null)
            return null;

        if(nodeMap.ContainsKey(anchor))
            return nodeMap[anchor];
        return null;
    }

    public Anchor GetClosestAnchorInGraph(Vector3 position, float maxDistance)
    {
        float minDist = float.MaxValue;
        Anchor closestAnchor = null;
        foreach (var anchor in nodeMap.Keys)
        {
            if (anchor == null || nodeMap[anchor].Connections.Count == 0)
                continue;

            var distance = (position - anchor.transform.position).magnitude;
            if(distance < minDist)
            {
                minDist = distance;
                closestAnchor = anchor;
            }
        }
        if (minDist > maxDistance)
            return null;

        return closestAnchor;
    }

    public bool HasConnectableAnchors()
    {
        var anchors = nodeMap.Keys.Where(a => nodeMap[a].Connections.Count > 0).ToList();
        if (anchors.Count == 0)
            return false;
        return true;
    }

    public Anchor GetRandomAnchor(Anchor exclude)
    {
        var anchors = nodeMap.Keys.Where(a => nodeMap[a].Connections.Count > 0).ToList();
        if (anchors.Count == 0)
            return null;

        Anchor randomAnchor = exclude;
        while (randomAnchor == exclude)
            randomAnchor = anchors[Random.Range(0, anchors.Count)];
        return randomAnchor;
    }

    public List<Anchor> CalculatePathToWorldPosition(Anchor from, Vector3 worldPosition, float maxDistance = 2, bool includeStart = false)
    {
        var anchor = GetClosestAnchorInGraph(worldPosition, maxDistance);
        if (anchor == null)
            return null;

        return CalculatePath(from, anchor, includeStart);
    }

    public List<Anchor> CalculatePath(Anchor from, Anchor to, bool includeStart = false)
    {
        if (from == null || to == null)
            return null;

        var start = nodeMap[from];
        var target = nodeMap[to];

        Dictionary<Node, Node> cameFrom = new Dictionary<Node, Node>();

        Queue<Node> frontier = new Queue<Node>();
        frontier.Enqueue(start);
                
        bool pathFound = false;
        

        while(frontier.Count > 0 && !pathFound)
        {
            var current = frontier.Dequeue();
            foreach (var connection in current.Connections)
            {
                var next = connection.to;
                if (cameFrom.ContainsKey(next))
                    continue;

                cameFrom.Add(next, current);

                if(next == target) {
                    pathFound = true;

                    var c = target;
                    var path = new List<Anchor>();
                    while(c != start)
                    {
                        path.Add(c.Anchor);
                        c = cameFrom[c];
                    }
                    if(includeStart)
                        path.Add(start.Anchor);
                    path.Reverse();
                    return path;
                }

                frontier.Enqueue(next);
            }
        }
        return null;
    }

}


public class Node
{
    public Anchor Anchor;
    public List<Connection> Connections = new List<Connection>();

    public void AddConnection(Node to)
    {
        Connections.Add(new Connection()
        {
            from = this,
            to = to
        });
    }


    public void RemoveConnection(Node node)
    {
        for (int i = Connections.Count-1; i >= 0; i--)
        {
            if (Connections[i].to == node)
            {
                Connections[i].to.Anchor.RemoveConnection(node.Anchor);
                Connections.RemoveAt(i);
            }
                
        }
    }
}

public class Connection
{
    public Node from;
    public Node to;

}



