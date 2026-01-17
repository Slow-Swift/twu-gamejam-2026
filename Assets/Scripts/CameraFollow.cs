using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float pitch;
    [SerializeField] float yaw;
    [SerializeField] float distance = 10;
    [SerializeField] Vector2 sensitivity = new Vector2(500, 300);
    [SerializeField] Vector2 deadZone;


    InputAction moveCameraInput;

    void Start()
    {
        moveCameraInput = InputSystem.actions.FindAction("Camera");
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera follow target not assigned");
        }

        Vector2 movement = moveCameraInput.ReadValue<Vector2>();
        if (Mathf.Abs(movement.y) < deadZone.y) movement.y = 0;
        if (Mathf.Abs(movement.x) < deadZone.x) movement.x = 0;

        pitch = Mathf.Clamp(pitch - movement.y * sensitivity.y * Time.deltaTime, 0, 89);
        yaw += movement.x * sensitivity.x * Time.deltaTime;

        Vector3 direction = new Vector3(
            Mathf.Sin(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad),
            Mathf.Sin(pitch * Mathf.Deg2Rad),
            Mathf.Cos(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad)
        );

        transform.position = target.transform.position + direction * distance;
        transform.LookAt(target);
    }
}
