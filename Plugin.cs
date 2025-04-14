using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace BrainrotMod;

[BepInPlugin(modGUID, modName, modVersion)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    public const string modGUID = "sunryze.BrainrotMod";
    public const string modName = "BrainrotMod";
    public const string modVersion = "1.0.0";

    public static Plugin PluginInstance;
    public static ManualLogSource LoggerInstance;
    private readonly Harmony harmony = new(modGUID);

    public static string[] Sentences =
        [
        "That is so skibidi!", "holy gyatt", "can someone rizz me pls", "only in ohio dude", "did you pray today?",
        "sussy imposter uwu", "STOP EDGING", "literally goonmaxxing rn", "sticking out my gyatt for the rizzler",
        "oh my god stop diddling my diddle!!!", "I SAID LET, HIM, COOK!", "L bozo", "what da dog doin", "im mewing rn",
        "i am literally the rizzler", "edgemaxxing", "ishowmeat", "thats cap", "im literally on the verge of gooning",
        "im literally on the verge of gooning rn", "i am literally the rizzler", "im literally on the verge of gooning rn",
        "thats so sigma!", "i just have that aura dude", "looksmaxxing with that gyatt",
        "literally goonmaxxing rn", "do u know da wae", "stop that fanum tax", "Ahh shit, here we go again",
        "tiktok rizz party"
        ];

    [HarmonyPatch(typeof(ChatManager))]
    public class Patch_ChatManager_MessageSend
    {
        static readonly AccessTools.FieldRef<ChatManager, string> chatMessageRef = AccessTools.FieldRefAccess<ChatManager, string>("chatMessage");

        [HarmonyPatch("MessageSend")]
        static void Prefix()
        {
            var __instance = ChatManager.instance;
            if (Random.RandomRangeInt(0, 10) >= 9)
            {
                Traverse.Create(__instance).Field("chatMessage").SetValue(Sentences[Random.RandomRangeInt(0, Sentences.Length)]);
            }
        }
    }

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;

        if (PluginInstance == null)
        {
            PluginInstance = this;
        }

        LoggerInstance = Logger;


        harmony.PatchAll();

        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }
}
