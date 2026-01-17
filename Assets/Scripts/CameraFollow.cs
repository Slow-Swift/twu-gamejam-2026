using System;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] GameObject target;
    [SerializeField] Vector3 offset;

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("Camera follow target not assigned");
        }

        transform.position = target.transform.position + offset;
    }
}
