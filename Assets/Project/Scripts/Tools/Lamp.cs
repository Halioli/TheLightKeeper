using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Lamp : MonoBehaviour
{
    // Private Attributes
    private const float LIGHT_INTENSITY_ON = 1f;
    private const float LIGHT_INTENSITY_OFF = 0.3f;

    private const int MAX_SOURCE_LEVELS = 6;
    private int sourceLevel = 0;
    private float[] LIGHT_ANGLE_LVL = { 40f, 50f, 60f, 70f, 80f, 90f };
    private float[] LIGHT_DISTANCE_LVL = { 10f, 12.5f, 15f, 20f, 25f };
    private float lightAngle;
    private float lightDistance;

    private const int MAX_TIME_LEVELS = 3;
    private int timeLevel = 0;
    private float lampTime;
    private float[] LAMP_TIME_LVL = { 5f, 5f, 10f };

    private bool coneIsActive = false;

    private float maxLampTime;
    private Animator playerAnimator;

    // Public Attributes
    public bool turnedOn;
    public bool active = false;
    public bool canRefill;

    [SerializeField] private CircleLight circleLight;
    [SerializeField] private ConeLight coneLight;

    public float flickerIntensity;
    public float flickerTime;
    private const float START_FLICK_COOLDOWN = 5f;
    private float flickCooldown = START_FLICK_COOLDOWN;
    private float lowLightflickCooldown = 0.75f;
    private const float SECONDS_HIGH_FREQUENCY_FLICK = 10f;

    System.Random rg;

    public delegate void PlayLanternSound();
    public static event PlayLanternSound turnOnLanternEvent;
    public static event PlayLanternSound turnOffLanternEvent;
    public static event PlayLanternSound turnOnLanternDroneSoundEvent;
    public static event PlayLanternSound turnOffLanternDroneSoundEvent;

    private void Awake()
    {
        lampTime = maxLampTime = 20f;
        turnedOn = false;
        rg = new System.Random();
        flickerTime = 0.08f;
        flickerIntensity = 1f;

        lightAngle = LIGHT_ANGLE_LVL[sourceLevel];
        lightDistance = LIGHT_DISTANCE_LVL[sourceLevel];
    }

    private void Start()
    {
        playerAnimator = GetComponentInParent<Animator>();

        circleLight.SetDistance(2f);
        coneLight.SetDistance(lightDistance);
        coneLight.SetAngle(lightAngle);
    }

    private void OnEnable()
    {
        LanternSourceUpgrade.OnLanternSourceUpgrade += UpgradeLampSource;
        LanternTimeUpgrade.OnLanternTimeUpgrade += UpgradeLampTime;
    }

    private void OnDisable()
    {
        LanternSourceUpgrade.OnLanternSourceUpgrade -= UpgradeLampSource;
        LanternTimeUpgrade.OnLanternTimeUpgrade -= UpgradeLampTime;
    }

    public void UpdateLamp()
    {
        if (LampTimeExhausted())
        {
            turnedOn = false;
            playerAnimator.SetBool("light", false);

            DeactivateConeLight();

            GetComponentInParent<PlayerLightChecker>().SetPlayerInLightToFalse();
            flickCooldown = START_FLICK_COOLDOWN;
            circleLight.SetIntensity(LIGHT_INTENSITY_OFF);

            if (turnOffLanternEvent != null)
            {
                turnOffLanternEvent();
            }
        }
        else
        {
            ConsumeLampTime();
            if (lampTime <= SECONDS_HIGH_FREQUENCY_FLICK)
            {
                flickCooldown = lowLightflickCooldown;
            }
        }
    }

    public bool LampTimeExhausted()
    {
        return lampTime <= 0;
    }

    public void ConsumeLampTime()
    {
        lampTime -= Time.deltaTime;
    }

    public void FullyRefillLampTime()
    {
        lampTime = maxLampTime;
    }
    public void RefillLampTime(float time)
    {
        if (lampTime + time > maxLampTime)
        {
            FullyRefillLampTime();
        }
        else
        {
            lampTime += time;
        }
        flickCooldown = START_FLICK_COOLDOWN;
    }

    public bool CanRefill()
    {
        if (turnedOn)  //Player in darkness
        {
            if (lampTime == 0 || lampTime == maxLampTime) //Player doesn't has lamp fuel
            {
                return false;
            }
            else //Player has lamp fuel
            {
                return true;
            }
        }
        else //Player in light
        {
            if(lampTime != maxLampTime) //Player has no max lamp fuel
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public void ActivateLampLight()
    {
        turnedOn = true;
        playerAnimator.SetBool("light", true);

        if (!active && turnOnLanternEvent != null)
            turnOnLanternEvent();

        if (!coneIsActive)
            ActivateConeLight();
        if (!active)
            ActivateCircleLight();

        StartCoroutine(LightFlicking());
    }

    public void ActivateConeLight()
    {
        if (!coneIsActive && turnOnLanternDroneSoundEvent != null)
            turnOnLanternDroneSoundEvent();

        coneIsActive = true;

        coneLight.SetIntensity(LIGHT_INTENSITY_ON);
        coneLight.Expand();
    }

    public void ActivateCircleLight()
    {
        active = true;

        circleLight.SetIntensity(LIGHT_INTENSITY_ON);
        circleLight.Expand();
    }

    public void DeactivateLampLight()
    {
        if (turnedOn && turnOffLanternEvent != null)
            turnOffLanternEvent();

        turnedOn = false;
        playerAnimator.SetBool("light", false);

        if (coneIsActive)
            DeactivateConeLight();

        if (active)
            DeactivateCircleLight();
    }

    public void DeactivateConeLight()
    {
        if (coneIsActive && turnOffLanternDroneSoundEvent != null)
            turnOffLanternDroneSoundEvent();

        coneIsActive = false;

        StopCoroutine(LightFlicking());

        coneLight.Shrink();

        circleLight.SetIntensity(LIGHT_INTENSITY_OFF);
    }

    public void DeactivateCircleLight()
    {
        active = false;

        circleLight.Shrink();
    }

    public float GetLampTimeRemaining()
    {
        return lampTime;
    }

    public float GetMaxLampTime()
    {
        return maxLampTime;
    }

    private void UpgradeLampSource()
    {
        if (sourceLevel >= MAX_SOURCE_LEVELS)
        {
            return;
        }
        ++sourceLevel;

        lightAngle = LIGHT_ANGLE_LVL[sourceLevel];
        lightDistance = LIGHT_DISTANCE_LVL[sourceLevel];

        coneLight.SetDistance(lightDistance);   
        coneLight.SetAngle(lightAngle);
    }

    private void UpgradeLampTime()
    {
        if (timeLevel >= MAX_TIME_LEVELS)
        {
            return;
        }

        maxLampTime += LAMP_TIME_LVL[timeLevel];
        lampTime = maxLampTime;
        ++timeLevel;
    }

    IEnumerator LightFlicking()
    {
        if (lampTime > SECONDS_HIGH_FREQUENCY_FLICK)
        {
            flickCooldown = START_FLICK_COOLDOWN;
        }

        float lightingTime;
        int flickerCount;
        float flickingIntensity;
        float flickingTime;

        while (turnedOn)
        {
            circleLight.SetIntensity(LIGHT_INTENSITY_ON);
            coneLight.SetIntensity(LIGHT_INTENSITY_ON);

            lightingTime = flickCooldown + ((float)rg.NextDouble() - 0.5f);
            yield return new WaitForSeconds(lightingTime);

            flickerCount = rg.Next(4, 9);
            
            for (int i = 0; i < flickerCount; ++i)
            {
                flickingIntensity = 1f - ((float)rg.NextDouble() * flickerIntensity);
                circleLight.SetIntensity(flickingIntensity);
                coneLight.SetIntensity(flickingIntensity);

                flickingTime = (float)rg.NextDouble() * flickerTime;
                if (lampTime < SECONDS_HIGH_FREQUENCY_FLICK)
                {
                    if (!turnedOn) break;
                    yield return new WaitForSeconds(flickingTime);
                }
                else
                {
                    yield return new WaitForSeconds(flickingTime);
                }
            }

        }

        //DeactivateConeLight();
    }
}
