using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameData
{
    //The instance storage
    //If I run into threading issues, I need to create a lock object
    public static GameData instance = null;

    //The actual members. Should contain character data for the party, map data for the world and how it has changed, and communal party data
    CharacterData[] party = new CharacterData[4];

    //This ensures the constructor is private
    GameData() { }

    public static void NewGame()
    {
        //Instantiate new game data
        instance = new GameData();

        //Create base stats for the characters
        int[] alecBase = new int[] { 155, 17, 11, 11, 7, 10, 7 };
        int[] marisaBase = new int[] { 120, 25, 9, 7, 10, 13, 10 };
        int[] jennaBase = new int[] { 102, 30, 7, 7, 13, 10, 11 };
        int[] garethBase = new int[] { 135, 20, 13, 10, 7, 8, 11 };

        //Create fresh equipment to give to the characters
        Equipment[] alecEquip = new Equipment[] { Resources.Load<Equipment>("Starter Items/Machete"), Resources.Load<Equipment>("Starter Items/Plank"), null,
            Resources.Load<Equipment>("Starter Items/Tunic"), null, null };
        Equipment[] marisaEquip = new Equipment[] { Resources.Load<Equipment>("Starter Items/Stick"), null, null, Resources.Load<Equipment>("Starter Items/Tunic"), null, null };
        Equipment[] jennaEquip = new Equipment[] { Resources.Load<Equipment>("Starter Items/Knife"), null, null, Resources.Load<Equipment>("Starter Items/Tunic"), null, null };
        Equipment[] garethEquip = new Equipment[] { Resources.Load<Equipment>("Starter Items/Hatchet"), null, null, Resources.Load<Equipment>("Starter Items/Tunic"), null, null };

        //Set up new characters
        instance.party[0] = new CharacterData(Resources.Load<Sprite>("Sprites/Actor1"), "Alec", alecBase, alecEquip);
        instance.party[1] = new CharacterData(Resources.Load<Sprite>("Sprites/Actor2"), "Marisa", marisaBase, marisaEquip);
        instance.party[2] = new CharacterData(Resources.Load<Sprite>("Sprites/Actor3"), "Jenna", jennaBase, jennaEquip);
        instance.party[3] = new CharacterData(Resources.Load<Sprite>("Sprites/Actor4"), "Gareth", garethBase, garethEquip);
    }

    public static void LoadGame()
    {

    }

    public IStatReader GetCharacter(Character character)
    {
        return party[(int)character];
    }
}
