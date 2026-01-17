using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{

    [SerializeField] float initialVelocity;

    void Start()
    {
        // Sets the initial velocity of the bullet. Could maybe move this to the player/crossbow
        GetComponent<Rigidbody>().linearVelocity = transform.forward * initialVelocity;
    }

    void OnCollisionEnter(Collision collision)
    {
        // TODO: Should probably limit what collisions this happens on
        Destroy(gameObject);
    }
}
