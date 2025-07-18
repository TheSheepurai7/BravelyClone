using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharacterDisplay : MonoBehaviour, IPointerClickHandler
{
    //Color constants
    Color AP_ZERO = new Color32(120, 0, 0, 255);
    Color AP_ONE = new Color32(0, 255, 0, 255);
    Color AP_TWO = new Color32(255, 255, 0, 255);
    Color AP_THREE = new Color32(255, 150, 0, 255);
    Color AP_MAX = new Color32(255, 0, 255, 255);

    //The stat block to read from
    IStatReader statBlock;
    Dictionary<Stats, List<GameObject>> displayDictionary = new Dictionary<Stats, List<GameObject>>(); //I may not need this, so I might delete it later

    //Mouse events
    event Action onLeftClick;
    event Action onRightClick;

    void Awake()
    {
        //Reset everything for simplicity's sake
        statBlock = null;
        displayDictionary.Clear();
    }
    public void AssignStatBlock(IStatReader statBlock)
    {
        //First thing's first: Copy the stat block
        this.statBlock = statBlock;

        //Display every stat possible. Just scattershot it. This is really only meant to be called once during the screen transition
        foreach (Transform display in GetComponentsInChildren<Transform>())
        {
            //I guess I can switch tags
            switch (display.tag)
            {
                case "Sprite":
                    if (display.TryGetComponent(out Image sprite))
                    {
                        sprite.sprite = statBlock.ReadImage();
                        if (!displayDictionary.ContainsKey(Stats.SPRITE)) { displayDictionary.Add(Stats.SPRITE, new List<GameObject> { display.gameObject }); }
                        else { displayDictionary[Stats.SPRITE].Add(display.gameObject); }
                    }
                    break;
                case "Name":
                    if (display.TryGetComponent(out Text name))
                    {
                        name.text = statBlock.ReadString(Stats.NAME);
                        if (!displayDictionary.ContainsKey(Stats.NAME)) { displayDictionary.Add(Stats.NAME, new List<GameObject>() { display.gameObject }); }
                        else { displayDictionary[Stats.NAME].Add(display.gameObject); }
                    }
                    break;
                case "Job":
                    if (display.TryGetComponent(out Text job))
                    {
                        job.text = statBlock.ReadString(Stats.JOB);
                        if (!displayDictionary.ContainsKey(Stats.JOB)) { displayDictionary.Add(Stats.JOB, new List<GameObject>() { display.gameObject }); }
                        else { displayDictionary[Stats.JOB].Add(display.gameObject); }
                    }
                    break;
                case "Level":
                    if (display.TryGetComponent(out Text level))
                    {
                        level.text = "Lv. " + statBlock.ReadInt(Stats.LVL);
                        if (!displayDictionary.ContainsKey(Stats.LVL)) { displayDictionary.Add(Stats.LVL, new List<GameObject>() { display.gameObject }); }
                        else { displayDictionary[Stats.LVL].Add(display.gameObject); }
                    }
                    break;
                case "HP":
                    if (display.TryGetComponent(out Text hp))
                    {
                        hp.text = statBlock.ReadInt(Stats.CHP) + "/" + statBlock.ReadInt(Stats.MHP);
                        if (!displayDictionary.ContainsKey(Stats.CHP)) { displayDictionary.Add(Stats.CHP, new List<GameObject>() { display.gameObject }); }
                        else { displayDictionary[Stats.CHP].Add(display.gameObject); }
                    }
                    break;
                case "MP":
                    if (display.TryGetComponent(out Text mp))
                    {
                        mp.text = statBlock.ReadInt(Stats.CMP) + "/" + statBlock.ReadInt(Stats.MMP);
                        if (!displayDictionary.ContainsKey(Stats.CMP)) { displayDictionary.Add(Stats.CMP, new List<GameObject>() { display.gameObject }); }
                        else { displayDictionary[Stats.CMP].Add(display.gameObject); }
                    }
                    break;
                case "HP Meter":
                    if (display.TryGetComponent(out RectTransform hpMeter) && display.parent.TryGetComponent(out RectTransform hpBack))
                    {
                        float maxWidth = hpBack.rect.width;
                        hpMeter.sizeDelta = new Vector2(maxWidth * statBlock.ReadInt(Stats.CHP) / statBlock.ReadInt(Stats.MHP), hpMeter.sizeDelta.y);
                        if (!displayDictionary.ContainsKey(Stats.CHP)) { displayDictionary.Add(Stats.CHP, new List<GameObject>() { display.gameObject }); }
                        else { displayDictionary[Stats.CHP].Add(display.gameObject); }
                    }
                    break;
                case "MP Meter":
                    if (display.TryGetComponent(out RectTransform mpMeter) && display.parent.TryGetComponent(out RectTransform mpBack))
                    {
                        float maxWidth = mpBack.rect.width;
                        mpMeter.sizeDelta = new Vector2(maxWidth * statBlock.ReadInt(Stats.CMP) / statBlock.ReadInt(Stats.MMP), mpMeter.sizeDelta.y);
                        if (!displayDictionary.ContainsKey(Stats.CMP)) { displayDictionary.Add(Stats.CMP, new List<GameObject>() { display.gameObject }); }
                        else { displayDictionary[Stats.CMP].Add(display.gameObject); }
                    }
                    break;
                case "AP":
                    if (display.TryGetComponent(out Text ap))
                    {
                        ap.text = statBlock.ReadInt(Stats.AP).ToString();
                        if (!displayDictionary.ContainsKey(Stats.AP)) { displayDictionary.Add(Stats.AP, new List<GameObject>() { display.gameObject }); }
                        else { displayDictionary[Stats.AP].Add(display.gameObject); }
                    }
                    break;
                case "AP Meter":
                    if (display.TryGetComponent(out RectTransform apMeter) && display.parent.TryGetComponent(out RectTransform apBack))
                    {
                        float maxHeight = apBack.rect.height;
                        apMeter.sizeDelta = new Vector2(apMeter.sizeDelta.x, maxHeight * statBlock.ReadFloat(Stats.AP));
                        if (apMeter.TryGetComponent(out Image meterImage) && apBack.TryGetComponent(out Image backImage)) { UpdateColors(meterImage, backImage); }
                        if (!displayDictionary.ContainsKey(Stats.AP)) { displayDictionary.Add(Stats.AP, new List<GameObject>() { display.gameObject }); }
                        else { displayDictionary[Stats.AP].Add(display.gameObject); }
                    }
                    break;
            }
        }

        //Pass on an UpdateStat delegate to what ever event the stat reader wants to use for displays
        //If I go the dictionary route, the function should see if the dictionary has that stat to display then cycle through the list of objects associated with that stat
        UpdateStats updateStat = UpdateStat;
        statBlock.UpdateDisplay(ref updateStat);
    }

    public void UpdateStat(Stats stat)
    {
        //*Cycle through all the game objects under the stat to be updated and update them according to their tag
        if (displayDictionary.ContainsKey(stat))
        {
            foreach (GameObject display in displayDictionary[stat])
            {
                switch (display.tag)
                {
                    case "AP":
                        if (display.TryGetComponent(out Text ap)) { ap.text = statBlock.ReadInt(Stats.AP).ToString(); }
                        break;
                    case "AP Meter":
                        if (display.TryGetComponent(out RectTransform apMeter) && display.transform.parent.TryGetComponent(out RectTransform apBack))
                        {
                            float maxHeight = apBack.rect.height;
                            apMeter.sizeDelta = new Vector2(apMeter.sizeDelta.x, maxHeight * statBlock.ReadFloat(Stats.AP));
                            if (apMeter.TryGetComponent(out Image meterImage) && apBack.TryGetComponent(out Image backImage)) { UpdateColors(meterImage, backImage); }
                        }
                        break;
                }
            }
        }
    }

    public bool ExtractCombatant(out CombatantInfo combatant)
    {
        try { combatant = (CombatantInfo)statBlock; return true; } 
        catch { combatant = null; return false; }
    }

    void UpdateColors(Image apMeter, Image apBack)
    {
        switch (statBlock.ReadInt(Stats.AP))
        {
            case 0:
                apMeter.color = AP_ONE; apBack.color = AP_ZERO;
                break;
            case 1:
                apMeter.color = AP_TWO; apBack.color = AP_ONE;
                break;
            case 2:
                apMeter.color = AP_THREE; apBack.color = AP_TWO;
                break;
            case 3:
                apMeter.color = AP_MAX; apBack.color = AP_THREE;
                break;
            case 4:
                apMeter.color = AP_MAX; apBack.color = AP_MAX;
                break;
        }
    }

    //Click event stuff
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) { onLeftClick?.Invoke(); }
        else if (eventData.button == PointerEventData.InputButton.Right) { onRightClick?.Invoke(); }
    }

    //Delegate subscriptions
    public void SubscribeLeftClick(Action theDelegate) { onLeftClick += theDelegate; }
    public void SubscribeRightClick(Action theDelegate) { onRightClick += theDelegate; }
}
