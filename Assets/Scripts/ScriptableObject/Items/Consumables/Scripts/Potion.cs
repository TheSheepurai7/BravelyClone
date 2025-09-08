using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Potion", menuName = "Scriptable Objects/Items/Consumables/Potion")]
public class Potion : Consumable
{
    public override void Use(List<StatBlock> targets)
    {
        targets[0].currentHP += 100;
        base.Use(targets);
    }
}
