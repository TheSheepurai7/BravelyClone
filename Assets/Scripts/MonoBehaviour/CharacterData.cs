using System.Collections.Generic;
using UnityEngine;

public partial class PartyManager : MonoBehaviour
{
    class CharacterData : IStatReader
    {
        //Parameters
        Sprite sprite;
        string name;
        int lvl = 1; //Switch to EXP later and simply derive level from an EXP conversion formula
        int[] baseStats; //HP, MP, STR, VIT, INT, MND, AGI
        float[] statGrowths; // For use later
        int currentHP;
        int currentMP;
        Dictionary<Job, int> jobPool; //Job is the key, job level is the value
        Job actingJob;
        Job actingSubJob;
        //Equipment[] equipment;

        public CharacterData(Character archetype, int level) //I'm going to try assigning these values based on character
        {
            //Conventional attack damage (for players at least) is calculated by (STR^1.75 + Weapon^1.5)
            //Starting conventional attack damage stats are:
            /*
                Alec - 10 STR + 5 Weapon (Machete)
                Marisa - 9 STR + 3 Weapon (Knife)
                Jenna - 9 STR + 3 Weapon (Knife)
                Gareth - 11 STR + 7 Weapon (Hatchet)
            //*/

            //Cast unique statlines
            switch (archetype)
            {
                case Character.ALEC:
                    name = "Alec";
                    sprite = Resources.Load<Sprite>("Sprites/Actor1");
                    baseStats = new int[] { 100, 30, 10, 10, 9 }; //HP, MP, STR, VIT, and AGI for now. HP for everyone is going to be 100 and MP is going to be 30 to start
                    break;
                case Character.MARISA:
                    name = "Marisa";
                    sprite = Resources.Load<Sprite>("Sprites/Actor2");
                    baseStats = new int[] { 100, 30, 9, 9, 9 }; //HP, MP, STR, VIT, and AGI for now. HP for everyone is going to be 100 and MP is going to be 30 to start
                    break;
                case Character.JENNA:
                    name = "Jenna";
                    sprite = Resources.Load<Sprite>("Sprites/Actor3");
                    baseStats = new int[] { 100, 30, 9, 8, 10 }; //HP, MP, STR, VIT, and AGI for now. HP for everyone is going to be 100 and MP is going to be 30 to start
                    break;
                case Character.GARETH:
                    name = "Gareth";
                    sprite = Resources.Load<Sprite>("Sprites/Actor4");
                    baseStats = new int[] { 100, 30, 11, 9, 10 }; //HP, MP, STR, VIT, and AGI for now. HP for everyone is going to be 100 and MP is going to be 30 to start
                    break;
            }

            //Cast dependencies
            currentHP = baseStats[0];
            currentMP = baseStats[1];
            jobPool = new Dictionary<Job, int>();
            jobPool.Add(Job.NONE, 0);
            actingJob = Job.NONE;
            actingSubJob = Job.NONE;
        }

        public List<CommandInfo> ReadCommands(Stats stat)
        {
            throw new System.NotImplementedException();
        }

        public float ReadFloat(Stats stat)
        {
            throw new System.NotImplementedException();
        }

        public Sprite ReadImage()
        {
            return sprite;
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
                case Stats.PATK: return (int) Mathf.Pow(baseStats[2], 1.75f); //Implement scaling later
                //case Stats.MATK: return EquipStat(Stats.MATK);
                //case Stats.INT: return baseStats[4]; //Implement scaling later
                case Stats.PDEF: return baseStats[3]; //Implement scaling later
                //case Stats.MDEF: return EquipStat(Stats.MDEF);
                //case Stats.MND: return baseStats[5]; //Implement scaling later
                case Stats.AGI: return baseStats[4]; //Implement scaling later (Should be 6 when all is completed)
                default: throw new System.Exception(stat + " cannot be parsed as an int in CharacterData (Or the functionality hasn't been implemented yet).");
            }
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

        public void UpdateDisplay(ref CharacterDisplay display)
        {
            
        }
    }
}
