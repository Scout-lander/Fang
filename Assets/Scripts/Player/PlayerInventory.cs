using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInventory : MonoBehaviour
{
    [System.Serializable]
    public class Slot
    {
        public Item item;
        public Image image;

        public void Assign(Item assignedItem)
        {
            item = assignedItem;
            if(item is Weapon)
            {
                Weapon w = item as Weapon;
                image.enabled = true;
                image.sprite = w.data.icon;
            }
            else
            {
                Passive p = item as Passive;
                image.enabled = true;
                image.sprite = p.data.icon;
            }
            Debug.Log(string.Format("Assigned {0} to player.", item.name));
        }

        public void Clear()
        {
            item = null;
            image.enabled = false;
            image.sprite = null;
        }

        public bool IsEmpty() { return item == null; }
    }
    public List<Slot> weaponSlots = new List<Slot>(6);
    public List<Slot> passiveSlots = new List<Slot>(6);

    [System.Serializable]
    public class UpgradeUI
    {
        public TMP_Text upgradeNameDisplay;
        public TMP_Text upgradeDescriptionDisplay;
        public Image upgradeIcon;
        public Button upgradeButton;
    }

    [Header("UI Elements")]
    public List<WeaponData> availableWeapons = new List<WeaponData>();    //List of upgrade options for weapons
    public List<PassiveData> availablePassives = new List<PassiveData>(); //List of upgrade options for passive items
    public List<UpgradeUI> upgradeUIOptions = new List<UpgradeUI>();    //List of ui for upgrade options present in the scene

    PlayerStats player;

    void Start()
    {
        player = GetComponent<PlayerStats>();
    }

    // Checks if the inventory has an item of a certain type.
    public bool Has(ItemData type) { return Get(type); }

    public Item Get(ItemData type)
    {
        if (type is WeaponData) return Get(type as WeaponData);
        else if (type is PassiveData) return Get(type as PassiveData);
        return null;
    }

    // Find a passive of a certain type in the inventory.
    public Passive Get(PassiveData type)
    {
        foreach (Slot s in passiveSlots)
        {
            Passive p = s.item as Passive;
            if (p && p.data == type)
                return p;
        }
        return null;
    }

    // Find a weapon of a certain type in the inventory.
    public Weapon Get(WeaponData type)
    {
        foreach (Slot s in weaponSlots)
        {
            Weapon w = s.item as Weapon;
            if (w && w.data == type)
                return w;
        }
        return null;
    }

    // Removes a weapon of a particular type, as specified by .
    public bool Remove(WeaponData data, bool removeUpgradeAvailability = false)
    {
        // Remove this weapon from the upgrade pool.
        if (removeUpgradeAvailability) availableWeapons.Remove(data);

        for(int i = 0; i < weaponSlots.Count; i++)
        {
            Weapon w = weaponSlots[i].item as Weapon;
            if (w.data == data)
            {
                weaponSlots[i].Clear();
                w.OnUnequip();
                Destroy(w.gameObject);
                return true;
            }
        }

        return false;
    }

    // Removes a passive of a particular type, as specified by .
    public bool Remove(PassiveData data, bool removeUpgradeAvailability = false)
    {
        // Remove this passive from the upgrade pool.
        if (removeUpgradeAvailability) availablePassives.Remove(data);

        for (int i = 0; i < weaponSlots.Count; i++)
        {
            Passive p = weaponSlots[i].item as Passive;
            if (p.data == data)
            {
                weaponSlots[i].Clear();
                p.OnUnequip();
                Destroy(p.gameObject);
                return true;
            }
        }

        return false;
    }

    // If an ItemData is passed, determine what type it is and call the respective overload.
    // We also have an optional boolean to remove this item from the upgrade list.
    public bool Remove(ItemData data, bool removeUpgradeAvailability = false)
    {
        if (data is PassiveData) return Remove(data as PassiveData, removeUpgradeAvailability);
        else if(data is WeaponData) return Remove(data as WeaponData, removeUpgradeAvailability);
        return false;
    }

    // Finds an empty slot and adds a weapon of a certain type, returns
    // the slot number that the item was put in.
    public int Add(WeaponData data)
    {
        int slotNum = -1;

        // Try to find an empty slot.
        for(int i = 0; i < weaponSlots.Capacity; i++)
        {
            if (weaponSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        // If there is no empty slot, exit.
        if (slotNum < 0) return slotNum;

        // Otherwise create the weapon in the slot.
        // Get the type of the weapon we want to spawn.
        Type weaponType = Type.GetType(data.behaviour);

        if (weaponType != null)
        {
            // Spawn the weapon GameObject.
            GameObject go = new GameObject(data.baseStats.name + " Controller");
            Weapon spawnedWeapon = (Weapon)go.AddComponent(weaponType);
            spawnedWeapon.transform.SetParent(transform); //Set the weapon to be a child of the player
            spawnedWeapon.transform.localPosition = Vector2.zero;
            spawnedWeapon.Initialise(data);
            spawnedWeapon.OnEquip();

            // Assign the weapon to the slot.
            weaponSlots[slotNum].Assign(spawnedWeapon);

            // Close the level up UI if it is on.
            if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
                GameManager.instance.EndLevelUp();

            return slotNum;
        }
        else
        {
            Debug.LogWarning(string.Format(
                "Invalid weapon type specified for {0}.",
                data.name
            ));
        }

        return -1;
    }

    // Finds an empty slot and adds a passive of a certain type, returns
    // the slot number that the item was put in.
    public int Add(PassiveData data)
    {
        int slotNum = -1;

        // Try to find an empty slot.
        for (int i = 0; i < passiveSlots.Capacity; i++)
        {
            if (passiveSlots[i].IsEmpty())
            {
                slotNum = i;
                break;
            }
        }

        // If there is no empty slot, exit.
        if (slotNum < 0) return slotNum;

        // Otherwise create the passive in the slot.
        // Get the type of the passive we want to spawn.
        GameObject go = new GameObject(data.baseStats.name + " Passive");
        Passive p = go.AddComponent<Passive>();
        p.Initialise(data);
        p.transform.SetParent(transform); //Set the weapon to be a child of the player
        p.transform.localPosition = Vector2.zero;

        // Assign the passive to the slot.
        passiveSlots[slotNum].Assign(p);

        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }
        player.RecalculateStats();

        return slotNum;
    }

    // If we don't know what item is being added, this function will determine that.
    public int Add(ItemData data)
    {
        if (data is WeaponData) return Add(data as WeaponData);
        else if (data is PassiveData) return Add(data as PassiveData);
        return -1;
    }

        // Overload so that we can use both ItemData or Item to level up an
    // item in the inventory.
    public bool LevelUp(ItemData data)
    {
        Item item = Get(data);
        if (item) return LevelUp(item);
        return false;
    }

    // Levels up a selected weapon in the player inventory.
    public bool LevelUp(Item item)
    {
        // Tries to level up the item.
        if(!item.DoLevelUp())
        {
            Debug.LogWarning(string.Format(
                "Failed to level up {0}.",
                 item.name
            ));
            return false;
        }

        // Close the level up screen afterwards.
        if (GameManager.instance != null && GameManager.instance.choosingUpgrade)
        {
            GameManager.instance.EndLevelUp();
        }

        // If it is a passive, recalculate player stats.
        if(item is Passive) player.RecalculateStats();
        return true;
    }

    // Checks a list of slots to see if there are any slots left.
    int GetSlotsLeft(List<Slot> slots)
    {
        
        int count = 0;
        foreach(Slot s in slots)
        {
            if (s.IsEmpty()) count++;
        }
        return count;
    }

    // Determines what upgrade options should appear.
    void ApplyUpgradeOptions()
    {
        //  is the list of possible upgrades that we will populate from
        // , which is a list of all available weapons and passives.
        List<ItemData> availableUpgrades = new List<ItemData>(availableWeapons.Count + availablePassives.Count);
        List<ItemData> allPossibleUpgrades = new List<ItemData>(availableWeapons);
        allPossibleUpgrades.AddRange(availablePassives);

        // We need to know how many weapon / passive slots are left.
        int weaponSlotsLeft = GetSlotsLeft(weaponSlots);
        int passiveSlotsLeft = GetSlotsLeft(passiveSlots);

        // Filters through the available weapons and passives and add those
        // that can possibly be an option.
        foreach(ItemData data in allPossibleUpgrades)
        {
            // If a weapon of this type exists, allow for the upgrade if the
            // level of the weapon is not already maxed out.
            Item obj = Get(data);
            if(obj)
            {
                if (obj.currentLevel < data.maxLevel) availableUpgrades.Add(data);
            }
            else
            {
                // If we don't have this item in the inventory yet, check if
                // we still have enough slots to take new items.
                if (data is WeaponData && weaponSlotsLeft > 0) availableUpgrades.Add(data);
                else if (data is PassiveData && passiveSlotsLeft > 0) availableUpgrades.Add(data);
            }
        }

        // Iterate through each slot in the upgrade UI and populate the options.
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            // If there are no more available upgrades, then we abort.
            if (availableUpgrades.Count <= 0) return;

            // Pick an upgrade, then remove it so that we don't get it twice.
            ItemData chosenUpgrade = availableUpgrades[UnityEngine.Random.Range(0, availableUpgrades.Count)];
            availableUpgrades.Remove(chosenUpgrade);

            // Ensure that the selected weapon data is valid.
            if (chosenUpgrade != null)
            {
                // Turns on the UI slot.
                EnableUpgradeUI(upgradeOption);

                // If our inventory already has the upgrade, we will make it a level up.
                Item item = Get(chosenUpgrade);
                if(item)
                {
                    upgradeOption.upgradeButton.onClick.AddListener(() => LevelUp(item)); //Apply button functionality
                    if (item is Weapon)
                    {
                        Weapon.Stats nextLevel = ((WeaponData)chosenUpgrade).GetLevelData(item.currentLevel + 1);
                        upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                        upgradeOption.upgradeNameDisplay.text = chosenUpgrade.name + " - " + nextLevel.name;
                        upgradeOption.upgradeIcon.sprite = chosenUpgrade.icon;
                    }
                    else
                    {
                        Passive.Modifier nextLevel = ((PassiveData)chosenUpgrade).GetLevelData(item.currentLevel + 1);
                        upgradeOption.upgradeDescriptionDisplay.text = nextLevel.description;
                        upgradeOption.upgradeNameDisplay.text = chosenUpgrade.name + " - " + nextLevel.name;
                        upgradeOption.upgradeIcon.sprite = chosenUpgrade.icon;
                    }
                }
                else
                {
                    if(chosenUpgrade is WeaponData)
                    {
                        WeaponData data = chosenUpgrade as WeaponData;
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenUpgrade)); //Apply button functionality
                        upgradeOption.upgradeDescriptionDisplay.text = data.baseStats.description;  //Apply initial description
                        upgradeOption.upgradeNameDisplay.text = data.baseStats.name;    //Apply initial name
                        upgradeOption.upgradeIcon.sprite = data.icon;
                    }
                    else
                    {
                        PassiveData data = chosenUpgrade as PassiveData;
                        upgradeOption.upgradeButton.onClick.AddListener(() => Add(chosenUpgrade)); //Apply button functionality
                        upgradeOption.upgradeDescriptionDisplay.text = data.baseStats.description;  //Apply initial description
                        upgradeOption.upgradeNameDisplay.text = data.baseStats.name;    //Apply initial name
                        upgradeOption.upgradeIcon.sprite = data.icon;
                    }
                    
                }
            }
        }
    }

    void RemoveUpgradeOptions()
    {
        foreach (UpgradeUI upgradeOption in upgradeUIOptions)
        {
            upgradeOption.upgradeButton.onClick.RemoveAllListeners();
            DisableUpgradeUI(upgradeOption);    //Call the DisableUpgradeUI method here to disable all UI options before applying upgrades to them
        }
    }

    public void RemoveAndApplyUpgrades()
    {
        RemoveUpgradeOptions();
        ApplyUpgradeOptions();
    }

    void DisableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(false);
    }

    void EnableUpgradeUI(UpgradeUI ui)
    {
        ui.upgradeNameDisplay.transform.parent.gameObject.SetActive(true);
    }

}