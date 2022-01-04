using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuEnemy : HostileEnemy
{
    private const float MIN_X_TARGET = -20f;
    private const float MAX_X_TARGET = 20f;
    private const float MIN_Y_TARGET = -20f;
    private const float MAX_Y_TARGET = -30f;

    private const float MIN_X_SPAWN = -20f;
    private const float MAX_X_SPAWN = 20f;
    private const float MIN_Y_SPAWN = 20f;
    private const float MAX_Y_SPAWN = 30f;

    private Vector2 targetArea = new Vector2(10, 10);
    private Vector2 targetPosition;

    private float currentSpeed;
    private float minSpeed = 4f;
    private float maxSpeed = 8f;
    private float speedVariationInterval = 2f;
    private float waitTime = 0.5f;
    private bool doSpeedVariation = true;

    // Sinusoidal movement
    public float amplitude = 0.1f;
    private float period;
    private float theta;
    public float sinWaveDistance;


    private void Awake()
    {
        attackSystem = GetComponent<AttackSystem>();
        healthSystem = GetComponent<HealthSystem>();
        rigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = null;
        collider = GetComponent<CapsuleCollider2D>();

        enemyState = EnemyState.SPAWNING;
        attackState = AttackState.MOVING_TOWARDS_PLAYER;

        currentBanishTime = BANISH_TIME;

        period = Random.Range(0.10f, 0.15f);
        collider = GetComponent<CapsuleCollider2D>();

        SetRandomTargetPosition();
        Spawn();
        StartCoroutine(SpeedVariation());
    }

    private void Update()
    {
        if (enemyState == EnemyState.AGGRO)
        {
            // Sinusoidal movement
            theta = Time.timeSinceLevelLoad / period;
            sinWaveDistance = amplitude * Mathf.Sin(theta);

            if (transform.position.Equals(targetPosition))
            {
                RandomRespawn();
            }

            if (collider.IsTouchingLayers(LayerMask.NameToLayer("Light")))
            {
                RandomRespawn();
            }

            if (doSpeedVariation)
            {
                StartCoroutine(SpeedVariation());
            }
        }

        Debug.Log(targetPosition);
    }

    private void FixedUpdate()
    {
        if (enemyState == EnemyState.AGGRO)
        {
            MoveTowardsTarget();
        }
    }

    private void MoveTowardsTarget()
    {
        directionTowardsPlayerPosition = targetPosition - (Vector2)transform.position;
        rigidbody.MovePosition((Vector2)transform.position + (Vector2.up * sinWaveDistance) + directionTowardsPlayerPosition * (currentSpeed * Time.deltaTime));
    }

    private void SetRandomTargetPosition()
    {
        targetPosition = new Vector2(Random.Range(MIN_X_TARGET, MAX_X_TARGET), Random.Range(MAX_Y_TARGET, MIN_Y_TARGET));
    }

    private void RandomRespawn()
    {
        transform.position = new Vector2(Random.Range(MIN_X_SPAWN, MAX_X_SPAWN), Random.Range(MIN_Y_SPAWN, MAX_Y_SPAWN));
        ResetStates();
        SetRandomTargetPosition();
    }

    private void ResetStates()
    {
        enemyState = EnemyState.SPAWNING;
        attackState = AttackState.MOVING_TOWARDS_PLAYER;

        currentBanishTime = BANISH_TIME;
    }

    IEnumerator SpeedVariation()
    {
        doSpeedVariation = false;
        currentSpeed = 0f;
        yield return new WaitForSeconds(waitTime);

        currentSpeed = Random.Range(minSpeed, maxSpeed);
        targetPosition = Random.insideUnitCircle * targetArea;
        yield return new WaitForSeconds(speedVariationInterval);
        doSpeedVariation = true;
    }
}
