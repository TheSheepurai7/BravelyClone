using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Scriptable Objects/Equipment")]
public class Equipment : Item
{
    //Equip stats
    [SerializeField] EquipType equipType;
    [SerializeField] int pAtk;
    [SerializeField] int mAtk;
    [SerializeField] int pDef;
    [SerializeField] int mDef;
    [SerializeField] int mND;
    //Probably a list for innates here to pass along and a function that does that

    public int GetStat(Stats stat)
    {
        switch (stat)
        {
            case Stats.PATK: return pAtk;
            case Stats.MATK: return mAtk;
            case Stats.PDEF: return pDef;
            case Stats.MDEF: return mDef;
            case Stats.MND: return mND;
            default: throw new System.Exception("Equipment don't have a(n) " +  stat + " stat.");
        }
    }
}
