using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Element { FIRE, WATER, EARTH, WIND, LIGHT, DARK }
public enum Status { POISON, BLIND, SILENCE, PARALYZE, CONFUSE, CHARM, OUT, DOOM, BERSERK }

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class Enemy : ScriptableObject
{
    //Its stats
    [SerializeField] Sprite sprite;
    [SerializeField] int hP;
    [SerializeField] int mP;
    [SerializeField] int pAtk;
    [SerializeField] int mAtk;
    [SerializeField] int iNT;
    [SerializeField] int pDef;
    [SerializeField] int mDef;
    [SerializeField] int mND;
    [SerializeField] int sPD;
    [SerializeField] int jP;
    [SerializeField] int xP;
    [SerializeField] int pG;

    //I think I should have these represented numerically and pass that along with the ReadOnlyEnemyStruct
    [SerializeField] Element[] eWeak;
    [SerializeField] Element[] eResist;
    [SerializeField] Element[] eImmune;
    [SerializeField] Element[] eAbsorb;
    [SerializeField] Status[] sWeak;
    [SerializeField] Status[] sResist;
    [SerializeField] Status[] sImmune;

    //The AI is the AI
    [SerializeField] AI ai;

    //For when I need just the sprite
    public Sprite GetSprite() { return sprite; }

    //For when I need the AI
    public AI GetAI() { return ai; }

    //The rewards are small enough that I can probably use pointers for them
    public void CopyRewards(ref int inJP, ref int inXP, ref int inPG)
    {
        inJP = jP;
        inXP = xP;
        inPG = pG;
    }

    //For when I need everything else
    public StatBlock ExportStats()
    {
        return new StatBlock(hP, mP, pAtk, mAtk, iNT, pDef, mDef, mND, sPD);
    }
}
