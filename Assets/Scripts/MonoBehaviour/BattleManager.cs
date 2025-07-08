using System;
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
    Text text;
    GameObject playerGFXTemplate;
    GameObject enemyGFXTemplate;
    GameObject actionMenuTemplate;
    HorizontalLayoutGroup playerGroup;
    HorizontalLayoutGroup enemyGroup;

    //Combatant info
    List<CombatantInfo> combatants;
    EnemyInfo[] enemies;
    PlayerInfo[] players = new PlayerInfo[4];

    //Misc variables
    Encounter activeEncounter;
    GameObject actionMenu;
    ActionBuilder actionQueue;
    List<string> messageQueue;
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
        if (actionMenuTemplate == null) { actionMenuTemplate = Resources.Load<GameObject>("Prefabs/ActionMenu"); }
        if (enemyGroup == null) { enemyGroup = transform.GetComponentsInChildren<HorizontalLayoutGroup>()[0]; }
        if (playerGroup == null) { playerGroup = transform.GetComponentsInChildren<HorizontalLayoutGroup>()[1]; }

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
        if (ready && actionQueue == null)
        {
            float deltaTime = Time.deltaTime;
            foreach (CombatantInfo combatant in combatants) { combatant.aP += (int)(deltaTime * 1000); }
        }

        //If there are messages in the queue then display them
        else if (messageQueue != null)
        {
            //Display the current message
            text.text = messageQueue[0];

            //Delete the current message if left click is pressed and nullify the messageQueue if there are no messages left
            if (Input.GetMouseButton(0))
            {
                messageQueue.RemoveAt(0);
                if (messageQueue.Count == 0) 
                { 
                    messageQueue = null;
                }
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

    //I might just have this do something with a menu object
    void PlayerButton(CombatantInfo combatant)
    {
        //This all only works if the action queue is empty
        if (actionQueue == null)
        {
            //Open up a new action queue
            actionQueue = new ActionBuilder(combatant);

            //Create the action menu at the click point
            actionMenu = Instantiate(actionMenuTemplate, transform);
            if (actionMenu.TryGetComponent(out RectTransform rectTransform)) { rectTransform.anchoredPosition = Input.mousePosition; }

            //Add job skills as necessary


            //Attach handlers to the menu buttons (Create ones for job skills as necessary)
            for (int i = 0; i < actionMenu.transform.childCount; i++)
            {
                //Attack
                if (i == 0)
                {
                    //The Action Builder needs the delegate to be the combatant's Attack function
                    actionMenu.GetComponentsInChildren<Button>()[i].onClick.AddListener(() => AssignActionDelegate(combatant, "Attack") );
                }

                //Job skills
                else if (i == 1 && i < actionMenu.transform.childCount - 2)
                {

                }

                //Sub-job skills
                else if (i == 2 && i < actionMenu.transform.childCount - 2)
                {

                }

                //Defend
                else if (i == actionMenu.transform.childCount - 2)
                {

                }

                //Item
                else if (i == actionMenu.transform.childCount - 1)
                {

                }
            }
        }
    }

    //Perhaps I could have this return a targeting procedure
    void AssignActionDelegate(CombatantInfo combatant, string action)
    {
        //This is where the delegate proper is assigned
        print("Succeeded in assigning a delegate for " + action);
        TargetTag targetTag = TargetTag.SELF;
        if (actionQueue != null) { actionQueue.action = combatant.Command(action, ref targetTag); }
        
        //This is where I transition to targeting procedure

    }
}
