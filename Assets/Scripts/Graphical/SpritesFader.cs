using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpritesFader : MonoBehaviour
{
    public Timer timerAppear;
    public Timer timerFull;
    public Timer timerRevert;
    float alpha;
    SpriteRenderer[] sprites;
    
    enum EState
    {
        EAppear,
        EFull,
        ERevert
    }
    EState state = EState.EAppear;


    void Start()
    {
        sprites = GetComponentsInChildren<SpriteRenderer>();
        alpha = sprites[0].color.a;
        timerAppear.Restart();

        Color cl = sprites[0].color;
        cl.a = 0;
        foreach (var it in sprites)
            it.color = cl;
    }

    // Update is called once per frame
    void Update()
    {
        Color cl = sprites[0].color;
        if(state == EState.EAppear)
        {
            cl.a = Mathf.Lerp(0, alpha, Mathf.Clamp01(timerAppear.ElapsedTime() / timerAppear.cd));
            foreach (var it in sprites)
                it.color = cl;
            if (timerAppear.IsReady())
            {
                timerFull.Restart();
                state = EState.EFull;
            }
        }else if(state == EState.EFull)
        {
            if(timerFull.IsReady())
            {
                state = EState.ERevert;
                timerRevert.Restart();
            }
        }
        else
        {
            cl.a = Mathf.Lerp(0, alpha, 1 - Mathf.Clamp01(timerRevert.ElapsedTime() / timerRevert.cd));
            foreach (var it in sprites)
                it.color = cl;
            if (timerRevert.IsReady())
            {
                Destroy(this);
            }
        }
    }
}
