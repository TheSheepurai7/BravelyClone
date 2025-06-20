using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIElements : MonoBehaviour
{
    //I'm thinking that I can turn the event system on and off if I know where it is
    [SerializeField] EventSystem eventSystem;

    public void NewGame()
    {
        //Initialize new game data
        GameData.NewGame();

        //Activate the main screen
        transform.GetChild(1).gameObject.SetActive(true);

        //Trigger display events
        GameData.instance.OnDisplay(Character.ALEC, GameData.instance.GetCharacter(Character.ALEC));
        GameData.instance.OnDisplay(Character.MARISA, GameData.instance.GetCharacter(Character.MARISA));
        GameData.instance.OnDisplay(Character.JENNA, GameData.instance.GetCharacter(Character.JENNA));
        GameData.instance.OnDisplay(Character.GARETH, GameData.instance.GetCharacter(Character.GARETH));

        //Deactivate the starting screen
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void StartBattle()
    {
        //Make absolute sure the subscribers are clear
        GameData.instance.RefreshSubscribers();

        //Activate the battle screen
        transform.GetChild(2).gameObject.SetActive(true);

        //Deactiate the main screen
        transform.GetChild(1).gameObject.SetActive(false);
    }
}
