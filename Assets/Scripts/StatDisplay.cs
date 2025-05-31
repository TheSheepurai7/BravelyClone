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
    Character focus = Character.ANY;
    Text text;

    void Awake()
    {
        //Cache text
        text = GetComponent<Text>();

        //Event assignment
        if (stat == Stats.NAME) { PartyManager.OnNameUpdate += DisplayName; }
        else { PartyManager.OnStatUpdate += DisplayStat; }

        //Assign an initial character focus
        if (character != Character.ANY) { focus = character; }
    }

    private void OnDisable()
    {
        //Unsubscribe from events to keep subscriptions from accumulating
        if (stat == Stats.NAME) { PartyManager.OnNameUpdate -= DisplayName; }
        else { PartyManager.OnStatUpdate -= DisplayStat; }
    }

    void DisplayStat(Character character, Stats stat)
    {
        if (character == focus)
        {
            switch (stat)
            {
                case Stats.LVL: text.text = prefix + PartyManager.GetStat(character, stat).ToString(); break;
            }
        }
    }

    void DisplayName(Character character)
    {
        if (character == focus)
        {
            switch(character)
            {
                case Character.ALEC: text.text = "Alec"; break;
                case Character.MARISA: text.text = "Marisa"; break;
                case Character.JENNA: text.text = "Jenna"; break;
                case Character.GARETH: text.text = "Gareth"; break;
            }
        }
    }
}
