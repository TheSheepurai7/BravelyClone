using System;
using System.Collections.Generic;
using UnityEngine;

public partial class BattleManager : MonoBehaviour
{
    class EnemyInfo : CombatantInfo
    {
        //Events
        event Action onDeath;
        event Action onRevive;

        public EnemyInfo(IStatReader statBlock, string name) : base(statBlock, name)
        {
            //HP/MP
            maxHP = statBlock.ReadInt(Stats.MHP);
            maxMP = statBlock.ReadInt(Stats.MMP);
            currentHP = maxHP;
            currentMP = maxMP;
        }

        public override List<CommandInfo> ReadCommands(Stats stat)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateDisplay(ref CharacterDisplay display)
        {
            base.UpdateDisplay(ref display);
            onDeath += display.FadeOut;
            onRevive += display.Restore;
        }

        protected override void OnDeath()
        {
            enabled = false;
            onDeath?.Invoke();
            instance.CheckWinCondition();
        }
    }

    class PlayerInfo : CombatantInfo
    {
        public PlayerInfo(IStatReader statBlock, string name) : base(statBlock, name)
        {
            maxHP = statBlock.ReadInt(Stats.MHP);
            maxMP = statBlock.ReadInt(Stats.MMP);
            currentHP = maxHP;
            currentMP = maxMP;
        }

        public override List<CommandInfo> ReadCommands(Stats stat)
        {
            switch (stat)
            {
                default: return null;
            }
        }
    }
}
