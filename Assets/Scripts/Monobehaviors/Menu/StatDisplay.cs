using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour
{
    //Serializable members
    [SerializeField] Character character;
    [SerializeField] Stats stat;

    //Private members
    Image image;
    Text text;
    Character focus = Character.ANY;

    void Awake()
    {
        //I should rewrite the cache function so that it deletes itself if there is neither an Image component or a Text component
        if (GetComponent<Image>() == null && GetComponent<Text>() == null)
        {
            Destroy(this);
        }
        else
        {
            if (TryGetComponent(out Image inImage)) { image = inImage; }
            if (TryGetComponent(out Text inText)) { text = inText; }
        }

        //Set a focus if there's a hard set one
        if (character != Character.ANY) { focus = character; }

        //Subscribe to a display event
        GameData.instance.onDisplay += Display;
    }

    public void ChangeFocus(Character newFocus)
    {
        if (character == Character.ANY) { focus = newFocus; }
    }

    void Display(Character inCharacter, IGetStat statHolder)
    {
        //I might do something with "focuses" later
        if (focus == inCharacter)
        {
            //Sprite display
            if (image != null)
            {
                if (stat == Stats.SPRITE) { image.sprite = statHolder.GetSprite(); }
            }

            //Text displays
            if (text != null)
            {
                switch (stat)
                {
                    case Stats.NAME: text.text = statHolder.GetName(Stats.NAME); break;
                    case Stats.LVL: text.text = "Lv. " + statHolder.GetStat(Stats.LVL); break;
                    case Stats.JOB: text.text = statHolder.GetName(Stats.JOB); break;
                    case Stats.JLV: text.text = statHolder.GetStat(Stats.JLV) > 0 ? statHolder.GetStat(Stats.JLV).ToString() : null; break;
                    case Stats.HP:
                        text.text = ((int)(statHolder.GetCompoundStat(Stats.HP).x * statHolder.GetCompoundStat(Stats.HP).y)).ToString() + "/" +
                            (int)statHolder.GetCompoundStat(Stats.HP).y; break;
                    case Stats.MP:
                        text.text = ((int)(statHolder.GetCompoundStat(Stats.MP).x * statHolder.GetCompoundStat(Stats.MP).y)).ToString() + "/" +
                            (int)statHolder.GetCompoundStat(Stats.MP).y; break;
                    case Stats.AP: text.text = statHolder.GetStat(Stats.AP).ToString(); break;
                }
            }
        }
    }
}
