using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUDHandler : MonoBehaviour
{
    // Private Attributes
    private const float DEATH_FADE_TIME = 1f;
    private const float DAMAGE_FADE_TIME = 0.2f;
    private const float FADE_TIME = 0.5f;

    public CanvasGroup endGameMessageGroup;
    public CanvasGroup deathImageGroup;
    public CanvasGroup fadeOutGroup;
    public CanvasGroup recieveDamageGroup;

    private void OnEnable()
    {
        LiftOffButton.OnLiftOff += ShowEndGameMessage;
        LoadBaseScenes.OnKeepBlackFade += KeepBlackFade;
        LoadBaseScenes.OnFadeToNormal += DoFadeToNormal;
        PlayerHandler.OnPlayerDeath += DoDeathImageFade;
        PlayerHandler.OnRestoreFades += RestoreFades;
        PlayerCombat.OnReceivesDamage += ShowReceiveDamageFades;

        Torch.OnTorchPreStartActivation += FadeOutThenInSequence;
        Torch.OnTorchPreEndActivation += FadeOutThenInSequence;

        DarknessFaint.OnFaintEnd += DoFadeToBlack;
        DarknessFaint.OnFaintEndRespawn += DoFadeToNormal;
    }

    private void OnDisable()
    {
        LiftOffButton.OnLiftOff -= ShowEndGameMessage;
        LoadBaseScenes.OnKeepBlackFade -= KeepBlackFade;
        LoadBaseScenes.OnFadeToNormal -= DoFadeToNormal;
        PlayerHandler.OnPlayerDeath -= DoDeathImageFade;
        PlayerHandler.OnRestoreFades -= RestoreFades;
        PlayerCombat.OnReceivesDamage -= ShowReceiveDamageFades;

        Torch.OnTorchPreStartActivation -= FadeOutThenInSequence;
        Torch.OnTorchPreEndActivation -= FadeOutThenInSequence;

        DarknessFaint.OnFaintEnd -= DoFadeToBlack;
        DarknessFaint.OnFaintEndRespawn -= DoFadeToNormal;
    }

    private void KeepBlackFade()
    {
        fadeOutGroup.alpha = 1f;
    }

    private void ShowEndGameMessage()
    {
        StartCoroutine("EndGameFadeAndLogic");
    }

    public void DoFadeToNormal()
    {
        StartCoroutine(CanvasFadeOut(fadeOutGroup, FADE_TIME));
    }

    public void DoDeathImageFade()
    {
        if (!deathImageGroup.gameObject.activeInHierarchy)
        {
            deathImageGroup.gameObject.SetActive(true);
        }

        StartCoroutine(CanvasFadeIn(deathImageGroup, DEATH_FADE_TIME));
    }

    public void DoFadeToBlack()
    {
        StartCoroutine(CanvasFadeIn(fadeOutGroup, FADE_TIME));
    }

    public void ShowReceiveDamageFades()
    {
        StartCoroutine(RecieveDamageFadeInAndOut());
    }

    public void RestoreFades()
    {
        StopCoroutine(CanvasFadeIn(deathImageGroup, DEATH_FADE_TIME));
        StopCoroutine(CanvasFadeIn(fadeOutGroup, FADE_TIME));

        DoFadeToNormal();

        deathImageGroup.gameObject.SetActive(false);
        //deathImageGroup.alpha = 0f;
    }

    private void FadeOutThenInSequence(float duration)
    {
        StartCoroutine(CanvasFadeOutThenIn(fadeOutGroup, duration/2f));
    }
    private void FadeInThenOutSequence(float duration)
    {
        StartCoroutine(CanvasFadeInThenOut(fadeOutGroup, duration / 2f));
    }
    

    IEnumerator CanvasFadeOut(CanvasGroup canvasGroup, float fadeTime)
    {
        Vector2 startVector = new Vector2(1f, 1f);
        Vector2 endVector = new Vector2(0f, 0f);

        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeTime;

            canvasGroup.alpha = Vector2.Lerp(startVector, endVector, normalizedTime).x;
            yield return null;
        }
        canvasGroup.alpha = endVector.x;
    }

    IEnumerator CanvasFadeIn(CanvasGroup canvasGroup, float fadeTime)
    {
        Vector2 startVector = new Vector2(0f, 0f);
        Vector2 endVector = new Vector2(1f, 1f);

        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeTime;

            canvasGroup.alpha = Vector2.Lerp(startVector, endVector, normalizedTime).x;
            yield return null;
        }
        canvasGroup.alpha = endVector.x;
    }

    IEnumerator CanvasFadeOutThenIn(CanvasGroup canvasGroup, float fadeTime)
    {
        Vector2 startVector = new Vector2(0f, 0f);
        Vector2 endVector = new Vector2(1f, 1f);

        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeTime;

            canvasGroup.alpha = Vector2.Lerp(startVector, endVector, normalizedTime).x;
            yield return null;
        }
        canvasGroup.alpha = 1f;


        yield return new WaitForSeconds(fadeTime);


        startVector = new Vector2(1f, 1f);
        endVector = new Vector2(0f, 0f);

        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeTime;

            canvasGroup.alpha = Vector2.Lerp(startVector, endVector, normalizedTime).x;
            yield return null;
        }
        canvasGroup.alpha = endVector.x;
    }

    IEnumerator CanvasFadeInThenOut(CanvasGroup canvasGroup, float fadeTime)
    {

        Vector2 startVector = new Vector2(1f, 1f);
        Vector2 endVector = new Vector2(0f, 0f);

        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeTime;

            canvasGroup.alpha = Vector2.Lerp(startVector, endVector, normalizedTime).x;
            yield return null;
        }
        canvasGroup.alpha = endVector.x;

        yield return fadeTime;

        startVector = new Vector2(0f, 0f);
        endVector = new Vector2(1f, 1f);

        for (float t = 0f; t < fadeTime; t += Time.deltaTime)
        {
            float normalizedTime = t / fadeTime;

            canvasGroup.alpha = Vector2.Lerp(startVector, endVector, normalizedTime).x;
            yield return null;
        }
        canvasGroup.alpha = endVector.x;
    }




    IEnumerator RecieveDamageFadeInAndOut()
    {
        Vector2 fadeInStartVector = new Vector2(0f, 0f);
        Vector2 fadeInEndVector = new Vector2(1f, 1f);

        // Fade in
        for (float t = 0f; t < DAMAGE_FADE_TIME; t += Time.deltaTime)
        {
            float normalizedTime = t / DAMAGE_FADE_TIME;

            recieveDamageGroup.alpha = Vector2.Lerp(fadeInStartVector, fadeInEndVector, normalizedTime).x;
            yield return null;
        }
        recieveDamageGroup.alpha = fadeInEndVector.x;

        // Fade out
        for (float t = 0f; t < DAMAGE_FADE_TIME; t += Time.deltaTime)
        {
            float normalizedTime = t / DAMAGE_FADE_TIME;

            recieveDamageGroup.alpha = Vector2.Lerp(fadeInEndVector, fadeInStartVector, normalizedTime).x;
            yield return null;
        }
        recieveDamageGroup.alpha = fadeInStartVector.x;
    }

    IEnumerator EndGameFadeAndLogic()
    {
        Vector2 startVector = new Vector2(0f, 0f);
        Vector2 endVector = new Vector2(1f, 1f);

        for (float t = 0f; t < FADE_TIME; t += Time.deltaTime)
        {
            float normalizedTime = t / FADE_TIME;

            endGameMessageGroup.alpha = Vector2.Lerp(startVector, endVector, normalizedTime).x;
            yield return null;
        }
        endGameMessageGroup.alpha = endVector.x;

        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
    }
}
