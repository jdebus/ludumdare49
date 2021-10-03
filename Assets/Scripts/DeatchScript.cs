using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeatchScript : MonoBehaviour
{
    public GameManager GameManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Imp"))
        {
            GameManager.ImpDied();
        }
        else
        {
            var anchor = collision.gameObject.GetComponent<Anchor>();
            if (anchor != null)
            {
                Graph.Instance.UnregisterAnchor(anchor);
                Destroy(anchor.gameObject);
                GameManager.CheckIfGraphIsValid();
            }
                
        }

    }
}
