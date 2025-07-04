using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Encounter", menuName = "Scriptable Objects/Encounter")]
public class Encounter : ScriptableObject
{
    [SerializeField] Enemy[] enemies;
    [SerializeField] Vector2Int encounterRating;

    public Enemy[] EnemyParty()
    {
        return enemies;
    }

    public Vector2Int EncounterRating()
    {
        return encounterRating;
    }
}
