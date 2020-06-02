using Ai;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 *
 * OverlapCircle check by memorableMask
 * to all hit targets which have AiPerceiveUnit and are in given cone cast ray
 * The ray will check if there is no obstacle at the way from perceiver to target
 *
 * All targets that pass the test will be pushed into special AiFocus type
 *      that will store targets and sort them when needed
 *
 */
[RequireComponent(typeof(AiBehaviourController))]
public class AiSenseSight : AiSenseBase
{
    [Header("VisionSettings")]
    public LayerMask memorableMask;
    public LayerMask obstacleMask;
    public bool trackEnemy = true;
    public bool trackAlly = false;
    public bool trackNeutrals = false;


    [Header("Shape")]
    public float coneAngle = 170.0f;
    public float searchDistance = 5.0f;

    protected new Transform transform;

    public StimuliStorage enemyStorage { get; protected set; }
    public StimuliStorage allyStorage { get; protected set; }
    public StimuliStorage neutralStorage { get; protected set; }
    void InsertEvent(MemoryEvent ev, AiFraction.EAttitude attitude)
    {
        switch (attitude)
        {
            case AiFraction.EAttitude.EEnemy:
                if(trackEnemy)
                    enemyStorage.PerceiveEvent(ev);
                break;
            case AiFraction.EAttitude.EFriendly:
                if (trackAlly)
                    allyStorage.PerceiveEvent(ev);
                break;
            case AiFraction.EAttitude.ENeutral:
                if (trackNeutrals)
                    neutralStorage.PerceiveEvent(ev);
                break;
        }
    }

    private new void Awake()
    {
        base.Awake();

        // cache transform
        transform = base.transform;

        enemyStorage = RegisterSenseInBlackboard("enemyStorage");
        allyStorage = RegisterSenseInBlackboard("allyStorage");
        neutralStorage = RegisterSenseInBlackboard("neutralStorage");
    }

    #region FocusManager
    static readonly List<AiSenseSight> _sightList = new List<AiSenseSight>();
    static float searchTime;
    protected void OnEnable() => _sightList.Add(this);
    protected void OnDisable() => _sightList.Remove(this);

    public static IEnumerator PerformSearch_Coroutine(float searchTime)
    {
        AiSenseSight.searchTime = searchTime;
        var wait = new WaitForSeconds(searchTime);
        while (true)
        {
            yield return wait;

            foreach (var it in _sightList)
                it.PerformSearch();
        }
    }

    #endregion FocusManager

    #region Search
    void PerformSearch()
    {
        AiFraction myFraction = myUnit.fraction;
        if (!myFraction)
        {
#if UNITY_EDITOR
            Debug.LogWarning("No fraction in perceive unit but trying to use sight");
#endif 
            // there's no way to determine where to put events
            return;
        }

        // perform cast
        int n = Physics2D.OverlapCircleNonAlloc(transform.position, searchDistance, StaticCacheLists.colliderCache, memorableMask);

        // preselect targets
        // they have to be in proper angle and contain PerceiveUnit
        for (int i = 0; i < n; ++i)
        {
            var it = StaticCacheLists.colliderCache[i];
            Transform itTransform = it.transform;

            //// check if the target is in proper angle
            Vector2 toIt = itTransform.position - transform.position;
            float cosAngle = Vector2.Dot(toIt.normalized, transform.up);
            float angle = Mathf.Acos(cosAngle) * 180 / Mathf.PI;
            //Debug.Log(angle);
            bool bProperAngle = angle < coneAngle * 0.5f;
            if (!bProperAngle)
                continue;

            // ok, now check if it has AiPerceiveUnit
            // we need it's fraction to determine our attitude

            AiPerceiveUnit perceiveUnit = it.GetComponent<AiPerceiveUnit>();
            if (perceiveUnit == myUnit)
                // oh, come on do not look at yourself... don't be soo narcissistic
                continue;

            if (!perceiveUnit)
                // no perceive unit, this target is invisible to us
                continue;

            AiFraction itFraction = perceiveUnit.fraction;
            if (!itFraction)
                // the same as above,
                return;

            //// determine attitude
            AiFraction.EAttitude attitude = myFraction.GetAttitude(itFraction);

            //// Check if obstacles blocks vision
            if (DoObstaclesBlockVision(itTransform.position))
                continue;

            //// create event
            MemoryEvent ev = new MemoryEvent();
            ev.exactPosition = itTransform.position;
            ev.forward = itTransform.up;
            // if collider has rigidbody then take its velocity
            // otherwise there is no simple way to determine event velocity
            ev.velocity = it.attachedRigidbody ? it.attachedRigidbody.velocity * velocityPredictionScale : Vector2.zero;

            // set up agent reponsible for this event
            ev.perceiveUnit = perceiveUnit;

            // ensure event will tick from now on
            ev.lifetimeTimer.Restart();


            Debug.DrawRay(ev.exactPosition, Vector3.up, Color.blue, searchTime * nEvents);
            Debug.DrawRay(ev.exactPosition, ev.velocity * searchTime, Color.gray, searchTime);
            InsertEvent(ev, attitude);
        }
    }

    bool DoObstaclesBlockVision(Vector2 target)
    {
        // we will change searchDistance based on visibility of obstacles;
        float searchDistance = this.searchDistance;

        Vector2 toTarget = target - (Vector2)transform.position;
        float toTargetSq = toTarget.sqrMagnitude;


        int n = Physics2D.RaycastNonAlloc(transform.position, toTarget, StaticCacheLists.raycastHitCache, toTarget.magnitude, obstacleMask);

        bool bObstaclesBlocksVision = false;
        for (int i = 0; i < n; ++i)
        {
            var it = StaticCacheLists.raycastHitCache[i];
            AiPerceiveUnit unit = it.collider.GetComponent<AiPerceiveUnit>();
            if (!unit)
            {
                // we assume objects that do not have perceive unit will behave as non transparent
                // so we can't see our target
                bObstaclesBlocksVision = true;
                break;
            }

            if (unit == myUnit)
                // well, i'm not that fat ... i guess
                continue;


            searchDistance *= unit.transparencyLevel;
            if (searchDistance * searchDistance < toTargetSq * myUnit.distanceModificator)
            // transparency is reduced too much to see the target
            {
                bObstaclesBlocksVision = true;
                break;
            }
        }

        Debug.DrawRay(transform.position, toTarget, bObstaclesBlocksVision ? Color.yellow : Color.green, 0.25f);


        return bObstaclesBlocksVision;
    }
    #endregion Search


    void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        UnityEditor.Handles.color = Color.red;

        if (!transform)
            // logged errors without that
            // btw that's weird 
            return;

        Vector3 start = transform.position;
        Vector3 end = start + Quaternion.Euler(0, 0, -coneAngle * 0.5f) * transform.up * searchDistance;
        UnityEditor.Handles.DrawLine(start, end);

        end = start + Quaternion.Euler(0, 0, coneAngle * 0.5f) * transform.up * searchDistance;
        UnityEditor.Handles.DrawLine(start, end);


        UnityEditor.Handles.DrawWireDisc(transform.position, transform.forward, searchDistance);
#endif
    }
}
