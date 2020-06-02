using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUiIndicatorAnimation : MonoBehaviour
{
    public bool use;
    public int id;
    protected CharacterUiIndicator indicator;

    private void Start()
    {
        indicator = GetComponentInParent<CharacterUiIndicator>();
    }
    private void Update()
    {
        indicator.animationIndicators[id].use = use;
    }
}