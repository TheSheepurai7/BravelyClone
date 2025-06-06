using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is for the game data's accessors
public partial class GameData
{
    public string GetJob(Character character)
    {
        //Figure out the identity of the character
        int id = -1;
        switch (character)
        {
            case Character.ALEC: id = 0; break;
            case Character.MARISA: id = 1; break;
            case Character.JENNA: id = 2; break;
            case Character.GARETH: id = 3; break;
            default: return "";
        }

        return party[id].GetJob();
    }

    public int GetStat(Character character, Stats stat)
    {
        //Figure out the identity of the character
        int id = -1;
        switch (character)
        {
            case Character.ALEC: id = 0; break;
            case Character.MARISA: id = 1; break;
            case Character.JENNA: id = 2; break;
            case Character.GARETH: id = 3; break;
            default: return 0;
        }

        //Find the proper stat to return
        switch (stat)
        {
            case Stats.LVL: return party[id].GetLvl();
            case Stats.JLV: return party[id].GetJlv();
        }

        return 0;
    }

    public Vector2 GetPercentageStat(Character character, Stats stat)
    {
        //Figure out the identity of the character
        int id = -1;
        switch (character)
        {
            case Character.ALEC: id = 0; break;
            case Character.MARISA: id = 1; break;
            case Character.JENNA: id = 2; break;
            case Character.GARETH: id = 3; break;
            default: return Vector2.zero;
        }

        //Find the proper stat to return
        switch (stat)
        {
            case Stats.HP: return party[id].GetHP();
            case Stats.MP: return party[id].GetMP();
        }

        return Vector2.zero;
    }
}
