using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // Private Attributes
    private bool inventoryIsOpen = false;
    private Collider2D itemCollectionCollider;

    // Public Attributes
    public Inventory inventory { get; private set; }
    public Canvas inventoryCanvas;
    [SerializeField] GameObject inventoryMenuGameObject;

    // Events
    public delegate void PlayPlayerSound();
    public static event PlayPlayerSound playerPicksUpItemEvent;

    public delegate void InventoryAction();
    public static event InventoryAction OnInventoryOpen;
    public static event InventoryAction OnInventoryClose;


    private void Start()
    {
        inventory = GetComponentInChildren<Inventory>();
        itemCollectionCollider = GetComponent<CapsuleCollider2D>();

        inventoryMenuGameObject.SetActive(inventoryIsOpen);
    }

    void Update()
    {
        if (PlayerInputs.instance.PlayerPressedInventoryButton())
        {
            if (inventoryIsOpen)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Item"))
        {
            ItemGameObject itemGameObject = GetItemGameObjectFromCollider(collider);

            if (collider.IsTouching(itemCollectionCollider))
            {
                if (itemGameObject.canBePickedUp)
                {
                    PickUpItem(itemGameObject);
                }
            }
            
        }
    }


    private void OnEnable()
    {
        InventoryUpgrade.OnInventoryUpgrade += UpgradeInventory;
        InteractStation.OnInteractOpen += OpenInventory;
        InteractStation.OnInteractClose += CloseInventory;
    }

    private void OnDisable()
    {
        InventoryUpgrade.OnInventoryUpgrade -= UpgradeInventory;
        InteractStation.OnInteractOpen -= OpenInventory;
        InteractStation.OnInteractClose -= CloseInventory;
    }


    public void OpenInventory()
    {
        inventoryIsOpen = true;
        inventoryCanvas.gameObject.SetActive(true);
        //inventoryMenu.UpdateInventory();
        
        if (OnInventoryOpen != null)
            OnInventoryOpen();
    }

    public void CloseInventory()
    {
        inventoryIsOpen = false;
        inventoryCanvas.gameObject.SetActive(false);

        if (OnInventoryClose != null)
            OnInventoryClose();
    }

    private ItemGameObject GetItemGameObjectFromCollider(Collider2D collider)
    {
        return collider.GetComponent<ItemGameObject>();
    }

    private bool PickUpItem(ItemGameObject itemToPickUp)
    {
        if (playerPicksUpItemEvent != null)
            playerPicksUpItemEvent();

        Destroy(itemToPickUp.gameObject);
        return false;
    }

    private void UpgradeInventory()
    {
        inventory.UpgradeInventory();
    }
}
