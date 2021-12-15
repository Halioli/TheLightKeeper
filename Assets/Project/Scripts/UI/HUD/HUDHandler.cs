using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDHandler : MonoBehaviour
{
    // Private Attributes
    private const float FADE_TIME = 2f;

    private float currentFadeTime;
    private int playerHealthValue;
    private int lampTimeValue;
    private int coreTimeValue;
    private CanvasGroup healthGroup;
    private CanvasGroup lampGroup;
    private CanvasGroup coreGroup;
    private CanvasGroup quickAccessGroup;
    private bool coreExists;

    // Public Attributes
    public HUDBar healthBar;
    public HUDBar lampBar;
    public HUDBar coreBar;

    public HUDItem itemRight;
    public HUDItem itemCenter;
    public HUDItem itemLeft;

    public HealthSystem playerHealthSystem;
    public Lamp lamp;
    public Furnace furnace;

    // Start is called before the first frame update
    private void Start()
    {
        currentFadeTime = 0f;

        // Initalize health variables
        healthGroup = GetComponentsInChildren<CanvasGroup>()[0];
        playerHealthValue = playerHealthSystem.GetMaxHealth();
        healthBar.SetMaxValue(playerHealthValue);
        healthBar.UpdateText(CheckTextForZeros(playerHealthValue.ToString()));

        // Initialize lamp variables
        lampGroup = GetComponentsInChildren<CanvasGroup>()[1];
        lampTimeValue = (int)lamp.GetLampTimeRemaining();
        lampBar.SetMaxValue(lampTimeValue);

        // Initialize core variables
        if (coreBar != null)
        {
            coreExists = true;
            coreGroup = GetComponentsInChildren<CanvasGroup>()[2];
            coreTimeValue = furnace.GetMaxFuel();
            coreBar.SetMaxValue(coreTimeValue);
            coreBar.UpdateText(CheckTextForZeros(coreTimeValue.ToString()));
        }
        else
        {
            coreExists = false;
        }

        // Initialize quick access variables
        quickAccessGroup = GetComponentsInChildren<CanvasGroup>()[3];
    }

    private void Update()
    {
        playerHealthValue = playerHealthSystem.GetHealth();
        ChangeValueInHUD(healthBar, playerHealthValue, playerHealthValue.ToString());

        lampTimeValue = (int)lamp.GetLampTimeRemaining();
        ChangeValueInHUD(lampBar, lampTimeValue, null);

        if (coreExists)
        {
            coreTimeValue = furnace.GetCurrentFuel();
            ChangeValueInHUD(coreBar, coreTimeValue, coreTimeValue.ToString());
        }
    }

    private string CheckTextForZeros(string text)
    {
        string zero = "0";

        if (text.Length < 2)
        {
            text = zero + text;
        }

        return text;
    }

    private void ChangeValueInHUD(HUDBar bar, int value, string text)
    {
        bar.SetValue(value);

        if (text != null)
            bar.UpdateText(CheckTextForZeros(text));
    }
}
