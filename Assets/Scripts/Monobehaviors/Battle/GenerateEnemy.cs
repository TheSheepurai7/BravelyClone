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

    void Awake()
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

        //Since the enemy structs themselves don't carry names, I guess I'll have to generate one from scratch and pass it along with the encounter
        string[] names = new string[liveEncounter.EnemyList().Length];
        for (int i = 0; i < names.Length; i++)
        {
            string baseName = liveEncounter.EnemyList()[i].name;
            if (i > 0)
            {
                int counter = 1;
                for (int i2 = 0; i2 < i; i2++)
                {
                    if (baseName == liveEncounter.EnemyList()[i2].name)
                    {
                        if (counter == 1) { names[i2] = baseName + " 1"; }
                        counter++;
                    }
                }
                if (counter > 1) { names[i] = baseName + " " + counter; }
                else { names[i] = baseName; }
            }
        }

        //Create graphics
        for (int i = 0; i < names.Length; i++)
        {
            GameObject gfx = Instantiate(enemyGFXTemplate);
            gfx.transform.SetParent(transform);
            gfx.GetComponentInChildren<Image>().sprite = liveEncounter.EnemyList()[i].GetSprite();
            gfx.GetComponentInChildren<Text>().text = names[i];
        }

        //Pass the encounter and names off to the battle manager
        BattleManager.instance.GenerateEnemies(liveEncounter, names);
    }

    public void DisplayGuard(int actorID, bool display)
    {

    }

    public void CombatantToggle(int actorId, bool toggle)
    {

    }
}
