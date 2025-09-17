using UnityEngine;
using UnityEngine.UI;

public abstract class CombatantScript : MonoBehaviour
{
    //Color constants
    Color AP_ZERO = new Color32(120, 0, 0, 255);
    Color AP_ONE = new Color32(0, 255, 0, 255);
    Color AP_TWO = new Color32(255, 255, 0, 255);
    Color AP_THREE = new Color32(255, 150, 0, 255);
    Color AP_MAX = new Color32(255, 0, 255, 255);

    //Stat block
    protected StatBlock _statBlock; //I may actually need to make this a class and not a struct to accomodate stat change display events
    public StatBlock statBlock { get { return _statBlock; } set { if (_statBlock == null) { _statBlock = value; DisplayStats(); } } }

    //Misc variables
    GameObject damageDisplay;

    protected virtual void Start()
    {
        //Spawn a damage display if HP changes
        if (damageDisplay == null) { damageDisplay = Resources.Load<GameObject>("Prefabs/HPChange"); }
        statBlock.onHPChanged += DisplayDamage;
    }

    protected void IncrementATB(float deltaTime)
    {
        if (_statBlock.currentHP > 0) { _statBlock.aP = Mathf.Clamp(_statBlock.aP + deltaTime * 1000, 0, _statBlock.apMax * 4); }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected void DisplayStats()
    {
        foreach (Transform display in GetComponentsInChildren<Transform>())
        {
            switch (display.tag)
            {
                case "AP":
                    if (display.TryGetComponent(out Text ap))
                    {
                        ap.text = ((int)(statBlock.aP / statBlock.apMax)).ToString();
                    }
                    break;
                case "AP Meter":
                    if (display.TryGetComponent(out RectTransform apMeter) && display.parent.TryGetComponent(out RectTransform apBack))
                    {
                        float maxHeight = apBack.rect.height;
                        apMeter.sizeDelta = new Vector2(apMeter.sizeDelta.x, maxHeight * ((float)(statBlock.aP % statBlock.apMax / statBlock.apMax)));
                        if (apMeter.TryGetComponent(out Image meterImage) && apBack.TryGetComponent(out Image backImage)) { UpdateColors(meterImage, backImage); }
                    }
                    break;
                case "HP":
                    if (display.TryGetComponent(out Text hpText)) { hpText.text = statBlock.currentHP + "/" + statBlock.maxHP; }
                    break;
                case "HP Meter":
                    if (display.TryGetComponent(out RectTransform hpMeter) && display.parent.TryGetComponent(out RectTransform hpBack))
                    {
                        float maxWidth = hpBack.rect.width;
                        hpMeter.sizeDelta = new Vector2(maxWidth * statBlock.currentHP / statBlock.maxHP, hpMeter.sizeDelta.y);
                    }
                    break;
                case "MP":
                    if (display.TryGetComponent(out Text mpText)) { mpText.text = statBlock.currentMP + "/" + statBlock.maxMP; }
                    break;
                case "MP Meter":
                    if (display.TryGetComponent(out RectTransform mpMeter) && display.parent.TryGetComponent(out RectTransform mpBack))
                    {
                        float maxWidth = mpBack.rect.width;
                        mpMeter.sizeDelta = new Vector2(maxWidth * statBlock.currentMP / statBlock.maxMP, mpMeter.sizeDelta.y);
                    }
                    break;
                case "Name":
                    if (display.TryGetComponent(out Text text)) { text.text = statBlock.name; }
                    break;
                case "Sprite":
                    if (display.TryGetComponent(out Image image)) { image.sprite = statBlock.sprite; }
                    break;
            }
        }
    }

    void DisplayDamage(int value)
    {
        GameObject damageInstance = Instantiate(damageDisplay, transform);
        if (damageInstance.TryGetComponent(out Text text))
        {
            text.text = Mathf.Abs(value).ToString();
            text.color = value < 0 ? Color.green : Color.white;
        }
    }

    void UpdateColors(Image apMeter, Image apBack)
    {
        switch ((int)(statBlock.aP / statBlock.apMax))
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
