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
    [SerializeField] Transform target;

    [SerializeField] string bulletTag = "Bullet";
    [SerializeField] string trashTag = "TrashPile";
    [SerializeField] float gameEdge = 50;
    [SerializeField] GameObject trashDisplay;
    [SerializeField] GameObject droppedTrashPrefab;
    
    private float trashAmount = 0;

    [Header("Target Settings")]

    [SerializeField] float wanderradius = 6f;

    [SerializeField] float repathdistance = 1f;

    private NavMeshAgent agent;
    private UnityEngine.Vector3 currentgoal;

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = 0.5f;
        agent.autoBraking = false;
        trashDisplay.SetActive(false);
    }

    void Update()
    {
        if (transform.position.sqrMagnitude > gameEdge * gameEdge)
        {
            if(trashAmount > 0)
            {
                Debug.Log($"Lost Trash: {trashAmount}");
            }
            Destroy(gameObject);
        }
        
    }


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
            if (trashAmount != 0)
            {
                Instantiate(droppedTrashPrefab, transform.position, transform.rotation);
            }

            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == trashTag)
        {
            target = (transform.position - target).normalized * 100;
            
            TrashPile pile = other.GetComponent<TrashPile>();
            
            if (pile.trashAmount > 0) {
                Debug.Log("Stealing Trash");
                pile.StealTrash(1);
                trashAmount = 1;
                trashDisplay.SetActive(true);
            }
        }
    }
}
