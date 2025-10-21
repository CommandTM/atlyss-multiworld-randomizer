
using System;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using BepInEx;
using BepInEx.Logging;

namespace AtlyssMultiworldRandomizer;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    public static ArchipelagoSession Session;
    public static LoginResult LoginResult;
    public static LoginSuccessful LoginSuccessful;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        Configuration.CreateConfigEntries(Config);
        On.CharacterSelectManager.Select_CharacterFile += _connectionMultiworld;
        On.OptionsMenuCell.SaveQuit_Game += _disconnectionMultiworld;
    }

    public void Hook()
    {
        On.Portal.Update += Hooks.CheckMultiworldPortalItem;
        On.DungeonPortalManager.Update += Hooks.CheckMultiworldDungeonItem;
        On.SteamAchievementManager.Init_EarnAchievement += Hooks.SendAchievementLocations;
        On.PlayerInteract.Queue_TriggerInteractionCommand_ItemDropEntity_Chest += Hooks.SendChestLocations;
        On.PlayerQuesting.Client_CompleteQuest += Hooks.SendQuestLocations;
        On.PlayerStats.GainExp += Hooks.ExpMultiplier;
        Session.Items.ItemReceived += Hooks.ReceiveFiller;
    }

    private void _disconnectionMultiworld(On.OptionsMenuCell.orig_SaveQuit_Game orig, OptionsMenuCell self)
    {
        Session.Socket.DisconnectAsync();
        Logger.LogInfo("Disconnected from multiworld...");
        orig(self);
    }

    private void _connectionMultiworld(On.CharacterSelectManager.orig_Select_CharacterFile orig, CharacterSelectManager self)
    {
        Session = ArchipelagoSessionFactory.CreateSession(
            $"{Configuration.connectionIP.Value}:{Configuration.connectionPort.Value}");
        
        try
        {
            LoginResult = Session.TryConnectAndLogin(
                "Atlyss",
                Configuration.connectionSlot.Value,
                ItemsHandlingFlags.AllItems,
                new Version(0, 6, 3),
                password: Configuration.connectionPassword.Value);
        }
        catch (Exception ex)
        {
            Logger.LogError("There was an error while connecting to the multiworld!");
            var result = new LoginFailure(ex.GetBaseException().Message);
            foreach (var error in result.Errors)
            {
                Logger.LogError(error);
            }
        }

        if (LoginResult.Successful)
        {
            LoginSuccessful = (LoginSuccessful)LoginResult;
            Logger.LogInfo($"Successfully connected to multiworld! Slot: {LoginSuccessful.Slot}");
            orig(self);
            Hook();
        }
    }
}