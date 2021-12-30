using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InGameHUDHandler : MonoBehaviour
{
    // Private Attributes
    private const float FADE_TIME = 0.5f;

    private int playerHealthValue;
    private int lampTimeValue;
    private bool playerIsDamaged;
    private bool healthIsTrembeling;
    private bool lampIsOn;
    private bool lampIsTrembeling;
    private CanvasGroup healthGroup;
    private CanvasGroup lampGroup;

    // Public Attributes
    public HUDBar healthBar;
    public HUDBar lampBar;

    public GameObject healthBarGameObject;
    public GameObject lampBarGameObject;

    public HealthSystem playerHealthSystem;
    public Lamp lamp;

    // Start is called before the first frame update
    private void Start()
    {
        // Initalize health variables
        healthGroup = GetComponentsInChildren<CanvasGroup>()[0];
        playerHealthValue = playerHealthSystem.GetMaxHealth();
        healthBar.SetMaxValue(playerHealthValue);
        playerIsDamaged = false;
        healthIsTrembeling = false;

        // Initialize lamp variables
        lampGroup = GetComponentsInChildren<CanvasGroup>()[1];
        lampTimeValue = (int)lamp.GetLampTimeRemaining();
        lampBar.SetMaxValue(lampTimeValue);
        lampIsOn = false;
        lampIsTrembeling = false;
    }

    private void Update()
    {
        playerHealthValue = playerHealthSystem.GetHealth();
        ChangeValueInHUD(healthBar, playerHealthValue, playerHealthValue.ToString());

        lampTimeValue = (int)lamp.GetLampTimeRemaining();
        ChangeValueInHUD(lampBar, lampTimeValue, null);

        // Check if any element needs to appear/disappear
        ManageShowingHealth();
        ManageShowingLampFuel();

        if (playerIsDamaged)
        {
            if (playerHealthSystem.GetHealth() <= 5 && !healthIsTrembeling)
            {
                // Tremble
                healthIsTrembeling = true;
                healthBarGameObject.transform.DOShakePosition(10f, 0.1f);//.SetLoops(-1);
            }
        }

        if (lampIsOn)
        {
            if (lamp.GetLampTimeRemaining() <= 5f && !lampIsTrembeling)
            {
                // Tremble
                lampIsTrembeling = true;
                lampBarGameObject.transform.DOShakePosition(10f, 0.1f);//.SetLoops(-1);
            }
        }
    }

    private void ManageShowingHealth()
    {
        if ((playerHealthSystem.GetHealth() < playerHealthSystem.GetMaxHealth()) && !playerIsDamaged)
        {
            StartCoroutine(CanvasFadeIn(healthGroup));
            playerIsDamaged = true;
        }
        else if (!(playerHealthSystem.GetHealth() < playerHealthSystem.GetMaxHealth()) && playerIsDamaged)
        {
            StartCoroutine(CanvasFadeOut(healthGroup));
            playerIsDamaged = false;
            healthIsTrembeling = false;
        }
    }

    private void ManageShowingLampFuel()
    {
        if (lamp.turnedOn && !lampIsOn)
        {
            StartCoroutine(CanvasFadeIn(lampGroup));
            lampIsOn = true;
        }
        else if (!lamp.turnedOn && lampIsOn)
        {
            StartCoroutine(CanvasFadeOut(lampGroup));
            lampIsOn = false;
            lampIsTrembeling = false;
        }
    }

    private void ChangeValueInHUD(HUDBar bar, int value, string text)
    {
        bar.SetValue(value);
    }

    IEnumerator CanvasFadeOut(CanvasGroup canvasGroup)
    {
        Vector2 startVector = new Vector2(1f, 1f);
        Vector2 endVector = new Vector2(0f, 0f);

        for (float t = 0f; t < FADE_TIME; t += Time.deltaTime)
        {
            float normalizedTime = t / FADE_TIME;

            canvasGroup.alpha = Vector2.Lerp(startVector, endVector, normalizedTime).x;
            yield return null;
        }
        canvasGroup.alpha = endVector.x;
    }

    IEnumerator CanvasFadeIn(CanvasGroup canvasGroup)
    {
        Vector2 startVector = new Vector2(0f, 0f);
        Vector2 endVector = new Vector2(1f, 1f);

        for (float t = 0f; t < FADE_TIME; t += Time.deltaTime)
        {
            float normalizedTime = t / FADE_TIME;

            canvasGroup.alpha = Vector2.Lerp(startVector, endVector, normalizedTime).x;
            yield return null;
        }
        canvasGroup.alpha = endVector.x;
    }
}
