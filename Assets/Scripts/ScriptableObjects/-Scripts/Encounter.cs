using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Encounter", menuName = "Scriptable Objects/Encounter")]
public class Encounter : ScriptableObject
{
    public int minLevel;
    public int maxLevel;

    [SerializeField] Enemy[] enemies;

    public Enemy[] EnemyList()
    {
        return enemies;
    }
}
