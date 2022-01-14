using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogSystem : MonoBehaviour
{
    public PlayerLightChecker playerLightChecker;
    private GameObject player;

    private bool playerInFog = false;
    private float timer;

    private Vector3 respawnPosition;

    public GameObject skullEnemy;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log(player.tag);
        timer = 1f;
        respawnPosition = new Vector3(30f, -11f, 0);
        skullEnemy.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInFog)
        {
            skullEnemy.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, 0);
            if (!playerLightChecker.IsPlayerInLight())
            {

                if(timer > 0f)
                {
                    timer -= Time.deltaTime;
                    Debug.Log(timer);
                    skullEnemy.SetActive(true);
                }
                else
                {
                    ResetTimer();
                    player.transform.position = respawnPosition;
                    skullEnemy.SetActive(false);
                }
            }
            else
            {
                ResetTimer();
                skullEnemy.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerInFog = true;
            Debug.Log("PlayerIn");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            
            playerInFog = false;
            Debug.Log("PlayerOut");
            ResetTimer();
            skullEnemy.SetActive(false);
        }
    }
    private void ResetTimer()
    {
        timer = 1f;
    }
}