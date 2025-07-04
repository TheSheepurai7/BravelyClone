using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public partial class BattleManager : MonoBehaviour
{
    const int AP_MAX = 3000;

    public abstract class CombatantInfo : IStatReader
    {
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

        protected CombatantInfo(IStatReader statBlock, string name)
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

        public void InitializeATB(int baseSpd, float rank) //Rank should be rank / ((Total combatants) + 2).
        {
            //First I need to calculate how speed reduces the AP cap with a formula I don't quite remember.
            apMax -= (int)(AP_MAX * (sPD - baseSpd) * 0.01f / 2);

            //Then I need to multiply that cap by the rank + or - up to 0.1f (But never exceeding 0.9f) to get the starting AP
            aP = (int)(apMax * Mathf.Clamp(rank + Random.Range(-0.1f, 0.1f), 0.1f, 0.9f));
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

    public class EnemyInfo : CombatantInfo
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

    public class PlayerInfo: CombatantInfo
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
        List<CombatantInfo> target;
        bool status; //false = open true = closed
    }
}
