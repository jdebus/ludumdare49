using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AreaEffector2D))]
public class WindScript : MonoBehaviour
{
    AreaEffector2D effector;

    public float force = 2;

    float timer = 0;
    float targetAngle = 90;

    private void Start()
    {
        effector = GetComponent<AreaEffector2D>();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > 1)
            targetAngle = -targetAngle;

        effector.forceAngle = Mathf.Lerp(effector.forceAngle, targetAngle, Time.deltaTime);
        effector.forceMagnitude = force;
    }

}
