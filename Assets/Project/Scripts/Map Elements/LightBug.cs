using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


public enum LightBugMovement { LINEAR, CIRCLE };

public class LightBug : Enemy
{

    public static LightBug instance;

    Interpolator horizontalLerp;
    Interpolator verticalLerp;


    private Light2D[] pointLightBug;
    private bool isTurnedOn = false;

    public LightBugMovement lightBugMovement;

    //Linear Movement Parameters
    public float timeToReachEachPoint;
    public float initialPositionX;
    public float finalPositionX;
    public float initialPositionY;
    public float finalPositionY;

    //Circular Movement Parameters
    public float speed;
    public float width;
    public float height;

    private float timeCounter;
    private Vector3 centerPosition;

    private float initialIntensity = 0.3f;
    private float maxIntensity = 1f;
    private float time;
    private bool cycleFinished;

    void Start()
    {
        horizontalLerp = new Interpolator(timeToReachEachPoint, Interpolator.Type.SMOOTH);
        verticalLerp = new Interpolator(0.5f, Interpolator.Type.SMOOTH);

        pointLightBug = GetComponentsInChildren<Light2D>();
        healthSystem = GetComponent<BeingHealthSystem>();
        attackSystem = GetComponent<AttackSystem>();
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        centerPosition = transform.position;
        FlipSprite();

    }

    void Update()
    {
        DoLightBugMovement();

        if (healthSystem.IsDead())
        {
            Die();
        }
    }

    private void DoLightBugMovement()
    {
        if(lightBugMovement == LightBugMovement.LINEAR)
        {
            UpdateInterpolators();

            transform.position = new Vector3(initialPositionX + (finalPositionX - initialPositionX) * horizontalLerp.Value, initialPositionY + (finalPositionY - initialPositionY) * horizontalLerp.Value, 0f);
            if ((initialPositionX - finalPositionX != 0) && (initialPositionY == 0) && (finalPositionY == 0))
            {
                transform.position = new Vector3(transform.position.x, (transform.position.y + 1f) - (transform.position.y + 1f) * verticalLerp.Value, 0f); 
            }
            else if((initialPositionY - finalPositionY != 0) && (initialPositionX == 0) && (finalPositionX == 0))
            {
                transform.position = new Vector3((transform.position.x + 1f) - (transform.position.x + 1f) * verticalLerp.Value, transform.position.y, 0f);
            }
        }
        else
        {
            timeCounter += Time.deltaTime * speed;
            transform.position = new Vector3(Mathf.Cos(timeCounter) * width, Mathf.Sin(timeCounter) * height, 0) + centerPosition;    
        }

    }

    protected override void Die()
    {
        base.Die();
        Destroy(gameObject);
    }

    private void UpdateInterpolators()
    {
        horizontalLerp.Update(Time.deltaTime);
        if (horizontalLerp.isMinPrecise)
        {
            horizontalLerp.ToMax();
            FlipSprite();

        }

        else if (horizontalLerp.isMaxPrecise) 
        {
            horizontalLerp.ToMin();
            FlipSprite();
        }


        verticalLerp.Update(Time.deltaTime);
        if (verticalLerp.isMinPrecise)
        {
            verticalLerp.ToMax();
        }
        else if (verticalLerp.isMaxPrecise)
        {
            verticalLerp.ToMin();
        }
    }

    public void FlipSprite()
    {
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }

}

