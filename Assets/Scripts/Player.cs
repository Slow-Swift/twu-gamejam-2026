using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{

    [SerializeField] float speed = 10;
    private bool isSpeedBoosted = false;
    [SerializeField] float boostedSpeed = 30;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawn;

    [SerializeField] LayerMask trashLayer;
    [SerializeField] string trashPileTag = "TrashPile";

    [SerializeField] float minForce = 20f;
    [SerializeField] float maxForce = 60f;
    [SerializeField] float maxChargeTime = 2f;
    [SerializeField] float chargeTime = 0f;
    [SerializeField] bool isCharging = false;

    Rigidbody rb;
    Vector2 movement;
    [SerializeField] InputActionAsset inputActions;
    InputAction shootAction;
    public int heldTrash {get; private set;}

    // Get a reference to the rigidbody component
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        InputSystem.actions.FindAction("Move").performed += ctx => OnMove(ctx.ReadValue<Vector2>());
        InputSystem.actions.FindAction("Move").canceled += ctx => OnMove(ctx.ReadValue<Vector2>());
        shootAction = InputSystem.actions.FindAction("Shoot");
        shootAction.Enable();
        shootAction.started += OnShoot;
        shootAction.canceled += OnShoot;
    }

    public void OnMove(Vector2 movement)
    {   
        this.movement = movement;
    }

    void Update()
    {
        if (isCharging)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
            Debug.Log($"Charging... chargeTime = {chargeTime}");
        }
    }

    public void OnShoot(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (!isCharging)
            {
                isCharging = true;
                chargeTime = 0f;
                Debug.Log("Started charging");
            }
        }
        if (ctx.canceled)
        {
            if (isCharging)
            {
                isCharging = false;
                Debug.Log($"Stopped charging at chargeTime={chargeTime}");
                float percent = Mathf.Clamp01(chargeTime / maxChargeTime);
                percent = percent * percent;
                float force = Mathf.Lerp(minForce, maxForce, percent);
                GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation);
                Bullet bulletScript = bullet.GetComponent<Bullet>();
                bulletScript.SetInitialVelocity(force);
                Debug.Log($"Spawning bullet: force={force} chargeTime={chargeTime}");
            }
        }
    }
    public void ActivatePowerUp(PowerUpType type, float duration)
    {
        switch (type)
        {
            case PowerUpType.Speed:
                if (!isSpeedBoosted)
                StartCoroutine(SpeedBoost(duration));
                break;
        }
    }

    private IEnumerator SpeedBoost(float duration)
    {
        isSpeedBoosted = true;
        float originalSpeed = speed;
        speed = boostedSpeed;
        yield return new WaitForSeconds(duration);
        speed = originalSpeed;
        isSpeedBoosted = false;
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

        if (other.tag == trashPileTag)
        {
            other.gameObject.GetComponent<TrashPile>().
                ReturnTrash(heldTrash);
            heldTrash = 0;
        }
    }

    void OnDestroy()
    {
        //shootAction.started -= OnShoot;
        //shootAction.canceled -= OnShoot;
    }
}
