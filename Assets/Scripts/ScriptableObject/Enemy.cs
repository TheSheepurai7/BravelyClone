using System.Buffers;
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

    public List<CommandInfo> ReadCommands(Stats stat)
    {
        throw new System.NotImplementedException();
    }

    public float ReadFloat(Stats stat)
    {
        throw new System.NotImplementedException("Float not parsed");
    }

    public Sprite ReadImage()
    {
        return sprite;
    }

    public int ReadInt(Stats stat)
    {
        switch (stat) 
        {
            case Stats.MHP: return hP;
            case Stats.MMP: return mP;
            case Stats.PATK: return pAtk;
            case Stats.PDEF: return pDef;
            case Stats.AGI: return sPD;
            default: throw new System.Exception(stat + " could not be parsed as an int in Enemy");
        }
    }

    public string ReadString(Stats stat)
    {
        throw new System.NotImplementedException("String not parsed");
    }

    public void UpdateDisplay(ref CharacterDisplay display)
    {
        
    }
}
