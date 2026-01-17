using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float pitch;
    [SerializeField] float yaw;
    [SerializeField] float distance = 10;
    [SerializeField] float sensistivityX = 5;
    [SerializeField] float sensistivityY = 1;


    InputAction moveCameraInput;

    void Start()
    {
        moveCameraInput = InputSystem.actions.FindAction("Camera");
    }

    void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera follow target not assigned");
        }

        Vector2 movement = moveCameraInput.ReadValue<Vector2>();

        pitch = Mathf.Clamp(pitch - movement.y * sensistivityY, 0, 89);
        yaw += movement.x * sensistivityX;

        Vector3 direction = new Vector3(
            Mathf.Sin(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad),
            Mathf.Sin(pitch * Mathf.Deg2Rad),
            Mathf.Cos(yaw * Mathf.Deg2Rad) * Mathf.Cos(pitch * Mathf.Deg2Rad)
        );

        transform.position = target.transform.position + direction * distance;
        transform.LookAt(target);
    }
}
