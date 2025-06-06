using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenerateEnemy : MonoBehaviour
{
    //Think I'll turn this into a singleton
    public static GenerateEnemy instance;

    //List of enemy encounters possible
    [SerializeField] Encounter[] encounterTable;
    Encounter liveEncounter = null;

    //Misc variables
    GameObject enemyGFXTemplate;
    public ReadOnlyEnemyStruct[] encounterData { get; private set; }

    // Start is called before the first frame update
    void OnEnable()
    {
        //Enforce singleton
        if (instance == null) { instance = this; } else { Destroy(this); }

        //Store the GFX template if it's not already stored
        if (enemyGFXTemplate == null) { enemyGFXTemplate = Resources.Load<GameObject>("Enemy"); }

        //Select a random encounter and make sure it's in the player party's avg level range
        int avgLvl = (GameData.instance.GetStat(Character.ALEC, Stats.LVL) + GameData.instance.GetStat(Character.MARISA, Stats.LVL) +
            GameData.instance.GetStat(Character.JENNA, Stats.LVL) + GameData.instance.GetStat(Character.GARETH, Stats.LVL)) / 4;

        do
        {
            int random = Random.Range(0, encounterTable.Length - 1);
            if (encounterTable[random].minLevel <= avgLvl && encounterTable[random].maxLevel >= avgLvl)
            {
                liveEncounter = encounterTable[random];
            }
        } while (liveEncounter == null);

        //Store encounter data
        encounterData = liveEncounter.GetEncounterData();

        //Create names for the encounters
        string[] names = new string[encounterData.Length];
        for (int i = 0; i < names.Length; i++)
        {
            //Check previous iterations for if there are others with the same name
            if (i > 0)
            {
                int counter = 0;
                for (int i2 = 0; i2 < i; i2++)
                {
                    if (encounterData[i].name == encounterData[i2].name)
                    {
                        counter++;
                        names[i2] = encounterData[i2].name + " " + counter;
                    }
                }
                if (counter > 0) { names[i] = encounterData[i].name + " " + counter; }
            }
        }

        //Create graphics for the encounters
        GameObject gfx;
        for (int i = 0; i < encounterData.Length; i++)
        {
            gfx = Instantiate(enemyGFXTemplate);
            gfx.transform.SetParent(transform);
            gfx.transform.localScale = Vector3.one;
            gfx.GetComponentInChildren<Image>().sprite = encounterData[i].sprite;
            gfx.GetComponentInChildren<Text>().text = names[i];
        }

        //Give the Battle Manager its encounter data
        BattleManager.instance.GenerateEnemy(encounterData);
    }
}
