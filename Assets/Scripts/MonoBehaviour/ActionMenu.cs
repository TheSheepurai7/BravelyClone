using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ActionMenu : MonoBehaviour
{
    //Return values
    CombatAction currentAction;
    List<CombatantInfo> currentTargets;

    //Misc variables
    GameObject buttonTemplate;

    private void Awake()
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
        int commandsAvailable = 1 + Convert.ToInt32(statBlock.ReadCommands(Stats.JOB) != null) + Convert.ToInt32(statBlock.ReadCommands(Stats.SJB) != null);
        for (int i = 0; i < commandsAvailable; i++)
        {
            //Instantiate a new button and attach it to the action menu
            GameObject button = Instantiate(buttonTemplate, transform);

            //Rename the button and assign it functionality based on order
            if (i == 0) { CreateCommandButton(ref button, new CommandInfo("Attack", TargetTag.SINGLE, Attack), false); } //Attack
            //Item creates a sub-menu and I need a different function for that entirely
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

    IEnumerator SelectTarget(CommandInfo info)
    {
        //I need to hide the menu first and foremost
        GetComponent<Image>().enabled = false;
        foreach (Transform child in transform) { child.gameObject.SetActive(false); }

        //Assign currentAction as info's delegate
        currentAction = info.action;

        //Preselect targets for target tags that require such a thing and simply wait for confirmation (Self, All, AllElse, and Field)
        if (info.tag == TargetTag.SELF || info.tag == TargetTag.ALL || info.tag == TargetTag.ALL_ELSE || info.tag == TargetTag.FIELD)
        {
            print("For the time being the functionality for confirmation tags hasn't been implemented, so have fun with this infinite loop");
            while (true)
            {
                yield return null;
            }
        }

        //Otherwise poll for a target
        else
        {
            Image buffer = null;
            PointerEventData pointer = new PointerEventData(EventSystem.current); //Keep an eye on this for clearing the event buffer on ATB return
            TargetTag proxyTag = info.tag; //Really only for use with the variable tag
            while (currentTargets == null)
            {
                //Clear buffer
                if (buffer != null) { buffer.color *= new Vector4(1, 1, 1, 0); buffer = null; }

                //Get raycast data
                pointer.position = Input.mousePosition;
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(pointer, results);

                //Process events based on target tag (if there are targets to process)
                if (results.Count > 0)
                {
                    switch (proxyTag)
                    {
                        case TargetTag.SINGLE:
                            //Hover procedure
                            if (results[0].gameObject.TryGetComponent(out Image image)) { buffer = image; image.color += (Color)new Vector4(0, 0, 0, 1); }

                            //Click procedure
                            if (Input.GetMouseButtonDown(0) && results[0].gameObject.TryGetComponent(out CharacterDisplay display))
                            { if (display.ExtractCombatant(out CombatantInfo combatant)) { currentTargets = new List<CombatantInfo> { combatant }; } }

                            break;
                    }
                }

                //Change the proxy tag if the default is variable

                //Wait until next frame
                yield return null;
            }

            //Clear the buffer one last time
            if (buffer != null) { buffer.color *= new Vector4(1, 1, 1, 0); buffer = null; }
        }

        //Ship off the delegate and targets to the Battle Manager
        BattleManager.instance.actionQueue[0].action = currentAction;
        BattleManager.instance.actionQueue[0].targets = currentTargets;

        //Delete all buttons
        foreach (Transform child in transform) { Destroy(child.gameObject); }

        //Wipe the component
        currentTargets = null;
        GetComponent<Image>().enabled = true;
        gameObject.SetActive(false);
    }

    //Universal commands
    string Attack(CombatantInfo source, List<CombatantInfo> targets)
    {
        int damage = (int)((source.ReadInt(Stats.PATK) - targets[0].ReadInt(Stats.PDEF)) * (1 + (source.ReadInt(Stats.LVL) / 4)) * UnityEngine.Random.Range(0.8f, 1));
        targets[0].currentHP -= damage;
        return source.name + " attacked " + targets[0].name + " and dealt " + damage.ToString() + " damage.";
    }
}
