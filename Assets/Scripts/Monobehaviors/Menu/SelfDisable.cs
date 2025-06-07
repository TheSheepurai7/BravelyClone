using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDisable : MonoBehaviour
{
    public void DisableSelf()
    {
        gameObject.SetActive(false);
    }
}
