using System.Collections.Generic;
using UnityEngine;

public class Consumable
{
    public readonly string name;
    public readonly TargetTag tag;
    CombatAction effect;
    public Consumable(string name, TargetTag tag, CombatAction effect) { this.name = name; this.tag = tag; this.effect = effect; }

    public void Use(StatBlock source, List<StatBlock> targets)
    {
        effect(source, targets);
        GameManager.instance.RemoveItem(this);
    }
}

public static class ConsumableEffects
{
    public static void Potion(StatBlock source, List<StatBlock> targets)
    {
        targets[0].currentHP += 100;
        source.aP -= source.apMax;
        GameManager.instance.ForceGlobalThreat(ref source, 100);
    }
}
