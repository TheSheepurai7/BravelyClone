using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ActionMenu : MonoBehaviour
{
    //I might want to try making an event to signal to the BattleManager to pick up the CombatAction and targets to integrate into the ActionBuilder
    CombatAction currentAction;
    List<CombatantInfo> currentTargets;

    //StatBlock from which to execute common commands
    IStatReader statBlock;

    //Button template
    GameObject buttonTemplate;

    //I should at least use awake to wipe events
    void Awake()
    {
        //Reawaken the image just in case
        GetComponent<Image>().enabled = true;

        //Make sure there's a BattleManager instance accessible
        if (BattleManager.instance == null) { throw new Exception("Action menu must report back to a battle manager to function"); }

        //Cache the button template if not already cached
        if (buttonTemplate == null) { buttonTemplate = Resources.Load<GameObject>("Prefabs/ActionButton"); }
    }

    //Public functions
    public void PopulateMenu(IStatReader statBlock)
    {
        //Store the stat block for when the commands are needed
        this.statBlock = statBlock;

        //Populate the command list
        int commandsAvailable = 3 + Convert.ToInt32(statBlock.ReadCommands(Stats.JOB) != null) + Convert.ToInt32(statBlock.ReadCommands(Stats.SJB) != null);
        for (int i = 0; i < commandsAvailable; i++)
        {
            //Instantiate a new button and attach it to the action menu
            GameObject button = Instantiate(buttonTemplate, transform);

            //Rename the button and assign it functionality based on order
            if (i == 0) { CreateCommandButton(ref button, new CommandInfo("Attack", TargetTag.SINGLE, Attack), false); } //Attack
            else if (i == 1 && i < commandsAvailable - 2) { } //Job command
            else if (i == 2 && i < commandsAvailable - 2) { } //SubJob command
            else if (i == commandsAvailable - 2) { CreateCommandButton(ref button, new CommandInfo("Defend", TargetTag.SELF, Defend), false); } //Defend
            else if (i == commandsAvailable - 1) { CreateCommandButton(ref button, new CommandInfo("Item", TargetTag.SINGLE, Item), false); } //Item (Delegate to sub-menu later)
        }
    }

    //Private functions
    void CreateCommandButton(ref GameObject button, CommandInfo info, bool bold)
    {
        if (button.TryGetComponent(out Button buttonComp))
        {
            if (button.GetComponentInChildren<Text>() != null)
            {
                if (bold) { button.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold; }
                button.GetComponentInChildren<Text>().text = info.name;
                buttonComp.onClick.AddListener(() => StartCoroutine(SelectTarget(info)));
            }
            else { throw new Exception("Button template needs a text component to print to"); }
        }
        else { throw new Exception("Button template needs a button component otherwise why are you calling it a button?"); }
    }

    //I should make this the coroutine and see if that works
    IEnumerator SelectTarget(CommandInfo info)
    {
        //I need to hide the menu first and foremost
        GetComponent<Image>().enabled = false;
        foreach (Transform child in transform) { child.gameObject.SetActive(false); }

        //Assign currentAction as info's delegate
        currentAction = info.action;

        //Preselect targets for target tags that require such a thing and simply wait for confirmation (Self, All, AllElse, and Field)

        //Otherwise poll for a target
        PointerEventData pointer = new PointerEventData(EventSystem.current);
        List<RaycastResult> results = new List<RaycastResult>();
        TargetTag proxyTag = info.tag; //Really only for use with the variable tag
        while (currentTargets == null)
        {
            //Get raycast data
            pointer.position = Input.mousePosition;
            ClearResults(ref results);
            results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointer, results);

            //Process events based on target tag (if there are targets to process)
            if (results.Count > 0)
            {
                switch (proxyTag)
                {
                    case TargetTag.SINGLE:
                        //Hover procedure
                        if (results[0].gameObject.TryGetComponent(out Image image)) { image.color += (Color)new Vector4(0, 0, 0, 1); }

                        //Click procedure
                        if (Input.GetMouseButtonDown(0) && results[0].gameObject.TryGetComponent(out CharacterDisplay cd))
                        { currentTargets = new List<CombatantInfo>() { cd.ExtractCombatant() }; ClearResults(ref results); }
                        break;

                    default: throw new Exception("Support for " + proxyTag + " targeting has not been implemented yet");
                }
            }

            //Change the proxy tag if the default tag is variable

            //Wait until next frame
            yield return null;
        }

        //Ship off the delegate and targets to the Battle Manager
        BattleManager.instance.actionQueue.action = currentAction;
        BattleManager.instance.actionQueue.targets = currentTargets;

        //Delete all buttons
        foreach (Transform child in transform) { Destroy(child.gameObject); }

        //Wipe the component
        currentTargets = null;
        GetComponent<Image>().enabled = true;
        gameObject.SetActive(false);
    }

    void ClearResults(ref List<RaycastResult> results)
    {
        //All of the images detected by the previous cast have their opacity reset to zero
        foreach(RaycastResult result in results) { if (result.gameObject.TryGetComponent(out Image image)) { image.color *= new Vector4(1, 1, 1, 0); } }

        //It's a fresh list
        results = new List<RaycastResult>();
    }

    //Universal commands
    List<string> Attack(CombatantInfo source, List<CombatantInfo> targets)
    {
        return new List<string> { "I have confirmed the attack was successful. Now to confirm the timer starts after this message disappears" };
    }

    List<string> Defend(CombatantInfo source, List<CombatantInfo> targets)
    {
        return null;
    }

    List<string> Item(CombatantInfo source, List<CombatantInfo> targets)
    {
        return null;
    }
}
