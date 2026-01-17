using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{

    [SerializeField] float speed = 10;
    
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawn;

    [SerializeField] float reloadcap = 2f;

    private float lastshottime = 0f;

    private Rigidbody rb;
    private Vector2 movement;

    public Image ReloadBar;


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

        // Shoot a bullet and start the reload timer where the player can't shoot again until the timer is up
       
        if (Time.time - lastshottime < reloadcap) return;
        Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        lastshottime = Time.time; // Update the last shot time to a baseline for next calculation
        
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

    void Update()
    {
        float progress = (Time.time - lastshottime) / reloadcap;
        ReloadBar.fillAmount =  Math.Clamp(progress, 0f, 1f);//MathF.clamp(progress);
        Debug.Log(ReloadBar.fillAmount);
    }

}
