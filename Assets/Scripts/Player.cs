using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{

    [SerializeField]
    private float speed = 10;

    private Rigidbody rb;
    private Vector2 movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {   
        movement = speed * context.ReadValue<Vector2>();
    }

    void FixedUpdate()
    {
        // Set the player's velocity to the inputted movement
        rb.linearVelocity = new Vector3(
            movement.x,
            0,
            movement.y
        );
    }

}
