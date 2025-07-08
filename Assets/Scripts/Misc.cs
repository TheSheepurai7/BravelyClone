//Namespaces for this miscellania
using System;
using System.Collections.Generic;
using UnityEngine;

//The miscellania
public delegate void UpdateStats(Stats stat);
public delegate List<string> CombatAction(CombatantInfo source, List<CombatantInfo> targets); //Actually I want this to return a list of strings and print it out to the display
public enum Character { ALEC, MARISA, JENNA, GARETH }
public enum Stats { NAME, SPRITE, LVL, CHP, MHP, CMP, MMP, STR, PATK, VIT, PDEF, INT, MATK, MND, MDEF, AGI, JOB, JLV, AP }
public enum EquipType { SWORD, STAFF, KNIFE, AXE, SHIELD, HEAD, BODY, ACCESSORY }
public enum TargetTag { SINGLE, PARTY, ALL, SELF, ELSE, PARTY_ELSE, ALL_ELSE, FIELD }

public interface IStatReader
{
    public Sprite ReadImage();
    public string ReadString(Stats stat);
    public int ReadInt(Stats stat);
    public float ReadFloat(Stats stat);
    public void SubscribeDelegate(ref UpdateStats theDelegate);
}

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

    //Properties
    private int _aP;
    public int aP { get { return _aP; } set { _aP = Mathf.Clamp(value, 0, apMax * 4); onUpdate?.Invoke(Stats.AP); } }
    private int _currentHP;
    public int currentHP { get { return _currentHP; } set { _currentHP = Mathf.Clamp(value, 0, maxHP + bonusMax); onUpdate?.Invoke(Stats.CHP); } }
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

    public CombatAction Command(string commandName, ref TargetTag targetTag)
    {
        switch(commandName)
        {
            case "Attack": targetTag = TargetTag.SINGLE; return Attack;
            case "Defend": targetTag = TargetTag.SELF; return Defend;
            default: throw new Exception(commandName + " is not a command that " + name + " possesses");
        }
    }

    //General commands
    List<string> Attack(CombatantInfo source, List<CombatantInfo> targets)
    {
        return new List<string> { "This is just a placeholder for Attack for now" };
    }

    List<string> Defend(CombatantInfo source, List<CombatantInfo> targets)
    {
        return new List<string> { "This is just a placeholder for Defend for now" };
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

    public int ReadInt(Stats stat)
    {
        switch (stat)
        {
            case Stats.CHP: return currentHP;
            case Stats.MHP: return maxHP;
            case Stats.CMP: return currentMP;
            case Stats.MMP: return maxMP;
            case Stats.AP: return aP / apMax;
            default: throw new System.Exception(stat + " cannot be parsed as an int in CombatantInfo (Or the functionality hasn't been implemented yet).");
        }
    }

    public float ReadFloat(Stats stat)
    {
        switch (stat)
        {
            case Stats.AP: return (float)(aP % apMax) / apMax;
            default: throw new System.Exception(stat + " cannot be parsed as a float in CombatantInfo (Or the functionality hasn't been implemented yet).");
        }
    }

    public void SubscribeDelegate(ref UpdateStats theDelegate)
    {
        onUpdate += theDelegate;
    }
}