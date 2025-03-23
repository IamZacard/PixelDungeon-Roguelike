using UnityEngine;
using UnityEngine.UI;

public class PlayerInventoryUI : MonoBehaviour
{
    [SerializeField] private Image[] slotImages; // Array of 6 slot images
    [SerializeField] private Button[] slotButtons; // Array of 6 slot buttons
    private PlayerInventory playerInventory;
    private bool isInitialized = false;

    void Start()
    {
        // Validate assignments
        if (slotImages == null || slotImages.Length != 6 || slotButtons == null || slotButtons.Length != 6)
        {
            Debug.LogError("InventoryUI: slotImages or slotButtons not properly assigned in Inspector! Ensure 6 slots are assigned.");
            return;
        }

        playerInventory = FindObjectOfType<PlayerInventory>();
        if (playerInventory != null)
        {
            Initialize();
        }
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

        playerInventory.OnItemAdded += UpdateSlots;
        playerInventory.OnItemRemoved += UpdateSlots;

        // Populate initial inventory
        UpdateSlots(null); // Pass null since OnItemAdded/Removed passes an Item

        isInitialized = true;
        Debug.Log("InventoryUI initialized successfully.");

        // Add click listeners to buttons
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int slotIndex = i; // Capture the index for the lambda
            slotButtons[i].onClick.AddListener(() => OnSlotClicked(slotIndex));
        }
    }

    private void UpdateSlots(Item _)
    {
        // Clear all slots first
        for (int i = 0; i < slotImages.Length; i++)
        {
            slotImages[i].sprite = null;
            slotImages[i].enabled = false;
            slotButtons[i].interactable = false;
        }

        // Populate slots with inventory items
        for (int i = 0; i < playerInventory.inventory.Count && i < slotImages.Length; i++)
        {
            Item item = playerInventory.inventory[i];
            slotImages[i].sprite = item.icon;
            slotImages[i].enabled = true;
            slotButtons[i].interactable = true;
        }
    }

    private void OnSlotClicked(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= playerInventory.inventory.Count)
        {
            Debug.LogWarning("InventoryUI: Invalid slot index clicked!");
            return;
        }

        Item item = playerInventory.inventory[slotIndex];
        playerInventory.EquipItem(item);
    }

    void OnDestroy()
    {
        // Clean up event subscriptions
        if (playerInventory != null)
        {
            playerInventory.OnItemAdded -= UpdateSlots;
            playerInventory.OnItemRemoved -= UpdateSlots;
        }
    }
}