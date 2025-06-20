using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public enum Character { ANY, ALEC, MARISA, JENNA, GARETH }
public enum Stats { NAME, SPRITE, LVL, HP, MP, STR, PATK, VIT, PDEF, INT, MATK, MND, MDEF, AGI, JOB, JLV, AP }
public enum EquipType { SWORD, STAFF, KNIFE, AXE, SHIELD, HEAD, BODY, ACCESSORY }
public delegate void DisplayEvent(Character character, IGetStat statBlock);

public partial class GameData
{
    //The instance storage
    //If I run into threading issues, I need to create a lock object
    public static GameData instance = null;

    //The actual members. Should contain character data for the party, map data for the world and how it has changed, and communal party data
    CharacterData[] party = new CharacterData[4];

    //This ensures the constructor is private
    GameData() { }

    public event DisplayEvent onDisplay;
    public void OnDisplay(Character character, IGetStat statBlock) { onDisplay?.Invoke(character, statBlock); }
    public void RefreshSubscribers() { onDisplay = null; }
    public bool HasSubscribers() { return onDisplay != null; }

    //So static functions CAN be accessed without an instance. And, as such, should be used to assign an instance
    public static void NewGame()
    {
        //Instantiate new game data
        instance = new GameData();

        //Create base stats for characters
        int[] alecBase = new int[] { 10, 5, 8, 8, 6, 7, 6 }; //HP, MP, STR, VIT, INT, MND, AGI
        int[] marisaBase = new int[] { 8, 9, 6, 5, 7, 9, 6 };
        int[] jennaBase = new int[] { 8, 9, 5, 5, 9, 7, 7 };
        int[] garethBase = new int[] { 9, 5, 10, 7, 6, 6, 7 };

        //Create max stats for characters
        float[] alecMax = new float[] { 71, 48, 65, 65, 46, 57, 48 };
        float[] marisaMax = new float[] { 50, 66, 54, 48, 57, 71, 54 };
        float[] jennaMax = new float[] { 45, 71, 48, 45, 71, 60, 60 };
        float[] garethMax = new float[] { 60, 55, 71, 54, 48, 48, 64 };

        //Create fresh equipment to give to the characters
        Equipment[] alecEquip = new Equipment[] { Resources.Load<Equipment>("Starter Items/Machete"), Resources.Load<Equipment>("Starter Items/Plank"), null,
            Resources.Load<Equipment>("Starter Items/Tunic"), null, null };
        Equipment[] marisaEquip = new Equipment[] { Resources.Load<Equipment>("Starter Items/Stick"), null, null, Resources.Load<Equipment>("Starter Items/Tunic"), null, null };
        Equipment[] jennaEquip = new Equipment[] { Resources.Load<Equipment>("Starter Items/Knife"), null, null, Resources.Load<Equipment>("Starter Items/Tunic"), null, null };
        Equipment[] garethEquip = new Equipment[] {Resources.Load<Equipment>("Starter Items/Hatchet"), null, null, Resources.Load<Equipment>("Starter Items/Tunic"), null, null };

        //Create fresh characters
        instance.party[0] = new CharacterData(Resources.Load<Sprite>("Sprites/Actor1"), "Alec", 1, alecBase, alecMax, alecEquip);
        instance.party[1] = new CharacterData(Resources.Load<Sprite>("Sprites/Actor2"), "Marisa", 1, marisaBase, marisaMax, marisaEquip);
        instance.party[2] = new CharacterData(Resources.Load<Sprite>("Sprites/Actor3"), "Jenna", 1, jennaBase, jennaMax, jennaEquip);
        instance.party[3] = new CharacterData(Resources.Load<Sprite>("Sprites/Actor4"), "Gareth", 1, garethBase, garethMax, garethEquip); 
    }

    public static void LoadGame()
    {

    }

    //Character accessor
    public IGetStat GetCharacter(Character character)
    {
        //Get the id
        int id = -1;
        switch (character)
        {
            case Character.ALEC: id = 0; break;
            case Character.MARISA: id = 1; break;
            case Character.JENNA: id = 2; break;
            case Character.GARETH: id = 3; break;
            default: throw new NotImplementedException();
        }

        return party[id];
    }
}
