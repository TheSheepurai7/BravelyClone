using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class BattleManager : MonoBehaviour
{
    //The instance storage
    //If I run into threading issues, I need to create a lock object
    public static BattleManager instance = null;

    //Combatant info
    List<CombatantInfo> combatants;
    List<EnemyInfo> enemies;
    List<PlayerInfo> players;

    //Properties
    public bool hasEnemies { get { return enemies != null; } }
    public bool hasPlayers { get { return players != null; } }
    public List<IStatReader> enemyList { get { return enemies.ToList<IStatReader>(); } }
    public List<IStatReader> playerList { get { return players.ToList<IStatReader>(); } }
    public bool ready;

    //Misc variables
    List<Action> actionQueue = new List<Action>();

    void Start()
    {
        //Enforce singleton
        if (instance == null) { instance = this; } else { Destroy(this); }
    }

    void Update()
    {
        if (ready)
        {
            //Increment ATB
            foreach (CombatantInfo combatant in combatants)
            {
                combatant.aP += (int)(Time.deltaTime * 1000);
            }

            //Perform actions in the queue
            if (actionQueue.Count > 0)
            {
                actionQueue[0].Invoke();
                actionQueue.RemoveAt(0);
            }
        }
    }

    //Public function
    public void CreateEnemies(Encounter encounterData)
    {
        //Instantiate info lists
        if (combatants == null) { combatants = new List<CombatantInfo>(); }
        enemies = new List<EnemyInfo>();

        //Modify names based on duplicate enemies
        List<string> names = new List<string>();
        for (int i = 0; i < encounterData.enemies.Count; i++)
        {
            int counter = 1;
            string name = encounterData.enemies[i].name;
            for (int p = 0; p < i; p++)
            {
                if (encounterData.enemies[p].name == name) { if (counter == 1) { names[p] = name + " " + counter; } counter++; }
            }
            if (counter > 1) { names.Add(name + " " + counter); }
            else { names.Add(name); }
        }

        //Create enemy data structs
        for (int i = 0; i < encounterData.enemies.Count; i++)
        {
            EnemyInfo enemy = new EnemyInfo(encounterData.enemies[i], names[i]);
            enemies.Add(enemy);
            combatants.Add(enemy);
        }
    }

    public void CreatePlayers(List<IStatReader> playerParty)
    {
        //Instantiate info lists
        if (combatants == null) { combatants = new List<CombatantInfo>(); }
        players = new List<PlayerInfo>();

        //Create player data structs
        foreach (IStatReader statBlock in playerParty)
        {
            PlayerInfo player = new PlayerInfo(statBlock, statBlock.ReadString(Stats.NAME));
            players.Add(player);
            combatants.Add(player);
        }
    }

    public void AddAction(CombatantInfo source, CombatAction action, List<CombatantInfo> targets)
    {
        actionQueue.Add(() => action(source, targets));
    }
}
