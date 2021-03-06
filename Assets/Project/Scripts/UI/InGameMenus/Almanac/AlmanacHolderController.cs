using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlmanacHolderController : MonoBehaviour
{
    public AlmanacScriptableObject[] items;
    public Image[] itemImages;

    // Start is called before the first frame update
    void Start()
    {
        SetItemImages();
    }

    private void OnEnable()
    {
        SetItemImages();
    }


    public void SetItemImages()
    {
        int index = 0;
        foreach (AlmanacScriptableObject item in items)
        {
            if (item.hasBeenFound)
            {
                itemImages[index].color = new Color(252, 252, 252);
            }
            else
            {
                itemImages[index].color = new Color(0, 0, 0);
            }
            index++;
        }
    }


}
