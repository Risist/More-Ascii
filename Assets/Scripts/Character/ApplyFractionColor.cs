using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyFractionColor : MonoBehaviour
{
    public void Start()
    {
        ApplyColor();
    }

    public void ApplyColor()
    {
        var unit = GetComponentInParent<AiPerceiveUnit>();
        var renderers = GetComponentsInChildren<SpriteRenderer>();


        var fraction = unit.fraction;
        if (fraction == null)
            return;

        var color = fraction.fractionColorUi;
        color.a = renderers[0].color.a;

        foreach (var it in renderers)
        {
            it.color = color;
        }
    }
}
