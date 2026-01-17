using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{

    [SerializeField] float speed = 10;
    
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawn;

    private Rigidbody rb;
    private Vector2 movement;

    // Get a reference to the rigidbody component
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {   
        movement = speed * context.ReadValue<Vector2>();
    }

    public void OnShoot(InputAction.CallbackContext context)
    {
        // We only want to shoot when the shoot button is pressed
        if (!context.performed) return;

        Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
    }

    void FixedUpdate()
    {
        UpdateDirection();

        // Set the player's velocity to the inputted movement
        rb.linearVelocity = new Vector3(
            movement.x,
            0,
            movement.y
        );
    }

    void UpdateDirection()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        RaycastHit hitInfo;

        if (Physics.Raycast(mouseRay, out hitInfo))
        {
            Vector3 direction = hitInfo.point - transform.position;
            direction.y = 0;

            transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        }
    }

}
