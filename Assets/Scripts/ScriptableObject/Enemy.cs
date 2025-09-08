using UnityEngine;

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy")]
public class Enemy : ScriptableObject, IStatBlock
{
    [SerializeField] Sprite sprite;
    [SerializeField] AI ai;
    [SerializeField] int hP;
    [SerializeField] int mP;
    [SerializeField] int pAtk;
    [SerializeField] int pDef;
    [SerializeField] int spd;

    public AI getAI {  get { return ai; } }

    public StatBlock ExportStatBlock()
    {
        return new StatBlock(
            name, 
            sprite,
            hP,
            mP,
            pAtk,
            pDef,
            spd
            );
    }
}
