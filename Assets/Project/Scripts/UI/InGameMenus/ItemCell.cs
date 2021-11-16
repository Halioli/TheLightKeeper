using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemCell : MonoBehaviour
{
    public Image itemImage;
    public TextMeshProUGUI itemAmount;
    public Button button;

    public void SetItemImage(Sprite sprite)
    {
        itemImage.sprite = sprite;
    }

    public void SetItemAmount(int amount)
    {
        itemAmount.text = amount.ToString();
    }

    public void ClickedButton()
    {

    }

    public void SetToEmpty()
    {
        //itemImage = empty;
        itemAmount.text = " ";
    }
}
