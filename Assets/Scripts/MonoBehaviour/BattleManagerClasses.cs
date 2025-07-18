using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleManager : MonoBehaviour
{
    class EnemyInfo : CombatantInfo
    {
        public EnemyInfo(IStatReader statBlock, string name) : base(statBlock, name)
        {
            //HP/MP
            maxHP = statBlock.ReadInt(Stats.MHP);
            maxMP = statBlock.ReadInt(Stats.MMP);
            currentHP = maxHP;
            currentMP = maxMP;
        }

        public override int ReadInt(Stats stat)
        {
            switch (stat)
            {
                case Stats.CHP: return currentHP;
                case Stats.MHP: return maxHP;
                case Stats.CMP: return currentMP;
                case Stats.MMP: return maxMP;
                case Stats.AP: return aP / apMax;
                case Stats.PATK: return pAtk;
                case Stats.PDEF: return pDef;
                default: throw new Exception(stat + " cannot be parsed as an int in EnemyInfo (Or the functionality hasn't been implemented yet).");
            }
        }

        public override List<CommandInfo> ReadCommands(Stats stat)
        {
            throw new Exception(stat + " cannot be parsed as a command list in EnemyInfo (Or the functionality hasn't been implemented yet).");
        }

        protected override void OnDeath()
        {
            print("Enemy killed");
            instance.actionQueue.Add(new ActionBuilder(this, null, KillSelf));
        }

        string KillSelf(CombatantInfo source, List<CombatantInfo> targets)
        {
            aP = 0;
            active = false;
            instance.RemoveEnemy(this);
            return name + " has been felled";
        }
    }

    //Player info should probably be what holds generic actions and them simply pulls job options from a struct or something if it knows it's there
    class PlayerInfo : CombatantInfo
    {
        List<CommandInfo> jobCommands;
        List<CommandInfo> subJobCommands;

        int level;

        public PlayerInfo(IStatReader statBlock, string name) : base(statBlock, name)
        {
            maxHP = statBlock.ReadInt(Stats.MHP);
            maxMP = statBlock.ReadInt(Stats.MMP);
            currentHP = statBlock.ReadInt(Stats.CHP);
            currentMP = statBlock.ReadInt(Stats.CMP);
            level = statBlock.ReadInt(Stats.LVL);
            jobCommands = statBlock.ReadCommands(Stats.JOB);
            subJobCommands = statBlock.ReadCommands(Stats.SJB);
        }

        public override int ReadInt(Stats stat)
        {
            switch (stat)
            {
                case Stats.LVL: return level;
                case Stats.CHP: return currentHP;
                case Stats.MHP: return maxHP;
                case Stats.CMP: return currentMP;
                case Stats.MMP: return maxMP;
                case Stats.AP: return aP / apMax;
                case Stats.PATK: return pAtk;
                case Stats.PDEF: return pDef;
                default: throw new Exception(stat + " cannot be parsed as an int in PlayerInfo (Or the functionality hasn't been implemented yet).");
            }
        }

        public override List<CommandInfo> ReadCommands(Stats stat)
        {
            switch (stat)
            {
                case Stats.JOB: return jobCommands;
                case Stats.SJB: return subJobCommands;
                default: throw new Exception(stat + " cannot be parsed as a command list in PlayerInfo (Or the functionality hasn't been implemented yet).");
            }
        }
    }

    public class ActionBuilder
    {
        CombatantInfo source;
        public List<CombatantInfo> targets { private get; set; }
        public CombatAction action { private get; set; }

        //The constructor only truly needs a source, but there's an option for if it can obtain all three (Usually reserved for AI)
        public ActionBuilder(CombatantInfo source)
        {
            this.source = source;
        }

        public ActionBuilder(CombatantInfo source, List<CombatantInfo> targets, CombatAction action)
        {
            this.source = source;
            this.targets = targets;
            this.action = action;
        }

        public bool Complete()
        {
            return source != null && targets != null && action != null;
        }

        public string RunAction()
        {
            return action(source, targets);
        }
    }
}
