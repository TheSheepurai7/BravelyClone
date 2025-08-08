using System;
using System.Collections;
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

    //Other constants
    float FADE_SPEED = 1.5f;

    //Mouse events
    event Action onLeftClick;
    event Action onRightClick;

    //Stat block
    public IStatReader statBlock { get { return _statBlock; } set { AssignStatBlock(value); } }
    private IStatReader _statBlock;

    //Misc variable
    Vector2 displayDimensions;

    void Awake()
    {
        if (displayDimensions == Vector2.zero) { displayDimensions = GetComponent<RectTransform>().sizeDelta; }
    }

    public bool ExtractCombatant(out CombatantInfo combatant)
    {
        try { combatant = (CombatantInfo)statBlock; return true; }
        catch { combatant = null; return false; }
    }

    void AssignStatBlock(IStatReader inStatBlock)
    {
        //Actual assignment
        _statBlock = inStatBlock;

        //Run down the list of stats to display
        foreach (Transform display in GetComponentsInChildren<Transform>())
        {
            switch (display.tag)
            {
                case "Sprite":
                    if (display.TryGetComponent(out Image image)) { image.sprite = _statBlock.ReadImage(); }
                    break;
                case "Name":
                    if (display.TryGetComponent(out Text nameText)) { nameText.text = _statBlock.ReadString(Stats.NAME); }
                    break;
                case "Job":
                    if (display.TryGetComponent(out Text jobText)) { jobText.text = _statBlock.ReadString(Stats.JOB); }
                    break;
                case "Level":
                    if (display.TryGetComponent(out Text lvlText)) { lvlText.text = "Lv. " + _statBlock.ReadInt(Stats.LVL); }
                    break;
                case "HP":
                    if (display.TryGetComponent(out Text hpText)) { hpText.text = _statBlock.ReadInt(Stats.CHP) + "/" + _statBlock.ReadInt(Stats.MHP); }
                    break;
                case "MP":
                    if (display.TryGetComponent(out Text mpText)) { mpText.text = _statBlock.ReadInt(Stats.CMP) + "/" + _statBlock.ReadInt(Stats.MMP); }
                    break;
                case "HP Meter":
                    if (display.TryGetComponent(out RectTransform hpMeter) && display.parent.TryGetComponent(out RectTransform hpBack))
                    {
                        float maxWidth = hpBack.rect.width;
                        hpMeter.sizeDelta = new Vector2(maxWidth * statBlock.ReadInt(Stats.CHP) / statBlock.ReadInt(Stats.MHP), hpMeter.sizeDelta.y);
                    }
                    break;
                case "MP Meter":
                    if (display.TryGetComponent(out RectTransform mpMeter) && display.parent.TryGetComponent(out RectTransform mpBack))
                    {
                        float maxWidth = mpBack.rect.width;
                        mpMeter.sizeDelta = new Vector2(maxWidth * statBlock.ReadInt(Stats.CMP) / statBlock.ReadInt(Stats.MMP), mpMeter.sizeDelta.y);
                    }
                    break;
                case "AP":
                    if (display.TryGetComponent(out Text ap))
                    {
                        ap.text = statBlock.ReadInt(Stats.AP).ToString();
                    }
                    break;
                case "AP Meter":
                    if (display.TryGetComponent(out RectTransform apMeter) && display.parent.TryGetComponent(out RectTransform apBack))
                    {
                        float maxHeight = apBack.rect.height;
                        apMeter.sizeDelta = new Vector2(apMeter.sizeDelta.x, maxHeight * statBlock.ReadFloat(Stats.AP));
                        if (apMeter.TryGetComponent(out Image meterImage) && apBack.TryGetComponent(out Image backImage)) { UpdateColors(meterImage, backImage); }
                    }
                    break;
            }
        }

        //Listen for any stat changes in the stat block
        CharacterDisplay outDisplay = this;
        statBlock.UpdateDisplay(ref outDisplay);
    }

    public void UpdateStats(Stats stat)
    {
        foreach (Transform display in GetComponentsInChildren<Transform>())
        {
            switch (display.tag)
            {
                case "Sprite":
                    if (display.TryGetComponent(out Image image)) { image.sprite = _statBlock.ReadImage(); }
                    break;
                case "Name":
                    if (display.TryGetComponent(out Text nameText)) { nameText.text = _statBlock.ReadString(Stats.NAME); }
                    break;
                case "Job":
                    if (display.TryGetComponent(out Text jobText)) { jobText.text = _statBlock.ReadString(Stats.JOB); }
                    break;
                case "Level":
                    if (display.TryGetComponent(out Text lvlText)) { lvlText.text = "Lv. " + _statBlock.ReadInt(Stats.LVL); }
                    break;
                case "HP":
                    if (display.TryGetComponent(out Text hpText)) { hpText.text = _statBlock.ReadInt(Stats.CHP) + "/" + _statBlock.ReadInt(Stats.MHP); }
                    break;
                case "MP":
                    if (display.TryGetComponent(out Text mpText)) { mpText.text = _statBlock.ReadInt(Stats.CMP) + "/" + _statBlock.ReadInt(Stats.MMP); }
                    break;
                case "HP Meter":
                    if (display.TryGetComponent(out RectTransform hpMeter) && display.parent.TryGetComponent(out RectTransform hpBack))
                    {
                        float maxWidth = hpBack.rect.width;
                        hpMeter.sizeDelta = new Vector2(maxWidth * statBlock.ReadInt(Stats.CHP) / statBlock.ReadInt(Stats.MHP), hpMeter.sizeDelta.y);
                    }
                    break;
                case "MP Meter":
                    if (display.TryGetComponent(out RectTransform mpMeter) && display.parent.TryGetComponent(out RectTransform mpBack))
                    {
                        float maxWidth = mpBack.rect.width;
                        mpMeter.sizeDelta = new Vector2(maxWidth * statBlock.ReadInt(Stats.CMP) / statBlock.ReadInt(Stats.MMP), mpMeter.sizeDelta.y);
                    }
                    break;
                case "AP":
                    if (display.TryGetComponent(out Text ap))
                    {
                        ap.text = statBlock.ReadInt(Stats.AP).ToString();
                    }
                    break;
                case "AP Meter":
                    if (display.TryGetComponent(out RectTransform apMeter) && display.parent.TryGetComponent(out RectTransform apBack))
                    {
                        float maxHeight = apBack.rect.height;
                        apMeter.sizeDelta = new Vector2(apMeter.sizeDelta.x, maxHeight * statBlock.ReadFloat(Stats.AP));
                        if (apMeter.TryGetComponent(out Image meterImage) && apBack.TryGetComponent(out Image backImage)) { UpdateColors(meterImage, backImage); }
                    }
                    break;
            }
        }
    }

    public void DisplayHPChange(int change)
    {
        //I guess I would have it spawn an HP change object that bounces until it deletes itself
        GameObject hpChange = Instantiate(Resources.Load<GameObject>("Prefabs/HPChange"), transform);
        hpChange.GetComponent<Text>().text = change.ToString();

        //I need to later put some kind of shader on the object so that it changes the text color if something other than the background is behind it
    }

    public void FadeOut() { StartCoroutine(FadeOutProcedure()); }
    IEnumerator FadeOutProcedure()
    {
        float alphaDecay = 1;
        while (alphaDecay > 0)
        {
            yield return null;
        }
        GetComponent<RectTransform>().sizeDelta = Vector2.zero;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left) { onLeftClick?.Invoke(); }
        else if (eventData.button == PointerEventData.InputButton.Right) { onRightClick?.Invoke(); }
    }

    //Delegate subscriptions
    public void SubscribeLeftClick(Action theDelegate) { onLeftClick += theDelegate; }
    public void SubscribeRightClick(Action theDelegate) { onRightClick += theDelegate; }

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
}
