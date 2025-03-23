using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    public string itemName;
    public ItemType type;
    public Sprite icon; // For UI display

    public int attackBonus;    // Bonus to attack when equipped
    public int defenseBonus;   // Bonus to defense when equipped
    public int healthBonus;    // Optional: Bonus to max health when equipped
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