using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameData
{
    //I think I can make each class private. Nothing needs to interact with these objects except through accessors
    class CharacterData
    {
        //Parameters
        int lvl; //Switch to EXP later and simply derive level from an EXP conversion formula
        float[] statMultipliers; //7 of them
        JobInfo actingJob = null;

        //Constructor
        public CharacterData(int lvl, float[] statMultipliers)
        {
            this.lvl = lvl;
            this.statMultipliers = statMultipliers;
        }

        //Technically accessors
        public int GetLvl() { return lvl; }
        public string GetJob() { if (actingJob != null) { return actingJob.name; } return "None"; }
        public int GetJlv() { if (actingJob != null) { return actingJob.lvl; } return 0; }

    }
}
