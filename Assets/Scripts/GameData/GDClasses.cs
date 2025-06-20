using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Attach this interface to anything that needs ostensibly readonly stats drawn from it
public interface IGetStat
{
    public string GetName(Stats stat);
    public int GetStat(Stats stat);
    public Vector2 GetCompoundStat(Stats stat);
    public float GetFloatStat(Stats stat);
    public Sprite GetSprite();
}

public partial class GameData
{
    //I think the stat formula was lvl * 0.5 * personal modifier * job modifier. * 10 for MP and * 100 for HP
    class CharacterData : IGetStat
    {
        //Parameters
        Sprite sprite;
        string name;
        int lvl; //Switch to EXP later and simply derive level from an EXP conversion formula
        int[] baseStats; //HP, MP, STR, VIT, INT, MND, AGI
        float[] statMultipliers; 
        float hpPercent;
        float mpPercent;
        JobInfo actingJob = null;
        Equipment[] equipment;

        //Constructor
        public CharacterData(Sprite sprite, string name, int lvl, int[] baseStats, float[] maxStats, Equipment[] equip)
        {
            //Just intialize the basic stuff first
            this.sprite = sprite;
            this.name = name;
            this.lvl = lvl;
            this.baseStats = baseStats;
            hpPercent = 1;
            mpPercent = 1;
            equipment = equip;

            //Calculate the stat multipliers
            statMultipliers = new float[baseStats.Length];
            for(int i = 0; i < baseStats.Length; i++)
            {
                statMultipliers[i] = (maxStats[i] - baseStats[i]) / 98;
            }
        }

        //The interface for stat extraction (Get rid of the accessors above)
        public string GetName(Stats stat)
        {
            switch (stat)
            {
                case Stats.NAME: return name;
                case Stats.JOB: if (actingJob == null) { return "None"; } else { return actingJob.name; }
                default: throw new System.NullReferenceException(stat + " either doesn't exist in this struct or can't be pulled from GetName()");
            }
        }

        public Vector2 GetCompoundStat(Stats stat)
        {
            switch (stat)
            {
                //Should I instead calculate HP as stat * (lvl - 1) and THEN add the class modifier?
                case Stats.HP: float hpStat = (baseStats[0] + (lvl - 1) * statMultipliers[0]); return new Vector2(hpPercent, hpStat * (hpStat + 1));
                case Stats.MP: float mpStat = (baseStats[1] + (lvl - 1) * statMultipliers[1]);  return new Vector2(mpPercent, mpStat * (mpStat + 1) / 10);
                default: throw new System.NullReferenceException("That stat does not exist");
            }
        }

        public int GetStat(Stats stat)
        {
            switch (stat)
            {
                case Stats.LVL: return lvl;
                case Stats.JLV: if (actingJob == null) { return 0; } else { return actingJob.lvl; }
                case Stats.PATK: return (int)(baseStats[2] + (lvl - 1) * statMultipliers[2] + CalculateEquipStats(Stats.PATK));
                case Stats.PDEF: return (int)(baseStats[3] + (lvl - 1) * statMultipliers[3] + CalculateEquipStats(Stats.PDEF));
                case Stats.MATK: return (int)(baseStats[4] + (lvl - 1) * statMultipliers[4] + CalculateEquipStats(Stats.MATK) + CalculateEquipStats(Stats.INT));
                case Stats.MDEF: return (int)(baseStats[5] + (lvl - 1) * statMultipliers[5] + CalculateEquipStats(Stats.MDEF) + CalculateEquipStats(Stats.MND));
                case Stats.INT: return (int)(baseStats[4] + (lvl - 1) * statMultipliers[4] + CalculateEquipStats(Stats.INT));
                case Stats.MND: return (int)(baseStats[5] + (lvl - 1) * statMultipliers[5] + CalculateEquipStats(Stats.MND));
                case Stats.AGI: return (int)(baseStats[6] + (lvl - 1) * statMultipliers[6]);
                default: throw new System.NullReferenceException(stat + " either doesn't exist in this struct or can't be pulled from GetStat()");
            }
        }

        public Sprite GetSprite()
        {
            return sprite;
        }

        int CalculateEquipStats(Stats stat)
        {
            int returnValue = 0;
            foreach(Equipment equip in equipment)
            {
                if (equip != null) { returnValue += equip.GetStat(stat); }
            }
            return returnValue;
        }

        public float GetFloatStat(Stats stat)
        {
            throw new System.NotImplementedException();
        }
    }
}
