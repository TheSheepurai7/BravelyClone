using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Element { FIRE, WATER, EARTH, WIND, LIGHT, DARK }
public enum Status { POISON, BLIND, SILENCE, PARALYZE, CONFUSE, CHARM, OUT, DOOM, BERSERK }

public readonly struct ReadOnlyEnemyStruct
{
    public readonly string name;
    public readonly Sprite sprite;
    public readonly int hP;
    public readonly  int mP;
    public readonly int pAtk;
    public readonly int mAtk;
    public readonly int iNT;
    public readonly int pDef;
    public readonly int mDef;
    public readonly int mND;
    public readonly int sPD;
    public readonly int jP;
    public readonly int xP;
    public readonly int pG;

    public ReadOnlyEnemyStruct(string name, Sprite sprite, int hP, int mP, int pAtk, int mAtk, int iNT, int pDef, int mDef, int mND, int sPD, int jP, int xP, int pG)
    {
        this.name = name;
        this.sprite = sprite;
        this.hP = hP;
        this.mP = mP;
        this.pAtk = pAtk;
        this.mAtk = mAtk;
        this.iNT = iNT;
        this.pDef = pDef;
        this.mDef = mDef;
        this.mND = mND;
        this.sPD = sPD;
        this.jP = jP;
        this.xP = xP;
        this.pG = pG;
    }
}

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

    public ReadOnlyEnemyStruct GetInfo()
    {
        return new ReadOnlyEnemyStruct(name, sprite, hP, mP, pAtk, mAtk, iNT, pDef, mDef, mND, sPD, jP, xP, pG);
    }
}
