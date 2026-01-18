using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [SerializeField] float speed = 3;
    [SerializeField] Vector3 target = Vector3.zero;
    [SerializeField] string bulletTag = "Bullet";
    [SerializeField] string trashTag = "TrashPile";
    [SerializeField] float gameEdge = 50;
    [SerializeField] GameObject trashDisplay;
    [SerializeField] GameObject droppedTrashPrefab;
    [SerializeField] int health = 4;
    [SerializeField] GameObject droppedPowerUpsPrefab;
    [SerializeField] float dropChance = 0.5f;

    private float trashAmount = 0;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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

    void FixedUpdate()
    {
        // Get the direction to the target (trash)
        Vector3 toTarget = target - transform.position;
        toTarget.y = 0;
        
        // Look and move towards the target
        transform.rotation = Quaternion.LookRotation(toTarget.normalized, Vector3.up);
        rb.linearVelocity = toTarget.normalized * speed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals(bulletTag))
        {
            if (trashAmount != 0)
            {
                Instantiate(droppedTrashPrefab, transform.position, transform.rotation);
            }
            health -= 1;
            if (health <= 0)
            {
                if (Random.value <= dropChance)
                {
                    Instantiate(droppedPowerUpsPrefab, transform.position, transform.rotation);
                }
                Destroy(gameObject);
            }
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
