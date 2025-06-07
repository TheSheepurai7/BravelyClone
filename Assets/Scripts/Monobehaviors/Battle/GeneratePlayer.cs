using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    }

    public void RequestPlayerParty()
    {
        //Give the player party to the Battle Manager when everything is finished

    }
}
