using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //Singleton instance
    public static GameManager instance;

    //Events
    public event TimeEvent onUpdate;

    //Serialize fields
    [SerializeField] Transform battleScreen;
    [SerializeField] Text battleDisplay;
    [SerializeField] Button readyButton;
    HorizontalLayoutGroup enemyGroup;
    HorizontalLayoutGroup playerGroup;
    GameObject actionMenu;

    //Party data
    public List<Enemy> enemyParty; //Hide in inspector later
    [HideInInspector] public List<CharacterData> playerParty; //I think later I can have some kind of stat assignment component in the party manager and import from that

    //Misc variables
    [HideInInspector] public bool ready;
    Dictionary<Consumable, int> inventory;
    public bool winconMet { get { foreach (EnemyScript enemy in enemyGroup.GetComponentsInChildren<EnemyScript>()) { if (enemy.statBlock.currentHP > 0) { return false; } } return true; } }
    bool loseconMet { get { foreach (PlayerScript player in playerGroup.GetComponentsInChildren<PlayerScript>()) { if (player.statBlock.currentHP > 0) { return false; } } return true; } }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Enforce singleton
        if (instance == null) { instance = this; } else if (instance != this) { Destroy(instance); }

        //Manage frame rate
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;

        //Relegate this to party confirm later
        CreateEnemies();
        playerParty = new List<CharacterData>() { new CharacterData(Character.ALEC, 4) , new CharacterData(Character.MARISA, 4),
            new CharacterData(Character.JENNA, 4) , new CharacterData(Character.GARETH, 4) };
        inventory = new Dictionary<Consumable, int> { { new Consumable("Potion", TargetTag.SINGLE, ConsumableEffects.Potion), 5 } };
    }

    // Update is called once per frame
    void Update()
    {
        //Basically just time scale
        if (ready)
        {
            onUpdate(Time.deltaTime);
        }

        //
    }

    public void CloseActionMenu(ref StatBlock statBlock)
    {
        if (actionMenu != null && actionMenu.TryGetComponent(out ActionMenu menu)) { if (statBlock == menu.source) { Destroy(actionMenu.gameObject); } }
    }

    public void RemoveItem(Consumable consumable)
    {
        if (inventory.ContainsKey(consumable)) { inventory[consumable]--; }
        else { print("Key not found"); }
    }

    public void ForceGlobalThreat(ref StatBlock source, int amt)
    {
        foreach(EnemyScript enemy in enemyGroup.GetComponentsInChildren<EnemyScript>()) { enemy.statBlock.GenerateThreat(ref source, amt); }
    }

    public void CreateEnemies()
    {
        //Make sure the object is even active first
        if (battleScreen != null && battleScreen.GetComponentsInChildren<HorizontalLayoutGroup>()[0] != null)
        {
            //Cache the enemy group if necessary
            if (enemyGroup == null) { enemyGroup = battleScreen.GetComponentsInChildren<HorizontalLayoutGroup>()[0]; }

            //Cycle through the enemy party
            foreach (Enemy enemy in enemyParty)
            {
                //I guess I start by instantiating and placing the prefab
                GameObject enemyGFX = Instantiate(Resources.Load<GameObject>("Prefabs/Enemy"), enemyGroup.transform);

                //Make sure the object has a combatant script and give it the enemy's stat block
                if (enemyGFX.TryGetComponent(out EnemyScript combat)) { combat.statBlock = enemy.ExportStatBlock(); combat.ai = enemy.getAI; }
            }
        }
        else { throw new Exception("Check if the battle screen is set or there is a Horizontal layout group among the children"); }
    }

    public void Ready()
    {
        //Make sure there's a horizontal layout group to print the player party to
        if (battleScreen != null && battleScreen.GetComponentsInChildren<HorizontalLayoutGroup>()[1] != null)
        {
            //Cache the player group if necessary
            if (playerGroup == null) { playerGroup = battleScreen.GetComponentsInChildren<HorizontalLayoutGroup>()[1]; }

            //Cycle through the player party
            foreach (CharacterData data in playerParty)
            {
                //I guess I start by instantiating and placing the prefab
                GameObject playerGFX = Instantiate(Resources.Load<GameObject>("Prefabs/Player"), playerGroup.transform);

                //Make sure the object has a combatant script and give it the player's stat block
                if (playerGFX.TryGetComponent(out CombatantScript combat)) { combat.statBlock = data.ExportStatBlock(); } //Also add a way to add commands here
            }
        }
        else { throw new Exception("Check if the battle screen is set or there is a second Horizontal layout group among the children"); }

        //Set apMax of all combatants based on the lowest speed
        List<StatBlock> combatants = new List<StatBlock>();
        foreach (CombatantScript script in enemyGroup.GetComponentsInChildren<CombatantScript>()) { combatants.Add(script.statBlock); }
        foreach (CombatantScript script in playerGroup.GetComponentsInChildren<CombatantScript>()) { combatants.Add(script.statBlock); }
        combatants = combatants.OrderBy(item => item.spd).ToList();
        foreach (StatBlock combatant in combatants) { combatant.apMax = combatants[0].spd; }

        //Set initial AP of each combatant
        for (int i = 0; i < combatants.Count; i++)
        {
            combatants[i].aP = (int)(combatants[i].apMax * ((i + 1f) / (combatants.Count + 2f))); //Add randomness later
        }

        //Then I would set the update loop as ready
        ready = true;
    }

    public void SpawnActionMenu(ref StatBlock requester) //Eventually I'm going to need a way to pass the action menu submenus based on job choice
    {
        //First it's prudent to freeze time
        ready = false;

        //Cache the action menu if not already cached
        if (actionMenu == null) { actionMenu = Instantiate(Resources.Load<GameObject>("Prefabs/ActionMenu"), battleScreen); }

        //Set the action menu active at the mouse cursor
        actionMenu.SetActive(true);
        actionMenu.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;

        //Open the action menu
        actionMenu.GetComponent<ActionMenu>().PopulateMenu(ref requester, null, CreateMenu());
    }

    public void SetLoseCon()
    {
        if (loseconMet)
        {
            //Make it impossible to complete or input any new commands (Just in case)
            ready = false;
            Destroy(actionMenu.gameObject);

            //Launch game over screen
            StartCoroutine(DefeatProcedure());
        }
    }

    public void SetWinCon(ref Action deathAnim)
    {
        //Actually check if the wincon has been met. Return if not
        if (winconMet)
        {
            //Make it impossible to complete or input any new commands
            ready = false;
            Destroy(actionMenu.gameObject);

            //Tell me when the final enemy is done dying
            deathAnim += () => StartCoroutine(VictoryProcedure());
        }
    }

    IEnumerator DefeatProcedure()
    {
        //Display game over message
        battleDisplay.text = "Game Over...";
        while (!Input.GetMouseButtonDown(0)) { yield return null; }
        yield return null;

        //Reset everything
        battleDisplay.text = string.Empty;
        readyButton.gameObject.SetActive(true);
        onUpdate = null;
        for (int i = 0; i < Mathf.Max(enemyGroup.transform.childCount, playerGroup.transform.childCount); i++)
        {
            if (i < enemyGroup.transform.childCount) { Destroy(enemyGroup.transform.GetChild(i).gameObject); }
            if (i < playerGroup.transform.childCount) { Destroy(playerGroup.transform.GetChild(i).gameObject); }
        }

        //Loop procedure
        Loop();
    }

    IEnumerator VictoryProcedure()
    {
        //Victory message 1
        battleDisplay.text = "Victory has been achieved";
        while (!Input.GetMouseButtonDown(0)) { yield return null; }
        yield return null;

        //Victory message 2
        battleDisplay.text = "This would be where I displayed rewards if I had any";
        while (!Input.GetMouseButtonDown(0)) { yield return null; }
        yield return null;

        //Reset everything
        battleDisplay.text = string.Empty;
        readyButton.gameObject.SetActive(true);
        onUpdate = null;
        for (int i = 0; i < Mathf.Max(enemyGroup.transform.childCount, playerGroup.transform.childCount); i++)
        {
            if (i < enemyGroup.transform.childCount) { Destroy(enemyGroup.transform.GetChild(i).gameObject); }
            if (i < playerGroup.transform.childCount) { Destroy(playerGroup.transform.GetChild(i).gameObject); }
        }

        //Loop procedure
        Loop();
    }

    void Loop()
    {
        //For now it's just creating new instances of the enemy and player parties
        CreateEnemies();
        playerParty = new List<CharacterData>() { new CharacterData(Character.ALEC, 4), new CharacterData(Character.MARISA, 4),
            new CharacterData(Character.JENNA, 4), new CharacterData(Character.GARETH, 4) };
    }

    List<CommandInfo> CreateMenu()
    {
        //Attack is pretty much guaranteed to be here
        List<CommandInfo> returnValue = new List<CommandInfo>() { new ActionInfo("Attack", TargetTag.SINGLE, Attack) };

        //Now how to create the item command
        List<CommandInfo> inventoryList = new List<CommandInfo>();
        foreach (KeyValuePair<Consumable, int> item in inventory) 
        {
            if (item.Value > 0) { inventoryList.Add(new ActionInfo(item.Key.name + " x" + item.Value, item.Key.tag, item.Key.Use)); }
        }
        returnValue.Add(new MenuInfo("Item", inventoryList));

        //Give me the menu
        return returnValue;
    }

    void Attack(StatBlock source, List<StatBlock> targets)
    {
        //First subtract the AP for the action
        source.aP -= source.apMax;

        //Calculate and apply damage
        int damage = (int)Mathf.Clamp((source.pAtk - targets[0].pDef) * UnityEngine.Random.Range(0.8f, 1), 1, Mathf.Infinity);
        targets[0].currentHP -= damage;
        targets[0].GenerateThreat(ref source, damage);
    }
}
