using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MeterDisplay : MonoBehaviour
{
    //Enum
    enum Dimension { X, Y }

    //Constants
    Color NULL = new Color(0.5f, 0, 0);
    Color MAX = new Color(1, 0.5f, 0);

    //Serialized variables
    [SerializeField] Character character;
    [SerializeField] Stats stat;
    [SerializeField] Dimension dimension;

    //Cache
    RectTransform rectTransform;
    Image image;
    Image parentImage;

    //Misc variables
    Character focus = Character.ANY;
    float max;

    // Start is called before the first frame update
    void Awake()
    {
        //Make sure only compound stats are represented
        if (stat != Stats.HP && stat != Stats.MP && stat != Stats.AP) { print("Invalid stat"); Destroy(this); }

        //Calculate width or height I guess
        if (transform.parent != null && transform.parent.GetComponent<Image>() != null)
        {
            rectTransform = GetComponent<RectTransform>();
            image = GetComponent<Image>();
            parentImage = transform.parent.GetComponent<Image>();
            if (dimension == Dimension.X) { max = transform.parent.GetComponent<RectTransform>().rect.width; }
            if (dimension == Dimension.Y) { max = transform.parent.GetComponent<RectTransform>().rect.height; }
        }
        else { print("Destroy this"); Destroy(this); }

        //Assign focus
        if (character != Character.ANY) { focus = character; }

        //Subscribe to the onDisplay event
        GameData.instance.onDisplay += Display;
    }

    public void ChangeColors(int level)
    {
        switch (level)
        {
            case 0: parentImage.color = NULL; image.color = Color.green; break;
            case 1: parentImage.color = Color.green; image.color = Color.yellow; break;
            default: parentImage.color = Color.yellow; image.color = MAX; break;
        }
    }

    public void ChangeFocus(Character newFocus) 
    { 
        focus = newFocus;
    }

    void Display(Character character, IGetStat statBlock)
    {
        //Do I need to fix all the GetComponent calls?
        if (character == focus)
        {
            if (stat == Stats.HP || stat == Stats.MP)
            {
                if (dimension == Dimension.X)
                { rectTransform.sizeDelta = new Vector2(max * statBlock.GetCompoundStat(stat).x, rectTransform.sizeDelta.y); }
                if (dimension == Dimension.Y)
                { rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, max * statBlock.GetCompoundStat(stat).x); }
            }
            else if (stat == Stats.AP)
            {
                //I need to be able to extract a float, not an int
                //So I need 2 different cases based on whether the aP count is at or below apMax. If below then do as normal. If at then hold at max
                if (dimension == Dimension.X)
                { rectTransform.sizeDelta = new Vector2(max * statBlock.GetFloatStat(stat), rectTransform.sizeDelta.y); }
                if (dimension == Dimension.Y)
                { rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, max * statBlock.GetFloatStat(stat)); }

                //I can use this chance to change its color based on AP acquired
                ChangeColors(statBlock.GetStat(stat));
            }
        }
    }


}
