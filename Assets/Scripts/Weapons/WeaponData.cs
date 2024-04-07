using UnityEngine;

public enum Rarity
{
    Common,
    Uncommon,
    Rare,
    Mythic,
    Legendary
}

[CreateAssetMenu(fileName = "Weapon Data", menuName = "Charactor/Weapon Data")]
public class WeaponData : ItemData
{
    [HideInInspector] public string behaviour;
    public Weapon.Stats baseStats;
    public Weapon.Stats[] linearGrowth;
    public RandomGrowthEntry[] randomGrowth;
    public Weapon.Stats currentWeapon; // Assign the current weapon instance in your game


    // Gives us the stat growth / description of the next level.
    public Weapon.Stats GetLevelData(int level)
    {
        // Pick the stats from the next level.
        if (level - 2 < linearGrowth.Length)
            return linearGrowth[level - 2];

        // Generate a random number between 0 and 100
        int randomNumber = Random.Range(0, 100);

        // Select a rarity based on random number and percentage chance
        foreach (RandomGrowthEntry entry in randomGrowth)
        {
            if (randomNumber < entry.percentageChanceOfRarity)
            {
                return entry.stats;
            }
            else
            {
                randomNumber -= entry.percentageChanceOfRarity;
            }
        }

        // Return an empty value and a warning.
        Debug.LogWarning("Weapon doesn't have its level up stats configured for Level " + level + "!");
        return new Weapon.Stats();
    }
}

[System.Serializable]
public class RandomGrowthEntry
{
    public Rarity rarity;
    public Weapon.Stats stats;
    [Range(0, 100)]
    public int percentageChanceOfRarity; // Percentage chance for this rarity

    // Constructor to set default percentage chances
    public RandomGrowthEntry()
    {
        // Set default percentage chances
        switch (rarity)
        {
            case Rarity.Common:
                percentageChanceOfRarity = 75;
                break;
            case Rarity.Uncommon:
                percentageChanceOfRarity = 30;
                break;
            case Rarity.Rare:
                percentageChanceOfRarity = 10;
                break;
            case Rarity.Mythic:
                percentageChanceOfRarity = 5;
                break;
            case Rarity.Legendary:
                percentageChanceOfRarity = 1;
                break;
            default:
                percentageChanceOfRarity = 100;
                break;
        }
    }
}
