using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UIElements : MonoBehaviour
{
    public void LoadGame()
    {
        //First check if the directory even exists
        if (Directory.Exists(Application.persistentDataPath + "/saves"))
        {
            //If the directory technically exists then search it for saves
            DirectoryInfo info = new DirectoryInfo(Application.persistentDataPath + "/saves");
            if (info.GetFiles().Length > 0)
            {
                print("There are files to choose from. Implement a way to load them");
                return;
            }
        }

        //Failing all that, alert the player that there are no saves to load
        print("There are no saves to load");
    }

    public void NewGame()
    {
        //Activate the main screen
        transform.GetChild(1).gameObject.SetActive(true);

        //Initialize a fresh party
        PartyManager.InitializeParty();

        //Fire events
        PartyManager.NameUpdate(Character.ALEC);
        PartyManager.StatUpdate(Character.ALEC, Stats.LVL);
        PartyManager.NameUpdate(Character.MARISA);
        PartyManager.StatUpdate(Character.MARISA, Stats.LVL);

        //Deactivate the starting screen
        transform.GetChild(0).gameObject.SetActive(false);
    }
}
