//Namespaces for this miscellania
using System;
using System.Collections.Generic;
using UnityEngine;

//Misc delegates
public delegate void UpdateStats(Stats stat);
public delegate string CombatAction(CombatantInfo source, List<CombatantInfo> targets);

//Misc enums
public enum Character { ALEC, MARISA, JENNA, GARETH }
public enum Command { ATTACK, JOB, SUBJOB, DEFEND, ITEM }
public enum EquipType { SWORD, STAFF, KNIFE, AXE, SHIELD, HEAD, BODY, ACCESSORY }
public enum Job { NONE, KNIGHT, CLERIC, WIZARD, DRAGOON } //Add more later
public enum Stats { NAME, SPRITE, LVL, CHP, MHP, CMP, MMP, STR, PATK, VIT, PDEF, INT, MATK, MND, MDEF, AGI, JOB, JLV, SJB, SJV, AP }
public enum TargetTag { SINGLE, PARTY, ALL, SELF, ELSE, PARTY_ELSE, ALL_ELSE, FIELD }

//Misc interfaces
public interface IStatReader
{
    public Sprite ReadImage();
    public string ReadString(Stats stat);
    public int ReadInt(Stats stat);
    public float ReadFloat(Stats stat);
    public List<CommandInfo> ReadCommands(Stats stat);
    public void UpdateDisplay(ref UpdateStats theDelegate);
}

//Misc structs
public struct CommandInfo   
{
    public readonly string name;
    public readonly TargetTag tag;
    public readonly CombatAction action;
    public CommandInfo(string name, TargetTag tag, CombatAction action) { this.name = name; this.tag = tag; this.action = action; }
}

//Misc classes


public abstract class CombatantInfo : IStatReader
{
    //Constants
    const int AP_MAX = 3000;

    //Event
    public event UpdateStats onUpdate;

    //Stats
    public readonly Sprite sprite;
    public readonly string name;
    public readonly int pAtk;
    public readonly int mAtk;
    public readonly int iNT;
    public readonly int pDef;
    public readonly int mDef;
    public readonly int mND;
    public readonly int sPD;
    protected int apMax = AP_MAX;
    protected int maxHP;
    protected int maxMP;
    protected int bonusMax;
    protected bool active = true;

    //Properties
    private int _aP;
    public int aP { get { return _aP; } set { _aP = Mathf.Clamp(value, 0, apMax * 4); onUpdate?.Invoke(Stats.AP); } }

    private int _currentHP;
    public int currentHP { get { return _currentHP; } set { _currentHP = Mathf.Clamp(value, 0, maxHP + bonusMax); onUpdate?.Invoke(Stats.CHP); 
            if (_currentHP == 0) { OnDeath(); } } }

    private int _currentMP;
    public int currentMP { get { return _currentMP; } set { _currentMP = Mathf.Clamp(value, 0, maxMP); onUpdate?.Invoke(Stats.CMP); } }

    //Constructor
    public CombatantInfo(IStatReader statBlock, string name)
    {
        //Name
        this.name = name;

        //Sprite
        sprite = statBlock.ReadImage();

        //Other stats
        pAtk = statBlock.ReadInt(Stats.PATK);
        mAtk = statBlock.ReadInt(Stats.MATK);
        iNT = statBlock.ReadInt(Stats.INT);
        pDef = statBlock.ReadInt(Stats.PDEF);
        mDef = statBlock.ReadInt(Stats.MDEF);
        mND = statBlock.ReadInt(Stats.MND);
        sPD = statBlock.ReadInt(Stats.AGI);
    }

    //Other functions
    public void InitializeATB(int baseSpd, float rank) //Rank should be rank / ((Total combatants) + 2).
    {
        //First I need to calculate how speed reduces the AP cap with a formula I don't quite remember.
        apMax -= (int)(AP_MAX * (sPD - baseSpd) * 0.01f / 2);

        //Then I need to multiply that cap by the rank + or - up to 0.1f (But never exceeding 0.9f) to get the starting AP
        aP = (int)(apMax * Mathf.Clamp(rank + UnityEngine.Random.Range(-0.1f, 0.1f), 0.1f, 0.9f));
    }

    //Interface functions
    public Sprite ReadImage()
    {
        return sprite;
    }

    public string ReadString(Stats stat)
    {
        switch (stat)
        {
            case Stats.NAME: return name;
            default: throw new System.Exception(stat + " cannot be parsed as a string in CombatantInfo (Or the functionality hasn't been implemented yet).");
        }
    }

    //Should I abstract this?
    public abstract int ReadInt(Stats stat);

    public abstract List<CommandInfo> ReadCommands(Stats stat);

    public float ReadFloat(Stats stat)
    {
        switch (stat)
        {
            case Stats.AP: return (float)(aP % apMax) / apMax;
            default: throw new System.Exception(stat + " cannot be parsed as a float in CombatantInfo (Or the functionality hasn't been implemented yet).");
        }
    }

    public void UpdateDisplay(ref UpdateStats theDelegate)
    {
        onUpdate += theDelegate;
    }

    //Virtual functions
    protected virtual void OnDeath()
    {

    }
}

public static class Knight
{
    public static List<CommandInfo> ReturnCommands(int level)
    {
        return new List<CommandInfo>();
    }
}