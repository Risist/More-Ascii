using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMotor : MonoBehaviour
{
    public float forceStrength;
    public RangedFloat forceStrengthRandomPercent;
    public float drag;

    Vector3 direction;

    void Start()
    {
        direction = Quaternion.Euler(0, 0, Random.value * 360) * Vector3.up * Random.Range(forceStrengthRandomPercent.min, forceStrengthRandomPercent.max);
    }

    private void FixedUpdate()
    {
        transform.position = transform.position + direction * forceStrength;

        forceStrength *= drag;
    }


}
