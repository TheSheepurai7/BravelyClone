using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public partial class BattleManager : MonoBehaviour
{
    //Singleton reference
    public static BattleManager instance;

    //Inspector variables
    [SerializeField] Encounter[] encounters;

    //Cache
    GameObject playerGFXTemplate;
    GameObject enemyGFXTemplate;
    HorizontalLayoutGroup playerGroup;
    HorizontalLayoutGroup enemyGroup;

    //Combatant info
    List<CombatantInfo> combatants;
    EnemyInfo[] enemies;
    PlayerInfo[] players = new PlayerInfo[4];

    //Misc variables
    ActionBuilder action;
    Encounter activeEncounter;
    bool ready = false;

    // Start is called before the first frame update
    void Awake()
    {
        //Enforce singleton
        if (instance == null) { instance = this; }
        else { Destroy(this); }

        //Cache templates if necessary
        if (playerGFXTemplate == null) { playerGFXTemplate = Resources.Load<GameObject>("Prefabs/Player"); }
        if (enemyGFXTemplate == null) { enemyGFXTemplate = Resources.Load<GameObject>("Prefabs/Enemy"); }
        if (enemyGroup == null) { enemyGroup = transform.GetComponentsInChildren<HorizontalLayoutGroup>()[0]; }
        if (playerGroup == null) { playerGroup = transform.GetComponentsInChildren<HorizontalLayoutGroup>()[1]; }

        //First I need to calculate the average level of all party members
        int avgLvl = (GameData.instance.GetCharacter(Character.ALEC).ReadInt(Stats.LVL) + GameData.instance.GetCharacter(Character.MARISA).ReadInt(Stats.LVL) +
            GameData.instance.GetCharacter(Character.JENNA).ReadInt(Stats.LVL) + GameData.instance.GetCharacter(Character.GARETH).ReadInt(Stats.LVL)) / 4;

        //Then I need to use that level to find an encounter
        activeEncounter = null;
        do
        {
            int id = Random.Range(0, encounters.Length);
            if (avgLvl >= encounters[id].EncounterRating().x && avgLvl <= encounters[id].EncounterRating().y) { activeEncounter = encounters[id]; }
        } while (activeEncounter == null);

        //Modify names based on duplicate enemies
        List<string> names = new List<string>();
        for (int i = 0; i < activeEncounter.EnemyParty().Length; i++)
        {
            int counter = 1;
            string name = activeEncounter.EnemyParty()[i].name;
            for (int p = 0; p < i; p++)
            {
                if (activeEncounter.EnemyParty()[p].name == name) { if (counter == 1) { names[p] = name + " " + counter; } counter++; }
            }
            if (counter > 1) { names.Add(name + " " + counter); }
            else { names.Add(name); }
        }

        //Create enemy data structs and GFX
        if (enemyGroup != null)
        {
            combatants = new List<CombatantInfo>();
            enemies = new EnemyInfo[activeEncounter.EnemyParty().Length];
            for (int i = 0; i < enemies.Length; i++)
            {
                //Create the EnemyInfo
                enemies[i] = new EnemyInfo(activeEncounter.EnemyParty()[i], names[i]);
                combatants.Add(enemies[i]);

                //Create the EnemyGFX for the EnemyInfo
                GameObject enemyGFX = Instantiate(enemyGFXTemplate);
                enemyGFX.transform.SetParent(enemyGroup.transform);
                if (enemyGFX.TryGetComponent(out CharacterDisplay display)) { display.AssignStatBlock(enemies[i]); }
            }
        }
        else { print("This component needs a child with a horizontal layout group to hold the enemy graphics"); Destroy(this); }
    }

    // Update is called once per frame
    void Update()
    {
        if (ready)
        {
            //I believe the loop I had was to first have an event that adds to everyones' ATB gauge
            float deltaTime = Time.deltaTime;
            foreach (CombatantInfo combatant in combatants) { combatant.aP += (int)(deltaTime * 1000); }

            //Then I would process all the action requests (notably from the AIs that would check to see if there's anything they can do)

            //Finally I update all the displays one last time
        }
    }

    //Here is where I put the functionality of the READY button
    public void Ready()
    {
        //I need to create CombatInfo structs out of each CharacterData
        if (playerGroup != null)
        {
            for (int i = 0; i < 4; i++)
            {
                //Create the PlayerInfo
                players[i] = new PlayerInfo(GameData.instance.GetCharacter((Character)i), GameData.instance.GetCharacter((Character)i).ReadString(Stats.NAME));
                combatants.Add(players[i]);

                //Create the EnemyGFX for the EnemyInfo
                GameObject playerGFX = Instantiate(playerGFXTemplate);
                playerGFX.transform.SetParent(playerGroup.transform);
                if (playerGFX.TryGetComponent(out CharacterDisplay display)) { display.AssignStatBlock(players[i]); }
                int temp = i;
                if (playerGFX.TryGetComponent(out Button button)) { button.onClick.AddListener(() => PlayerButton(players[temp])); } 
            }
        }
        else { print("This component needs a second child with a horizontal layout group to hold the player graphics"); Destroy(this); }

        //Then I need to sort the combatants by speed and use those rankings to initialize each combatant's starting AP
        combatants.Sort((info1, info2) => info1.sPD.CompareTo(info2.sPD));
        for (int i = 0; i < combatants.Count; i++)
        {
            combatants[i].InitializeATB(combatants[0].sPD, (float)i / (combatants.Count + 2));
        }

        //Finally I need to flag the combat loop as ready
        ready = true;
    }

    //Just check that the player buttons work
    void PlayerButton(CombatantInfo combatant)
    {
        print("Accessed player button for " + combatant.name);
    }
}
