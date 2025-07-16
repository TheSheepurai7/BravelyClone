using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public partial class BattleManager : MonoBehaviour
{
    //Singleton reference
    public static BattleManager instance;

    //Inspector variables
    [SerializeField] Encounter[] encounters;
    [SerializeField] GameObject actionMenu;

    //Cache
    Text text;
    GameObject playerGFXTemplate;
    GameObject enemyGFXTemplate;
    HorizontalLayoutGroup playerGroup;
    HorizontalLayoutGroup enemyGroup;

    //Combatant info
    List<CombatantInfo> combatants;
    EnemyInfo[] enemies;
    PlayerInfo[] players = new PlayerInfo[4];

    //Public fields
    public ActionBuilder actionQueue;
    List<String> messageQueue;

    //Misc variables
    Encounter activeEncounter;
    bool ready = false;

    // Start is called before the first frame update
    void Awake()
    {
        //Enforce singleton
        if (instance == null) { instance = this; }
        else { Destroy(this); }

        //Cache the display
        text = GetComponent<Text>();

        //Cache templates as necessary
        if (playerGFXTemplate == null) { playerGFXTemplate = Resources.Load<GameObject>("Prefabs/Player"); }
        if (enemyGFXTemplate == null) { enemyGFXTemplate = Resources.Load<GameObject>("Prefabs/Enemy"); }
        if (enemyGroup == null) { enemyGroup = transform.GetComponentsInChildren<HorizontalLayoutGroup>()[0]; }
        if (playerGroup == null) { playerGroup = transform.GetComponentsInChildren<HorizontalLayoutGroup>()[1]; }

        //Ensure the actionMenu is off
        actionMenu.SetActive(false);

        //First I need to calculate the average level of all party members
        int avgLvl = (GameData.instance.GetCharacter(Character.ALEC).ReadInt(Stats.LVL) + GameData.instance.GetCharacter(Character.MARISA).ReadInt(Stats.LVL) +
            GameData.instance.GetCharacter(Character.JENNA).ReadInt(Stats.LVL) + GameData.instance.GetCharacter(Character.GARETH).ReadInt(Stats.LVL)) / 4;

        //Then I need to use that level to find an encounter
        activeEncounter = null;
        do
        {
            //int id = Random.Range(0, encounters.Length);
            int id = UnityEngine.Random.Range(0, encounters.Length);
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
        //Add to everyones' ATB gauge if a command isn't being built
        if (ready && actionQueue == null && messageQueue == null)
        {
            if (!EventSystem.current.currentInputModule.IsActive()) { EventSystem.current.currentInputModule.ActivateModule(); }
            float deltaTime = Time.deltaTime;
            foreach (CombatantInfo combatant in combatants) { combatant.aP += (int)(deltaTime * 1000); }
        }

        //Otherwise check the action queue for actions that can be processed and process them
        else if (actionQueue != null && messageQueue == null)
        {
            //This SHOULD be a concat, but that's not working so let's make sure just directly assigning it works
            if (actionQueue.Complete()) { messageQueue = actionQueue.RunAction(); actionQueue = null; }
        }

        //If there are messages in the queue then display them
        //This is done in a very weird way to make sure it doesn't simultaneously register button presses. It works for now, but might need to be scaled up later
        else if (messageQueue != null)
        {
            if (messageQueue.Count > 0) { text.text = messageQueue[0]; }
            else { text.text = string.Empty; messageQueue = null; }

            if (Input.GetMouseButtonDown(0))
            {
                EventSystem.current.currentInputModule.DeactivateModule();
                messageQueue.RemoveAt(0);
            }
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
                if (playerGFX.TryGetComponent(out Button button)) { button.onClick.AddListener(() => OpenActionMenu(players[temp])); }
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

    void OpenActionMenu(PlayerInfo player)
    {
        //None of this works if action menu doesn't even have an action menu component
        if (actionMenu.TryGetComponent(out ActionMenu amComponent))
        {
            //None of this works if there's already an action being built (but won't throw an exception if the queue isn't empty)
            if (actionQueue == null && messageQueue == null)
            {
                //Activate the action menu at the spot clicked
                actionMenu.SetActive(true);
                actionMenu.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;

                //Open a new ticket in the action queue
                actionQueue = new ActionBuilder(player);

                //This is where I would pass functions to make the action menu work
                amComponent.PopulateMenu(player);
            }
        }
        else { throw new Exception("Action menu needs an action menu component otherwise why are you calling it an action menu?"); }
    }
}
