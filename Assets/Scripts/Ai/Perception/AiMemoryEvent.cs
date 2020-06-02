using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{

    /*
     * Stores singular information about perceived event
     */
    public class MemoryEvent
    {
        public MinimalTimer lifetimeTimer = new MinimalTimer();

        /// position of the event source at record time
        public Vector2 exactPosition;

        /// facing direction of the event; Vector.zero if not applicable
        public Vector2 forward;

        /// direction the movement proceeds when perceived
        public Vector2 velocity;

        /// unity responsible for this event or null if unknown or none
        public AiPerceiveUnit perceiveUnit;


        /// distance which target could possibly travel from the start of the event assuming same speed
        public float elapsedDistance => velocity.magnitude * lifetimeTimer.ElapsedTime();

        public float elapsedTime => lifetimeTimer.ElapsedTime();

        /// fully computed position predicted by this event
        /// comes up with uncertainity area, and linear interpolation of current position by direction and speed of the event
        public Vector2 position => exactPosition + velocity * elapsedTime.Sq() / (elapsedTime.Sq() + 1);

        public Vector2 GetPosition(float maxRespectedTime = float.MaxValue, float timeScale = 1.0f)
        {
            float time = lifetimeTimer.ElapsedTime();
            time = Mathf.Clamp(time, 0, maxRespectedTime);

            return exactPosition + timeScale * velocity * time;
        }
    }
}
