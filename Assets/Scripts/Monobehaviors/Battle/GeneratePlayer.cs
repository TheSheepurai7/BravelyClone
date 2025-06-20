using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneratePlayer : MonoBehaviour
{
    //Think I'll turn this into a singleton
    public static GeneratePlayer instance;

    //Misc variables
    GameObject playerGFXTemplate;

    // Start is called before the first frame update
    void Awake()
    {
        //Enforce singleton
        if (instance == null) { instance = this; } else { Destroy(this); }

        //Retrieve the GFX template
        playerGFXTemplate = Resources.Load<GameObject>("Prefabs/Player");
    }

    public void GenerateGFX()
    {
        //You know what? First let's just print out the player profiles with nothing on them.
        //Actually I may not need to do anything at the moment beyond just making the graphics. Later I'll need to handle click events for actions, though.
        for (int i = 0; i < 4; i++)
        {
            //Create the graphic holder
            GameObject gfx = Instantiate(playerGFXTemplate);
            gfx.transform.SetParent(transform);

            //I need to figure out how to assign a focus here
            foreach (StatDisplay display in gfx.GetComponentsInChildren<StatDisplay>())
            {
                display.ChangeFocus((Character)(i + 1));
            }
            foreach(MeterDisplay display in gfx.GetComponentsInChildren<MeterDisplay>())
            {
                display.ChangeFocus((Character)(i + 1));
            }
        }
    }
}
