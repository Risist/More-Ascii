using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EDamageType
{
    EPhysical,
    EFire,
};

[Serializable]
public class DamageData
{
    public float damage;
    public float staggerIncrease;
    public EDamageType damageType = EDamageType.EPhysical;
    [NonSerialized] public Vector3 position;
    [NonSerialized] public Vector3 direction;
}

/// interface for all objects that can be damaged
public interface IDamageable
{
    void DealDamage(DamageData data);
}

