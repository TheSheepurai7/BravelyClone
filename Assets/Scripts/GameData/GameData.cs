using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Security.Cryptography;
using UnityEngine;

public enum Character { ANY, ALEC, MARISA, JENNA, GARETH }
public enum Stats { NAME, LVL, HP, MP, STR, VIT, INT, MND, AGI, JOB, JLV }

public partial class GameData
{
    //The instance storage
    //If I run into threading issues, I need to create a lock object
    //Maybe I SHOULD open up the instance and have just NewGame and LoadGame be the only static functions solely for instantiation
    public static GameData instance = null;

    //The actual members. Should contain character data for the party, map data for the world and how it has changed, and communal party data
    CharacterData[] party = new CharacterData[4];

    //This ensures the constructor is private
    GameData() { }

    //So static functions CAN be accessed without an instance. And, as such, should be used to assign an instance
    public static void NewGame()
    {
        //Instantiate new game data
        instance = new GameData();

        //Create fresh characters
        instance.party[0] = new CharacterData(1, new float[] { 1.3f, 0.85f, 1.15f, 1.3f, 0.85f, 1f, 0.85f }); //Alec
        instance.party[1] = new CharacterData(1, new float[] { 0.85f, 1.3f, 1f, 0.85f, 1f, 1.3f, 1f }); //Marisa
        instance.party[2] = new CharacterData(1, new float[] { 0.85f, 1.3f, 0.85f, 0.85f, 1.3f, 1f, 1.15f }); //Jenna
        instance.party[3] = new CharacterData(1, new float[] { 1.15f, 0.85f, 1.3f, 1f, 0.85f, 0.85f, 1.15f }); //Gareth
    }

    public static void LoadGame()
    {

    }
}
