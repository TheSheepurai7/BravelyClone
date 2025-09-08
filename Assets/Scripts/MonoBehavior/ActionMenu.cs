using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionMenu : MonoBehaviour
{
    //Action components
    public StatBlock source { get; private set; }
    List<StatBlock> targets;
    CommandInfo action;

    //Misc variables
    ActionMenu parent;
    GameObject buttonTemplate;
    Coroutine pollTarget;
    Image buffer = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //The GameManager must be detectable
        if (GameManager.instance == null ) { Destroy(gameObject); }

        //Cache the button template if not already cached
        if (buttonTemplate == null) { buttonTemplate = Resources.Load<GameObject>("Prefabs/ActionButton"); }
    }

    void Update()
    {
        //I need the ability to go back
        if (Input.GetMouseButtonDown(1))
        {
            //An action hasn't been selected yet, so I should clear the whole thing
            if (action.Equals(default(CommandInfo)))
            {
                foreach (Transform t in transform) { Destroy(t.gameObject); }
                source = null; action = default(CommandInfo); targets = null;
                GameManager.instance.ready = true;
                gameObject.SetActive(false);
            }

            //Return to action selection otherwise
            else
            {
                StopCoroutine(pollTarget); pollTarget = null;
                if (buffer != null) { buffer.color *= new Vector4(1, 1, 1, 0); buffer = null; }
                action = default(CommandInfo);
                GetComponent<Image>().enabled = true;
                foreach (Transform t in transform) { t.gameObject.SetActive(true); }
            }
        }
    }

    public void EnableMenu(bool buttons, bool images)
    {

    }

    public void PopulateMenu(ref StatBlock statBlock, ActionMenu parent) //Instead of hard-coding Attack and Item and all that stuff I should have commands as arguments
    {
        //Assign parent
        this.parent = parent;

        //Assign source
        source = statBlock;

        //Resize the menu to fit all the commands it will need (For now it's just attack)
        int commandsAvailable = 2; // + Convert.ToInt32(combatant.ReadCommands(Stats.JOB) != null) + Convert.ToInt32(combatant.ReadCommands(Stats.SJB) != null);
        GetComponent<RectTransform>().sizeDelta =
            new Vector2(GetComponent<RectTransform>().sizeDelta.x, buttonTemplate.GetComponent<RectTransform>().sizeDelta.y * commandsAvailable);

        //Create commands
        for (int i = 0; i < commandsAvailable; i++)
        {
            //Instantiate a new button and attach it to the action menu
            GameObject button = Instantiate(buttonTemplate, transform);

            //Rename the button and assign it functionality based on order
            //if (i == 0) { CreateCommandButton(ref button, new CommandInfo("Attack", TargetTag.SINGLE, Attack), false); } //Attack
            //else if (i == commandsAvailable - 1) { CreateSubMenu(ref button, new MenuInfo("Item", new List<CommandInfo>()), false); } //Item
        }
    }

    void CreateCommandButton(ref GameObject button, CommandInfo info, bool bold)
    {
        if (button.TryGetComponent(out Button buttonComp))
        {
            if (button.GetComponentInChildren<Text>() != null)
            {
                if (bold) { button.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold; }
                button.GetComponentInChildren<Text>().text = info.name;
                buttonComp.onClick.AddListener(() =>
                {
                    action = info;
                    GetComponent<Image>().enabled = false;
                    foreach (Transform t in transform) { t.gameObject.SetActive(false); }
                    pollTarget = StartCoroutine(PollTarget());
                });
            }
            else { throw new Exception("Button template needs a text component to print to"); }
        }
        else { throw new Exception("Button template needs a button component otherwise why are you calling it a button?"); }
    }

    void CreateSubMenu(ref GameObject button, MenuInfo info, bool bold)
    {
        if (button.TryGetComponent(out Button buttonComp))
        {
            if (button.GetComponentInChildren<Text>() != null)
            {
                if (bold) { button.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold; }
                button.GetComponentInChildren<Text>().text = info.name;
                buttonComp.onClick.AddListener(() => { print("Accessed " + info.name); });
            }
            else { throw new Exception("Button template needs a text component to print to"); }
        }
        else { throw new Exception("Button template needs a button component otherwise why are you calling it a button?"); }
    }

    IEnumerator PollTarget()
    {
        //Overhead that has to be outside the loop
        PointerEventData pointer = new PointerEventData(EventSystem.current); //Keep an eye on this for clearing the event buffer on ATB return
        TargetTag proxyTag = action.tag; //Really only for use with the variable tag

        while (targets == null)
        {
            //Special cases that simply demand confirmation
            if (action.tag == TargetTag.SELF || action.tag == TargetTag.ALL || action.tag == TargetTag.ALL_ELSE || action.tag == TargetTag.FIELD)
            {
                //I'll deal with this when I have to
            }

            //Otherwise do simple target polling
            else
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
                            if (Input.GetMouseButtonDown(0) && results[0].gameObject.TryGetComponent(out CombatantScript display))
                            { targets = new List<StatBlock>() { display.statBlock }; }

                            break;
                    }
                }
            }

            //Change the proxy tag if the default is variable

            //Wait until next frame
            yield return null;
        }

        //Clear the buffer at least
        if (buffer != null) { buffer.color *= new Vector4(1, 1, 1, 0); buffer = null; }

        //Now I can actually execute the function. Should make sure the source is still alive, though
        if (source.currentHP > 0) { action.action(source, targets); }

        //Check for a wincon and close the menu regardless
        GameManager.instance.ready = !GameManager.instance.winconMet;
        Destroy(gameObject);
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
