using UnityEngine;

public class SelfHide : MonoBehaviour
{
    public void HideSelf()
    {
        gameObject.SetActive(false);
    }
}
