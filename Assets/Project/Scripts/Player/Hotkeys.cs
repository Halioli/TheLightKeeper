using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hotkeys : MonoBehaviour
{
    private HealthSystem playerHealthSystem;
    private PlayerCombat playerCombat;
    private Lamp playerLamp;
    private PlayerMovement playerMovement;
    private bool canUseCheats = false;
    private bool zooming = false;

    public GameObject coalMineral;
    public GameObject ironMineral;
    public GameObject darkEssenceMineral;
    public GameObject luxuniteMineral;
    public GameObject enrichedMetalMineral;
    public GameObject electricOrbMineral;
    public GameObject healingFlowerMineral;
    public GameObject autoMiner;
    public GameObject lightBeacon;
    public GameObject bomb;

    public delegate void PlayerHealed(int healthToAdd);
    public static event PlayerHealed OnHealed;

    public delegate void CheatTeleport(Vector3 position);
    public static event CheatTeleport OnCheatTeleport;

    void Start()
    {
        playerCombat = GetComponent<PlayerCombat>();
        playerHealthSystem = GetComponent<HealthSystem>();
        playerLamp = GetComponentInChildren<Lamp>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (canUseCheats)
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                // Spawn 1 iron at the player's feet
                Instantiate(ironMineral, transform);
            }
            else if (Input.GetKeyDown(KeyCode.O))
            {
                // Spawn 1 coal at the player's feet
                Instantiate(coalMineral, transform);
            }
            else if (Input.GetKeyDown(KeyCode.P))
            {
                // Spawn 1 dark essence at the player's feet
                Instantiate(darkEssenceMineral, transform);
            }
            else if (Input.GetKeyDown(KeyCode.L))
            {
                // Spawn 1 luxinite at the player's feet
                Instantiate(luxuniteMineral, transform);
            }
            else if (Input.GetKeyDown(KeyCode.U))
            {
                // Spawn 1 enriched metal at the player's feet
                Instantiate(enrichedMetalMineral, transform);
            }
            else if (Input.GetKeyDown(KeyCode.M))
            {
                // Spawn 1 electric orb at the player's feet
                Instantiate(electricOrbMineral, transform);
            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                // Spawn 1 electric orb at the player's feet
                Instantiate(healingFlowerMineral, transform);
            }
            else if (Input.GetKeyDown(KeyCode.J) || Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                // +5 health
                if (OnHealed != null) 
                    OnHealed(5);
            }
            else if (Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                // -5 health
                playerCombat.ReceiveDamage(5);
            }
            else if (Input.GetKeyDown(KeyCode.Period))
            {
                // +5 lantern
                playerLamp.RefillLampTime(5f);
            }
            else if (Input.GetKeyDown(KeyCode.Comma))
            {
                // -5 lantern
                playerLamp.ConsumeSpecificLampTime(5f);
            }
            else if (Input.GetKeyDown(KeyCode.Backspace))
            {
                if (!zooming)
                {
                    playerMovement.moveSpeed = 1000;
                    zooming = true;
                }
                else
                {
                    playerMovement.moveSpeed = 200;
                    zooming = false;
                }
            }
            else if (Input.GetKeyDown(KeyCode.RightShift))
            {
                StartCoroutine(TeleportToFogZone());
            }
            else if (Input.GetKeyDown(KeyCode.RightControl))
            {
                StartCoroutine(TeleportToMountain());
            }
        }
    }

    private void OnEnable()
    {
        PauseMenu.OnToggleCheats += SetCheatsEnabled;
    }

    private void OnDisable()
    {
        PauseMenu.OnToggleCheats -= SetCheatsEnabled;
    }

    private void SetCheatsEnabled(bool value)
    {
        canUseCheats = value;
    }

    IEnumerator TeleportToFogZone()
    {
        Vector3 fogZoneLanding = new Vector3(75, -120, 0);

        if (OnCheatTeleport != null)
            OnCheatTeleport(fogZoneLanding);

        yield return null;

        Instantiate(autoMiner, transform);
        Instantiate(lightBeacon, transform);
    }

    IEnumerator TeleportToMountain()
    {
        Vector3 mountainZoneLanding = new Vector3(123, 144, 0);

        if (OnCheatTeleport != null)
            OnCheatTeleport(mountainZoneLanding);

        yield return null;

        Instantiate(bomb, transform);
        Instantiate(bomb, transform);
        Instantiate(bomb, transform);
    }
}
