using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class MeterDisplay : MonoBehaviour
{
    [SerializeField] Character character;
    [SerializeField] Stats stat;

    // Start is called before the first frame update
    void OnEnable()
    {
        //Verify that the meter records a valid stat for this kind of display
        if (stat != Stats.HP || stat != Stats.MP) { Destroy(this); }

        //Calculate the meter's width
        float width = 0;
        if (transform.parent != null) { width = transform.parent.GetComponent<RectTransform>().rect.width; }
        else { Destroy(this); }

        //Calculate this component's width
        GetComponent<RectTransform>().sizeDelta = new Vector2(width * GameData.instance.GetPercentageStat(character, stat).x, GetComponent<RectTransform>().sizeDelta.y);
    }
}
