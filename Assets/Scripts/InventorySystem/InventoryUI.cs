using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    public Transform inventoryPanel;
    public Transform equipmentPanel;
    public GameObject slotPrefab;
    Inventory inventory;
    Equipment equipment;

    void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        equipment = FindObjectOfType<Equipment>();
        inventory.onInventoryChangedCallback += UpdateUI;
        equipment.onEquipmentChangedCallback += UpdateUI;
        UpdateUI();
    }

    void UpdateUI()
    {
        // Clear existing slots
        foreach (Transform child in inventoryPanel) Destroy(child.gameObject);
        foreach (Transform child in equipmentPanel) Destroy(child.gameObject);

        // Inventory slots
        for (int i = 0; i < inventory.inventorySize; i++)
        {
            GameObject slot = Instantiate(slotPrefab, inventoryPanel);
            if (i < inventory.items.Count)
            {
                slot.GetComponent<Image>().sprite = inventory.items[i].icon;
                int index = i;
                //slot.GetComponent<Button>().onClick.AddListener(() => FindObjectOfType<Player>().EquipFromInventory(index));
            }
        }

        // Equipment slots
        foreach (Equipment.EquipmentSlot slot in equipment.slots)
        {
            GameObject equipSlot = Instantiate(slotPrefab, equipmentPanel);
            if (slot.equippedItem != null)
            {
                equipSlot.GetComponent<Image>().sprite = slot.equippedItem.icon;
            }
        }
    }
}