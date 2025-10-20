using BepInEx.Configuration;

namespace AtlyssMultiworldRandomizer;

public class Configuration
{
    public static ConfigEntry<string> connectionIP { get; private set; }
    public static ConfigEntry<string> connectionPort { get; private set; }
    public static ConfigEntry<string> connectionSlot { get; private set; }
    public static ConfigEntry<string> connectionPassword { get; private set; }

    public static void CreateConfigEntries(ConfigFile config)
    {
        connectionIP = config.Bind(MyPluginInfo.PLUGIN_GUID, "localhost", "IP used to connect to the multiworld");
        connectionPort = config.Bind(MyPluginInfo.PLUGIN_GUID, "38281", "Port used to connect to the multiworld");
        connectionSlot = config.Bind(MyPluginInfo.PLUGIN_GUID, "", "Slot name used to connect to the multiworld");
        connectionPassword = config.Bind(MyPluginInfo.PLUGIN_GUID, "", "Password to connect to the multiworld");
    }
}