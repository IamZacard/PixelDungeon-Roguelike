using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentUI : MonoBehaviour
{
    [Header("Equipment Slots")]
    public Image helmetSlot;
    public Image coreArmorSlot;
    public Image handsSlot;
    public Image bootsSlot;
    public Image swordSlot;
    public Image shieldSlot;

    [Header("Stats UI")]
    [SerializeField] private TextMeshProUGUI attackText;
    [SerializeField] private TextMeshProUGUI defenseText;
    [SerializeField] private TextMeshProUGUI maxHealthText;

    private PlayerInventory playerInventory;
    private PlayerHealth playerHealth;
    private bool isInitialized = false;

    void Start()
    {
        // Try to find PlayerInventory and PlayerHealth immediately
        playerInventory = FindObjectOfType<PlayerInventory>();
        playerHealth = FindObjectOfType<PlayerHealth>();
        if (playerInventory != null && playerHealth != null)
        {
            Initialize();
        }
        // If not found, we'll check in Update
    }

    void Update()
    {
        if (!isInitialized && (playerInventory == null || playerHealth == null))
        {
            playerInventory = FindObjectOfType<PlayerInventory>();
            playerHealth = FindObjectOfType<PlayerHealth>();
            if (playerInventory != null && playerHealth != null)
            {
                Initialize();
                enabled = false; // Disable Update after initialization to save performance
            }
        }
    }

    private void Initialize()
    {
        if (playerInventory == null)
        {
            Debug.LogError("EquipmentUI: PlayerInventory is still null during initialization!");
            return;
        }
        if (playerHealth == null)
        {
            Debug.LogError("EquipmentUI: PlayerHealth is still null during initialization!");
            return;
        }

        // Subscribe to equipment changes
        playerInventory.OnEquipmentChanged += UpdateEquipmentSlot;
        UpdateEquipmentSlot(ItemType.Helmet, playerInventory.equipment.helmet);
        UpdateEquipmentSlot(ItemType.CoreArmor, playerInventory.equipment.coreArmor);
        UpdateEquipmentSlot(ItemType.Hands, playerInventory.equipment.hands);
        UpdateEquipmentSlot(ItemType.Boots, playerInventory.equipment.boots);
        UpdateEquipmentSlot(ItemType.Sword, playerInventory.equipment.sword);
        UpdateEquipmentSlot(ItemType.Shield, playerInventory.equipment.shield);

        // Add unequip functionality to buttons
        helmetSlot.GetComponent<Button>().onClick.AddListener(() => playerInventory.UnequipItem(ItemType.Helmet));
        coreArmorSlot.GetComponent<Button>().onClick.AddListener(() => playerInventory.UnequipItem(ItemType.CoreArmor));
        handsSlot.GetComponent<Button>().onClick.AddListener(() => playerInventory.UnequipItem(ItemType.Hands));
        bootsSlot.GetComponent<Button>().onClick.AddListener(() => playerInventory.UnequipItem(ItemType.Boots));
        swordSlot.GetComponent<Button>().onClick.AddListener(() => playerInventory.UnequipItem(ItemType.Sword));
        shieldSlot.GetComponent<Button>().onClick.AddListener(() => playerInventory.UnequipItem(ItemType.Shield));

        // Initial update of stats UI
        UpdateStatsUI();

        isInitialized = true;
        Debug.Log("EquipmentUI initialized successfully.");
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

        // Update stats UI whenever equipment changes
        UpdateStatsUI();
    }

    private void UpdateStatsUI()
    {
        if (attackText != null)
        {
            attackText.text = $"Attack: {playerHealth.GetTotalAttack()}";
        }
        else
        {
            Debug.LogWarning("EquipmentUI: attackText is not assigned!");
        }

        if (defenseText != null)
        {
            defenseText.text = $"Defense: {playerHealth.GetTotalDefense()}";
        }
        else
        {
            Debug.LogWarning("EquipmentUI: defenseText is not assigned!");
        }

        if (maxHealthText != null)
        {
            Debug.Log($"EquipmentUI: CurrentMaxHealth = {playerHealth?.CurrentMaxHealth ?? -1}");
            maxHealthText.text = $"Max HP: {playerHealth?.CurrentMaxHealth ?? 0}";
        }
        else
        {
            Debug.LogWarning("EquipmentUI: maxHealthText is not assigned!");
        }
    }

    void OnDestroy()
    {
        if (playerInventory != null)
        {
            playerInventory.OnEquipmentChanged -= UpdateEquipmentSlot;
        }
    }
}