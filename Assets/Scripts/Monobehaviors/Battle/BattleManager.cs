using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    //These are malleable entities used for this battle
    public abstract class Combatant
    {
        //Some universal properties that these entities have
        public readonly string name;
        public int hP;
        public int mP;
        public readonly int pAtk;
        public readonly int mAtk;
        public readonly int iNT;
        public readonly int pDef;
        public readonly int mDef;
        public readonly int mND;
        public readonly int sPD;

        public Combatant(string name, int hP, int mP, int pAtk, int mAtk, int iNT, int pDef, int mDef, int mND, int sPD)
        {
            this.name = name;
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

    public class EnemyCombatant : Combatant
    {
        //Rewards upon defeat
        public readonly int jP;
        public readonly int xP;
        public readonly int pG;

        //I'm going to need to do something with delegates for the UI
        AI ai;

        public EnemyCombatant(ReadOnlyEnemyStruct enemyStruct) : 
            base(enemyStruct.name, enemyStruct.hP, enemyStruct.mP, enemyStruct.pAtk, enemyStruct.mAtk,
                enemyStruct.iNT, enemyStruct.pDef, enemyStruct.mDef, enemyStruct.mND, enemyStruct.sPD)
        {
            jP = enemyStruct.jP;
            xP = enemyStruct.xP;
            pG = enemyStruct.pG;
        }
    }

    //There should only ever be one of these
    public static BattleManager instance;

    //Both parties are stored as malleable entities here
    EnemyCombatant[] enemyParty;
    //PlayerCombatant[] playerParty; //I'll revisit this later

    //It should only ever work with one client at a time (Might change this to a queue or add a queue in addition to this)
    Combatant client;

    void OnEnable()
    {
        //Ensure singleton
        if (instance == null) { instance = this; } else { Destroy(this); }
    }

    void Update()
    {
        //This is where the AP system is managed obviously
        //For the time being I'm going to use this as an independent function to tell me if the enemy party was copied correctly
        if (enemyParty != null) 
        {
            print("First enemy is " + enemyParty[0].name); //I need to refactor the system so it gets the right fucking name
        }
    }

    public void GenerateEnemy(ReadOnlyEnemyStruct[] encounterData)
    {
        //Assign the party size
        enemyParty = new EnemyCombatant[encounterData.Length];

        //Fill in the combatant profiles
        for(int i = 0; i < enemyParty.Length; i++)
        {
            enemyParty[i] = new EnemyCombatant(encounterData[i]);
        }
    }
}
