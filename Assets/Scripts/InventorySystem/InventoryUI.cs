using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab; // Serialized for Inspector assignment
    [SerializeField] private Transform content;     // ScrollView's "Content" transform
    private PlayerInventory playerInventory;
    private bool isInitialized = false;

    void Start()
    {
        if (slotPrefab == null || content == null)
        {
            Debug.LogError("InventoryUI: slotPrefab or content not assigned in Inspector!");
            return;
        }

        playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory != null)
        {
            Initialize();
        }
        // If not found, Update will handle it
    }

    void Update()
    {
        if (!isInitialized && playerInventory == null)
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            if (playerInventory != null)
            {
                Initialize();
                enabled = false; // Stop Update after initialization
            }
        }
    }

    private void Initialize()
    {
        if (playerInventory == null)
        {
            Debug.LogError("InventoryUI: PlayerInventory is still null during initialization!");
            return;
        }

        playerInventory.OnItemAdded += AddItemSlot;
        playerInventory.OnItemRemoved += RemoveItemSlot;

        // Populate initial inventory
        foreach (var item in playerInventory.inventory)
        {
            AddItemSlot(item);
        }

        isInitialized = true;
        Debug.Log("InventoryUI initialized successfully.");
    }

    private void AddItemSlot(Item item)
    {
        GameObject slot = Instantiate(slotPrefab, content);
        ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
        if (itemSlot == null)
        {
            itemSlot = slot.AddComponent<ItemSlot>();
        }
        itemSlot.item = item;

        Image iconImage = slot.GetComponentInChildren<Image>();
        if (iconImage != null)
        {
            iconImage.sprite = item.icon;
        }
        else
        {
            Debug.LogWarning($"InventoryUI: No Image component found in slot for item {item.name}");
        }

        Button button = slot.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => OnItemSelected(item));
        }
        else
        {
            Debug.LogWarning($"InventoryUI: No Button component found on slot for item {item.name}");
        }
    }

    private void RemoveItemSlot(Item item)
    {
        foreach (Transform child in content)
        {
            ItemSlot slot = child.GetComponent<ItemSlot>();
            if (slot != null && slot.item == item)
            {
                Destroy(child.gameObject);
                break; // Exit after removing the first match
            }
        }
    }

    private void OnItemSelected(Item item)
    {
        // Equip the item directly (could expand to context menu later)
        playerInventory.EquipItem(item);
    }

    void OnDestroy()
    {
        // Clean up event subscriptions
        if (playerInventory != null)
        {
            playerInventory.OnItemAdded -= AddItemSlot;
            playerInventory.OnItemRemoved -= RemoveItemSlot;
        }
    }
}

public class ItemSlot : MonoBehaviour
{
    public Item item; // Reference to the item this slot represents
}