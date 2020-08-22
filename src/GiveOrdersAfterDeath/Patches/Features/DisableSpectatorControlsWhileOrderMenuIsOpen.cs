using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.View.Missions;
using TaleWorlds.MountAndBlade.View.Screen;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD;
using static HarmonyLib.AccessTools;

namespace GiveOrdersAfterDeath
{
    public class DisableSpectatorControlsWhileOrderMenuIsOpen : IPatch
    {
        private static readonly MethodInfo FindNextCameraAttachableAgentMethodInfo = typeof(MissionScreen).GetMethod("FindNextCameraAttachableAgent", all); // Disables Next Character when the order menu is opened.
        
        private static readonly MethodInfo FindNextCameraAttachableAgentPrefixMethodInfo = typeof(DisableSpectatorControlsWhileOrderMenuIsOpen).GetMethod(nameof(DisableSpectatorControlsPrefix), all);
        
        private static readonly MethodInfo OnMissionTickMethodInfo = typeof(MissionSpectatorControl).GetMethod("OnMissionTick", all); // Disables Next Character UI when the order menu is opened.
        
        private static readonly MethodInfo OnMissionTickPostfixMethodInfo = typeof(DisableSpectatorControlsWhileOrderMenuIsOpen).GetMethod(nameof(DisableNextCharacterSpectatorUiPostfix), all);
        
        public bool Applied { get; private set; }
        
        public void Apply()
        {
            if (Applied)
                return;

            GiveOrdersAfterDeathSubModule.Harmony.Patch(FindNextCameraAttachableAgentMethodInfo,
                new HarmonyMethod(FindNextCameraAttachableAgentPrefixMethodInfo));
            
            GiveOrdersAfterDeathSubModule.Harmony.Patch(OnMissionTickMethodInfo,
                postfix: new HarmonyMethod(OnMissionTickPostfixMethodInfo));

            Applied = true;
        }
        
        public void Reset()
        {
            if (!Applied)
                return;

            GiveOrdersAfterDeathSubModule.Harmony.Unpatch(FindNextCameraAttachableAgentMethodInfo, FindNextCameraAttachableAgentPrefixMethodInfo);
            GiveOrdersAfterDeathSubModule.Harmony.Unpatch(OnMissionTickMethodInfo, OnMissionTickPostfixMethodInfo);
            Applied = false;
        }
        
        private static bool DisableSpectatorControlsPrefix(MissionScreen __instance) {
            if (__instance?.Mission == null)
                return true;

            if (__instance.Mission.IsOrderMenuOpen)
                return false;

            return true;
        }
        
        private static void DisableNextCharacterSpectatorUiPostfix(MissionView __instance, ref MissionSpectatorControlVM ____dataSource) {
            if (____dataSource.IsEnabled && (!__instance?.Mission?.IsOrderMenuOpen ?? false))
                return;

            ____dataSource.IsEnabled = false;
        }
        
    }
}