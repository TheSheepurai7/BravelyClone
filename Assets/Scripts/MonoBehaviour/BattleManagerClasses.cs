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

        public override List<CommandInfo> ReadCommands(Stats stat)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnDeath()
        {
            print(name + " has died");

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
