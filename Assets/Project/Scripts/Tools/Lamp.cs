using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Lamp : MonoBehaviour
{
    // Private Attributes
    private const int MAX_LEVELS = 5;
    private int level = 1;

    private const float LIGHT_INTENSITY_ON = 0.5f;
    private const float LIGHT_INTENSITY_OFF = 0.0f;

    private const float RADIUS_DIFFERENCE = 20f;

    private float[] LIGHT_ANGLE_LVL = {35f, 55f, 75f, 95f, 115f };
    private float[] LIGHT_DISTANCE_LVL = {5f, 10f, 15f, 20f, 25f };
    private float lightAngle;
    private float lightDistance;

    private const float LIGHT_CIRCLE_RADIUS = 2f;


    private float maxLampTime;
    private SpriteRenderer lampSpriteRenderer;
    private Inventory playerInventory;

    // Public Attributes
    public bool turnedOn;
    public float lampTime;

    public GameObject lampCircleLight;
    public GameObject lampConeLight;

    public Animator animator;

    public float flickerIntensity;
    public float flickerTime;

    System.Random rg;


    public delegate void PlayLanternSound();
    public static event PlayLanternSound turnOnLanternSoundEvent;
    public static event PlayLanternSound turnOffLanternSoundEvent;



    private void Awake()
    {
        lampTime = maxLampTime = 20f;
        turnedOn = false;
        lampSpriteRenderer = GetComponent<SpriteRenderer>();
        rg = new System.Random();
        flickerTime = 0.08f;
        flickerIntensity = 1f;

        lightAngle = LIGHT_ANGLE_LVL[0];
        lightDistance = LIGHT_DISTANCE_LVL[0];
    }

    private void Start()
    {
        playerInventory = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<Inventory>();
        StartCoroutine(Flicker());
    }

    private void Update()
    {
        if (turnedOn)
        {
            UpdateLamp();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            LevelUp();
        }
    }

    public void UpdateLamp()
    {
        if (LampTimeExhausted())
        {
            DeactivateConeLightButNotPointLight();
            GetComponentInParent<PlayerLightChecker>().SetPlayerInLightToFalse();
        }
        else
        {
            ConsumeLampTime();
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

    public void ActivateLampLight()
    {
        turnedOn = true;
        animator.SetBool("light", true);

        ActivateConeLight();
        ActivateCircleLight();

        StartCoroutine("LightFlicking");
    }

    public void ActivateConeLight()
    {
        lampConeLight.SetActive(true);

        lampConeLight.GetComponent<Light2D>().intensity = LIGHT_INTENSITY_ON;
        StartCoroutine("ExpandConeLight");

        if (turnOnLanternSoundEvent != null)
            turnOnLanternSoundEvent();
    }
    public void ActivateCircleLight()
    {
        lampCircleLight.SetActive(true);

        lampCircleLight.GetComponent<Light2D>().intensity = LIGHT_INTENSITY_ON;
        StartCoroutine("ExpandCircleLight");
    }


    public void DeactivateLampLight()
    {
        turnedOn = false;
        animator.SetBool("light", false);

        DeactivateConeLight();
        DeactivateCircleLight();

        if (turnOffLanternSoundEvent != null)
            turnOffLanternSoundEvent();
    }

    public void DeactivateConeLightButNotPointLight()
    {
        //lampConeLight.GetComponent<Light2D>().intensity = LIGHT_INTENSITY_OFF;
        //StartCoroutine("ShrinkConeLightOnActivate");
        StopCoroutine("LightFlicking");

        StartCoroutine("ShrinkConeLight");
    }
    public void DeactivateCircleLight()
    {
        StartCoroutine("ShrinkCircleLight");
    }

    public float GetLampTimeRemaining()
    {
        return lampTime;
    }

    public void LevelUp()
    {
        if (level >= MAX_LEVELS)
        {
            return;
        }

        ++level;

        lightAngle = LIGHT_ANGLE_LVL[level - 1];
        lightDistance = LIGHT_DISTANCE_LVL[level - 1];

        lampConeLight.GetComponent<Light2D>().pointLightInnerRadius = lightDistance - 5f;
        lampConeLight.GetComponent<Light2D>().pointLightOuterRadius = lightDistance;
    }



    IEnumerator LightFlicking()
    {
        while (true)
        {
            circlePointLight.intensity = CIRCLE_LIGHT_INTENSITY_ON;
            coneParametricLight.intensity = CONE_LIGHT_INTENSITY_ON;

            float lightingTime = 5 + ((float)rg.NextDouble() - 0.5f);
            yield return new WaitForSeconds(lightingTime);

            int flickerCount = rg.Next(4, 9);

            for(int i = 0; i < flickerCount; i++)
            {
                float flickingIntensity = 1f - ((float)rg.NextDouble() * flickerIntensity);
                circlePointLight.intensity = flickingIntensity;
                coneParametricLight.intensity = flickingIntensity;

                float flickingTime = (float)rg.NextDouble() * flickerTime;
                yield return new WaitForSeconds(flickingTime);
            }
        }

        
        DeactivateConeLight();

    }

    IEnumerator ExpandConeLight()
    {
        StopCoroutine("ShrinkConeLight");

        for (float i = 0f; i < lightAngle; i += Time.deltaTime * lightAngle * 4)
        {
            lampConeLight.GetComponent<Light2D>().pointLightOuterAngle = i;
            lampConeLight.GetComponent<Light2D>().pointLightInnerAngle = (i >= RADIUS_DIFFERENCE ? i - RADIUS_DIFFERENCE : 0f);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    IEnumerator ShrinkConeLight()
    {
        StopCoroutine("ExpandConeLight");

        for (float i = lightAngle; i > 0f; i -= Time.deltaTime * lightAngle * 8)
        {
            lampConeLight.GetComponent<Light2D>().pointLightOuterAngle = i;
            lampConeLight.GetComponent<Light2D>().pointLightInnerAngle = (i <= RADIUS_DIFFERENCE ? 0f : i - RADIUS_DIFFERENCE);
            yield return new WaitForSeconds(Time.deltaTime);
        }

        lampConeLight.SetActive(false);
    }


    IEnumerator ExpandCircleLight()
    {
        for (float i = 0f; i < LIGHT_CIRCLE_RADIUS; i += Time.deltaTime * LIGHT_CIRCLE_RADIUS * 8)
        {
            lampCircleLight.GetComponent<Light2D>().pointLightOuterRadius = i;
            lampCircleLight.GetComponent<Light2D>().pointLightInnerRadius = i / 2f;
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    IEnumerator ShrinkCircleLight()
    {
        for (float i = LIGHT_CIRCLE_RADIUS; i > 0f; i -= Time.deltaTime * LIGHT_CIRCLE_RADIUS * 8)
        {
            lampCircleLight.GetComponent<Light2D>().pointLightOuterRadius = i;
            lampCircleLight.GetComponent<Light2D>().pointLightInnerRadius = i / 2f;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        lampCircleLight.GetComponent<Light2D>().intensity = LIGHT_INTENSITY_OFF;

        lampCircleLight.SetActive(false);
    }

}
