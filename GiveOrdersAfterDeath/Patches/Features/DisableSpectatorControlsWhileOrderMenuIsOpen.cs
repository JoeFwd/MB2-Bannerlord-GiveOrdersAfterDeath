using System.Collections.Generic;
using System.Reflection;
using GiveOrdersAfterDeath.Patches.Model;
using HarmonyLib;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.View.Missions;
using TaleWorlds.MountAndBlade.View.Screen;
using TaleWorlds.MountAndBlade.ViewModelCollection.HUD;
using static HarmonyLib.AccessTools;
using static HarmonyLib.HarmonyPatchType;

namespace GiveOrdersAfterDeath.Patches.Features
{
    public class DisableSpectatorControlsWhileOrderMenuIsOpen : Patch<DisableSpectatorControlsWhileOrderMenuIsOpen>
    {
        private static readonly MethodInfo FindNextCameraAttachableAgentMethodInfo = typeof(MissionScreen).GetMethod("FindNextCameraAttachableAgent", all); // Disables Next Character when the order menu is opened.
        
        private static readonly MethodInfo FindNextCameraAttachableAgentPrefixMethodInfo = typeof(DisableSpectatorControlsWhileOrderMenuIsOpen).GetMethod(nameof(DisableSpectatorControlsPrefix), all);
        
        private static readonly MethodInfo OnMissionTickMethodInfo = typeof(MissionSpectatorControl).GetMethod("OnMissionTick", all); // Disables Next Character UI when the order menu is opened.
        
        private static readonly MethodInfo OnMissionTickPostfixMethodInfo = typeof(DisableSpectatorControlsWhileOrderMenuIsOpen).GetMethod(nameof(DisableNextCharacterSpectatorUiPostfix), all);
        
        public DisableSpectatorControlsWhileOrderMenuIsOpen(Harmony harmony) : base(harmony) { }
        
        public override IEnumerable<PatchMethodInfo> GetPatchMethodsInfo()
        {
            yield return new PatchMethodInfo(FindNextCameraAttachableAgentMethodInfo, Prefix, FindNextCameraAttachableAgentPrefixMethodInfo);
            yield return new PatchMethodInfo(OnMissionTickMethodInfo, Postfix, OnMissionTickPostfixMethodInfo);
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