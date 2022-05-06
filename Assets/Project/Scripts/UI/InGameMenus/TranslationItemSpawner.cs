using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslationItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject translationItem;
    [SerializeField] float randomSpawnOffset = 10.0f;


    public void Spawn(KeyValuePair<Item, int> itemAndAmount, Vector2 startPosition, Vector2 endPosition)
    {
        for (int i = 0; i < itemAndAmount.Value; ++i)
        {
            GameObject spawnedTranslationItem = Instantiate(translationItem, transform);

            startPosition += Random.insideUnitCircle * randomSpawnOffset;
            spawnedTranslationItem.GetComponent<TranslationItem>().Init(itemAndAmount.Key.sprite, startPosition, endPosition);
        }

    }


}
