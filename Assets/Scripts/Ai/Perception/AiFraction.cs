using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Fraction", menuName = "Ris/Ai/Fraction")]
public class AiFraction : ScriptableObject
{
    public enum EAttitude
    {
        EFriendly,
        ENeutral,
        EEnemy,
        ENone
    }
    public Color fractionColorUi;
    public AiFraction[] friendlyFractions;
    public AiFraction[] enemyFractions;

    public EAttitude GetAttitude(AiFraction fraction)
    {
        if (Equals(fraction))
            return EAttitude.EFriendly;

        foreach (var it in friendlyFractions)
            if (it.Equals(fraction))
                return EAttitude.EFriendly;

        foreach (var it in enemyFractions)
            if (it.Equals(fraction))
                return EAttitude.EEnemy;

        return EAttitude.ENeutral;
    }
}
