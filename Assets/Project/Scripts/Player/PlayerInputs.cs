using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    // Public Attributes
    protected Vector2 mousePosition = new Vector2();
    protected Vector2 mouseWorldPosition = new Vector2();


    // Methods
    public bool PlayerClickedMineButton()
    {
        return Input.GetKeyDown(KeyCode.Mouse0);
    }

    public void SetNewMousePosition()
    {
        mousePosition = Input.mousePosition;
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
    }

    public bool PlayerClickedInteractButton()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    public bool PlayerClickedUseButton()
    {
        return Input.GetKeyDown(KeyCode.Q);
    }

    public bool PlayerClickedAttackButton()
    {
        return Input.GetKeyDown(KeyCode.Mouse1);
    }

    public Vector2 PlayerClickedMovementButtons()
    {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public Vector2 PlayerMouseScroll()
    {
        return Input.mouseScrollDelta;
    }
}
