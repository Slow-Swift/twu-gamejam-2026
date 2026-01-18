using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class Enemy : MonoBehaviour
{    
    [Header("Target Settings")]
    // [SerializeField] float speed = 3;
    // [SerializeField] UnityEngine.Vector3 target = UnityEngine.Vector3.zero;
    [SerializeField] Transform target;
    [SerializeField] string bulletTag = "Bullet";
    [SerializeField] string targetTag = "TrashPile";

   // [SerializeField] int curvepoints = 3;// number of points in curve
   // [SerializeField] float curveradius = 2f; //offset off path


    [Header("Target Settings")]

    [SerializeField] float wanderradius = 6f;

    [SerializeField] float repathdistance = 1f;

    private NavMeshAgent agent;
    private UnityEngine.Vector3 currentgoal;

    // private Rigidbody rb;
    // private Vector3[] pathpoints;
    // private int currentpoint = 0;




    void Start()
    {
        //rb = GetComponent<Rigidbody>();

        //GeneratePath();

        agent = GetComponent<NavMeshAgent>();

        agent.stoppingDistance = 0.5f;
        agent.autoBraking = false;
    }

    // void FixedUpdate()
    // // {
    // //     // Get the direction to the target (trash)
    // //     Vector3 toTarget = target - transform.position;
    // //     toTarget.y = 0;
        
    // //     // Look and move towards the target
    // //     transform.rotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
    // //     rb.linearVelocity = toTarget.normalized * speed;

    //  {
    //     if (pathpoints == null || pathpoints.Length == 0) return;

    //     Vector3 nextPoint = pathpoints[currentpoint];
    //     Vector3 dir = nextPoint - transform.position;
    //     dir.y = 0;
    //     Vector3 moveDir = dir.normalized;

    //     // Look and move toward next point
    //     transform.rotation = Quaternion.LookRotation(moveDir, Vector3.up);
    //     rb.linearVelocity = moveDir * speed;

    //     // Check if reached waypoint
    //     if (Vector3.Distance(transform.position, nextPoint) < 0.1f)
    //     {
    //         currentpoint++;
    //         if (currentpoint >= pathpoints.Length)
    //         {
    //             // Stop at target
    //             rb.linearVelocity = Vector3.zero;
    //         }
    //     }
    // //}


    // }

    // private void GeneratePath()
    // {
    //     pathpoints = new Vector3[curvepoints + 1];
    //     Vector3 start = transform.position;
    //     Vector3 end = target;

    //     for (int i = 0; i < curvepoints; i++)
    //     {
    //         float t = (i + 1f) / (curvepoints + 1f);
    //         Vector3 point = Vector3.Lerp(start, end, t);

    //         // Offset sideways to create curve
    //         Vector3 sideways = Vector3.Cross(Vector3.up, (end - start).normalized);
    //         point += sideways * UnityEngine.Random.Range(-curveradius, curveradius);

    //         pathpoints[i] = point;
    //     }
    //     pathpoints[curvepoints] = end;

    // }

     void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < repathdistance)
        {
            PickNewGoal();
        }

        if (UnityEngine.Vector3.Distance(transform.position, target.position) < 0.5f)
        {
            Destroy(gameObject);
        }
        
    }

    void PickNewGoal()
    {
        UnityEngine.Vector3 randomOffest = Random.insideUnitSphere * wanderradius; // pick a random point near the target
        randomOffest.y = 0f; // make sure its on the ground

        UnityEngine.Vector3 candidate = target.position + randomOffest;

        if (NavMesh.SamplePosition(candidate,out NavMeshHit hit, wanderradius, NavMesh.AllAreas))
        {
            currentgoal = hit.position;
            agent.SetDestination(currentgoal);
        } else
        {
            agent.SetDestination(target.position);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals(bulletTag))
        {
            Destroy(gameObject);
        } 
        else if (collision.gameObject.tag.Equals(targetTag))
        {
            Destroy(gameObject);
        }

    }
}
