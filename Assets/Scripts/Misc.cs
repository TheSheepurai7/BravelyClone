//Namespaces for this miscellania
using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

//The miscellania
public delegate void UpdateStats(Stats stat);
public enum Character { ALEC, MARISA, JENNA, GARETH }
public enum Stats { NAME, SPRITE, LVL, CHP, MHP, CMP, MMP, STR, PATK, VIT, PDEF, INT, MATK, MND, MDEF, AGI, JOB, JLV, AP }
public enum EquipType { SWORD, STAFF, KNIFE, AXE, SHIELD, HEAD, BODY, ACCESSORY }
public interface IStatReader 
{ 
    public Sprite ReadImage(); 
    public string ReadString(Stats stat); 
    public int ReadInt(Stats stat);
    public float ReadFloat(Stats stat);
    public void SubscribeDelegate(ref UpdateStats theDelegate);
}


