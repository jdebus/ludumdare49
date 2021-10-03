using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;

public class Imp : MonoBehaviour
{
    Anchor currentTarget;
    List<Anchor> currentPath;

    public float speed = 2;

    public ImpTarget theTarget;
    bool movingToTarget = false;

    public float maxMoveToGraphDistance = 1;
    public float maxReachTargetDistance = 1;

    Rigidbody2D body;
    Animator anim;

    public GameManager gameManager { get; set; }

    float speedMultiplier = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<DeatchScript>() != null)
        {
            Destroy(this.gameObject);
        }

    }
    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        speed += Random.Range(-0.2f, 0.2f);
        transform.localScale += Vector3.one * Random.Range(-0.15f, 0.0f);
        anim.enabled = false;
        StartCoroutine(EnableAnimator());
    }

    IEnumerator EnableAnimator()
    {
        yield return new WaitForSeconds(Random.Range(0, .5f));
        anim.enabled = true;
    }

    bool IsTargetInRange()
    {
        return theTarget != null && Vector3.Distance(transform.position, theTarget.transform.position) <= maxReachTargetDistance;
    }

    void TargetReached()
    {
        gameManager?.ImpSaved();
        Destroy(gameObject);
    }

    private void FixedUpdate()
    {
        body.centerOfMass = Vector2.zero;
        body.simulated = true;
        if (movingToTarget || currentTarget != null)
            body.simulated = false;

        if(transform.position.y < -14)
            body.simulated = true;

        if (!body.simulated)
        {
            body.velocity = Vector2.zero;
            body.angularVelocity = 0;
        }
    }

    void Update()
    {
        //if(currentTarget != null && Vector3.Distance(lastPosition, transform.position) < (speed * Time.deltaTime * 0.5f))
        //{
        //    framesStuck++;
        //}
        //lastPosition = transform.position;

        //if(framesStuck > 60)
        //{
        //    framesStuck = 0;
        //    currentTarget = null;
        //    currentPath = null;
        //}


        if (movingToTarget)
        {
            var direction = (theTarget.transform.position - transform.position);
            transform.Translate(direction.normalized * speed * 4 * Time.deltaTime);
            if(direction.magnitude < 0.2f)
            {
                TargetReached();
                return;
            }

            return;
        }

        anim.SetBool("moving", false);

        if(currentTarget == null)
            currentTarget = Graph.Instance.GetClosestAnchorInGraph(transform.position, maxMoveToGraphDistance);

        if(currentTarget != null)
        {
            var direction = (currentTarget.transform.position - transform.position);
            var distance = direction.magnitude;

            body.velocity = Vector2.zero;
            transform.Translate(direction.normalized * speed * speedMultiplier * Time.deltaTime);
            anim.SetBool("moving", true);
            if(distance < 0.1f)
            {
                transform.position = currentTarget.transform.position;
                if (currentPath != null && currentPath.Count > 0)
                    currentPath.RemoveAt(0);

                // can we move along path
                if (currentPath != null && currentPath.Count > 0)
                {
                    currentTarget = currentPath[0];
                }
                // end of path reached
                else if (currentPath != null && currentPath.Count == 0)
                {
                    currentPath = null;
                    if (IsTargetInRange())
                    {
                        movingToTarget = true;
                        return;
                    }
                }

                // no path              
                if(currentPath == null)
                {
                    if(theTarget != null)
                    {
                        // try find path to level target
                        currentPath = Graph.Instance.CalculatePathToWorldPosition(currentTarget, theTarget.transform.position, maxReachTargetDistance);
                        speedMultiplier = currentPath != null ? 3 : 1;
                    }

                    // no path yet, select a random one
                    if(currentPath == null)
                    {
                        var nextTarget = Graph.Instance.GetRandomAnchor(currentTarget);
                        currentPath = Graph.Instance.CalculatePath(currentTarget, nextTarget);
                    }
                    
                    // if we have a path, start moving
                    if (currentPath != null && currentPath.Count > 0)
                    {
                        currentTarget = currentPath[0];
                    }
                }
                   
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(currentTarget != null && currentPath != null && currentPath.Count > 1)
        {
            Gizmos.color = Colors.Pink;
            Gizmos.DrawLine(transform.position, currentTarget.transform.position);
            
            Gizmos.color = Colors.OrangeRed;
            for (int i = 0; i < currentPath.Count - 1; i++)
            {
                Gizmos.DrawLine(currentPath[i].transform.position, currentPath[i + 1].transform.position);
            }
        }

        //Handles.Label(body.position, $"vel: {body.velocity}");
        //Handles.Label(body.position + Vector2.down, $"targer: {currentTarget?.name}");
    }
}
