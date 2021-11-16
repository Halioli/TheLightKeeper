using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ItemType { NULL, MINERAL, CONSUMIBLE }

public abstract class Item : ScriptableObject
{
    // Public Attributes
    public GameObject prefab;
    
    public string itemName;
    [TextArea(5, 20)] public string description;

    public int ID; // item identifier
    public ItemType itemType;
    public int stackSize;


    // Private Attributes
    private Sprite sprite;


    //private void Start()
    //{
    //    sprite = GetComponent<Sprite>();
    //}



    // Getter Methods
    public int GetID() { return ID; }

    public int GetStackSize() { return stackSize; }

    public Sprite GetItemSprite() { return sprite; }


    // Bool Methods 
    public bool SameID(Item other)
    {
        return ID == other.ID;
    }

    public bool DifferentID(Item other)
    {
        return ID != other.ID;
    }


    // Virtual Methods
    public virtual void DoFunctionality() { }
}
