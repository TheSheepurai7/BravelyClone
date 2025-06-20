using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    PlayerInfo[] players;

    //Misc variables
    bool ready = false;

    void Awake()
    {
        //Ensure singleton
        if (instance == null) { instance = this; } else { Destroy(this); }
    }

    void Update()
    {
        if (ready)
        {
            //I need to progress the ATB of all active combatants
            float deltaTime = Time.deltaTime;

            //I need to update the ATB gauges
            foreach (CombatantInfo enemy in enemies) { enemy.ProgressATB(deltaTime); }
            foreach (CombatantInfo player in players) { player.ProgressATB(deltaTime); }

            //This is where I'll process action requests

            //And this is where I'll update the displays
            GameData.instance.OnDisplay(Character.ALEC, players[0]);
            GameData.instance.OnDisplay(Character.MARISA, players[1]);
            GameData.instance.OnDisplay(Character.JENNA, players[2]);
            GameData.instance.OnDisplay(Character.GARETH, players[3]);
        }
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
        //Create the player party
        players = new PlayerInfo[4];

        //Fill in player data
        for(int i  = 0; i < players.Length; i++)
        {
            players[i] = new PlayerInfo(GameData.instance.GetCharacter((Character)i + 1), GameData.instance.GetCharacter((Character)i + 1).GetName(Stats.NAME));
        }

        //Create the player party graphics
        GeneratePlayer.instance.GenerateGFX();

        //Put combatants in a list and order them by speed
        List<CombatantInfo> speedOrder = new List<CombatantInfo>();
        foreach (PlayerInfo player in players) { speedOrder.Add(player); }
        foreach (EnemyInfo enemy in enemies) { speedOrder.Add(enemy); }
        speedOrder.Sort((s1, s2) => s1.sPD.CompareTo(s2.sPD));

        //I need to initialize the ATB bars which means I need to think about how ATB speeds work
        int baseSpeed = speedOrder[0].sPD;
        foreach (CombatantInfo combatant in speedOrder)
        {
            combatant.InitializeATB(baseSpeed);
        }

        //This is probably where I'll update the displays
        GameData.instance.OnDisplay(Character.ALEC, players[0]);
        GameData.instance.OnDisplay(Character.MARISA, players[1]);
        GameData.instance.OnDisplay(Character.JENNA, players[2]);
        GameData.instance.OnDisplay(Character.GARETH, players[3]);

        //Don't forget to flag the battle as ready
        ready = true;
    }
}
