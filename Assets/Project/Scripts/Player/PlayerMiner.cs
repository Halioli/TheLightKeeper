using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

enum CriticalMiningState { NONE, FAILED, SUCCEESSFUL };

public class PlayerMiner : PlayerBase
{
    // Private Attributes
    private const float OVERLAP_CIRCLE_RADIUS = 1.5f;

    private Ore oreToMine;
    private CriticalMiningState criticalMiningState = CriticalMiningState.NONE;
    private const float MINING_TIME = 0.35f;
    private float miningTime = 0;

    private bool canCriticalMine = false;
    private bool miningAnOre = false;
    private Vector2 raycastStartingPosition;
    private Vector2 raycastEndingPosition;

    // Overlap Circle & Dot Product
    private Vector2 overlapCirclePosition;
    private Vector2 mouseDirection;
    private Vector2 oreDirection;
    private Collider2D[] collidedElements;
    private Collider2D maxColl;
    private float max = -2f;
    private float dotRes;

    [SerializeField] Pickaxe pickaxe;

    // Public Attributes
    //public GameObject interactArea;
    public LayerMask defaultLayerMask;
    public static Collider2D OverlapCircle;
    public Animator animator;

    // Events
    public delegate void PlayPlayerSound();
    public static event PlayPlayerSound playerMinesEvent;
    public static event PlayPlayerSound pickaxeNotStrongEnoughEvent;
    public static event PlayPlayerSound playerMinesNothingEvent;

    private void Start()
    {
        animator = GetComponent<Animator>();

        // SHITY WORKAROUND
        //animator.SetBool("isMining", true);
    }

    void Update()
    {
        MineTargetCheck();

        if (PlayerInputs.instance.PlayerClickedMineButton() && playerStates.PlayerStateIsFree() && !playerStates.PlayerActionIsMining())
        {
            if (miningAnOre)
                SetOreToMine(maxColl.GetComponent<Ore>());
            else
            {
                //if (playerMinesNothingEvent != null) playerMinesNothingEvent();
            }


            StartMining();
        }
    }

    private void MineTargetCheck()
    {
        PlayerInputs.instance.SetNewMousePosition();

        // Update & check all colliders
        UpdateOverlapCirlcePositionAndMouseDirection();
        collidedElements = ReturnAllOverlapedColliders();

        // Get the dot product from every collider in reach
        miningAnOre = false;
        maxColl = null;
        max = -2f;
        dotRes = max;
        for (int i = 0; i < collidedElements.Length; ++i)
        {
            if (collidedElements[i].CompareTag("Ore"))
            {
                oreDirection = (transform.position - collidedElements[i].transform.position).normalized;
                dotRes = Vector2.Dot(mouseDirection, oreDirection);

                if (dotRes > max)
                {
                    max = dotRes;
                    maxColl = collidedElements[i];
                }

                miningAnOre = true;

            }
        }


        if (maxColl != null && miningAnOre)
        {

            maxColl.GetComponentInChildren<SelectSpot>().DoSelect();

        }
        else if (maxColl != null)
        {
            maxColl.GetComponentInChildren<SelectSpot>().StopSelect();
        }
    }



    // METHODS
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(raycastStartingPosition, raycastEndingPosition * 3f);
    }

    private void SetOreToMine(Ore ore)
    {
        oreToMine = ore;

        //PlayerInputs.instance.SpawnSelectSpotAtTransform(oreToMine.transform);
    }

    private void CheckCriticalMining()
    {
        /*if (PlayerInputs.instance.PlayerClickedMineButton())
        {
            if (canCriticalMine)
            {
                criticalMiningState = CriticalMiningState.SUCCEESSFUL;
                playerSucceessfulMineEvent();
                animator.SetBool("isPerfect", true);
            }
            else
            {
                criticalMiningState = CriticalMiningState.FAILED;
                playerFailMineEvent();
            }
        }*/
    }

    private void StartMining()
    {
        //playerMinesEvent();

        FlipPlayerSpriteFacingOreToMine();
        playerStates.SetCurrentPlayerState(PlayerState.BUSSY); 
        playerStates.SetCurrentPlayerAction(PlayerAction.MINING);
        criticalMiningState = CriticalMiningState.NONE;

        PlayerInputs.instance.canMove = false;
        animator.SetBool("isMining", true);
    }

    private void MineOre(int damageToDeal)
    {
        if (oreToMine.CanBeMined())
        {
            if (oreToMine.hardness <= pickaxe.hardness)
            {
                oreToMine.GetsMined(damageToDeal, pickaxe.extraDrop);
            }
            else
            {
                if (pickaxeNotStrongEnoughEvent != null) pickaxeNotStrongEnoughEvent();
            }
        }
    }

    private void ResetMining()
    {
        miningTime = 0;
        criticalMiningState = CriticalMiningState.NONE;

        playerStates.SetCurrentPlayerState(PlayerState.FREE);
        playerStates.SetCurrentPlayerAction(PlayerAction.IDLE);

        animator.SetBool("isMining", false);

        Array.Clear(collidedElements, 0, collidedElements.Length);
        miningAnOre = false;
        maxColl = null;
        max = -2f;
        dotRes = max;

        PlayerInputs.instance.canMove = true;
    }

    private void Mine()
    {
        if (!miningAnOre || oreToMine == null)
        {
            if (playerMinesNothingEvent != null) playerMinesNothingEvent();
            return;
        }


        MineOre(pickaxe.damageValue);
    }

    private void UpdateOverlapCirlcePositionAndMouseDirection()
    {
        overlapCirclePosition = transform.position;
        overlapCirclePosition.y -= 1;

        mouseDirection = ((Vector2)transform.position - PlayerInputs.instance.GetMousePositionInWorld()).normalized;
    }

    private Collider2D[] ReturnAllOverlapedColliders()
    {
        if (collidedElements != null && collidedElements.Length != 0)
            Array.Clear(collidedElements, 0, collidedElements.Length);

        return Physics2D.OverlapCircleAll(overlapCirclePosition, OVERLAP_CIRCLE_RADIUS, defaultLayerMask);
    }

    public void StartCriticalInterval()
    {
        canCriticalMine = true;

    }

    public void FinishCriticalInterval()
    {
        canCriticalMine = false;
    }

    private void FlipPlayerSpriteFacingOreToMine()
    {
        Vector2 targetPosition = Vector2.zero;

        if (miningAnOre)
            targetPosition = oreToMine.transform.position;
        else
            targetPosition = PlayerInputs.instance.GetMousePositionInWorld();

        if ((transform.position.x < targetPosition.x && !PlayerInputs.instance.facingLeft) ||
            (transform.position.x > targetPosition.x && PlayerInputs.instance.facingLeft))
        {
            Vector2 direction = targetPosition - (Vector2)transform.position;
            PlayerInputs.instance.FlipSprite(direction);
        }
    }


}