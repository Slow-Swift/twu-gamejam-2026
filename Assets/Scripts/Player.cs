using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{

    [SerializeField] float speed = 10;
    
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawn;

    [SerializeField] LayerMask trashLayer;
    [SerializeField] string trashPileTag = "TrashPile";


    Rigidbody rb;
    Vector2 movement;
    public int heldTrash {get; private set;}

    // Get a reference to the rigidbody component
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        InputSystem.actions.FindAction("Move").performed += ctx => OnMove(ctx.ReadValue<Vector2>());
        InputSystem.actions.FindAction("Move").canceled += ctx => OnMove(ctx.ReadValue<Vector2>());
        InputSystem.actions.FindAction("Shoot").performed += ctx => OnShoot();
    }

    public void OnMove(Vector2 movement)
    {   
        this.movement = movement;
    }

    public void OnShoot()
    {
        Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
    }

    void FixedUpdate()
    {
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 movementDirection = new Vector3(movement.x, 0, movement.y).normalized;
        if (movementDirection.sqrMagnitude > 0.05)
        {
            transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
            transform.rotation = Quaternion.LookRotation(transform.localToWorldMatrix * movementDirection, Vector3.up);
        }

        // Set the player's velocity to the inputted movement
        rb.linearVelocity = transform.forward * movement.magnitude * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if ((trashLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            Destroy(other.gameObject);
            heldTrash += 1;
        }

        if (other.tag == trashPileTag)
        {
            other.gameObject.GetComponent<TrashPile>().
                ReturnTrash(heldTrash);
            heldTrash = 0;
        }
    }
}
