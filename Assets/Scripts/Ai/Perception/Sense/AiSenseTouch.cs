using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ai;

public class AiSenseTouch : AiSenseBase
{
    public Timer tTouch = new Timer(0);
    public float minimalRelativeVelocity;

    public StimuliStorage touchStorage { get; protected set; }

    new void Awake()
    {
        base.Awake();

        //// initialize focus
        touchStorage = RegisterSenseInBlackboard("touchStorage");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // omit too weak touches
        if (collision.relativeVelocity.sqrMagnitude < minimalRelativeVelocity * minimalRelativeVelocity)
            return;

        // only targets with perceive unit will report
        // mostly to allow to tune what is perceived
        AiPerceiveUnit otherUnit = collision.gameObject.GetComponent<AiPerceiveUnit>();
        if (!otherUnit)
            return;


        // only report every given seconds
        if (!tTouch.IsReadyRestart())
            return;

        var otherRigidbody = collision.rigidbody;
        var otherTransform = collision.transform;

        MemoryEvent ev = new MemoryEvent();
        ev.exactPosition = otherTransform.position;
        ev.forward = otherTransform.up;
        ev.velocity = otherRigidbody ? otherRigidbody.velocity : Vector2.zero;
        ev.lifetimeTimer.Restart();

        touchStorage.PerceiveEvent(ev);

#if UNITY_EDITOR
        Debug.DrawRay(ev.exactPosition, Vector3.up, Color.blue);
#endif
    }
}