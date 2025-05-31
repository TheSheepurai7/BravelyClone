using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public enum Character { ANY, ALEC, MARISA, JENNA, GARETH }
public enum Stats { NAME, LVL, HP, MP, STR, VIT, INT, MND, AGI }
public delegate void StatUpdateEvent(Character character, Stats stat);
public delegate void NameUpdateEvent(Character character);

//I should try making an event with delegates for simplicity. I simply have "public static event [delegate] [name declaration] to make it work

public static class PartyManager
{
    class CharacterSheet //I need to find a way to make it so that only 
    {
        //Parameters
        int lvl; //Switch to EXP later and simply derive level from an EXP conversion formula
        float[] statMultipliers; //7 of them

        //Constructor
        public CharacterSheet(int lvl, float[] statMultipliers)
        {
            this.lvl = lvl;
            this.statMultipliers = statMultipliers;
        }

        //Technically accessors
        public int GetLvl() { return lvl; }
    }

    //The party and their communal stats
    static CharacterSheet[] party; //I think I can just hide the party behind private here and simply have accessors that take in the party member and stat

    //Events
    public static event StatUpdateEvent OnStatUpdate;
    public static event NameUpdateEvent OnNameUpdate;

    //Functions
    public static void StatUpdate(Character character, Stats stat) { OnStatUpdate?.Invoke(character, stat); }
    public static void NameUpdate(Character character) { OnNameUpdate?.Invoke(character); }

    public static int GetStat(Character character, Stats stat)
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
        }

        return 0;
    }

    public static string GetName(Character character)
    {
        switch (character)
        {
            case Character.ALEC: return "Alec";
            case Character.MARISA: return "Marisa";
            case Character.JENNA: return "Jenna";
            case Character.GARETH: return "Gareth";
        }

        return string.Empty;
    }

    public static void InitializeParty()
    {
        party = new CharacterSheet[4];
        party[0] = new CharacterSheet(1, new float[] { 1.3f, 0.85f, 1.15f, 1.3f, 0.85f, 1f, 0.85f }); //Alec
        party[1] = new CharacterSheet(1, new float[] { 0.85f, 1.3f, 1f, 0.85f, 1f, 1.3f, 1f }); //Marisa
        party[2] = new CharacterSheet(1, new float[] { 0.85f, 1.3f, 0.85f, 0.85f, 1.3f, 1f, 1.15f }); //Jenna
        party[3] = new CharacterSheet(1, new float[] { 1.15f, 0.85f, 1.3f, 1f, 0.85f, 0.85f, 1.15f }); //Gareth
    }
}
