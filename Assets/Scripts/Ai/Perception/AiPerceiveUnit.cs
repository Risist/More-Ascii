using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Marker for objects captured by perception system
 */
public class AiPerceiveUnit : MonoBehaviour
{
    [Tooltip("modifies how far the agents will perceive this unit")]
    public float distanceModificator = 1.0f;

    [Tooltip("modifies how far (%) agents will be able to see through the object")]
    [Range(0, 1)] public float transparencyLevel = 1.0f;

    public AiFraction fraction;
}
