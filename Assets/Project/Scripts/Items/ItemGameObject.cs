using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemGameObject : MonoBehaviour
{
    // Private Attributes
    protected Rigidbody2D rigidbody2D;
    public bool permanentNotPickedUp = false;
    public bool canBePickedUp;
    public bool isPickedUpAlready = false;

    private const float DROP_DOWN_FORCE_Y = 1.5f;
    private const float DROP_DOWN_TIME = 0.37f;

    private const float DROP_FORWARD_FORCE_X = 2.0f;
    private const float DROP_FORWARD_FORCE_Y = 2.5f;
    private const float DROP_FORWARD_TIME = 0.55f;

    private const float DESPAWN_TIME_IN_SECONDS = 10.0f;
    public float currentDespawnTimeInSeconds = DESPAWN_TIME_IN_SECONDS;
    private const float START_DESPAWN_FADING_TIME = 3.0f;

    // Public Attributes
    public Item item;

    // Audio
    public AudioSource audioSource;
    public AudioClip itemIsDropped;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        rigidbody2D.gravityScale = 0f;
    }



    public void DropsDown()
    {
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        rigidbody2D.AddForce(transform.up * DROP_DOWN_FORCE_Y, ForceMode2D.Impulse);

        PlayDropSound();

        StartCoroutine("StopDroping", DROP_DOWN_TIME);
    }

    public void DropsForward(int directionX)
    {
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        rigidbody2D.AddForce(new Vector2(directionX * DROP_FORWARD_FORCE_X, DROP_FORWARD_FORCE_Y), ForceMode2D.Impulse);

        PlayDropSound();

        StartCoroutine("StopDroping", DROP_FORWARD_TIME);
    }

    public void DropsRandom(float despawnTime)
    {
        transform.DOJump(new Vector3(transform.position.x + Random.Range(-0.5f, 0.5f), transform.position.y + Random.Range(-0.5f, 0.5f), 0), 0.1f, 1, 0.3f);
        StartDespawning(despawnTime);
    }

    public void DropsRandom()
    {
        transform.DOJump(new Vector3(transform.position.x + Random.Range(-0.5f, 0.5f), transform.position.y + Random.Range(-0.5f, 0.5f), 0), 0.1f, 1, 0.3f);
        StartDespawning(DESPAWN_TIME_IN_SECONDS);
    }

    public void DropsRandom(bool willDespawn, float dropRandomness = 0.5f, float despawnTime = DESPAWN_TIME_IN_SECONDS)
    {
        transform.DOJump(new Vector3(transform.position.x + Random.Range(-dropRandomness, dropRandomness), 
            transform.position.y + Random.Range(-dropRandomness, dropRandomness), 0), 0.1f, 1, 0.3f);
        if (willDespawn) StartDespawning(despawnTime);
    }

    private void PlayDropSound()
    {
        audioSource.clip = itemIsDropped;
        audioSource.Play();
    }

    IEnumerator StopDroping(float secondsToWait)
    {
        yield return new WaitForSeconds(secondsToWait);
        //rigidbody2D.bodyType = RigidbodyType2D.Static;
        rigidbody2D.gravityScale = 0f;
    }

    public void StartDespawning(float despawnTime)
    {
        StartCoroutine(Despawning(despawnTime));
    }

    IEnumerator Despawning(float despawnTime)
    {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Color transparentColor = spriteRenderer.color;
        transparentColor.a = 0.0f;

        Color semiTransparentColor = spriteRenderer.color;
        semiTransparentColor.a = 0.5f;

        currentDespawnTimeInSeconds = despawnTime;
        while (currentDespawnTimeInSeconds > START_DESPAWN_FADING_TIME)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            currentDespawnTimeInSeconds -= Time.deltaTime;
        }

        while (currentDespawnTimeInSeconds > 0.0f)
        {
            spriteRenderer.color = semiTransparentColor;
            yield return new WaitForSeconds(currentDespawnTimeInSeconds / DESPAWN_TIME_IN_SECONDS);

            spriteRenderer.color = transparentColor;
            yield return new WaitForSeconds(currentDespawnTimeInSeconds / DESPAWN_TIME_IN_SECONDS);

            semiTransparentColor.a -= 0.025f;

            currentDespawnTimeInSeconds -= 0.25f;
        }

        Destroy(gameObject);
    }

    public virtual void DoFunctionality()
    {
        // Consumible does functionality
    }

    public void SetSelfStatic()
    {
        rigidbody2D.bodyType = RigidbodyType2D.Static;
        GetComponent<Collider2D>().enabled = false;
    }

    public void SetSelfDynamic()
    {
        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        GetComponent<Collider2D>().enabled = true;
    }



    public void MakeNotPickupableForDuration(float duration = 0.5f)
    {
        StartCoroutine(NotPickupableForDuration(duration));
    }

    IEnumerator NotPickupableForDuration(float duration)
    {
        permanentNotPickedUp = true;
        canBePickedUp = false;
        GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(duration);

        permanentNotPickedUp = false;
        canBePickedUp = true;
        GetComponent<Collider2D>().enabled = true;
    }

}
