using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour
{

    [SerializeField] float speed = 10;
    private bool isSpeedBoosted = false;
    [SerializeField] float boostedSpeed = 20;
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletSpawn;

    [SerializeField] LayerMask trashLayer;
    [SerializeField] string trashPileTag = "TrashPile";

    [SerializeField] float minForce = 20f;
    [SerializeField] float maxForce = 60f;
    [SerializeField] float maxChargeTime = 2f;
    [SerializeField] float chargeTime = 0f;
    [SerializeField] Vector2 damageRange;
    [SerializeField] GameObject trajectory;
    [SerializeField] Vector2 trajectoryRange;
    [SerializeField] Material trajectoryMaterial;
    [SerializeField] Gradient trajectoryGradient;

    float chargeStartTime;
    bool isCharging = false;

    Rigidbody rb;
    Vector2 movement;
    [SerializeField] InputActionAsset inputActions;
    public int heldTrash {get; private set;}

    // Get a reference to the rigidbody component
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        InputSystem.actions.FindAction("Move").performed += ctx => OnMove(ctx.ReadValue<Vector2>());
        InputSystem.actions.FindAction("Move").canceled += ctx => OnMove(ctx.ReadValue<Vector2>());
        InputSystem.actions.FindAction("Shoot").started += ctx => ShootPressed();
        InputSystem.actions.FindAction("Shoot").canceled += ctx => ShootReleased();
    }

    public void OnMove(Vector2 movement)
    {   
        this.movement = movement;
    }

    void Update()
    {
        trajectory.SetActive(isCharging);
        if (isCharging)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0f, maxChargeTime);
            float chargePercent = Mathf.Clamp((Time.time - chargeStartTime) / maxChargeTime, 0, 1);
            trajectory.transform.localScale = new Vector3(
                1, 1, 
                Mathf.Lerp(trajectoryRange.x, trajectoryRange.y, chargePercent)
            );
            trajectoryMaterial.color = trajectoryGradient.Evaluate(chargePercent);
            Debug.Log($"Charging... chargeTime = {chargeTime}, {chargePercent}");
        }
    }

    void ShootPressed()
    {
        Debug.Log("Shoot Pressed");
        isCharging = true;
        chargeStartTime = Time.time;
    }

    void ShootReleased()
    {
        Debug.Log("Shoot Released");

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
            bulletScript.damage = Mathf.Lerp(damageRange.x, damageRange.y, percent);
            Debug.Log($"Spawning bullet: force={force} chargeTime={chargeTime}");
            chargeTime = 0;
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

        // ## New movement style
        // transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        // rb.linearVelocity = transform.localToWorldMatrix * movementDirection * speed;
        
        // ## Old movement style:
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
            heldTrash += 9;
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
