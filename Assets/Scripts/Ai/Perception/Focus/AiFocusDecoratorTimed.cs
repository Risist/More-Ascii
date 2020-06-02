using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    /*
     * Ensures that event is in given time range
     */
    public class FocusDecoratorTimed : FocusDecoratorBase
    {
        public FocusDecoratorTimed(Transform transform, FocusBase child, float maxTime) : base(transform, child)
        {
            _timeRestriction = new RangedFloat(0,maxTime);
        }
        public FocusDecoratorTimed(Transform transform, FocusBase child, RangedFloat timeRestriction) : base(transform, child)
        {
            _timeRestriction = timeRestriction;
        }

        public override bool HasTarget()
        {
            return _timeRestriction.InRange(_child.GetTarget().elapsedTime);
        }
        public override MemoryEvent GetTarget()
        {
            return _child.GetTarget();
        }

        readonly RangedFloat _timeRestriction;
    }
}
