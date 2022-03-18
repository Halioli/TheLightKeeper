using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHandler : PlayerBase
{
    // Private Attributes
    private HealthSystem playerHealthSystem;
    private Rigidbody2D playerRigidbody2D;
    private bool inCoroutine = false;

    // Public Attributes
    public Animator animator;
    public HUDHandler hudHandler;

    public bool animationEnds = false;


    public delegate void PlayerHandlerAction();
    public static event PlayerHandlerAction OnPlayerDeath;




    private void Start()
    {
        playerHealthSystem = GetComponent<HealthSystem>();
        playerRigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (playerHealthSystem.IsDead())
        {
            Debug.Log("player is dead");

            //Start corroutine and play animation
            if (!animationEnds)
            {
                playerStates.SetCurrentPlayerState(PlayerState.DEAD);
                gameObject.layer = LayerMask.NameToLayer("Default"); // Enemies layer can't collide with Default layer

                // Send Action
                if (OnPlayerDeath != null) 
                    OnPlayerDeath();

                if (!inCoroutine)
                    StartCoroutine(DeathAnimation());
            }
            else
            {
                // Teleport to starting position (0, 0)
                gameObject.layer = LayerMask.NameToLayer("Player");
                playerRigidbody2D.transform.position = Vector3.zero;
                //playerHealthSystem.RestoreHealthToMaxHealth();
                animationEnds = false;
            }
        }

        if (PlayerInputs.instance.PlayerPressedPauseButton())
        {
            // Pause game
        }
    }

    public void DoFadeToBlack()
    {
        hudHandler.DoFadeToBlack();
    }

    public void RestoreHUD()
    {
        Debug.Log("Restore HUD");
        hudHandler.RestoreFades();
    }

    public void DeathAnimationFinished()
    {
        animationEnds = true;
    }

    IEnumerator DeathAnimation()
    {
        inCoroutine = true;

        PlayerInputs.instance.canMove = false;
        animator.SetBool("isDead", true);
        yield return new WaitForSeconds(2f);
        RestoreHUD();
        animator.SetBool("isDead", false);
        playerStates.SetCurrentPlayerState(PlayerState.FREE);

        PlayerInputs.instance.canMove = true;
        inCoroutine = false;
    }
}