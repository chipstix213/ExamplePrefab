using System;
using Chipstix.ExamplePrefab;
using HarmonyLib;
using StationeersMods.Interface;
[StationeersMod("ExamplePrefab","ExamplePrefab [StationeersMods]","0.2.4657.21547.1")]
public class ExamplePrefab : ModBehaviour
{
    // private ConfigEntry<bool> configBool;
    
    public override void OnLoaded(ContentHandler contentHandler)
    {
        UnityEngine.Debug.Log("ExamplePrefab says: Hello World!");
        
        //Config example
        // configBool = Config.Bind("Input",
        //     "Boolean",
        //     true,
        //     "Boolean description");
        
        Harmony harmony = new Harmony("ExamplePrefab");
        PrefabPatch.prefabs = contentHandler.prefabs;
        harmony.PatchAll();

        ModStructureToolSetup.SetupModStructureToolForPrefabs(contentHandler.prefabs);

        UnityEngine.Debug.Log("ExamplePrefab Loaded with " + contentHandler.prefabs.Count + " prefab(s)");


    }
}
