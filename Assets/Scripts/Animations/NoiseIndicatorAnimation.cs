using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseIndicatorAnimation : MonoBehaviour
{
    public Timer tAnimation;
    public float desiredObjectScale;
    [Range(0, 1)] public float desiredObjectScaleLerp;
    public float distanceScale = 1.01f;

    private void Start()
    {
        tAnimation.Restart();
    }

    void FixedUpdate()
    {
        if (tAnimation.IsReady()){
            Destroy(gameObject);
            return;
        }

        int n = transform.childCount;
        for(int i = 0; i < n; ++i)
        {
            var it = transform.GetChild(i);
            it.localPosition *= distanceScale;
        }
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one*desiredObjectScale, desiredObjectScaleLerp);
    }
}
