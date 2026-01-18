using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [SerializeField] float speed = 3;
    [SerializeField] Vector3 target = Vector3.zero;
    [SerializeField] string trashTag = "TrashPile";
    [SerializeField] float gameEdge = 50;
    [SerializeField] GameObject trashDisplay;
    [SerializeField] GameObject droppedTrashPrefab;
    [SerializeField] int maxHealth = 4;
    [SerializeField] GameObject droppedPowerUpsPrefab;
    [SerializeField] float dropChance = 0.5f;
    [SerializeField] Image healthSlider;

    private float trashAmount = 0;
    private float health;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        trashDisplay.SetActive(false);
        health = maxHealth;

        // Hard coded, I know. IDK right now
        healthSlider.transform.parent.parent.gameObject.SetActive(false);
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

    public void TakeDamage(float amount)
    {
        health -= amount;
        // Hard coded, I know. IDK right now
        healthSlider.transform.parent.parent.gameObject.SetActive(health < maxHealth);
        healthSlider.fillAmount = ((float) health) / maxHealth;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (trashAmount != 0)
        {
            Instantiate(droppedTrashPrefab, transform.position, transform.rotation);
        }

        if (Random.value <= dropChance)
        {
            Instantiate(droppedPowerUpsPrefab, transform.position, transform.rotation);
        }

        Destroy(gameObject);
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
