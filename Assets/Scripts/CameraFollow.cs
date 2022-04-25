using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float lerpValue;

    [SerializeField] private Transform target;

    [SerializeField] private Vector3 offset;

    void LateUpdate()
    {
        Vector3 targetPos = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, targetPos, lerpValue * Time.deltaTime);
    }
}
