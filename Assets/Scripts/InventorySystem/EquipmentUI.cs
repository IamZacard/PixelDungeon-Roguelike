using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    public Image helmetSlot;
    public Image coreArmorSlot;
    public Image handsSlot;
    public Image bootsSlot;
    public Image swordSlot;
    public Image shieldSlot;
    private PlayerInventory playerInventory;

    void Start()
    {
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerInventory.OnEquipmentChanged += UpdateEquipmentSlot;
        // Initial update
        UpdateEquipmentSlot(ItemType.Helmet, playerInventory.equipment.helmet);
        UpdateEquipmentSlot(ItemType.CoreArmor, playerInventory.equipment.coreArmor);
        UpdateEquipmentSlot(ItemType.Hands, playerInventory.equipment.hands);
        UpdateEquipmentSlot(ItemType.Boots, playerInventory.equipment.boots);
        UpdateEquipmentSlot(ItemType.Sword, playerInventory.equipment.sword);
        UpdateEquipmentSlot(ItemType.Shield, playerInventory.equipment.shield);

        // Add unequip functionality
        helmetSlot.GetComponent<Button>().onClick.AddListener(() => playerInventory.UnequipItem(ItemType.Helmet));
        coreArmorSlot.GetComponent<Button>().onClick.AddListener(() => playerInventory.UnequipItem(ItemType.CoreArmor));
        handsSlot.GetComponent<Button>().onClick.AddListener(() => playerInventory.UnequipItem(ItemType.Hands));
        bootsSlot.GetComponent<Button>().onClick.AddListener(() => playerInventory.UnequipItem(ItemType.Boots));
        swordSlot.GetComponent<Button>().onClick.AddListener(() => playerInventory.UnequipItem(ItemType.Sword));
        shieldSlot.GetComponent<Button>().onClick.AddListener(() => playerInventory.UnequipItem(ItemType.Shield));
    }

    void UpdateEquipmentSlot(ItemType type, Item item)
    {
        Sprite sprite = item != null ? item.icon : null;
        switch (type)
        {
            case ItemType.Helmet:
                helmetSlot.sprite = sprite;
                break;
            case ItemType.CoreArmor:
                coreArmorSlot.sprite = sprite;
                break;
            case ItemType.Hands:
                handsSlot.sprite = sprite;
                break;
            case ItemType.Boots:
                bootsSlot.sprite = sprite;
                break;
            case ItemType.Sword:
                swordSlot.sprite = sprite;
                break;
            case ItemType.Shield:
                shieldSlot.sprite = sprite;
                break;
        }
    }
}