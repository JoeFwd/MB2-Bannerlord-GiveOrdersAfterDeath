using System.Collections.Generic;
using System.Reflection;
using GiveOrdersAfterDeath.Patches.Model;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;
using static HarmonyLib.AccessTools;
using static HarmonyLib.HarmonyPatchType;

namespace GiveOrdersAfterDeath.Patches.PreventCrashes
{
    public class PreventCrashWhenOrderingFormationToFollowThePlayer : Patch<PreventCrashWhenOrderingFormationToFollowThePlayer>
    {
        private static readonly MethodInfo SetOrderWithAgentMethodInfo = 
            typeof(MissionOrderVM).GetMethod("OnOrder", all); // Prevent crash when Agent.Main is called
        
        private static readonly MethodInfo SetOrderWithNonNullAgentMethodInfo = 
            typeof(PreventCrashWhenOrderingFormationToFollowThePlayer).GetMethod(nameof(DisableFollowMeOrderWhenPlayerIsDead), all);
        
        public override IEnumerable<PatchMethodInfo> GetPatchMethodsInfo()
        {
            yield return new PatchMethodInfo(SetOrderWithAgentMethodInfo, Prefix, SetOrderWithNonNullAgentMethodInfo);
        }

        private static readonly MethodInfo OrderSubTypeGetter = typeof(OrderItemVM).GetMethod("get_OrderSubType", all);
        
        private static bool DisableFollowMeOrderWhenPlayerIsDead(MissionOrderVM __instance, Mission ____mission, OrderItemVM orderItem)
        {
            var orderSubType = OrderSubTypeGetter?.Invoke(orderItem, null).ToString() ?? "";
            if (____mission.MainAgent != null || orderSubType != "FollowMe")
                return true;
            
            TextObject textObject = new TextObject("Formations cannot follow a dead corpse.");
            InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
            __instance.CloseToggleOrder();

            return false;
        }

        public PreventCrashWhenOrderingFormationToFollowThePlayer(Harmony harmony) : base(harmony)
        {
        }
    }
}