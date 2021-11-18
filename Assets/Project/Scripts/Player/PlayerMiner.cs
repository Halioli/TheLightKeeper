using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum CriticalMiningState { NONE, FAILED, SUCCEESSFUL };

public class PlayerMiner : PlayerInputs
{
    // Private Attributes
    private Collider2D colliderDetectedByMouse = null;
    Ore oreToMine;

    private float miningReachRadius = 3f;

    private const int START_MINING_DAMAGE = 1;
    private int miningDamage = START_MINING_DAMAGE;
    private const int START_CRITICAL_MINING_DAMAGE = 2;
    private int criticalMiningDamage = START_CRITICAL_MINING_DAMAGE;

    private bool isMining = false;
    CriticalMiningState criticalMiningState = CriticalMiningState.NONE;
    private const float START_MINING_TIME = 1.0f;
    private float miningTime = START_MINING_TIME;
    private const float LOWER_INTERVAL_CRITICAL_MINING = 0.5f;
    private const float UPPER_INTERVAL_CRITICAL_MINING = 0.7f;


    void Update()
    {
        if (PlayerClickedMineButton() && !isMining)
        {
            SetNewMousePosition();
            if (PlayerIsInReachToMine(mouseWorldPosition) && MouseClickedOnAnOre(mouseWorldPosition))
            {
                SetOreToMine();
                StartMining();
            }
        }
        
    }




    // METHODS

    public bool IsMining() { return isMining; }

    private bool PlayerIsInReachToMine(Vector2 mousePosition)
    {
        float distancePlayerMouseClick = Vector2.Distance(mousePosition, transform.position);
        return distancePlayerMouseClick <= miningReachRadius;
    }

    private bool MouseClickedOnAnOre(Vector2 mousePosition)
    {
        colliderDetectedByMouse = Physics2D.OverlapCircle(mousePosition, 0.05f);
        return colliderDetectedByMouse != null && colliderDetectedByMouse.gameObject.CompareTag("Ore");
    }

    private void SetOreToMine()
    {
        oreToMine = colliderDetectedByMouse.gameObject.GetComponent<Ore>();
    }


    private void CheckCriticalMining()
    {
        if (PlayerClickedMineButton())
        {
            if (WithinCriticalInterval())
            {
                criticalMiningState = CriticalMiningState.SUCCEESSFUL;
            }
            else
            {
                criticalMiningState = CriticalMiningState.FAILED;
            }
        }
    }

    private bool WithinCriticalInterval()
    {
        return miningTime >= LOWER_INTERVAL_CRITICAL_MINING && miningTime <= UPPER_INTERVAL_CRITICAL_MINING;
    }


    private void StartMining()
    {
        isMining = true;
        StartCoroutine("Mining");
    }

    private void MineOre(int damageToDeal)
    {
        if (oreToMine.CanBeMined())
            oreToMine.GetsMined(damageToDeal);
    }

    private void ResetMining()
    {
        miningTime = START_MINING_TIME;
        criticalMiningState = CriticalMiningState.NONE;
        isMining = false;
    }

    IEnumerator Mining()
    {
        while (miningTime > 0.0f)
        {
            CheckCriticalMining();

            yield return new WaitForSeconds(Time.deltaTime);
            miningTime -= Time.deltaTime;

        }

        if (criticalMiningState == CriticalMiningState.SUCCEESSFUL)
        {
            Debug.Log("CRITICAL MINING");
            MineOre(criticalMiningDamage);
        }
        else
        {
            Debug.Log("MINING");
            MineOre(miningDamage);
        }

        ResetMining();
    }



}