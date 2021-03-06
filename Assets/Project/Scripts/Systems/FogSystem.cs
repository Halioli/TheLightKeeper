using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogSystem : MonoBehaviour
{
    private GameObject player;
    private Animator playerAnimator;

    private bool playerInFog = false;
    private bool hasFaded = false;
    private bool playerDead = false;
    
    [SerializeField] private HUDHandler hudHandler;

    public PlayerLightChecker playerLightChecker;
    private float timer;
    private float deathTimer;
    

    public Vector3 respawnPosition;

    public GameObject skullEnemy;
    [SerializeField] AudioSource fogAreaAudioSource;
    [SerializeField] AudioSource acidAudioSource;

    // Tp player
    public delegate void TeleportPlayerAction(Vector3 landingPos);
    public static event TeleportPlayerAction OnTeleportPlayer;

    public delegate void PlayerCaughtAction();
    public static event PlayerCaughtAction OnPlayerCaughtStart;
    public static event PlayerCaughtAction OnPlayerCaughtEnd;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerAnimator = player.GetComponent<Animator>();
        skullEnemy.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInFog)
        {
            fogAreaAudioSource.Play();
            skullEnemy.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 0);
            player.GetComponentInChildren<Lamp>().lampTime = 0;
            
            if (!playerLightChecker.IsPlayerInLight() || playerDead)
            {
                timer -= Time.deltaTime;
                if (timer > 0f)
                {
                    //Debug.Log(timer);
                    skullEnemy.SetActive(true);
                    PlayerInputs.instance.canMove = false;
                    acidAudioSource.Play();
                }
                else
                {
                    playerAnimator.SetBool("isDeadByAcid", true);
                    playerDead = true;
                    deathTimer -= Time.deltaTime;
                    if(deathTimer < 0f)
                    {
                        if (!hasFaded)
                        {
                            StartCoroutine(RespawnFade());
                        }
                    }     
                }
            }
            else
            {
                PlayerInputs.instance.canMove = true;
                ResetTimer();
                skullEnemy.SetActive(false);
            }
        }
        else if (!playerInFog && fogAreaAudioSource.isPlaying)
        {
            fogAreaAudioSource.Stop();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInFog = true;
            //Debug.Log("PlayerIn");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInFog = false;
            //Debug.Log("PlayerOut");
            ResetTimer();
            skullEnemy.SetActive(false);
        }
    }
    private void ResetTimer()
    {
        timer = 1f;
        deathTimer = 2.1f;
        playerAnimator.SetBool("isDeadByAcid", false);
        playerDead = false;
    }

    IEnumerator RespawnFade()
    {
        Debug.Log(playerDead);
        playerAnimator.SetBool("isDeadByAcid", false);
        hasFaded = true;
        hudHandler.DoFadeToBlack();

        if (OnPlayerCaughtStart != null)
            OnPlayerCaughtStart();

        //PlayerInputs.instance.canMove = false;
        skullEnemy.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        if (OnTeleportPlayer != null)
            OnTeleportPlayer(respawnPosition);

        yield return new WaitForSeconds(1.5f);
        hudHandler.RestoreFades();

        if (OnPlayerCaughtEnd != null)
            OnPlayerCaughtEnd();

        PlayerInputs.instance.canMove = true;
        hasFaded = false;
        ResetTimer();
    }
}
