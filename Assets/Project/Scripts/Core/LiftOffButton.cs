using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftOffButton : InteractStation
{
    public delegate void LiftOffAction();
    public static event LiftOffAction OnLiftOff;

    private PopUp popUp;

    private void Start()
    {
        popUp = GetComponentInChildren<PopUp>();
    }

    private void Update()
    {
        // If player enters the trigger area the interactionText will appears
        if (playerInsideTriggerArea)
        {
            // Waits the input from interactStation
            GetInput();
            PopUpAppears();
        }
        else
        {
            PopUpDisappears();
        }
    }

    //From InteractStation script
    public override void StationFunction()
    {
        CinemachineShake.Instance.ShakeCamera(3f, 3f);
        PlayerInputs.instance.canMove = false;

        if (OnLiftOff != null)
            OnLiftOff();
    }

    // Interactive pop up disappears
    private void PopUpAppears()
    {
        popUp.ShowInteraction();
        //popUp.ShowAll();
    }

    // Interactive pop up disappears
    private void PopUpDisappears()
    {
        popUp.HideInteraction();
        //popUp.HideAll();
    }
}
