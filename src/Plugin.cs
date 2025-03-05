using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;

#if BEPINEX
using BepInEx;

namespace ReachForceChecker {
    [BepInPlugin("com.github.Kaden5480.poy-reach-force-checker", "ReachForceChecker", PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin {
        public void Awake() {
            Harmony.CreateAndPatchAll(typeof(PatchReachForceL));
            Harmony.CreateAndPatchAll(typeof(PatchReachForceR));

            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.sceneUnloaded += OnSceneUnloaded;

            CommonAwake();
        }

        public void OnDestroy() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneUnloaded -= OnSceneUnloaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode) {
            CommonSceneLoad();
        }

        private void OnSceneUnloaded(Scene scene) {
            CommonSceneUnload();
        }

        public void Update() {
            CommonUpdate();
        }

#elif MELONLOADER
using MelonLoader;

[assembly: MelonInfo(typeof(ReachForceChecker.Plugin), "ReachForceChecker", PluginInfo.PLUGIN_VERSION, "Kaden5480")]
[assembly: MelonGame("TraipseWare", "Peaks of Yore")]

namespace ReachForceChecker {
    public class Plugin : MelonMod {
        public override void OnInitializeMelon() {
            CommonAwake();
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName) {
            CommonSceneLoad();
        }

        public override void OnSceneWasUnloaded(int buildIndex, string sceneName) {
            CommonSceneUnload();
        }

        public override void OnUpdate() {
            CommonUpdate();
        }

#endif

        public static bool addedForceL = false;
        public static bool addedForceR = false;

        private AudioSource ding;

        private void ResetChecks() {
            addedForceL = false;
            addedForceR = false;
        }

        private void CommonAwake() {
            ding = gameObject.AddComponent<AudioSource>();
            ding.volume = 0.12f;
        }

        private void CommonSceneLoad() {
            TimeAttack timeAttack = GameObject.FindObjectOfType<TimeAttack>();
            if (timeAttack == null) {
                return;
            }

            ding.clip = timeAttack.s_reachSummit;
        }

        private void CommonSceneUnload() {
            ResetChecks();
            ding.clip = null;
        }

        private void CommonUpdate() {
            if (addedForceL == true || addedForceR == true) {
                ResetChecks();
                ding.Play();
            }
        }
    }

    [HarmonyPatch(typeof(Climbing), "AddReachForceL")]
    static class PatchReachForceL {
        static void Prefix() {
            Plugin.addedForceL = true;
        }
    }

    [HarmonyPatch(typeof(Climbing), "AddReachForceR")]
    static class PatchReachForceR {
        static void Prefix() {
            Plugin.addedForceR = true;
        }
    }
}
