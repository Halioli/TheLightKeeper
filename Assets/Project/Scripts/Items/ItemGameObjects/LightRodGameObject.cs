using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class LightRodGameObject : ItemGameObject
{
    public AudioClip lightRodUseSound;
    public GameObject light;

    public override void DoFunctionality()
    {
        canBePickedUp = false;
        StartCoroutine("Functionality");
    }

    private void FunctionalitySound()
    {
        audioSource.clip = lightRodUseSound;
        audioSource.pitch = Random.Range(0.8f, 1.3f);
        audioSource.Play();
    }

    IEnumerator Functionality()
    {
        FunctionalitySound();

        rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        rigidbody2D.AddForce(new Vector2(0, -3), ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.4f);
        rigidbody2D.bodyType = RigidbodyType2D.Static;


        GameObject spawnedLight = Instantiate(light, transform);

        float lightTime = 3f;

        while (lightTime > 0f)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            lightTime -= Time.deltaTime;

            if (lightTime <= 1f)
            {
                spawnedLight.GetComponent<Light2D>().intensity -= Time.deltaTime;
            }
        }

        Destroy(spawnedLight);
        Destroy(gameObject);
    }
}
