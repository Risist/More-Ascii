using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ai;

public class AiNoiseSourceVelocity : MonoBehaviour
{
    public float noiseThreshold;
    public float noiseAccumulatorFallPerSecond = 1.0f;
    [Space]
    public float hitVelocityScale = 1.0f;
    public float walkVelocityScale = 1.0f;
    float _noiseAccumulator;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.rigidbody)
            return;

        AiPerceiveUnit unit = collision.gameObject.GetComponent<AiPerceiveUnit>();
        if (!unit)
            return;

        _noiseAccumulator += collision.rigidbody.velocity.magnitude * hitVelocityScale;

        if (_noiseAccumulator > noiseThreshold && AiSenseNoise.CanSpreadNoise())
        {
            _noiseAccumulator = 0;

            NoiseData noiseData = new NoiseData();
            noiseData.position = collision.transform.position;
            noiseData.velocity = collision.rigidbody.velocity;
            noiseData.fraction = unit.fraction;

            AiSenseNoise.SpreadNoise(noiseData);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.attachedRigidbody)
            return;

        AiPerceiveUnit unit = collision.GetComponent<AiPerceiveUnit>();
        if (!unit)
            return;

        _noiseAccumulator += collision.attachedRigidbody.velocity.magnitude * walkVelocityScale;

        if (_noiseAccumulator > noiseThreshold && AiSenseNoise.CanSpreadNoise())
        {
            _noiseAccumulator = 0;

            NoiseData noiseData = new NoiseData();
            noiseData.position = collision.transform.position;
            noiseData.velocity = collision.attachedRigidbody.velocity;
            noiseData.fraction = unit.fraction;

            AiSenseNoise.SpreadNoise(noiseData);
        }

    }

    void FixedUpdate()
    {
        _noiseAccumulator -= noiseAccumulatorFallPerSecond * Time.fixedDeltaTime;
        if (_noiseAccumulator < 0)
            _noiseAccumulator = 0;
    }
}
