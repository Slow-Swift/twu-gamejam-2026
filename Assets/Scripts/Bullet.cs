using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{

    [SerializeField] float initialVelocity;
    [SerializeField] float lifeTimeSeconds = 3f;
    Rigidbody rb;
    void Awake()
    {
        // Sets the initial velocity of the bullet. Could maybe move this to the player/crossbow
        rb = GetComponent<Rigidbody>();
    }

    public void SetInitialVelocity(float velocity)
    {
        rb.linearVelocity = transform.forward * velocity;
        Destroy(gameObject, lifeTimeSeconds);
    }
     void OnCollisionEnter(Collision collision)
    {
        // TODO: Should probably limit what collisions this happens on
        Destroy(gameObject);
    }
}
