///
// This class helps filling up tools and other prefabs for 
// repair/upgrade or construction/deconstruction tools.
//
// You need to add this code to the main mod class at the end of the
// OnLoaded method:
//
// 
//
///
using System;
using Assets.Scripts.Objects;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.ObjectModel;

namespace Chipstix.ExamplePrefab
{

    /// <summary>
    /// Class used for constructors
    /// </summary>
    [Serializable]
    public class ToolEntryStruct
    {
        [Header("Construction")]
        [Range(0, 60)]
        public float EntryTime = 2f;
        public string ToolEntry;
        public int EntryQuantity;
        public string ToolEntry2;
        public int EntryQuantity2;
    }

    /// <summary>
    /// Class used for constructors/deconstructors (ToolUse)
    /// </summary>
    [Serializable]
    public class ToolExitStruct : ToolEntryStruct
    {
        [Header("Deconstruction")]
        [Range(0, 60)]
        public float ExitTime = 2f;
        public string ToolExit;
        public int ExitQuantity;
    }

    /// <summary>
    /// Special case for BrokenBuildState that can contain WreckageItem prefabs
    /// </summary>
    [Serializable]
    public class BrokenBuildState
    {
        [SerializeReference]
        public object BuildState = new ToolExitStruct();
        public List<string> BrokenPieces = new List<string>();
    }

    /// <summary>
    /// Special case for upgrading elements
    /// </summary>
    [Serializable]
    public class UpgradePrefab
    {
        [SerializeReference]
        public object UpgradeTools = new ToolEntryStruct();
        public string Prefab;
    }

    public class ModStructureToolSetup : MonoBehaviour
    {
        [Tooltip("If not initialized, it will find the Structure in the same GameObject.")]
        public Assets.Scripts.Objects.Structure StructureParent;

        [Header("Structure")]
        [SerializeReference]
        public List<object> BuildStates = BuildStatesConstructor();
        [SerializeReference]
        public List<object> BrokenBuildStates = BrokenBuildStatesConstructor();
        [SerializeReference]
        public object RepairTools = new ToolEntryStruct();
        [SerializeReference]
        public object upgradePrefab = new UpgradePrefab();

        public static List<object> BuildStatesConstructor()
        {
            List<object> states = new List<object>();
            states.Add(new ToolExitStruct());
            states.Add(new ToolExitStruct());
            states.Add(new ToolExitStruct());
            states.Add(new ToolExitStruct());
            states.Add(new ToolExitStruct());
            return states;
        }

        private static List<object> BrokenBuildStatesConstructor()
        {
            List<object> states = new List<object>();
            states.Add(new BrokenBuildState());
            states.Add(new BrokenBuildState());
            states.Add(new BrokenBuildState());
            states.Add(new BrokenBuildState());
            states.Add(new BrokenBuildState());
            return states;
        }

        public static void SetupModStructureToolForPrefabs(ReadOnlyCollection<GameObject> modPrefabs)
        {
            // Patch both prefabs just in case a mod uses any of them.
            Prefab.OnPrefabsLoaded += () => {
                foreach (var modPrefab in modPrefabs)
                {
                    var prefab = (Assets.Scripts.Objects.Thing)null;
                    prefab = Prefab.Find(modPrefab.name);
                    FindComponentAndSetupTools(prefab);

                    var prefab2 = WorldManager.Instance.SourcePrefabs.Find(p => p.name == modPrefab.name);
                    FindComponentAndSetupTools(prefab);

                }
            };
        }

        static void FindComponentAndSetupTools(Assets.Scripts.Objects.Thing go)
        {
            if (go != null)
            {
                var setup = go.GetComponent<ModStructureToolSetup>();
                if (setup != null)
                {
                    //Debug.Log($"Patching Tools for {go.name}");
                    setup.PrivateSetupStructure();
                }
            }
        }


        // Start is called before the first frame update
        void PrivateSetupStructure()
        {
            //Debug.Log("ModStructureToolSetup.PrivateSetupStructure()");

            if (StructureParent == null)
                StructureParent = GetComponent<Assets.Scripts.Objects.Structure>();
            

            //Debug.Log($"ModStructureToolSetup.PrivateSetupStructure({StructureParent})");
            Assets.Scripts.Objects.Structure structure = StructureParent;
            //if (structure == null)
            //    return;
            //Debug.Log($"ModStructureToolSetup.PrivateSetupStructure({structure})");

            // Repair tools
            structure.RepairTools = CopyToolBasic(structure.RepairTools, RepairTools as ToolEntryStruct);

            // Upgrade prefab and tools
            structure.UpgradePrefab = CopyUpgradePrefab(structure.UpgradePrefab, upgradePrefab as UpgradePrefab);

            // BuildState tools
            for (int i = 0; i < structure.BuildStates.Count; i++)
            {
                //Debug.Log($"BuildState[{i}]");
                structure.BuildStates[i].Tool = CopyToolUse(structure.BuildStates[i].Tool, BuildStates[i] as ToolExitStruct);
            }

            // Broken build states tool and pieces
            for (int i = 0; i < structure.BrokenBuildStates.Count; i++)
            {
                //Debug.Log($"BrokenBuildState[{i}]");
                BrokenBuildState brokenBuildState = BrokenBuildStates[i] as BrokenBuildState;
                structure.BrokenBuildStates[i].BuildState.Tool = CopyToolUse(structure.BrokenBuildStates[i].BuildState.Tool, brokenBuildState.BuildState as ToolExitStruct);

                // Apeend items to the list
                for (int j = 0; j < brokenBuildState.BrokenPieces.Count; j++) {
                    //Debug.Log($"Piece: {brokenBuildState.BrokenPieces[j]}");
                    structure.BrokenBuildStates[i].BrokenPieces.Add(Prefab.Find<Wreckage>(brokenBuildState.BrokenPieces[j]));
                }
            }

            // Remove this component
            Destroy((ModStructureToolSetup)this);

        }

        /// <summary>
        /// Only replace elements if the string has a value, otherwise just ignore
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="src"></param>
        private static Assets.Scripts.Objects.ToolBasic CopyToolBasic(Assets.Scripts.Objects.ToolBasic dst, ToolEntryStruct src) {
            //Debug.Log("CopyToolBasic()");
            //Debug.Log($"ToolEntry: {src.ToolEntry}");
            if (src.ToolEntry != "")
            {
                dst.EntryTime = src.EntryTime;
                dst.ToolEntry = Prefab.Find<Assets.Scripts.Objects.Item>(src.ToolEntry);
                dst.EntryQuantity = src.EntryQuantity;
            }
            //Debug.Log($"ToolEntry2: {src.ToolEntry2}");
            if (src.ToolEntry2 != "")
            {
                dst.EntryTime = src.EntryTime;
                dst.ToolEntry2 = Prefab.Find<Assets.Scripts.Objects.Item>(src.ToolEntry2);
                dst.EntryQuantity2 = src.EntryQuantity2;
            }
            return dst;
        }

        /// <summary>
        /// Only replace elements if the string has a value, otherwise just ignore
        /// </summary>
        /// <param name="dst"></param>
        /// <param name="src"></param>
        private static Assets.Scripts.Objects.ToolUse CopyToolUse(Assets.Scripts.Objects.ToolUse dst, ToolExitStruct src)
        {
            //Debug.Log("CopyToolUse()");
            //Debug.Log($"ToolExit: {src.ToolExit}");
            if (src.ToolExit != "")
            {
                dst.ExitTime = src.EntryTime;
                dst.ToolExit = Prefab.Find<Assets.Scripts.Objects.Item>(src.ToolExit);
                dst.ExitQuantity = src.ExitQuantity;
            }
            CopyToolBasic(dst, src);
            return dst;
        }

        private static Assets.Scripts.Objects.UpgradePrefab CopyUpgradePrefab(Assets.Scripts.Objects.UpgradePrefab dst, UpgradePrefab src)
        {
            //Debug.Log("CopyUpgradePrefab()");
            //Debug.Log($"Prefab: {src.Prefab}");

            // Upgrade prefab
            if (src.Prefab != "")
            {
                dst.Prefab = Prefab.Find<Assets.Scripts.Objects.Structure>(src.Prefab);
            }

            // Upgrade Tools
            dst.UpgradeTools = CopyToolBasic(dst.UpgradeTools, src.UpgradeTools as ToolEntryStruct);
            return dst;
        }

        /// <summary>
        /// Cheap attempt to keep the number of BuildStates and BrokenBuildStates in Sync with the structure
        /// Unfortunatelly, I can't use any of the editor tools to refresh the inspector UI
        /// </summary>
        private void OnValidate()
        {
            //Debug.Log("OnValidate");
            Assets.Scripts.Objects.Structure structure = StructureParent;
            if (structure == null)
                return;

            //Debug.Log($"BuildState count {BuildStates.Count} {structure.BuildStates.Count}");
            // Remove extra BuildStates
            if (BuildStates.Count > structure.BuildStates.Count)
            {
                //Debug.Log("Removing BuildStates");
                BuildStates.RemoveRange(structure.BuildStates.Count, BuildStates.Count - structure.BuildStates.Count);
            }
            // Add extra BuildStates
            if (BuildStates.Count < structure.BuildStates.Count)
            {
                //Debug.Log("Adding BuildStatest");
                for (int i = BuildStates.Count; i < structure.BuildStates.Count; i++)
                {
                    BuildStates.Add(new ToolExitStruct());
                }
            }

            // Remove extra BuildStates
            if (BrokenBuildStates.Count > structure.BrokenBuildStates.Count)
            {
                //Debug.Log("Removing BrokenBuildStates");
                BrokenBuildStates.RemoveRange(structure.BrokenBuildStates.Count, BrokenBuildStates.Count - structure.BrokenBuildStates.Count);
            }
            // Add extra BrokenBuildStates
            if (BrokenBuildStates.Count < structure.BrokenBuildStates.Count)
            {
                //Debug.Log("Adding BrokenBuildStates");
                for (int i = BrokenBuildStates.Count; i < structure.BrokenBuildStates.Count; i++)
                {
                    BrokenBuildStates.Add(new BrokenBuildState());
                }
            }

        }
    }
}
