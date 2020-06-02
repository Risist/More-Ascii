using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Tooltip("Target to follow")]
    public Transform target;
    [Tooltip("offset from follow point")]
    public Vector3 offset;
    [Tooltip("learp factor of how fast camera follows target"), Range(0.0f, 1.0f)]
    public float followFactor = 0.3f;
    [Tooltip("When target distance is clamped to the value")]
    public float maxDifference = float.PositiveInfinity;

    [Header("Shakes")]
    [Range(0, 1)] public float shakeDrag = 0.95f;
    [Range(0, 1)] public float shakeLerp = 0.05f;
    Vector3 shakeOffset;
    Vector3 desiredShake;

    public void ApplyShake(Vector3 force)
    {
        desiredShake += force;
    }
    public void ApplyShake(float force)
    {
        desiredShake += (Vector3)Random.insideUnitCircle*force;
    }

    public delegate Vector3 DirectionOffsetMethod();
    public DirectionOffsetMethod directionOffset = () => Vector3.zero;

    new Transform transform;

    void Start()
    {
        transform = base.transform;
        if (!target)
        {
            Debug.LogWarning("Camera target not set up");
        }
    }

    private void FixedUpdate()
    {
        shakeOffset = Vector3.Lerp(shakeOffset, desiredShake, shakeLerp);
        desiredShake *= shakeDrag;
    }
    Vector3 lastTargetPosition;

    void LateUpdate()
    {
        if(target)
        {
            lastTargetPosition = target.position;
        }


        // vector to target, counted without offset

        var directionOffsetValue = directionOffset();
        Vector3 toTarget = transform.position - offset - lastTargetPosition - directionOffsetValue - shakeOffset;
        toTarget.z = 0;
        float toTargetDistSq = toTarget.sqrMagnitude;


        if (toTargetDistSq > maxDifference * maxDifference)
        {
            Vector3 pos = lastTargetPosition + toTarget.normalized * maxDifference + offset + directionOffsetValue + shakeOffset;
            transform.position = pos;
        }
        transform.position = Vector3.Lerp(transform.position, lastTargetPosition + offset + directionOffsetValue + shakeOffset, followFactor);
    }
}
