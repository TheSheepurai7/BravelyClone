using System;
using System.Collections.Generic;
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

    //Queues
    public List<ActionBuilder> actionQueue { get; private set; }
    string message;

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
        if (ready)
        {
            //Run actions and display messages if there are such things to do and show
            if (actionQueue != null || message != null)
            {
                //Process actions
                if (actionQueue != null)
                {
                    if (actionQueue[0].Complete() && message == null)
                    {
                        message = actionQueue[0].RunAction();
                        actionQueue.RemoveAt(0);
                        if (actionQueue.Count == 0) { actionQueue = null; }
                    }
                }

                //Display messages
                if (message != null)
                {
                    text.text = message;

                    //Erase message on button down
                    if (Input.GetMouseButtonDown(0))
                    {
                        //Erase the message (and presumably start the timer back up again
                        text.text = string.Empty;
                        message = null;

                        //Nullify any pointer down events
                        EventSystem.current.currentInputModule.DeactivateModule();
                    }
                }
            }

            //Otherwise increment everyones' ATB counters
            else
            {

            }
        }
    }

    //Here is where I put the functionality of the READY button
    public void Ready()
    {
        if (playerGroup != null)
        {
            //I need to create CombatInfo structs out of each CharacterData
            for (int i = 0; i < 4; i++)
            {
                //Create the PlayerInfo
                players[i] = new PlayerInfo(GameData.instance.GetCharacter((Character)i), GameData.instance.GetCharacter((Character)i).ReadString(Stats.NAME));
                combatants.Add(players[i]);

                //Create the PlayerGFX for the PlayerInfo
                GameObject playerGFX = Instantiate(playerGFXTemplate);
                playerGFX.transform.SetParent(playerGroup.transform);
                int temp = i;
                if (playerGFX.TryGetComponent(out CharacterDisplay display))
                {
                    display.AssignStatBlock(players[i]);
                    display.SubscribeLeftClick(() => OpenActionMenu(players[temp]));
                }
                else { throw new Exception("PlayerGFX needs a CharacterDisplay component to work"); }
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

    //I think I need functions that can be passed onto the button at setup so that it can be invoked when clicked to either defend or open the action menu
    void OpenActionMenu(PlayerInfo playerInfo)
    {
        //None of this works if action menu doesn't even have an action menu component
        if (actionMenu.TryGetComponent(out ActionMenu amComponent))
        {
            //None of this works if there's already an action being built or a message displayed (but won't throw an exception if the queue isn't empty)
            if (actionQueue == null && message == null)
            {
                //Activate the action menu at the spot clicked
                actionMenu.SetActive(true);
                actionMenu.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;

                //Open a new ticket in the action queue
                actionQueue = new List<ActionBuilder> { new ActionBuilder(playerInfo) };

                //This is where I would pass functions to make the action menu work
                amComponent.PopulateMenu(playerInfo);
            }
        }
        else { throw new Exception("Action menu needs an action menu component otherwise why are you calling it an action menu?"); }
    }

    void RemoveEnemy(EnemyInfo enemy)
    {
        //Remove the enemy
        foreach (Transform child in enemyGroup.transform)
        {
            if (child.TryGetComponent(out CharacterDisplay display) && display.ExtractCombatant(out CombatantInfo combatant) && enemy.Equals(combatant))
            { child.gameObject.SetActive(false); break; }
        }

        //Check to see if the win condition is met

    }
}
