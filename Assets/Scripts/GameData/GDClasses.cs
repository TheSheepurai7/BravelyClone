using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameData
{
    //I think I can make each class private. Nothing needs to interact with these objects except through accessors
    //I think the stat formula was lvl * 0.5 * personal modifier * job modifier. * 10 for MP and * 100 for HP
    class CharacterData
    {
        //Parameters
        int lvl; //Switch to EXP later and simply derive level from an EXP conversion formula
        float[] statMultipliers; //7 of them
        float hpPercent;
        float mpPercent;
        JobInfo actingJob = null;

        //Constructor
        public CharacterData(int lvl, float[] statMultipliers)
        {
            this.lvl = lvl;
            this.statMultipliers = statMultipliers;
            hpPercent = 1;
            mpPercent = 1;
        }

        //Technically accessors
        public int GetLvl() { return lvl; }
        public string GetJob() { if (actingJob != null) { return actingJob.name; } return "None"; }
        public int GetJlv() { if (actingJob != null) { return actingJob.lvl; } return 0; }
        public Vector2 GetHP() { return new Vector2(hpPercent, lvl * statMultipliers[0] * 50); }
        public Vector2 GetMP() { return new Vector2(mpPercent, lvl * statMultipliers[1] * 5); }
    }
}
