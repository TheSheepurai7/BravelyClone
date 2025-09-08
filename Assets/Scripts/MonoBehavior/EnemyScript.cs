using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyScript : CombatantScript
{
    const float FADE_TIME = 0.7f;

    event Action onFadeComplete;

    AI _ai;
    public AI ai { set { if (_ai == null) { _ai = value; } } }

    protected override void Start()
    {
        base.Start();

        //Subscribe to game manager events here
        GameManager.instance.onUpdate += (float deltaTime) => { if (statBlock.currentHP > 0) { IncrementATB(deltaTime); _ai.ExecuteAI(statBlock); } };

        //Subscribe to statblock events here
        statBlock.onDeath += () => StartCoroutine(DeathProcedure());
        statBlock.onThreat += (ref StatBlock source, int value) => _ai.AddThreat(ref source, value);
    }

    IEnumerator DeathProcedure()
    {
        //Check the wincon
        GameManager.instance.SetWinCon(ref onFadeComplete);

        //Remove text
        foreach(Text text in GetComponentsInChildren<Text>()) { if (!text.TryGetComponent(out AnimateHPChange hpChange)) text.text = string.Empty; }

        //Fade the enemy out
        float alphaFade = 1;
        while (alphaFade > 0)
        {
            float deltaFade = Time.deltaTime / FADE_TIME;
            alphaFade -= deltaFade;
            foreach(Image image in GetComponentsInChildren<Image>()) { image.color -= (Color)new Vector4(0, deltaFade, deltaFade, deltaFade); }
            yield return null;
        }

        //Signal that the fade is complete
        onFadeComplete();
    }
}
