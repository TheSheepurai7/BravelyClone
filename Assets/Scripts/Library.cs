using System;
using System.Collections.Generic;
using UnityEngine;

//Delegates
public delegate void CombatAction(StatBlock source, List<StatBlock> targets);
public delegate void TimeEvent(float deltaTime);
public delegate void IntEvent(int inInt);
public delegate void ThreatEvent(ref StatBlock source, int value);

//Enums
public enum Character { ALEC, MARISA, JENNA, GARETH }
public enum TargetTag { SINGLE, PARTY, ALL, SELF, ELSE, PARTY_ELSE, ALL_ELSE, FIELD }

//Interfaces
public interface IStatBlock { public StatBlock ExportStatBlock(); }

//Structs
public abstract class CommandInfo //I should have this be abstract and have an ActionInfo and MenuInfo that inherit from it
{
    public readonly string name;
    //public readonly TargetTag tag;
    //public readonly CombatAction action;
    //public CommandInfo(string name, TargetTag tag, CombatAction action) { this.name = name; this.tag = tag; this.action = action; }
    public CommandInfo(string name) { this.name = name; }
}

public class ActionInfo : CommandInfo
{
    public readonly TargetTag tag;
    public readonly CombatAction action;

    public ActionInfo(string name, TargetTag tag, CombatAction action) : base(name)
    {
        this.tag = tag;
        this.action = action;
    }
}

public class MenuInfo : CommandInfo
{
    public readonly List<CommandInfo> commands;
    public MenuInfo(string name, List<CommandInfo> commands) : base(name)
    { 
        this.commands = commands;
    }
}

//Classes
public class StatBlock
{
    //Events
    public event IntEvent onHPChanged;
    public event ThreatEvent onThreat; 
    public event Action onDeath;

    //Variable fields
    private int _currentHP;
    private int _currentMP;
    public float aP;

    //Accessors
    public int currentHP { get { return _currentHP; } set { onHPChanged?.Invoke(_currentHP - value); _currentHP = Mathf.Clamp(value, 0, maxHP); if (_currentHP <= 0) { onDeath?.Invoke(); } } }
    public int currentMP { get { return _currentMP; } }
    int _apMax = 4000;
    public int apMax { get { return _apMax; } set { _apMax = 4000 - (int)(2500 * ((spd - value) / 98f)); } } //Takes lowest speed as input

    //Invariable fields
    public readonly string name;
    public readonly Sprite sprite;
    public readonly int maxHP;
    public readonly int maxMP;
    public readonly int pAtk;
    public readonly int pDef;
    public readonly int spd;

    public StatBlock(string name, Sprite sprite, int hp, int mp, int pAtk, int pDef, int spd)
    {
        this.name = name;
        this.sprite = sprite;
        this.pAtk = pAtk;
        this.pDef = pDef;
        this.spd = spd;
        maxHP = hp;
        _currentHP = hp;
        maxMP = mp;
        _currentMP = mp;
        aP = 0;
        onDeath += ClearStats;
    }

    public void GenerateThreat(ref StatBlock source, int value) { onThreat(ref source, value); }

    void ClearStats()
    {
        _currentHP = 0;
        aP = 0; //Actually I might want to double check if dying clears AP
    }
}

public class CharacterData : IStatBlock
{
    Sprite sprite;
    string name;
    int lvl;
    int[] baseStats; //HP, MP, STR, VIT, INT, MND, AGI
    float[] statGrowths; // For use later
    int currentHP;
    int currentMP;

    //Dictionary<Job, int> jobPool; //Job is the key, job level is the value
    //Job actingJob;
    //Job actingSubJob;
    //Equipment[] equipment;

    public CharacterData(Character archetype, int lvl)
    {
        //Conventional attack damage (for players at least) is calculated by (STR^1.75 + Weapon^1.5)
        //Second formula is ((STR^1.15 + Weapon) * (1 + lvl / 4)
        //Starting conventional attack damage stats are:
        /*
            Alec - 10 STR + 5 Weapon (Machete)
            Marisa - 9 STR + 3 Weapon (Knife)
            Jenna - 9 STR + 3 Weapon (Knife)
            Gareth - 11 STR + 7 Weapon (Hatchet)
        //*/

        //First and foremost assign the level I guess
        this.lvl = lvl;

        //Now switch archetypes to fill in the rest
        switch (archetype)
        {
            //76.96f is the hard cap for HP/MP. 0.7771f is the hard cap for all other stats
            case Character.ALEC:
                name = "Alec";
                sprite = Resources.Load<Sprite>("Sprites/Actor1");
                baseStats = new int[] { 150, 30, 10, 10, 9 }; //HP, MP, STR, VIT, and AGI for now. HP for everyone is going to be 100 and MP is going to be 30 to start
                statGrowths = new float[] { 76.96f, 0, 0.6667f, 0.6667f, 0.3334f };
                break;
            case Character.MARISA:
                name = "Marisa";
                sprite = Resources.Load<Sprite>("Sprites/Actor2");
                baseStats = new int[] { 130, 30, 9, 9, 10 }; //HP, MP, STR, VIT, and AGI for now. HP for everyone is going to be 100 and MP is going to be 30 to start
                statGrowths = new float[] { 68.34f, 0, 0.3334f, 0.6667f, 0.3334f };
                break;
            case Character.JENNA:
                name = "Jenna";
                sprite = Resources.Load<Sprite>("Sprites/Actor3");
                baseStats = new int[] { 110, 30, 9, 8, 10 }; //HP, MP, STR, VIT, and AGI for now. HP for everyone is going to be 100 and MP is going to be 30 to start
                statGrowths = new float[] { 60f, 0, 0.3334f, 0.6667f, 0.6667f };
                break;
            case Character.GARETH:
                name = "Gareth";
                sprite = Resources.Load<Sprite>("Sprites/Actor4");
                baseStats = new int[] { 135, 30, 11, 9, 9 }; //HP, MP, STR, VIT, and AGI for now. HP for everyone is going to be 100 and MP is going to be 30 to start
                statGrowths = new float[] { 65f, 0, 0.7771f, 0.6667f, 0.6667f };
                break;
        }

        //*Finally cast dependencies (Don't know how much if any of this I will need later
        currentHP = baseStats[0];
        currentMP = baseStats[1];
        //jobPool = new Dictionary<Job, int>();
        //jobPool.Add(Job.NONE, 0);
        //actingJob = Job.NONE;
        //actingSubJob = Job.NONE;
        //*/
    }

    public StatBlock ExportStatBlock()
    {
        return new StatBlock(
            name,
            sprite,
            (int)(baseStats[0] + (statGrowths[0] * (lvl - 1))),
            (int)(baseStats[1] + (statGrowths[1] * (lvl - 1))),
            //(int)Mathf.Pow(baseStats[2] + statGrowths[2] * (lvl - 1), 1.75f), //Add weapon stats later. Damage formula #1
            (int)(Mathf.Pow(baseStats[2] + statGrowths[2] * (lvl - 1), 1.15f) * (1 + lvl / 4)), //Add weapon stats later. Damage formula #2
            (int)(baseStats[3] + (statGrowths[3] * (lvl - 1))), //Add armor stats later
            (int)(baseStats[4] + (statGrowths[4] * (lvl - 1)))
            );
    }
}

