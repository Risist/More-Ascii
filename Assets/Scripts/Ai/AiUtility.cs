using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ai;
using Pathfinding;

namespace Ai
{
    public class LookAround
    {
        public LookAround() { }
        public LookAround(RangedFloat tChangeAngle)
        {
            this.tChangeAngle = tChangeAngle;
        }
        public LookAround(RangedFloat tChangeAngle, RangedFloat desiredAngleDifference)
        {
            this.tChangeAngle = tChangeAngle;
            this.desiredAngleDifference = desiredAngleDifference;
        }

        public float rotationLerp;
        public RangedFloat desiredAngleDifference = new RangedFloat(45.0f, 120.0f);
        public RangedFloat tChangeAngle = new RangedFloat(1.0f, 1.0f);

        Timer tChangeDesiredAngle = new Timer();
        float desiredAngle;
        float angle;

        #region Events
        public void Begin(Transform transform)
        {
            SetNewDesiredAngle(transform);
            tChangeDesiredAngle.cd = Random.Range(tChangeAngle.min, tChangeAngle.max);
            tChangeDesiredAngle.Restart();

            angle = transform.eulerAngles.z;
        }
        #endregion Events

        #region Public functions
        public void SetNewDesiredAngle(Transform transform)
        {
            SetNewDesiredAngle(transform, transform.rotation.eulerAngles.z);
        }
        public void SetNewDesiredAngle(Transform transform, float rotation)
        {
            float diff = Random.Range(desiredAngleDifference.min, desiredAngleDifference.max);
            diff = Random.value > 0.5f ? diff : -diff;

            desiredAngle = rotation + diff;
        }
        public void SetNewDesiredAngle(Transform transform, float rotation, RangedFloat desiredAngleDifference)
        {
            float diff = Random.Range(desiredAngleDifference.min, desiredAngleDifference.max);
            diff = Random.value > 0.5f ? diff : -diff;

            desiredAngle = rotation + diff;
        }

        public Vector2 GetRotationInput(Transform transform, float rotation, float rotationLerp = 0.1f)
        {
            return GetRotationInput(transform, rotation, desiredAngleDifference, rotationLerp);
        }
        public Vector2 GetRotationInput(Transform transform, float rotationLerp = 0.1f)
        {
            return GetRotationInput(transform, transform.eulerAngles.z, desiredAngleDifference, rotationLerp);
        }
        public Vector2 GetRotationInput(Transform transform, RangedFloat desiredAngleDifference, float rotationLerp = 0.1f)
        {
            return GetRotationInput(transform, transform.eulerAngles.z, desiredAngleDifference, rotationLerp);
        }
        public Vector2 GetRotationInput(Transform transform, float rotation, RangedFloat desiredangleDifference, float rotationLerp = 0.1f)
        {
            if (tChangeDesiredAngle.IsReadyRestart())
            {
                SetNewDesiredAngle(transform, rotation, desiredangleDifference);
                tChangeDesiredAngle.cd = Random.Range(tChangeAngle.min, tChangeAngle.max);
            }

            angle = Mathf.LerpAngle(angle, desiredAngle, rotationLerp);
            Vector2 desiredRotationInput = Quaternion.Euler(0, 0, angle) * Vector2.up;
            return desiredRotationInput;
        }

        public bool CloseEnoughToDediredAngle(Transform transform, float minAngleDifference = 0.0f)
        {
            return Mathf.Abs(Mathf.DeltaAngle(transform.rotation.eulerAngles.z, desiredAngle)) < minAngleDifference;
        }
        #endregion Public functions
    }

    public class MoveToDestination
    {
        public MoveToDestination(Transform transform)
        {
            this.transform = transform;
        }

        public Transform transform { get; private set; }
        public Vector2 destination { get; private set; }
        public Vector2 toDestination { get { return destination - (Vector2)transform.position; } }

        #region New Target
        /// moves to destination in area around target (max in search area)
        /// if target is null treat yourself as target
        /// target position is predicted in future, You can adjust the prediction via @maxRespectedTime and @timeScale 
        /// @maxRespectedTime -> clamps how much in future the event will be predicted
        /// @timeScale -> specifies how important prediction is 
        public void SetDestination_Search(FocusOwned focus, float searchArea, float maxRespectedTime = float.MaxValue, float timeScale = 1.0f)
        {
            if (!focus.HasTarget())
            {
                destination = (Vector2)transform.position + Random.insideUnitCircle * searchArea;
                return;
            }

            destination = focus.GetTarget().GetPosition(maxRespectedTime, timeScale) + Random.insideUnitCircle * searchArea;
        }
        public void SetDestination_Flee(FocusOwned focus, RangedFloat desiredDistance, float searchArea = 1.0f)
        {
            if (!focus.HasTarget())
            {
                destination = (Vector2)transform.position + Random.insideUnitCircle.normalized * desiredDistance.GetRandom() + Random.insideUnitCircle * searchArea;
                return;
            }

            Vector2 toTarget = -focus.ToTarget();
            destination = (Vector2)transform.position + toTarget.normalized * desiredDistance.GetRandom() + Random.insideUnitCircle * searchArea;
        }
        public void SetDestination(Vector2 d)
        {
            destination = d;
        }


        #endregion New Target


        public bool IsCloseToDestination(float closeDistance)
        {
            return toDestination.sqrMagnitude < closeDistance * closeDistance;
        }
    }
    public class MoveToDestinationNavigation : MoveToDestination
    {
        public MoveToDestinationNavigation(Seeker seeker) : base(seeker.transform)
        {
            this.seeker = seeker;
        }

        public Seeker seeker { get; private set; }
        public Path path { get; private set; }
        public int nodeId { get; private set; }
        public bool finished { get => path != null && nodeId >= path.vectorPath.Count; }

        #region New Target
        /// moves to destination in area around target (max in search area)
        /// if target is null treat yourself as target
        /// target position is predicted in future, You can adjust the prediction via @maxRespectedTime and @timeScale 
        /// @maxRespectedTime -> clamps how much in future the event will be predicted
        /// @timeScale -> specifies how important prediction is 
        new public void SetDestination_Search(FocusOwned focus,float searchArea, float maxRespectedTime = float.MaxValue, float timeScale = 1.0f)
        {
            base.SetDestination_Search(focus, searchArea, maxRespectedTime, timeScale);
            SetDestination(destination);
        }
        new public void SetDestination_Flee(FocusOwned focus, RangedFloat desiredDistance, float searchArea = 1.0f)
        {
            base.SetDestination_Flee(focus, desiredDistance, searchArea);
            SetDestination(destination);
        }
        new public void SetDestination(Vector2 d)
        {
            base.SetDestination(d);
            seeker.detailedGizmos = true;
            seeker.StartPath(transform.position, destination, (Path p) => { path = p; });
            nodeId = 0;
        }


        #endregion New Target
        public void RepathAsNeeded(Vector2 currentTargetPosition, float maxDistanceFromDestination)
        {
            Vector2 toDestination = currentTargetPosition - destination;
            if (toDestination.sqrMagnitude >= maxDistanceFromDestination.Sq() || finished)
                SetDestination(currentTargetPosition);
        }


        public Vector2 ToDestination(Vector2 currentTargetPosition, float correctionScale = 0.5f, float closeDist = 1.5f, float nextNodeDist = 0.5f)
        {
            Vector2 toDestination = ToDestination(closeDist, nextNodeDist);
            if (toDestination == Vector2.zero)
                return Vector2.zero;

            return Vector2.Lerp(toDestination, currentTargetPosition - (Vector2)transform.position, correctionScale);
        }
        public Vector2 ToDestination(float closeDist = 1.5f, float nextNodeDist = 0.5f)
        {
            if (IsCloseToDestination(closeDist))
                return Vector2.zero;

            if (path != null && path.IsDone())
            {
                Vector2 toDest = Vector2.zero;
                float nextNodeDistSq = nextNodeDist.Sq();
                while (nodeId < path.vectorPath.Count)
                {
                    toDest = path.vectorPath[nodeId] - transform.position;
                    if (toDest.sqrMagnitude >= nextNodeDistSq)
                        break;

                    ++nodeId;
                };

                //Debug.DrawRay(path.vectorPath[nodeId], Vector2.up, Color.yellow, 0.05f);
                return toDest;
            }

            return toDestination;
        }
    }

    // TODO character state detection


    // TODO interface as base a.ka FocusOwned
    // having virtual functions for velocity, position

    public class VelocityAccumulator
    {
        public VelocityAccumulator(float damping = 0.95f, float correctionFactor = 0.5f)
        {
            this.damping = damping;
            this.correctionFactor = correctionFactor;
        }
        public Vector2 velocity { get; private set; }
        public Vector2 position { get; private set; }
        public float damping;
        public float correctionFactor;


        public void ResetVelocity()
        {
            velocity = Vector2.zero;
        }
        public void Update(Transform transform, MemoryEvent memoryEvent, float ratio = 1.0f)
        {
            if (memoryEvent == null)
                return;


            // when close 0, when far 1, scaled with distance (1 - sqrtMagnitude * smthing) ? 
            float distance = Mathf.Clamp(((Vector2)transform.position - memoryEvent.position).sqrMagnitude  , 0, 20);


            //float distSq = ((Vector2)transform.position - memoryEvent.position).sqrMagnitude;
            /*float d = Mathf.Clamp01(distance / 20);
            float map01(float s, float b1, float b2)
            {
                return b1 + s * (b2 - b1);
            }
            d = map01(d, 0.65f, 0.9f);*/

            /*{
                Vector2 circlePosition = transform.position;
                Vector2 linePosition = memoryEvent.position;
                float circleRadius = 2.5f;

                Vector2 vNorm = memoryEvent.velocity.normalized;
                Vector2 diff = circlePosition - linePosition;
                float dot = Vector2.Dot(vNorm, diff);
                Vector2 d = linePosition + vNorm * dot;

                float distSq = (d - circlePosition).sqrMagnitude;
                //if ( distSq < circleRadius * circleRadius )
                   velocity *= Mathf.Lerp(0f, damping, dot * distSq/250);
                //else
                    //velocity *= damping;

            }*/
            if (CollisionCheck(transform.position, 2.5f, memoryEvent.position, memoryEvent.velocity * 3))
                velocity = Vector2.zero;
            else 
                velocity *= damping;

            velocity += memoryEvent.velocity * ratio;
            
            position = Vector2.Lerp(position, memoryEvent.position, correctionFactor);

            float f = velocity.sqrMagnitude;// * 0.5f;
            position += velocity * f;// * 0.1f;// * f;
        }

        bool CollisionCheck(Vector2 circlePosition, float circleRadius, Vector2 linePosition, Vector2 velocity)
        {
            Vector2 vNorm = velocity.normalized;
            Vector2 diff = circlePosition - linePosition;
            float dot = Vector2.Dot(vNorm, diff);
            Vector2 d = linePosition + vNorm * dot;

            return 
                // line collision
                (d - circlePosition).sqrMagnitude < circleRadius * circleRadius && 
                // segment collision
                dot * dot < diff.sqrMagnitude;
        }
    }
}
