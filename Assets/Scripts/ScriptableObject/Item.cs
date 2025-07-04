using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    [SerializeField] bool tradeable;
    [SerializeField] int price;

    public bool IsTradeable(out int price)
    {
        if (tradeable) { price = this.price; return true; }
        price = 0;
        return false;
    }
}
