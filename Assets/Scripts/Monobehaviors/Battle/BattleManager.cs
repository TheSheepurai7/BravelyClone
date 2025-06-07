using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class BattleManager : MonoBehaviour
{
    //There should only ever be one of these
    public static BattleManager instance;

    //Apparently there's no getting around having a reference to the start button
    [SerializeField] Button readyButton;

    //Store parties
    EnemyInfo[] enemies;

    //Misc variables
    bool ready = false;

    void Awake()
    {
        //Ensure singleton
        if (instance == null) { instance = this; } else { Destroy(this); }

        //I'm going to need to set up some events connected to the GeneratePlayer and GenerateEnemy objects so the code dependency is at a minimum
    }

    void Update()
    {
        if (ready) { print("MY BODY IS READY"); }
    }

    public void GenerateEnemies(Encounter encounter, string[] names)
    {
        //Declare the enemy party size
        enemies = new EnemyInfo[encounter.EnemyList().Length];

        //Fill in enemy data
        for (int i = 0; i < encounter.EnemyList().Length; i++)
        {
            enemies[i] = new EnemyInfo(encounter.EnemyList()[i], names[i]);
        }
    }

    public void StartBattle()
    {
        //Then I'll create the player party

        //Don't forget to flag the battle as ready
        ready = true;
    }

    public void ReceivePlayerParty()
    {

    }
}
