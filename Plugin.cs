using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace BrainrotMod;

[BepInPlugin(ModGuid, ModName, ModVersion)]
public class Plugin : BaseUnityPlugin
{
    private static ManualLogSource _logger;

    private const string ModGuid = "sunryze.BrainrotMod";
    private const string ModName = "BrainrotMod";
    private const string ModVersion = "1.0.0";

    private static Plugin _pluginInstance;
    public static ManualLogSource LoggerInstance;
    private readonly Harmony _harmony = new(ModGuid);

    private static readonly string[] Sentences =
    [
        "that is so skibidi!", "holy gyatt", "can you rizz me pls", "only in ohio does this shit happen", 
        "did you pray today?",
        "u sussy imposter", "STOP EDGING", "bro is goonmaxxing rn", "sticking out my gyatt for the rizzler",
        "if diddy did diddle dudes, how many dudes did diddy diddle?",
        "I SAID LET, HIM, COOK!", "L bozo", "what da dog doin", "im mewing rn",
        "i am the rizzler", "edgemaxxing", "dont be ishowmeat", "thats cap", "im gooning to your sigma aura",
        "im literally on the verge of gooning rn", "i am literally the rizzler", 
        "okay. 19 dollar fortnite card. who wants it?",
        "thats so sigma!", "i just have that aura dude", "looksmaxxing with that gyatt",
        "do u know da wae", "fanum tax", "ahh shit, here we go again",
        "tiktok rizz party", "skibidi rizz", "nah thats cap", "this is fire.", "ur so sigma",
        "blud shut the fuck up"
    ];

    [HarmonyPatch(typeof(ChatManager))]
    public class PatchChatManagerMessageSend
    {
        [HarmonyPatch("MessageSend")]
        static void Prefix()
        {
            var instance = ChatManager.instance;
            if (Random.Range(0.0f,1.0f) < 0.10f)
            {
                Traverse.Create(instance).Field("chatMessage").SetValue(
                    Sentences[Random.RandomRangeInt(0, Sentences.Length)]);
            }
        }
    }

    private void Awake()
    {
        // Plugin startup logic
        _logger = Logger;
        if (_pluginInstance == null)
        {
            _pluginInstance = this;
        }

        LoggerInstance = _logger;
        
        _harmony.PatchAll();

        _logger.LogInfo($"welcome to the brainrot zone from {ModGuid}");
    }
}
