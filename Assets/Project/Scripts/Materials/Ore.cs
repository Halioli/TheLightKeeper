using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;

public enum OreState { WHOLE, BROKEN};

public class Ore : MonoBehaviour
{
    // Private Attributes
    protected OreState breakState;
    protected HealthSystem healthSystem;
    protected int currentSpriteIndex;
    protected Sprite currentSprite;

    // Public Attributes
    public List<Sprite> spriteList;
    public ItemGameObject mineralItemToDrop;
    public ParticleSystem[] oreParticleSystem;
    public Animator animator;

    private void Start()
    {
        breakState = OreState.WHOLE;

        currentSpriteIndex = 0;
        currentSprite = spriteList[currentSpriteIndex];

        healthSystem = GetComponent<HealthSystem>();
        foreach (ParticleSystem particleSystem in oreParticleSystem)
        {
            particleSystem.Stop();
        }
    }



    public bool CanBeMined() { return breakState == OreState.WHOLE; }

    public bool Broke() { return healthSystem.IsDead(); }

    public virtual void GetsMined(int damageAmount)
    {
        
        transform.DOPunchScale(new Vector3(-0.6f, -0.6f, 0), 0.40f);
        // Damage the Ore
        healthSystem.ReceiveDamage(damageAmount);
        // Update ore Sprite
        ProgressNAmountOfSprites(damageAmount);

        if (healthSystem.IsDead())
        {
            breakState = OreState.BROKEN;

            // Drop mineralItemToDrop
            DropMineralItem();

            // Start disappear coroutine
            StartCoroutine("Disappear");
        }
        UpdateCurrentSprite();
        StartCoroutine("PlayBreakParticles");

    }

    protected void ProgressNAmountOfSprites(int numberOfProgressions)
    {
        if (currentSpriteIndex + numberOfProgressions >= spriteList.Count)
        {
            currentSpriteIndex = spriteList.Count - 1;
        }
        else
        {
            currentSpriteIndex += numberOfProgressions;
        }

        currentSprite = spriteList[currentSpriteIndex];
        animator.SetInteger("actualState", currentSpriteIndex);
    }

    protected void DropMineralItem()
    {
        ItemGameObject droppedMineralItem = Instantiate(mineralItemToDrop, GetDropSpawnPosition(), Quaternion.identity);
        droppedMineralItem.transform.DOJump(new Vector3(transform.position.x + Random.Range(-0.5f,0.5f),transform.position.y + Random.Range(-0.5f, 0.5f),0),0.1f,1,0.3f);
        //droppedMineralItem.transform.DOPunchPosition(new Vector3(Random.Range(-0.3f,0.3f), Random.Range(0.4f, 0.6f), 0), 0.3f);
        //droppedMineralItem.DropsDown();
        droppedMineralItem.StartDespawning();
    }

    protected Vector2 GetDropSpawnPosition()
    {
        return new Vector2(transform.position.x + 0.1f, transform.position.y);
    }

    protected void UpdateCurrentSprite()
    {
        GetComponent<SpriteRenderer>().sprite = currentSprite;
    }

    IEnumerator Disappear()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        Color transparentColor = spriteRenderer.material.color;
        transparentColor.a = 0.0f;

        Color semiTransparentColor = spriteRenderer.material.color;
        semiTransparentColor.a = 0.5f;
        
        spriteRenderer.material.color = semiTransparentColor;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.material.color = transparentColor;
        yield return new WaitForSeconds(0.2f);

        Destroy(gameObject);
    }

    IEnumerator PlayBreakParticles()
    {
        foreach (ParticleSystem particleSystem in oreParticleSystem)
        {
            particleSystem.Play();
        }
        yield return new WaitForSeconds(0.3f);
        foreach (ParticleSystem particleSystem in oreParticleSystem)
        {
            particleSystem.Stop();
        }
    }
}
