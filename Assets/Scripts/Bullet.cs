using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{

    [SerializeField] float initialVelocity;
    [SerializeField] float lifeTimeSeconds = 3f;
    [SerializeField] string enemyTag = "Enemy";

    public float damage;

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
        if (collision.gameObject.tag.Equals(enemyTag))
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage);
        }
        
        Destroy(gameObject);
    }
}
