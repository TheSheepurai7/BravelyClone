using System.Collections.Generic;
using UnityEngine;

public abstract class Consumable : ScriptableObject
{
    public TargetTag tag;

    public virtual void Use(List<StatBlock> targets)
    {
        Destroy(this);
    }
}
