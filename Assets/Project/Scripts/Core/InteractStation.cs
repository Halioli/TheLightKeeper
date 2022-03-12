using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InteractStation : MonoBehaviour
{
    public BoxCollider2D triggerArea;

    protected bool playerInsideTriggerArea;
    protected Inventory playerInventory;

    // Action
    public delegate void InteractStationAction();
    public static event InteractStationAction OnInteractOpen;
    public static event InteractStationAction OnInteractClose;

    public delegate void InteractStationDescriptionAction(string description);
    public static event InteractStationAction OnDescriptionOpen;
    public static event InteractStationDescriptionAction OnDescriptionSet;


    private void Awake()
    {
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Inventory>();
        Debug.Log(playerInventory);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInsideTriggerArea = true;
            
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInsideTriggerArea = false;
        }
    }

    public void GetInput()
    {
        if (PlayerInputs.instance.PlayerPressedInteractButton())
        {
            StationFunction();
        }
    }

    virtual public void StationFunction()
    {
        //Code from child
    }

    virtual public void UpgradeFunction()
    {
        //Code from child
    }

    protected void DoOnInteractOpen()
    {
        if (OnInteractOpen != null)
        {
            OnInteractOpen();
        }
    }

    protected void DoOnInteractClose()
    {
        if (OnInteractClose != null)
        {
            OnInteractClose();
        }
    }

    protected void DoOnInteractDescriptionOpen()
    {
        if (OnDescriptionOpen != null)
        {
            OnDescriptionOpen();
        }
    }

}
