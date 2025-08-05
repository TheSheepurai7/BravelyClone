using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PartyManager : MonoBehaviour
{
    //The instance storage
    //If I run into threading issues, I need to create a lock object
    public static PartyManager instance = null;

    //The Party
    CharacterData[] party = new CharacterData[4];

    void Awake()
    {
        //Enforce singleton
        if (instance == null) { instance = this; } else { Destroy(this); }
    }

    public void SetParameters(int level, int salary, int jpAlloc)
    {
        //Create the player party
        for (int i = 0; i < 4; i++)
        {
            party[i] = new CharacterData((Character)i, level);
        }
    }

    public IStatReader GetStatBlock(int index)
    {
        return party[index];
    }

    public void GoToBattle()
    {
        //Open the battle screen
        GameObject battleManager = GameObject.Find("Canvas").transform.GetChild(2).gameObject;
        battleManager.SetActive(true);

        //Tell the battle manager to create player structs
        BattleManager.instance.CreatePlayers(party.ToList<IStatReader>());

        //Close this screen
        GameObject.Find("Canvas").transform.GetChild(1).gameObject.SetActive(false);
    }
}
