using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public partial class BattleManager : MonoBehaviour
{
    abstract class CombatantInfo
    {
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
        public int currentHP;
        public int currentMP;
        protected int aP;
        protected int apMax = 5000;
        protected int maxHP;
        protected int maxMP;

        public CombatantInfo(IGetStat statBlock, string name)
        {
            //Name
            this.name = name;

            //Sprite
            sprite = statBlock.GetSprite();

            //Other stats
            pAtk = statBlock.GetStat(Stats.PATK);
            mAtk = statBlock.GetStat(Stats.MATK);
            iNT = statBlock.GetStat(Stats.INT);
            pDef = statBlock.GetStat(Stats.PDEF);
            mDef = statBlock.GetStat(Stats.MDEF);
            mND = statBlock.GetStat(Stats.MND);
            sPD = statBlock.GetStat(Stats.AGI);
        }

        public void InitializeATB(int baseSpeed)
        {
            //I need to take in the base speed then use it to subtract from the apMax somehow
            apMax -= (sPD - baseSpeed) * 30;

            //Now I need to take the hard max, compare it to the current max, and add some variation to create a working AP percentage
            aP = (int)Mathf.Clamp((5000 - apMax) * Random.Range(0.8f, 1), 0, apMax);
        }

        public void ProgressATB(float deltaTime)
        {
            aP = Mathf.Clamp(aP + (int)(deltaTime * 1000), 0, apMax * 4);
        }
    }

    class EnemyInfo : CombatantInfo
    {
        //Rewards
        int jP;
        int xP;
        int pG;

        //Probably some delegate here for the AI

        //Constructor
        public EnemyInfo(Enemy enemy, string name) : base(enemy, name)
        {
            //I need to do HP/MP copying here instead of in the base class
            //HP/MP
            maxHP = enemy.GetStat(Stats.HP);
            maxMP = enemy.GetStat(Stats.MP);
            currentHP = maxHP;
            currentMP = maxMP;

            //Rewards
            enemy.CopyRewards(ref jP, ref xP, ref pG);

            //Some stuff with the AI delegate
        }

        public void AddReward(ref int jpReward, ref int xpReward, ref int pgReward)
        {
            jpReward += jP;
            xpReward += xP;
            pgReward += pG;
        }
    }

    class PlayerInfo : CombatantInfo, IGetStat
    {
        public PlayerInfo(IGetStat statBlock, string name) : base(statBlock, name)
        {
            maxHP = (int)statBlock.GetCompoundStat(Stats.HP).y;
            maxMP = (int)statBlock.GetCompoundStat(Stats.MP).y;
            currentHP = (int)statBlock.GetCompoundStat(Stats.HP).x * maxHP;
            currentMP = (int)statBlock.GetCompoundStat(Stats.MP).x * maxMP;
        }

        public Vector2 GetCompoundStat(Stats stat)
        {
            switch(stat)
            {
                case Stats.HP: return new Vector2(currentHP / (float)maxHP, maxHP);
                case Stats.MP: return new Vector2(currentMP / (float)maxMP, maxMP);
                default: throw new System.NotImplementedException();
            }
        }

        public string GetName(Stats stat)
        {
            return name;
        }

        public Sprite GetSprite()
        {
            return sprite;
        }

        public int GetStat(Stats stat)
        {
            switch (stat)
            {
                case Stats.AP: return aP / apMax;
                default: throw new System.NotImplementedException();
            }
        }

        public float GetFloatStat(Stats stat)
        {
            switch (stat)
            {
                case Stats.AP: return aP % apMax / (float)apMax; //I'm going to return an AP percentage here instead of the actual thing
                default: throw new System.NotImplementedException();
            }
        }
    }
}
