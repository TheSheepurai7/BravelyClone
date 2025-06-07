using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct StatBlock
{
    public readonly int hP;
    public readonly int mP;
    public readonly int pAtk;
    public readonly int mAtk;
    public readonly int iNT;
    public readonly int pDef;
    public readonly int mDef;
    public readonly int mND;
    public readonly int sPD;

    public StatBlock(int hP, int mP, int pAtk, int mAtk, int iNT, int pDef, int mDef, int mND, int sPD)
    {
        this.hP = hP;
        this.mP = mP;
        this.pAtk = pAtk;
        this.mAtk = mAtk;
        this.iNT = iNT;
        this.pDef = pDef;
        this.mDef = mDef;
        this.mND = mND;
        this.sPD = sPD;
    }
}

public partial class BattleManager : MonoBehaviour
{
    class EnemyInfo
    {
        //Stats
        public readonly StatBlock statBlock;
        public readonly string name;
        public int currentHP;
        public int currentMP;

        //Rewards
        int jP;
        int xP;
        int pG;

        //Probably some delegate here for the AI

        //Constructor
        public EnemyInfo(Enemy enemy, string name)
        {
            //Name
            this.name = name;

            //Stats
            statBlock = enemy.ExportStats();
            currentHP = statBlock.hP;
            currentMP = statBlock.mP;

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
}
