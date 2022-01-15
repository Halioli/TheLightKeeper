using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class Torch : InteractStation
{
    public ParticleSystem smokeTorchParticles;
    public Light2D torchLight;
    public CanvasGroup popUpCanvasGroup;
    public CircleCollider2D lightRadius;
    public GameObject linkedRune;
    public GameObject desactivatedTorch;

    public float torchIntensity = 1f;
    public float turnedOffIntensity = 0f;
    public bool hasToBurn;

    private const float innerRadiusOn = 1.8f;
    private const float outerRadiusOn = 3.3f;
    public bool turnedOn = false;

    public Animator animTorch;

    public TorchPuzzleSystem puzzleSystem;

    // Start is called before the first frame update
    void Start()
    {
        smokeTorchParticles.Stop();
        if (!turnedOn)
        {
            torchLight.intensity = turnedOffIntensity;
        }

        popUpCanvasGroup.alpha = 0f;

        if(hasToBurn == false)
        {
            DoPuzzle();
        }
        PuzzleChecker();
        linkedRune.SetActive(false);
    }

    private void Update()
    {
        if (playerInsideTriggerArea)
        {
            popUpCanvasGroup.alpha = 1f;
            GetInput();
        }
        else
        {
            popUpCanvasGroup.alpha = 0f;
        }
    }
    public override void StationFunction()
    {
        if (!turnedOn)
        {
            SetTorchLightOn();
        }
        else
        {
            SetTorchLightOff();
        }
        DoPuzzle();
        if (PuzzleChecker())
        {
            //Debug.Log("Puzzle Completed");
            puzzleSystem.animator.SetBool("isCompleted", true);

        }

    }
    void SetTorchLightOff()
    {
        smokeTorchParticles.Stop();
        torchLight.intensity = 1f;
        StartCoroutine(LightsOff());
        turnedOn = false;
        lightRadius.radius = 0.1f;
        animTorch.SetBool("isBurning", false);
        if (hasToBurn)
        {
            linkedRune.SetActive(false);
        }
    }

    void SetTorchLightOn()
    {
        smokeTorchParticles.Play();
        torchLight.intensity = 1f;
        StartCoroutine(LightsOn());
        turnedOn = true;
        lightRadius.radius = 2.8f;
        animTorch.SetBool("isBurning", true);
        if (hasToBurn)
        {
            linkedRune.SetActive(true);
        }
    }

    IEnumerator LightsOn()
    {
        Interpolator lightLerp = new Interpolator(0.2f, Interpolator.Type.SIN);
        lightLerp.ToMax();

        while (!lightLerp.isMaxPrecise)
        {
            lightLerp.Update(Time.deltaTime);
            //DO STUFF HERE
            torchLight.pointLightOuterRadius = lightLerp.Value * outerRadiusOn;
            torchLight.pointLightInnerRadius = lightLerp.Value * innerRadiusOn;
            //WAIT A FRAME
            yield return null;
        }
    }

    IEnumerator LightsOff()
    {
        Interpolator lightLerp = new Interpolator(0.2f, Interpolator.Type.SIN);
        lightLerp.ToMax();

        while (!lightLerp.isMaxPrecise)
        {
            lightLerp.Update(Time.deltaTime);
            //DO STUFF HERE
            torchLight.pointLightOuterRadius = lightLerp.Inverse * outerRadiusOn;
            torchLight.pointLightInnerRadius = lightLerp.Inverse * innerRadiusOn;
            //WAIT A FRAME
            yield return null;
        }
    }

    public void DoPuzzle()
    {
        if (turnedOn && hasToBurn)
        {
            puzzleSystem.torchesOn += 1;
        }
        else if (!turnedOn && hasToBurn)
        {
            puzzleSystem.torchesOn -= 1;
        }
        else if (turnedOn && !hasToBurn)
        {
            puzzleSystem.torchesOff -= 1;
        }
        else if (!turnedOn && !hasToBurn)
        {
            puzzleSystem.torchesOff += 1;
        }

    }

    private bool PuzzleChecker()
    {
        //Debug.Log("TORCHES ON: " + puzzleSystem.torchesOn);
        //Debug.Log("TORCHES OFF:" + puzzleSystem.torchesOff);
        if (puzzleSystem.torchesOn == puzzleSystem.maxTorchesOn && puzzleSystem.torchesOff == puzzleSystem.maxTorchesOff)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DesactivateTorchSprite()
    {
        desactivatedTorch.SetActive(false);
    }
    private void ActivateTorchSprite()
    {
        desactivatedTorch.SetActive(true);
    }
}
