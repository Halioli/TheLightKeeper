using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Teleporter : InteractStation
{
    // Private Attributes
    private Vector2 spawnPosition;
    private Animator animator;
    private string[] messagesToShow = { "", "No dark essence found", "Dark essence consumed" };

    // Public Attributes
    //Station
    public GameObject interactText;
    public GameObject canvasTeleportSelection;
    public GameObject hudGameObject;
    public TextMeshProUGUI mssgText;

    //Teleport
    public Item darkEssence;
    public string teleportName;
    public Vector3 teleportTransformPosition;
    public bool activated = false;
    public GameObject[] teleporterLights;
    public GameObject teleportSprite;

    //Events / Actions
    public delegate void TeleportActivation(string teleportName);
    public static event TeleportActivation OnActivation;

    private void Start()
    {
        teleportTransformPosition = GetComponent<Transform>().position;
        teleportTransformPosition.y -= 1.3f;
        spawnPosition = transform.position;
        animator = GetComponent<Animator>();
        teleportSprite.SetActive(true);
    }

    private void Update()
    {
        if (playerInsideTriggerArea)
        {
            GetInput();
            PopUpAppears();
        }
        else
        {
            PopUpDisappears();
        }
    }

    // Interactive pop up disappears
    private void PopUpAppears()
    {
        interactText.SetActive(true);
    }

    // Interactive pop up disappears
    private void PopUpDisappears()
    {
        interactText.SetActive(false);
        mssgText.text = messagesToShow[0];
    }

    public override void StationFunction()
    {
        if (!activated && playerInventory.InventoryContainsItem(darkEssence))
        {
            playerInventory.SubstractItemFromInventory(darkEssence);
            mssgText.text = messagesToShow[2];

            PlayerInputs.instance.canMove = false;
            animator.SetBool("isActivated", true);
        }
        else if (!activated && !playerInventory.InventoryContainsItem(darkEssence))
        {
            mssgText.text = messagesToShow[1];
        }
        else
        {
            if (!canvasTeleportSelection.activeInHierarchy)
            {
                hudGameObject.SetActive(false);
                canvasTeleportSelection.SetActive(true);
                PauseMenu.gameIsPaused = true;
            }
            else
            {
                hudGameObject.SetActive(true);
                canvasTeleportSelection.SetActive(false);
                PauseMenu.gameIsPaused = false;
            }
        }
    }

    public void SetTeleporterActive()
    {
        activated = true;
        PlayerInputs.instance.canMove = true;
        mssgText.text = messagesToShow[0];

        if (OnActivation != null)
            OnActivation(teleportName);
    }

    private void DesactivateSprite()
    {
        teleportSprite.SetActive(false);
    }

}
