using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInputs : MonoBehaviour
{
    // Public Attributes
    public static PlayerInputs instance;

    public Vector2 mousePosition = new Vector2();
    public Vector2 mouseWorldPosition = new Vector2();
    public bool facingLeft = true;
    public bool canFlip = true;
    public bool canMove = true;
    public float playerReach = 3f;

    public bool canMine = true;
    public bool canAttack = true;

    public GameObject selectSpotGameObject;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Methods
    public bool PlayerClickedMineButton()
    {
        if (PauseMenu.gameIsPaused || !instance.canMine) { return false; }

        return Input.GetKeyDown(KeyCode.Mouse0);
    }

    public bool PlayerClickedAttackButton()
    {
        if (PauseMenu.gameIsPaused || !instance.canAttack) { return false; }

        return Input.GetKeyDown(KeyCode.Mouse1);
    }

    public void SetNewMousePosition()
    {
        mousePosition = Input.mousePosition;
        mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
    }

    public Vector2 GetMousePositionInWorld()
    {
        SetNewMousePosition();
        return mouseWorldPosition;
    }

    public bool PlayerPressedInteractButton()
    {
        return Input.GetKeyDown(KeyCode.E);
    }

    public bool PlayerPressedUseButton()
    {
        if (PauseMenu.gameIsPaused) { return false; }

        return Input.GetKeyDown(KeyCode.Q);
    }

    public bool PlayerPressedInventoryButton()
    {
        if (PauseMenu.gameIsPaused) { return false; }

        return Input.GetKeyDown(KeyCode.Tab);
    }

    public bool PlayerPressedQuickAccessButton()
    {
        if (PauseMenu.gameIsPaused) { return false; }

        return Input.GetKeyDown(KeyCode.LeftShift);
    }

    public bool PlayerReleasedQuickAccessButton()
    {
        if (PauseMenu.gameIsPaused) { return false; }

        return Input.GetKeyUp(KeyCode.LeftShift);
    }

    public bool PlayerPressedPauseButton()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }

    public Vector2 PlayerPressedMovementButtons()
    {
        if (canMove && !PauseMenu.gameIsPaused)
        {
            return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        else
        {
            return Vector2.zero;
        }
    }

    public Vector2 PlayerMouseScroll()
    {
        return Input.mouseScrollDelta;
    }

    public void SpawnSelectSpotAtTransform(Transform transform)
    {
        GameObject selectedSpot = Instantiate(selectSpotGameObject, transform);
        selectedSpot.GetComponent<SelectSpot>().DoSelectLoop();
    }

    public void FlipSprite(Vector2 direction)
    {
        if (!instance.canFlip)
            return;

        if ((direction.x > 0 && !instance.facingLeft) || direction.x < 0 && instance.facingLeft)
        {
            instance.facingLeft = !instance.facingLeft;
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
        }
    }

    public void QuitGame()
    {
        Debug.Log("Closing application...");
        Application.Quit();
    }


}
