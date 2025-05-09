
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Assets.Scripts;
using Assets.Scripts.Objects;
using Assets.Scripts.Util;
using HarmonyLib;
using StationeersMods.Interface;
using UnityEngine;
using UnityEngine.Rendering;
namespace Chipstix.ExamplePrefab
{
    [HarmonyPatch]
    public class PrefabPatch
    {
        public static ReadOnlyCollection<GameObject> prefabs { get; set; }
        [HarmonyPatch(typeof(Prefab), "LoadAll")]
        public static void Prefix()
        {
            try
            {
                ref List<ColorSwatch> customColors = ref Singleton<GameManager>.Instance.CustomColors;
                Debug.Log("Prefab Patch started");
                foreach (var gameObject in prefabs)
                {
                    Thing thing = gameObject.GetComponent<Thing>();

                    if (thing is TestConstuct)
                    {
                        Debug.Log("patch TestConstruct");
                        TestConstuct testConstuct = gameObject.GetComponent<TestConstuct>();
                        /*MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
                        meshRenderer.materials = new[]
                        {
                            customColors[4].Normal
                        };
                        thing.PaintableMaterial = customColors[6].Normal;*/
                        testConstuct.BuildStates[0].Tool.ToolExit = StationeersModsUtility.FindTool(StationeersTool.DRILL);
                        testConstuct.Blueprint.GetComponent<MeshRenderer>().materials = StationeersModsUtility.GetBlueprintMaterials(2);
                    }

                    if (thing is TestWallLight)
                    {
                        Debug.Log("patch TestLight");
                        TestWallLight testlight = gameObject.GetComponent<TestWallLight>();
                        /*MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
                        meshRenderer.materials = new[]
                        {
                            customColors[4].Normal
                        };
                        thing.PaintableMaterial = customColors[6].Normal;*/
                        testlight.BuildStates[0].Tool.ToolExit = StationeersModsUtility.FindTool(StationeersTool.DRILL);
                        testlight.Blueprint.GetComponent<MeshRenderer>().materials = StationeersModsUtility.GetBlueprintMaterials(2);
                    }


                    if (thing is TestRoundWallLight)
                    {
                        Debug.Log("patch TestLight");
                        TestRoundWallLight testroundlight = gameObject.GetComponent<TestRoundWallLight>();
                        /*MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
                        meshRenderer.materials = new[]
                        {
                            customColors[4].Normal
                        };
                        thing.PaintableMaterial = customColors[6].Normal;*/
                        testroundlight.BuildStates[0].Tool.ToolExit = StationeersModsUtility.FindTool(StationeersTool.DRILL);
                        testroundlight.Blueprint.GetComponent<MeshRenderer>().materials = StationeersModsUtility.GetBlueprintMaterials(2);
                    }
                   /* if (thing is MegaWire)
                    {
                        Debug.Log("patch MegaCable");
                        MegaWire megacable = gameObject.GetComponent<MegaWire>();

                        megacable.BuildStates[0].Tool.ToolExit = StationeersModsUtility.FindTool(StationeersTool.DRILL);
                        megacable.BuildStates[0].Tool.ToolEntry2 = StationeersModsUtility.FindTool(StationeersTool.CABLE_CUTTERS);
                        megacable.Blueprint.GetComponent<MeshRenderer>().materials = StationeersModsUtility.GetBlueprintMaterials(2);
                    }
                   */

                    if (thing is MegaPipeConst)
                    {
                        MegaPipeConst megacable = gameObject.GetComponent<MegaPipeConst>();
                        megacable.ToolExit = StationeersModsUtility.FindTool(StationeersTool.DRILL);

                    }

                    if (thing is generator)
                    {
                        Debug.Log("patch SoldGen");
                        generator SoldGen = gameObject.GetComponent<generator>();
                        /*MeshRenderer meshRenderer = gameObject.GetComponent<MeshRenderer>();
                        meshRenderer.materials = new[]
                        {
                            customColors[4].Normal
                        };
                        thing.PaintableMaterial = customColors[6].Normal;*/

                        SoldGen.BuildStates[0].Tool.ToolExit = StationeersModsUtility.FindTool(StationeersTool.DRILL);
                        SoldGen.Blueprint.GetComponent<MeshRenderer>().materials = StationeersModsUtility.GetBlueprintMaterials(2);


                    }
                    //note



                    // Additional patching goes here, like setting references to materials(colors) or tools from the game
                    if (thing != null)
                    {
                        Debug.Log(gameObject.name + " added to WorldManager");
                        WorldManager.Instance.SourcePrefabs.Add(thing);
                    }




                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                Debug.LogException(ex);
            }
        }
    }
}


