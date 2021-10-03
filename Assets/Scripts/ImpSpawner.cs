using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpSpawner : MonoBehaviour
{
    public Imp impPrefab;

    public ImpTarget theTarget;

    public GameManager gameManager;
    private void Start()
    {
        var imps = FindObjectsOfType<Imp>();
        foreach (var imp in imps)
        {
            imp.theTarget = theTarget;
            imp.gameManager = gameManager;
        }
            

        gameManager.AddTotalImps(imps.Length);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            var world = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            world.z = 0;

            var imp = Instantiate(impPrefab, world, Quaternion.identity);
            imp.transform.SetParent(transform);
            imp.theTarget = theTarget;
            imp.gameManager = gameManager;

            gameManager.AddTotalImps(1);
        }    
    }
}
