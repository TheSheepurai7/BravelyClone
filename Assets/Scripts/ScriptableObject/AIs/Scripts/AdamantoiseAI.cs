using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AdamantoiseAI", menuName = "Scriptable Objects/AI/AdamantoiseAI")]
public class AdamantoiseAI : AI
{
    public override void ExecuteAI(StatBlock myStatBlock)
    {
        //Basic attack
        if (myStatBlock.aP >= myStatBlock.apMax)
        {
            //Calculate threat
            StatBlock topTarget = null; float topThreatValue = 0;
            foreach (KeyValuePair<StatBlock, List<MemoryObject>> threat in threatDictionary)
            {
                if (threat.Key.currentHP > 0) //Make sure the enemy doesn't bother with opponents who are already dead
                {
                    float totalThreat = 0;
                    foreach (MemoryObject memory in threatDictionary[threat.Key]) { totalThreat += memory.threatValue; }
                    if (totalThreat > topThreatValue) { topTarget = threat.Key; topThreatValue = totalThreat; }
                }
            }

            //Stand in for executing an attack
            if (topTarget != null) { Attack(myStatBlock, topTarget); }
            myStatBlock.aP -= myStatBlock.apMax;
        }
    }

    void Attack(StatBlock source, StatBlock target)
    {
        //First subtract the AP for the action
        source.aP -= source.apMax;

        //Calculate and apply damage
        int damage = (int)Mathf.Clamp((source.pAtk - target.pDef) * UnityEngine.Random.Range(0.8f, 1), 1, Mathf.Infinity);
        target.currentHP -= damage;
    }
}
