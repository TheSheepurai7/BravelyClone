using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Maybe I should just look for what ever is in the textbox already and use THAT as a prefix
[RequireComponent(typeof(Text))]
public class StatDisplay : MonoBehaviour
{
    //Serializable members
    [SerializeField] string prefix;
    [SerializeField] Character character;
    [SerializeField] Stats stat;

    //Private members
    Text text;

    private void OnEnable()
    {
        //Cache text
        text = GetComponent<Text>();

        //Display the stat
        if (stat == Stats.NAME) { DisplayName(character); }
        else if (stat == Stats.JOB) { DisplayJob(character); }
        else if (stat == Stats.JLV) { DisplayJlv(character); }
        else if (stat == Stats.HP || stat == Stats.MP) { DisplayHPMP(character, stat); }
        else { DisplayStat(character, stat); }
    }

    void DisplayName(Character character)
    {
        switch (character)
        {
            case Character.ALEC: text.text = "Alec"; break;
            case Character.MARISA: text.text = "Marisa"; break;
            case Character.JENNA: text.text = "Jenna"; break;
            case Character.GARETH: text.text = "Gareth"; break;
        }
    }

    void DisplayJob(Character character)
    {
        text.text = GameData.instance.GetJob(character);
    }

    //This works a little differently from normal stats so I should give it its own function
    void DisplayJlv(Character character)
    {
        if (GameData.instance.GetStat(character, Stats.JLV) > 0) { text.text = "Lv. " + GameData.instance.GetStat(character, Stats.JLV).ToString(); }
        else { text.text = ""; }
    }

    //HP and MP have coloring to do, so they need to be separate
    void DisplayHPMP(Character character, Stats stat)
    {
        switch (stat)
        {
            case Stats.HP:
                Vector2 hp = GameData.instance.GetPercentageStat(character, Stats.HP);
                text.text = ((int)(hp.y * hp.x)).ToString() + "/" + ((int)hp.y).ToString();
                if (hp.x > 0) { text.color = new Color(1, hp.x, hp.x); } else { text.color = Color.black; }
                break;
            case Stats.MP:
                Vector2 mp = GameData.instance.GetPercentageStat(character, Stats.MP);
                text.text = ((int)(mp.y * mp.x)).ToString() + "/" + ((int)mp.y).ToString();
                if (mp.x > 0) { text.color = new Color(1, mp.x, 1); } else { text.color = Color.black; }
                break;
        }
    }

    void DisplayStat(Character character, Stats stat)
    {
        switch (stat)
        {
            case Stats.LVL: text.text = prefix + GameData.instance.GetStat(character, stat).ToString(); break;
        }
    }
}
