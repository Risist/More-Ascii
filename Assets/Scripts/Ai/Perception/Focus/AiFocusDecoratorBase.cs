using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ai
{
    public abstract class FocusDecoratorBase : FocusOwned
    {
        protected FocusDecoratorBase(Transform transform, FocusBase child) : base(transform)
        {
            _child = child;
        }
        protected FocusBase _child;
    }
}
