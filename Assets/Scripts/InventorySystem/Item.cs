using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public ItemType type;
    public Sprite icon; // For UI display
}

public enum ItemType
{
    Helmet,
    CoreArmor,
    Hands,
    Boots,
    Sword,
    Shield
    // You can add more types like Potion later if needed
}