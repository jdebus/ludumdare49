using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class ConnectionAdder : MonoBehaviour
{
    public Anchor anchorPrefab;

    public ConnectionRenderer connectionRenderPrefab;

    const int maxConnections = 3;
    ConnectionRenderer[] conRenders = new ConnectionRenderer[maxConnections];

    public Anchor previewAnchor;
    public Color connectionPreviewColor;

    public GameManager gameManager;

    private void Start()
    {
        for (int i = 0; i < maxConnections; i++)
        {
            conRenders[i] = Instantiate(connectionRenderPrefab, transform);
            conRenders[i].AnchorA = previewAnchor;
            conRenders[i].AnchorB = previewAnchor;
            conRenders[i].BreakFactor = 0;
            conRenders[i].opacity = 1;
            conRenders[i].autoDestroy = false;
            conRenders[i].colorOverride = true;
            conRenders[i].overrideColor = connectionPreviewColor;
            conRenders[i].gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        worldPos.z = 0;
        previewAnchor.transform.position = worldPos;

        var anchorsInRange = Graph.Instance.FindAnchorsInRange(worldPos, 2.75f, maxConnections);
        if (anchorsInRange != null)
            anchorsInRange = anchorsInRange.Where(a => a != null).ToList();
        
        if(anchorsInRange.Count < 2)
        {
            foreach (var item in conRenders)
                item.gameObject.SetActive(false);
            return;
        }

        for (int i = 0; i < maxConnections; i++)
        {
            if(anchorsInRange.Count - 1 >= i)
            {
                conRenders[i].gameObject.SetActive(true);
                conRenders[i].AnchorA = previewAnchor;
                conRenders[i].overrideColor = connectionPreviewColor;
                conRenders[i].AnchorB = anchorsInRange[i];
            }
            else
                conRenders[i].gameObject.SetActive(false);
        }

        if(anchorsInRange.Count > 1 && Input.GetMouseButtonUp(0))
        {
            var a = Instantiate(anchorPrefab, worldPos, Quaternion.identity);
            foreach (var anchor in anchorsInRange)
                a.AddConnection(anchor);

            gameManager.ConnectionMade();
            Graph.Instance.RegisterAnchor(a, anchorsInRange);
        }
    }

    List<Anchor> FindAnchorsInRange(List<Anchor> anchors, Vector2 position, float maxRange, float maxAnchors = maxConnections)
    {
        List<Anchor> inRange = new List<Anchor>();
        foreach (var a in anchors)
        {
            var ap = (Vector2)a.transform.position;
            var distance = (ap - position).magnitude;
            if (distance <= maxRange)
                inRange.Add(a);

            if (inRange.Count >= maxAnchors)
                break;
        }

        return inRange;
    }

    private void OnDrawGizmos()
    {
        //var worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Gizmos.color = Colors.LimeGreen;
        //foreach (var a in anchorsInRange)
        //{
        //    if (a is null) continue;
        //    Gizmos.DrawLine(a.transform.position, worldPos);
        //}
            
    }
}
