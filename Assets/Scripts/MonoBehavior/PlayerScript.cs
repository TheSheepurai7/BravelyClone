using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerScript : CombatantScript, IPointerClickHandler, IPointerDownHandler
{
    bool clickEnabled;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();

        //Subscribe to game manager events here
        GameManager.instance.onUpdate += (float deltaTime) => { IncrementATB(deltaTime); DisplayStats(); };

        //Subscribe to statBlock events
        _statBlock.onDeath += () => { GameManager.instance.CloseActionMenu(ref _statBlock); GameManager.instance.SetLoseCon(); };
    }

    void Update()
    {
        //No matter what, clickEnabled becomes false any time the mouse button is up
        if (Input.GetMouseButtonUp(0)) { clickEnabled = false; }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //None of this works unless the game's ATB is running
        //Later I should find the lowest AP cost among possible commands as the minimum, but for now a strict 1 AP requirement is adequate
        if (GameManager.instance.ready && ((int)(statBlock.aP / statBlock.apMax)) >= 1 && clickEnabled)
        {
            //Left click opens the action menu
            if (eventData.button == PointerEventData.InputButton.Left) { GameManager.instance.SpawnActionMenu(ref _statBlock); }
            else if (eventData.button == PointerEventData.InputButton.Right) { print("Set a defend action"); }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GameManager.instance.ready) { clickEnabled = true; }
    }
}
