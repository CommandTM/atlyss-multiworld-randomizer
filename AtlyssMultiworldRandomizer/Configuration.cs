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
        connectionIP = config.Bind(MyPluginInfo.PLUGIN_GUID, "Multiworld Connection IP", "localhost");
        connectionPort = config.Bind(MyPluginInfo.PLUGIN_GUID, "Multiworld Connection Port", "38281");
        connectionSlot = config.Bind(MyPluginInfo.PLUGIN_GUID, "Multiworld Slot Name", "");
        connectionPassword = config.Bind(MyPluginInfo.PLUGIN_GUID, "Multiworld Password", "");
    }
}