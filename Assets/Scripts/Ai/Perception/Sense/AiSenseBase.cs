using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ai;

namespace Ai
{
    public class FocusFilterStorage : FocusOwned
    {
        public FocusFilterStorage( Transform transform, StimuliStorage storage,
            StimuliStorage.MeasureEventMethod measureMethod) 
            : base(transform)
        {
            Debug.Assert(storage != null);
            Debug.Assert(measureMethod != null);

            this.storage = storage;
            this.measureMethod = measureMethod;

        }

        // filters by distance from center
        public FocusFilterStorage(Transform transform, StimuliStorage storage, 
            float distanceUtilityScale = 0.0f, float timeUtilityScale = 1.0f, float maxLifetime = float.MaxValue )
            : base(transform)
        {
            Debug.Assert(storage != null);

            this.storage = storage;
            this.measureMethod = (MemoryEvent memoryEvent) =>
            {
                if (memoryEvent.elapsedDistance > maxLifetime)
                    return float.MinValue;

                float timeUtility = -memoryEvent.elapsedTime;
                float distanceUtility = -((Vector2)transform.position - memoryEvent.position).magnitude;

                float utility = timeUtility * timeUtilityScale + distanceUtility * distanceUtilityScale;
                return utility;
            };
        }

        // filters by distance from local position
        public FocusFilterStorage(Transform transform, StimuliStorage storage,
            Vector2 localPoint, float distanceUtilityScale = 0.0f, float timeUtilityScale = 1.0f, float maxLifetime = float.MaxValue)
            : base(transform)
        {
#if UNITY_EDITOR
            Debug.Assert(storage != null);
#endif

            this.storage = storage;
            this.measureMethod = (MemoryEvent memoryEvent) =>
            {
                /*if (memoryEvent.elapsedDistance > maxLifetime)
                    return float.MaxValue;*/

                float timeUtility = -memoryEvent.elapsedTime;
                float distanceUtility = -((Vector2)transform.TransformPoint(localPoint) - memoryEvent.position).magnitude;

                float utility = timeUtility * timeUtilityScale + distanceUtility * distanceUtilityScale;
                return utility;
            };
        }


        protected StimuliStorage.MeasureEventMethod measureMethod;
        protected StimuliStorage storage;

        // event cache mechanism
        // used to ensure only one memory search will be performed
        // Time.time stays constant throughout frame so we can check if the value has changed since
        // if soo we update stored event
        protected void UpdateEvent()
        {
            if (Time.time != lastFrame)
            {
                lastFrame = Time.time;
                lastEvent = storage.FindBestEvent(measureMethod);
                if(lastEvent != null)
                    Debug.DrawLine(lastEvent.exactPosition, lastEvent.position, Color.yellow, Time.fixedDeltaTime, false);
            }
        }
        float lastFrame = -1;
        MemoryEvent lastEvent;

        public override MemoryEvent GetTarget()
        {
            UpdateEvent();
            return lastEvent;
        }
    }

    public class StimuliStorage
    {
        public StimuliStorage(int nEvents, float maxEventLifetime)
        {
            memoryEvents = new MemoryEvent[nEvents];
            this.maxEventLifetime = maxEventLifetime;
        }

        float maxEventLifetime;
        MemoryEvent[] memoryEvents;
        int lastAddedEvent = -1;

        // pushes event onto perceived events list
        // restarts lifetimeTimer
        public void PerceiveEvent(MemoryEvent newEvent)
        {
#if UNITY_EDITOR
            if (newEvent == null)
            {
                Debug.LogWarning("Trying to register null as MemoryEvent");
                return;
            }
#endif
            // check if in memory there is any event caused by the same unit as newEvent
            if(newEvent.perceiveUnit)
            {
                for(int i = 0; i < memoryEvents.Length; ++i)
                    if(memoryEvents[i] != null && memoryEvents[i].perceiveUnit == newEvent.perceiveUnit)
                    {
                        memoryEvents[i] = null;
                        break;
                    }
            }

            lastAddedEvent = (lastAddedEvent + 1) % memoryEvents.Length;
            memoryEvents[lastAddedEvent] = newEvent;
            newEvent.lifetimeTimer.Restart();
        }

        // measures passed memory event
        // argument will never be a null
        public delegate float MeasureEventMethod(MemoryEvent memoryEvent);

        // returns best event from registered ones by measureEventMethod
        // the one with greatest evaluation value will be returned or null if none is valid
        // Events are considered as valid if they are not null and they are not older than maxEventLifeTime
        public MemoryEvent FindBestEvent(MeasureEventMethod measureEventMethod)
        {
            MemoryEvent bestEvent = null;
            float bestUtility = float.MinValue;
            
            foreach (var it in memoryEvents)
            {
                if (it == null || it.elapsedTime > maxEventLifetime)
                    continue;

                
                float utility = measureEventMethod(it);
                if (utility > bestUtility)
                {
                    bestUtility = utility;
                    bestEvent = it;
                }
            }
            return bestEvent;
        }
    }
}

public abstract class AiSenseBase : MonoBehaviour
{
    [Header("General")]
    public int nEvents;
    public float maxEventLifeTime;
    public float velocityPredictionScale = 1.0f;

    
    protected AiPerceiveUnit myUnit;
    protected AiBehaviourController behaviourController;


    protected void Awake()
    {
        myUnit = GetComponentInParent<AiPerceiveUnit>();
        behaviourController = GetComponentInChildren<AiBehaviourController>();
    }

    // registers event
    protected StimuliStorage RegisterSenseInBlackboard(string blackboardName)
    {
        Debug.Assert(behaviourController);
        var storage = behaviourController.InitBlackboardValue(blackboardName, () => new StimuliStorage(nEvents, maxEventLifeTime) );
        return storage.value;
    }

}
