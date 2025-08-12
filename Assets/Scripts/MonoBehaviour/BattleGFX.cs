using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BattleGFX : MonoBehaviour
{
    //The instance storage
    //If I run into threading issues, I need to create a lock object
    public static BattleGFX instance = null;

    //Serialize fields
    [SerializeField] Text textOutput;
    [SerializeField] Button readyButton;

    //Cache
    GameObject playerGFXTemplate;
    GameObject enemyGFXTemplate;
    GameObject actionMenu;
    HorizontalLayoutGroup playerGroup;
    HorizontalLayoutGroup enemyGroup;

    void OnEnable()
    {
        //Enforce singleton
        if (instance == null) { instance = this; } else if (instance != this) { Destroy(this); }

        //Cache things as necessary
        if (playerGFXTemplate == null) { playerGFXTemplate = Resources.Load<GameObject>("Prefabs/PlayerGFX"); }
        if (enemyGFXTemplate == null) { enemyGFXTemplate = Resources.Load<GameObject>("Prefabs/EnemyGFX"); }
        if (enemyGroup == null) { enemyGroup = transform.GetComponentsInChildren<HorizontalLayoutGroup>()[0]; }
        if (playerGroup == null) { playerGroup = transform.GetComponentsInChildren<HorizontalLayoutGroup>()[1]; }
        if (actionMenu == null) { actionMenu = Instantiate(Resources.Load<GameObject>("Prefabs/ActionMenu"), transform); }

        //Clear the text output
        textOutput.text = string.Empty;

        //Make sure the action menu is off
        actionMenu.SetActive(false);

        //Add listener to the ready button's onclick

        //I need to get the enemy data from the party manager and make graphics based on it
        if (BattleManager.instance.hasEnemies && enemyGroup != null)
        {
            foreach (IStatReader statBlock in BattleManager.instance.enemyList)
            {
                if (enemyGFXTemplate.GetComponent<CharacterDisplay>() != null)
                {
                    GameObject enemyGFX = Instantiate(enemyGFXTemplate, enemyGroup.transform);
                    enemyGFX.GetComponent<CharacterDisplay>().statBlock = statBlock;
                }
            }
        }
    }

    public void Ready()
    {
        //Create the player graphics 
        if (BattleManager.instance.hasPlayers && playerGroup != null)
        {
            foreach (CombatantInfo statBlock in BattleManager.instance.playerList)
            {
                if (playerGFXTemplate.GetComponent<CharacterDisplay>() != null)
                {
                    //Create graphic
                    GameObject playerGFX = Instantiate(playerGFXTemplate, playerGroup.transform);
                    playerGFX.GetComponent<CharacterDisplay>().statBlock = statBlock;

                    //Link interactivity
                    CombatantInfo temp = statBlock;
                    playerGFX.GetComponent<CharacterDisplay>().SubscribeLeftClick(() => OpenActionMenu(temp));
                }
            }
        }
        else{ if (!BattleManager.instance.hasPlayers) { print("Already has players"); } if (playerGroup == null) { print("Player group is null"); } }

        //Tell the Battle Manager to initalize the ATB

        //Tell the Battle Manager to start the battle
        BattleManager.instance.ready = true;
    }

    void OpenActionMenu(CombatantInfo playerInfo)
    {
        //None of this works if action menu doesn't even have an action menu component
        if (actionMenu.TryGetComponent(out ActionMenu amComponent))
        {
            if (BattleManager.instance.ready)
            {
                //Activate the action menu at the spot clicked
                actionMenu.SetActive(true);
                actionMenu.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;
                amComponent.PopulateMenu(playerInfo);
                BattleManager.instance.ready = false;
            }
        }
    }

    public IEnumerator VictoryProcedure()
    {
        //I guess I run through each character display to check which ones still have dimensions?
        bool confirm = false;
        while (!confirm)
        {
            confirm = true;

            foreach (CharacterDisplay display in enemyGroup.GetComponentsInChildren<CharacterDisplay>())
            { if (display.GetComponent<RectTransform>().sizeDelta != Vector2.zero) { confirm = false; break; } }

            yield return null;
        }

        //Display that the player has won
        textOutput.text = "Victory!";
        confirm = false;
        while (!confirm)
        {
            if (Input.GetMouseButtonDown(0)) { confirm = true; }
            yield return null;
        }

        //Display rewards
        textOutput.text = "This is the part where the player would receive rewards";
        confirm = false;
        while (!confirm)
        {
            if (Input.GetMouseButtonDown(0)) { confirm = true; }
            yield return null;
        }

        //Reset the graphics
        foreach (CharacterDisplay display in enemyGroup.GetComponentsInChildren<CharacterDisplay>()) { Destroy(display.gameObject); }
        foreach (CharacterDisplay display in playerGroup.GetComponentsInChildren<CharacterDisplay>()) { Destroy(display.gameObject); }
        readyButton.gameObject.SetActive(true);

        //Return to the encounter select menu
        transform.parent.GetChild(0).gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
