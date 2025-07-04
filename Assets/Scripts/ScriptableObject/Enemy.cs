using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class Enemy : ScriptableObject, IStatReader
{
    //Its stats (for the time being)
    [SerializeField] Sprite sprite;
    [SerializeField] int hP;
    [SerializeField] int mP;
    [SerializeField] int pAtk;
    [SerializeField] int mAtk;
    [SerializeField] int iNT;
    [SerializeField] int pDef;
    [SerializeField] int mDef;
    [SerializeField] int mND;
    [SerializeField] int sPD;
    [SerializeField] int jP;
    [SerializeField] int xP;
    [SerializeField] int pG;

    public Sprite ReadImage()
    {
        return sprite;
    }

    public string ReadString(Stats stat)
    {
        switch (stat)
        {
            case Stats.NAME: return name;
            default: throw new System.Exception(stat + " cannot be parsed as a name in Enemy (Or the functionality hasn't been implemented yet).");
        }
    }

    public int ReadInt(Stats stat)
    {
        switch (stat)
        {
            case Stats.PATK: return pAtk;
            case Stats.MATK: return mAtk;
            case Stats.INT: return iNT;
            case Stats.PDEF: return pDef;
            case Stats.MDEF: return mDef;
            case Stats.MND: return mND;
            case Stats.AGI: return sPD;
            case Stats.MHP: return hP;
            case Stats.MMP: return mP;
            default: throw new System.Exception(stat + " cannot be parsed as an int in Enemy (Or the functionality hasn't been implemented yet).");
        }
    }

    public float ReadFloat(Stats stat)
    {
        throw new System.Exception(stat + " cannot be parsed as a float in Enemy (Or the functionality hasn't been implemented yet).");
    }

    public void SubscribeDelegate(ref UpdateStats theDelegate)
    {

    }
}
