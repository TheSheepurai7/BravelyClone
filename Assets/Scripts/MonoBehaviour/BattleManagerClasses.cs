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
    }

    //Player info should probably be what holds generic actions and them simply pulls job options from a struct or something if it knows it's there
    class PlayerInfo : CombatantInfo
    {
        public PlayerInfo(IStatReader statBlock, string name) : base(statBlock, name)
        {
            maxHP = statBlock.ReadInt(Stats.MHP);
            maxMP = statBlock.ReadInt(Stats.MMP);
            currentHP = statBlock.ReadInt(Stats.CHP);
            currentMP = statBlock.ReadInt(Stats.CMP);
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

        public List<string> RunAction()
        {
            if (action != null && targets != null) { return action(source, targets); }
            else { return null; }
        }
    }
}
