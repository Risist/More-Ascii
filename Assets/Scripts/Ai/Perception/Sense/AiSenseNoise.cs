using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Ai;
using Random = UnityEngine.Random;

namespace Ai
{
    public class NoiseData
    {
        public Vector2 position;
        public Vector2 velocity;
        public AiFraction fraction;
    }
}

[RequireComponent(typeof(AiBehaviourController))]
public class AiSenseNoise : AiSenseBase
{
    #region Static
    static Action<NoiseData> onSpreadNoise = (data) => { };
    public static Timer tSpreadNoise = new Timer(0.25f);
    public static bool CanSpreadNoise() { return tSpreadNoise.IsReady(); }
    public static void SpreadNoise(NoiseData data)
    {
        if (tSpreadNoise.IsReadyRestart())
        {
            onSpreadNoise(data);
        }
    }
    #endregion Static

    [Header("Detecting")]
    [Range(0, 1)] public float reactionChance = 1.0f;
    public float hearingDistance = 1.0f;

    public StimuliStorage noiseStorage { get; protected set; }

    new void Awake()
    {
        base.Awake();

        //// initialize focus
        noiseStorage = RegisterSenseInBlackboard("noiseStorage");
    }

    void ReactToNoise(NoiseData data)
    {
        if (data.fraction && myUnit.fraction.GetAttitude(data.fraction) != AiFraction.EAttitude.EEnemy)
            return;

        Vector2 toNoise = (Vector2)transform.position - data.position;
        if (toNoise.sqrMagnitude > hearingDistance * hearingDistance || Random.value > reactionChance)
            return;

        MemoryEvent ev = new MemoryEvent();
        ev.velocity = data.velocity * velocityPredictionScale;
        ev.forward = data.velocity;
        ev.exactPosition = data.position;
        ev.lifetimeTimer.Restart();

        noiseStorage.PerceiveEvent(ev);

#if UNITY_EDITOR
        Debug.DrawRay(ev.exactPosition, Vector3.up, Color.blue, 0.5f);
#endif
    }

    void OnEnable() => onSpreadNoise += ReactToNoise;
    void OnDisable() => onSpreadNoise -= ReactToNoise;
}