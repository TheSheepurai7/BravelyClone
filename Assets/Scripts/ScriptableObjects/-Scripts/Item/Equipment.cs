using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Scriptable Objects/Equipment")]
public class Equipment : Item, IGetStat
{
    //Type
    [SerializeField] EquipType equipType;

    //Stats (For now these are the only ones I need)
    [SerializeField] int pAtk;
    [SerializeField] int pDef;
    [SerializeField] int mAtk;
    [SerializeField] int mDef;
    [SerializeField] int iNT;
    [SerializeField] int mND;

    //Interface
    public string GetName(Stats stat)
    {
        return name;
    }

    public Vector2 GetCompoundStat(Stats stat)
    {
        throw new System.NotImplementedException();
    }

    public int GetStat(Stats stat)
    {
        switch (stat) 
        {
            case(Stats.PATK): return pAtk;
            case(Stats.PDEF): return pDef;
            case(Stats.MATK): return mAtk;
            case(Stats.MDEF): return mDef;
            case(Stats.INT): return iNT;
            case(Stats.MND): return mND;
            default: throw new System.NullReferenceException("GetStat in " + name + " cannot return a " + stat);
        }
    }

    public Sprite GetSprite()
    {
        throw new System.NotImplementedException();
    }

    public float GetFloatStat(Stats stat)
    {
        throw new System.NotImplementedException();
    }

    //Supertype checker
    public bool IsWeapon()
    {
        switch (equipType)
        {
            case EquipType.SWORD: return true;
            case EquipType.STAFF: return true;
            case EquipType.KNIFE: return true;
            case EquipType.AXE: return true;
            default: return false;
        }
    }

    public bool IsArmor()
    {
        switch (equipType)
        {
            case EquipType.HEAD: return true;
            case EquipType.BODY: return true;
            default: return false;
        }
    }
}
