using System;
using System.Collections;
using System.Linq;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using AtlyssMultiworldRandomizer.data;
using UnityEngine;

namespace AtlyssMultiworldRandomizer;

public class Hooks
{
    public static void CheckMultiworldPortalItem(On.Portal.orig_Update orig, Portal self)
    {
        self._isPortalOpen = true;
        self._openPortalIfQuestComplete = false;
        self._openPortalIfOnQuest = false;
        orig(self);
        long itemId; 
        bool worked = PortalItems.PortalNameToItemId.TryGetValue(self._scenePortal._portalCaptionTitle, out itemId);

        if (worked)
        {
            self._netDisablePortal = !Plugin.Session.Items.AllItemsReceived.Any(i => i.ItemId == itemId);
        }
    }

    public static void CheckMultiworldDungeonItem(On.DungeonPortalManager.orig_Update orig, DungeonPortalManager self)
    {
        orig(self);
        string dungeonName = self._scenePortal._portalCaptionTitle;
        var difficulties = new[]
        {
            "-EASY", "-NORMAL", "-HARD"
        };
        foreach (var difficulty in difficulties)
        {
            long itemId;
            bool worked = DungeonItems.DungeonNameToItemId.TryGetValue(dungeonName + difficulty, out itemId);

            if (worked)
            {
                switch (difficulty)
                {
                    case "-EASY":
                        self._enterDungeonButton_easy.interactable = Plugin.Session.Items.AllItemsReceived.Any(i => i.ItemId == itemId);
                        break;
                    case "-NORMAL":
                        self._enterDungeonButton_normal.interactable = Plugin.Session.Items.AllItemsReceived.Any(i => i.ItemId == itemId);
                        break;
                    case "-HARD":
                        self._enterDungeonButton_hard.interactable = Plugin.Session.Items.AllItemsReceived.Any(i => i.ItemId == itemId);
                        break;
                }
            }
        }
    }

    public static void SendAchievementLocations(On.SteamAchievementManager.orig_Init_EarnAchievement orig, SteamAchievementManager self, string achievementTag)
    {
        long locationId;
        bool worked = AchievementLocations.AchievementNameToLocationId.TryGetValue(achievementTag, out locationId);

        if (worked)
        {
            Plugin.Session.Locations.CompleteLocationChecks(locationId);
        }
        
        orig(self, achievementTag);

        if (achievementTag.Equals("ATLYSS_ACHIEVEMENT_13"))
        {
            Plugin.Session.SetGoalAchieved();
        }
    }

    public static IEnumerator SendChestLocations(On.PlayerInteract.orig_Queue_TriggerInteractionCommand_ItemDropEntity_Chest orig, PlayerInteract self, ItemDropEntity_Chest foundChestEntity)
    {
        long locationId;
        bool worked = ChestLocations.ChestNameToLocationId.TryGetValue(foundChestEntity.ToString(), out locationId);

        if (worked)
        {
            Plugin.Session.Locations.CompleteLocationChecks(locationId);
        }
        
        return orig(self, foundChestEntity);
    }

    public static void SendQuestLocations(On.PlayerQuesting.orig_Client_CompleteQuest orig, PlayerQuesting self, int index)
    {
        var quest = GameManager._current.Locate_Quest(self._questProgressData[index]._questTag);

        long locationId;
        bool worked = QuestLocations.QuestNameToLocationId.TryGetValue(quest._questName, out locationId);

        if (worked)
        {
            Plugin.Session.Locations.CompleteLocationChecks(locationId);
        }
        
        orig(self, index);
    }

    public static void ExpMultiplier(On.PlayerStats.orig_GainExp orig, PlayerStats self, int expGain, int passedLevel)
    {
        int multiplier = Convert.ToInt32(Plugin.LoginSuccessful.SlotData["exp_mult"]);
        
        int multExpGain = expGain * multiplier;
        
        orig(self, multExpGain, passedLevel);
    }

    public static void ReceiveFiller(ReceivedItemsHelper helper)
    {
        string multiworldItemName = helper.PeekItem().ItemName;
        Plugin.Logger.LogInfo($"Received Item {multiworldItemName}");
        if (helper.PeekItem().Flags == ItemFlags.None)
        {
            ScriptableItem item = GameManager._current.Locate_Item(multiworldItemName);

            if (!Player._mainPlayer._pInventory.Check_InventoryFull(item, 1))
            {
                Plugin.Logger.LogInfo($"Adding {item._itemName} to inventory");
                ItemData itemData = new ItemData()
                {
                    _itemName = item._itemName,
                    _quantity = 1,
                    _maxQuantity = item._maxStackAmount,
                    _isEquipped = false,
                    _isAltWeapon = false
                };
            
                Player._mainPlayer._pInventory.Add_Item(itemData, true);
            }
        }

        helper.DequeueItem();
    }
}