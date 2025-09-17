using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionMenu : MonoBehaviour
{
    //Action components
    public StatBlock source { get { return _source; } private set { _source = value; } }
    StatBlock _source;
    List<StatBlock> targets;
    ActionInfo action;

    //Misc variables
    ActionMenu parent;
    GameObject buttonTemplate;
    Coroutine pollTarget;
    Image buffer = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //The GameManager must be detectable
        if (GameManager.instance == null) { Destroy(gameObject); }

        //Cache the button template if not already cached
        if (buttonTemplate == null) { buttonTemplate = Resources.Load<GameObject>("Prefabs/ActionButton"); }
    }

    void Update()
    {
        //I need the ability to go back
        //I wonder if I can just deactivate the parent action menu entirely when a child menu is populated.
        if (Input.GetMouseButtonDown(1))
        {
            //An action hasn't been selected yet, so I should clear the whole thing
            if (action.Equals(default(CommandInfo)))
            {
                foreach (Transform t in transform) { Destroy(t.gameObject); }
                source = null; action = null; targets = null;
                GameManager.instance.ready = true;
                gameObject.SetActive(false);
            }

            //Return to action selection otherwise
            else
            {
                StopCoroutine(pollTarget); pollTarget = null;
                if (buffer != null) { buffer.color *= new Vector4(1, 1, 1, 0); buffer = null; }
                action = null;
                GetComponent<Image>().enabled = true;
                foreach (Transform t in transform) { t.gameObject.SetActive(true); }
            }
        }
    }

    public void DestroyRoot()
    {
        if (parent != null) { parent.DestroyRoot(); }
        else { Destroy(gameObject); }
    }

    public void EnableMenu(bool buttons, bool images)
    {
        if (parent != null) { parent.EnableMenu(buttons, images); }
        foreach (Button button in GetComponentsInChildren<Button>()) { button.enabled = buttons; }
        foreach (Text text in GetComponentsInChildren<Text>()) { text.enabled = images; }
        foreach (Image image in GetComponentsInChildren<Image>()) { image.enabled = images; }
    }

    public void PopulateMenu(ref StatBlock statBlock, ActionMenu parent, List<CommandInfo> commands) 
    {
        //Assign parent
        this.parent = parent;
        //Disable the parent's buttons if there's a parent at all
        if (this.parent != null) { parent.EnableMenu(false, true); }

        //Assign source
        source = statBlock;

        //Resize the menu to fit all the commands it will need
        GetComponent<RectTransform>().sizeDelta =
            new Vector2(GetComponent<RectTransform>().sizeDelta.x, buttonTemplate.GetComponent<RectTransform>().sizeDelta.y * commands.Count);

        //Create commands
        for (int i = 0; i < commands.Count; i++)
        {
            //Instantiate a new button and attach it to the action menu
            GameObject button = Instantiate(buttonTemplate, transform);

            //Assign the button functionality
            if (commands[i] is ActionInfo) { CreateCommandButton(ref button, (ActionInfo)commands[i]); }
            else if (commands[i] is MenuInfo) { CreateSubMenu(ref button, (MenuInfo)commands[i]); }
        }
    }

    void CreateCommandButton(ref GameObject button, ActionInfo info)
    {
        if (button.TryGetComponent(out Button buttonComp))
        {
            if (button.GetComponentInChildren<Text>() != null)
            {
                button.GetComponentInChildren<Text>().text = info.name;
                buttonComp.onClick.AddListener(() =>
                {
                    action = info;
                    GetComponent<Image>().enabled = false;
                    EnableMenu(false, false);
                    pollTarget = StartCoroutine(PollTarget());
                });
            }
            else { throw new Exception("Button template needs a text component to print to"); }
        }
        else { throw new Exception("Button template needs a button component otherwise why are you calling it a button?"); }
    }

    void CreateSubMenu(ref GameObject button, MenuInfo info)
    {
        if (button.TryGetComponent(out Button buttonComp))
        {
            if (button.GetComponentInChildren<Text>() != null)
            {
                //Set the name
                button.GetComponentInChildren<Text>().text = info.name;

                //Functionality depends on if there's any menu info at all
                if (info.commands.Count > 0)
                {
                    buttonComp.onClick.AddListener(() =>
                    {
                        GameObject subMenu = Instantiate(gameObject, transform.parent);
                        subMenu.GetComponent<RectTransform>().anchoredPosition = Input.mousePosition;
                        foreach (Transform child in subMenu.transform) { Destroy(child.gameObject); }
                        subMenu.GetComponent<ActionMenu>().PopulateMenu(ref _source, this, info.commands);
                    });
                }

                //Otherwise gray out the button
                else { button.GetComponentInChildren<Text>().color = Color.gray; }
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
        DestroyRoot();
    }

    
}
