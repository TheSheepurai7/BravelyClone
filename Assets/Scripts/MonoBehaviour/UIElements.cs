using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIElements : MonoBehaviour
{
    //I'm thinking that I can turn the event system on and off if I know where it is
    [SerializeField] EventSystem eventSystem;

    void Start()
    {
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
    }

    public void NewGame()
    {
        //Instantiate new game
        GameData.NewGame();

        //Activate main screen
        transform.GetChild(1).gameObject.SetActive(true);

        //Run through each character
        for (int i = 0; i < transform.GetChild(1).GetComponentsInChildren<CharacterDisplay>().Length; i++)
        {
            //Assign stat block
            transform.GetChild(1).GetComponentsInChildren<CharacterDisplay>()[i].AssignStatBlock(GameData.instance.GetCharacter((Character)i));

            //Assign delegate
            int temp = i;
            transform.GetChild(1).GetComponentsInChildren<CharacterDisplay>()[i].SubscribeLeftClick(() => ThatPersonsNameIs(GameData.instance.GetCharacter((Character)temp)));
        }

        //Turn off the load screen
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void GenerateBattle()
    {
        //Activate battle screen
        transform.GetChild(2).gameObject.SetActive(true);

        //Turn off the main screen
        transform.GetChild(1).gameObject.SetActive(false);
    }

    void ThatPersonsNameIs(IStatReader statBlock)
    {
        print("That person's name is " + statBlock.ReadString(Stats.NAME));
    }
}
