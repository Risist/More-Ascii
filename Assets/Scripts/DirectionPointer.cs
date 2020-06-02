using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionPointer : MonoBehaviour
{
    [Range(0, 1)] public float directionLerpFactor = 0.2f;
    [Range(0, 1)] public float colorLerpFactor = 0.2f;
    InputHolder inputHolder;
    float lastAngle;

    SpriteRenderer[] sprites;
    float[] alphas;

    private void Start()
    {
        inputHolder = GetComponentInParent<InputHolder>();
        sprites = GetComponentsInChildren<SpriteRenderer>();
        alphas = new float[sprites.Length];
        for (int i = 0; i < sprites.Length; ++i)
            alphas[i] = sprites[i].color.a;
    }

    private void Update()
    {
        if(inputHolder.atDirection)
        {
            for (int i = 0; i < sprites.Length; ++i)
            {
                Color cl = sprites[i].color;
                cl.a = Mathf.LerpAngle(cl.a, alphas[i], colorLerpFactor);
                sprites[i].color = cl;
            }

            float desiredAngle = Vector2.SignedAngle(Vector2.up, inputHolder.directionInput);
            lastAngle = Mathf.LerpAngle(lastAngle, desiredAngle, directionLerpFactor);
            transform.rotation = Quaternion.Euler(0, 0, lastAngle);
        }
        else
        {
            for (int i = 0; i < sprites.Length; ++i)
            {
                Color cl = sprites[i].color;
                cl.a = Mathf.LerpAngle(cl.a, 0, colorLerpFactor);
                sprites[i].color = cl;
            }
        }
    }
}
