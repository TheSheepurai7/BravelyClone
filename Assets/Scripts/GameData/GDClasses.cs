using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public partial class GameData
{
    class CharacterData : IStatReader
    {
        //Parameters
        Sprite sprite;
        string name;
        int lvl = 1; //Switch to EXP later and simply derive level from an EXP conversion formula
        int[] baseStats; //HP, MP, STR, VIT, INT, MND, AGI
        float[] statMultipliers; // For use later
        int currentHP;
        int currentMP;
        Dictionary<Job, int> jobPool;
        Job actingJob;
        Job actingSubJob;
        Equipment[] equipment;

        public CharacterData(Sprite sprite, string name, int[] baseStats, Equipment[] equipment)
        {
            this.sprite = sprite;
            this.name = name;
            this.baseStats = baseStats;
            this.equipment = equipment;
            currentHP = baseStats[0];
            currentMP = baseStats[1];
            jobPool = new Dictionary<Job, int>();
            jobPool.Add(Job.NONE, 0);
            actingJob = Job.NONE;
            actingSubJob = Job.NONE;
        }

        public Sprite ReadImage()
        {
            return sprite;
        }

        public string ReadString(Stats stat)
        {
            switch (stat)
            {
                case Stats.NAME: return name;
                case Stats.JOB: return actingJob.ToString();
                default: throw new System.Exception(stat + " cannot be parsed as a name in CharacterData (Or the functionality hasn't been implemented yet).");
            }
        }

        public int ReadInt(Stats stat)
        {
            switch (stat)
            {
                case Stats.LVL: return lvl;
                case Stats.CHP: return currentHP;
                case Stats.MHP: return baseStats[0]; //Implement scaling later
                case Stats.CMP: return currentMP;
                case Stats.MMP: return baseStats[1]; //Implement scaling later
                case Stats.PATK: return baseStats[2] + EquipStat(Stats.PATK); //Implement scaling later
                case Stats.MATK: return EquipStat(Stats.MATK);
                case Stats.INT: return baseStats[4]; //Implement scaling later
                case Stats.PDEF: return baseStats[3] + EquipStat(Stats.PDEF); //Implement scaling later
                case Stats.MDEF: return EquipStat(Stats.MDEF);
                case Stats.MND: return baseStats[5]; //Implement scaling later
                case Stats.AGI: return baseStats[6]; //Implement scaling later
                default: throw new System.Exception(stat + " cannot be parsed as an int in CharacterData (Or the functionality hasn't been implemented yet).");
            }
        }

        public float ReadFloat(Stats stat)
        {
            throw new System.Exception(stat + " cannot be parsed as a float in CharacterData (Or the functionality hasn't been implemented yet).");
        }

        public List<CommandInfo> ReadCommands(Stats stat)
        {
            switch (stat)
            {
                case Stats.JOB: return GetJobCommands(actingJob);
                case Stats.SJB: return GetJobCommands(actingSubJob);
                default: throw new System.Exception(stat + " cannot be parsed as a command list in CharacterData (Or the functionality hasn't been implemented yet).");
            }
        }

        int EquipStat(Stats stat)
        {
            //Eventually I'm going to have to calculate hand and affinity modifiers for weapons
            int returnValue = 0;
            foreach (Equipment equip in equipment) { if (equip != null) { returnValue += equip.GetStat(stat); } }
            return returnValue;
        }

        List<CommandInfo> GetJobCommands(Job job)
        {
            switch(job)
            {
                case Job.NONE: return null;
                default: throw new System.Exception(job + " does not have a corrosponding static class.");
            }
        }

        public void UpdateDisplay(ref UpdateStats theDelegate)
        {
            
        }
    }
}
