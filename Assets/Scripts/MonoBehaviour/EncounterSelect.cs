using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Encounter information should include a suggested salary, level, JP, and job set. They can be edited here, though. 

public class EncounterSelect : MonoBehaviour
{
    //Component references
    [SerializeField] Transform encounterHolder;
    [SerializeField] Transform jobHolder;

    //Templates
    GameObject encounterDisplay;
    GameObject jobDisplay;

    //Misc fields
    [SerializeField] List<Encounter> encounters;
    List<Job> jobs;
    Encounter selectEncounter;
    Image currentSelect;

    void OnEnable()
    {
        //Cache templates
        if (encounterDisplay == null) encounterDisplay = Resources.Load<GameObject>("Prefabs/EncounterDisplay");
        if (jobDisplay == null) jobDisplay = Resources.Load<GameObject>("Prefabs/JobButton");

        //Run down the list of encounters and create a functioning button for each of them
        foreach (Encounter encounter in encounters)
        {
            //Create a new button
            GameObject button = Instantiate(encounterDisplay, encounterHolder != null ? encounterHolder : transform);
            button.GetComponentsInChildren<Image>()[1].sprite = encounter.displayImage;
            button.GetComponentInChildren<Text>().text = encounter.displayName;
            Encounter temp = encounter;

            //Button handler
            button.GetComponentInChildren<Button>().onClick.AddListener(() => 
            { 
                selectEncounter = temp;
                if (currentSelect != null) { currentSelect.color *= new Vector4(1, 1, 1, 0); }
                button.GetComponent<Image>().color += (Color)new Vector4(0, 0, 0, 1);
                currentSelect = button.GetComponent<Image>();
            });
        }

        //Run down the list of jobs and create a functioning button for each of them (In theory)
        //The first time around it creates buttons just fine. The second time it seems to be having some difficulty
        if (jobHolder == null) { jobHolder = new GameObject().transform; }
        jobs = new List<Job>();
        for (int i = 1; i < Enum.GetNames(typeof(Job)).Length - 1; i++) 
        {
            //Create job display
            GameObject jobButton = Instantiate(jobDisplay, jobHolder);

            switch ((Job)i)
            {
                case Job.KNIGHT:
                    jobButton.GetComponentInChildren<Text>().text = "Knight";
                    jobButton.GetComponentInChildren<Toggle>().onValueChanged.AddListener((bool value) => { if (value) { jobs.Add(Job.KNIGHT); } else { jobs.Remove(Job.KNIGHT); } });
                    break;
                case Job.CLERIC:
                    jobButton.GetComponentInChildren<Text>().text = "Cleric";
                    jobButton.GetComponentInChildren<Toggle>().onValueChanged.AddListener((bool value) => { if (value) { jobs.Add(Job.CLERIC); } else { jobs.Remove(Job.CLERIC); } });
                    break;
                case Job.WIZARD:
                    jobButton.GetComponentInChildren<Text>().text = "Wizard";
                    jobButton.GetComponentInChildren<Toggle>().onValueChanged.AddListener((bool value) => { if (value) { jobs.Add(Job.WIZARD); } else { jobs.Remove(Job.WIZARD); } });
                    break;
                case Job.DRAGOON:
                    jobButton.GetComponentInChildren<Text>().text = "Dragoon";
                    jobButton.GetComponentInChildren<Toggle>().onValueChanged.AddListener((bool value) => { if (value) { jobs.Add(Job.DRAGOON); } else { jobs.Remove(Job.DRAGOON); } });
                    break;
                default: throw new Exception(i + " cannot be converted into a job (or the functionality for that job doesn't exist).");
            }
        }
    }

    public void GoToPartyDisplay()
    {
        if (selectEncounter != null)
        {
            //First I need to give the battle manager the encounter data to create enemy structures
            BattleManager.instance.CreateEnemies(selectEncounter);

            //Activate the party manager
            PartyManager.instance.SetParameters(1, 0, 0);

            //Fiddle with party manager graphics
            GameObject partyManager = transform.parent.GetChild(1).gameObject;
            partyManager.SetActive(true);
            int counter = 0;
            foreach (CharacterDisplay display in partyManager.GetComponentsInChildren<CharacterDisplay>())
            {
                display.statBlock = PartyManager.instance.GetStatBlock(counter++);
            }

            //Completely clear the screen so repeated visits don't duplicate everything
            foreach (Transform child in encounterHolder.transform) { Destroy(child.gameObject); }
            foreach (Transform child in jobHolder.transform) { Destroy(child.gameObject); }

            //Hide this screen
            gameObject.SetActive(false);
        }
    }
}
