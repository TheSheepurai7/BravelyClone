using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Encounter", menuName = "Scriptable Objects/Encounter")]
public class Encounter : ScriptableObject
{
    public int minLevel;
    public int maxLevel;

    [SerializeField] Enemy[] enemies;

    public ReadOnlyEnemyStruct[] GetEncounterData()
    {
        ReadOnlyEnemyStruct[] returnValue = new ReadOnlyEnemyStruct[enemies.Length];

        for(int i = 0; i < returnValue.Length; i++)
        {
            returnValue[i] = enemies[i].GetInfo();
        }

        return returnValue;
    }
}
