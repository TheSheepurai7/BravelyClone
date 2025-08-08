using System;
using System.Collections.Generic;
using UnityEngine;

//Delegates
public delegate void StatFunction(Stats stat);
public delegate void CombatAction(CombatantInfo source, List<CombatantInfo> targets);
public delegate void HPChange(int change);

//Enums
public enum Character { ALEC, MARISA, JENNA, GARETH }
public enum Job { NONE, KNIGHT, CLERIC, WIZARD, DRAGOON } //Add more later
public enum Stats { NAME, SPRITE, LVL, CHP, MHP, CMP, MMP, STR, PATK, VIT, PDEF, INT, MATK, MND, MDEF, AGI, JOB, JLV, SJB, SJV, AP }
public enum TargetTag { SINGLE, PARTY, ALL, SELF, ELSE, PARTY_ELSE, ALL_ELSE, FIELD }

//Interfaces
public interface IStatReader
{
    public Sprite ReadImage();
    public string ReadString(Stats stat);
    public int ReadInt(Stats stat);
    public float ReadFloat(Stats stat);
    public List<CommandInfo> ReadCommands(Stats stat);
    public void UpdateDisplay(ref CharacterDisplay display);
}

//Structs
public struct CommandInfo
{
    public readonly string name;
    public readonly TargetTag tag;
    public readonly CombatAction action;
    public CommandInfo(string name, TargetTag tag, CombatAction action) { this.name = name; this.tag = tag; this.action = action; }
}

//Classes
public abstract class CombatantInfo : IStatReader
{
    //Constants
    const int AP_MAX = 3000;

    //Events
    public event StatFunction onUpdate;
    public event HPChange onHPChange;
    public event Action onDeath;

    //Stats
    public bool enabled = true;
    public readonly Sprite sprite;
    public readonly string name;
    public readonly int pAtk;
    public readonly int pDef;
    public readonly int sPD;
    protected int maxHP;
    protected int maxMP;
    protected int apMax = AP_MAX;

    //Properties
    private int _aP;
    public int aP { get { return _aP; } set { _aP = Mathf.Clamp(value, 0, apMax * 4); onUpdate?.Invoke(Stats.AP); } }
    private int _currentHP;
    public int currentHP { get { return _currentHP; } 
        set { onHPChange?.Invoke(_currentHP - value); _currentHP = Mathf.Clamp(value, 0, maxHP); onUpdate?.Invoke(Stats.CHP); if (_currentHP == 0) { OnDeath(); } } }

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
        pDef = statBlock.ReadInt(Stats.PDEF);
        sPD = statBlock.ReadInt(Stats.AGI);
    }

    //Interface
    public float ReadFloat(Stats stat)
    {
        switch (stat)
        {
            case Stats.AP: return (float)(aP % apMax) / apMax;
            default: throw new System.Exception(stat + " cannot be parsed as a float in CombatantInfo (Or the functionality hasn't been implemented yet).");
        }
    }

    public Sprite ReadImage()
    {
        return sprite;
    }

    public int ReadInt(Stats stat)
    {
        switch(stat)
        {
            case Stats.CHP: return currentHP;
            case Stats.MHP: return maxHP;
            case Stats.CMP: return currentMP;
            case Stats.MMP: return maxMP;
            case Stats.AP: return aP / apMax;
            default: throw new System.Exception(stat + " cannot be parsed as an int in CombatantInfo (Or the functionality hasn't been implemented yet)");
        }
    }

    public string ReadString(Stats stat)
    {
        switch (stat)
        {
            case Stats.NAME: return name;
            default: throw new System.Exception(stat + " cannot be parsed as a string in CombatantInfo (Or the functionality hasn't been implemented yet).");
        }
    }

    //I think I can change this to a display link 
    public virtual void UpdateDisplay(ref CharacterDisplay display)
    {
        onUpdate += display.UpdateStats;
        onHPChange += display.DisplayHPChange;
    }

    public abstract List<CommandInfo> ReadCommands(Stats stat);

    //Virtual functions
    protected virtual void OnDeath()
    {

    }
}

