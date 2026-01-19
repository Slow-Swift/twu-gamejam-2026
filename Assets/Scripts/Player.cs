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

    [SerializeField] LayerMask trashLayer;
    [SerializeField] string trashPileTag = "TrashPile";


    [SerializeField] float reloadcap = 2f;

    [SerializeField] AudioClip attackSound;
    AudioSource audioSource;

    private float lastshottime = 0f;

    private Rigidbody rb;
    private Vector2 movement;
    public int heldTrash {get; private set;}

    public Image ReloadBar;



    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

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
        // We only want to shoot when the shoot button is pressed
        //if (!context.performed) return;

        // Shoot a bullet and start the reload timer where the player can't shoot again until the timer is up
       
        if (Time.time - lastshottime < reloadcap) return;
        Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
        audioSource.PlayOneShot(attackSound);
        lastshottime = Time.time; // Update the last shot time to a baseline for next calculation
        
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
            Debug.Log(heldTrash);
        }

        else if (other.tag == trashPileTag)
        {
            other.gameObject.GetComponent<TrashPile>().
                ReturnTrash(heldTrash);
            heldTrash = 0;
        }
    }
    void Update()
    {
        float progress = (Time.time - lastshottime) / reloadcap;
        ReloadBar.fillAmount =  Math.Clamp(progress, 0f, 1f);//MathF.clamp(progress);
        Debug.Log(ReloadBar.fillAmount);
    }

      

}
