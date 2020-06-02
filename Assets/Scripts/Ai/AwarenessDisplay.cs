using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AwarenessDisplay : MonoBehaviour
{
    [Range(0, 1)] public float appearLerp;
    SpriteRenderer[] _renderers;
    float[] _initialAlphas;

    int currentLevel = -1;
    void Start()
    {
        _renderers = GetComponentsInChildren<SpriteRenderer>();
        _initialAlphas = new float[_renderers.Length];
        for (int i = 0; i < _renderers.Length; ++i)
        {
            _initialAlphas[i] = _renderers[i].color.a;
        }
    }
    void Update()
    {
        int n = _renderers.Length;
        for (int i = 0; i < n; ++i)
        {
            Color cl = _renderers[i].color;
            float desiredAlpha = i == currentLevel ? _initialAlphas[i] : 0.0f;
            cl.a = Mathf.Lerp(cl.a, desiredAlpha, appearLerp);
        }
    }

    public void SetAwareness(int i)
    {
        if (i >= _renderers.Length)
        {
            Debug.LogWarning("Trying to set awareness display to " + i + " which is more than available levels");
            return;
        }
        if (i < 0)
            return;

        currentLevel = i;
    }

    
}
