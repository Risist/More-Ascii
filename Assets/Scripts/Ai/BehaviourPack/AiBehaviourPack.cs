using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AiBehaviourPack : ScriptableObject
{
    public abstract void InitBehaviours(AiBehaviourController controller);
}
