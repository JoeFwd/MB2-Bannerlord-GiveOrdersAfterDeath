using System.Collections.Generic;
using System.Reflection;
using GiveOrdersAfterDeath.Patches.Model;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;
using static HarmonyLib.AccessTools;
using static HarmonyLib.HarmonyPatchType;

namespace GiveOrdersAfterDeath.Patches.PreventCrashes
{
    public class PreventCrashWhenOrderingFormationToFollowThePlayer : Patch<PreventCrashWhenOrderingFormationToFollowThePlayer>
    {
        private static readonly MethodInfo SetOrderWithAgentMethodInfo = 
            typeof(OrderController).GetMethod("SetOrderWithAgent", all); // Prevent crash when Agent.Main is called
        
        private static readonly MethodInfo SetOrderWithNonNullAgentMethodInfo = 
            typeof(PreventCrashWhenOrderingFormationToFollowThePlayer).GetMethod(nameof(DisableFollowMeOrderWhenPlayerIsDead), all);
        
        public override IEnumerable<PatchMethodInfo> GetPatchMethodsInfo()
        {
            yield return new PatchMethodInfo(SetOrderWithAgentMethodInfo, Prefix, SetOrderWithNonNullAgentMethodInfo);
        }

        private static readonly MethodInfo OrderSubTypeGetter = typeof(OrderItemVM).GetMethod("get_OrderSubType", all);
        
        private static bool DisableFollowMeOrderWhenPlayerIsDead(Agent agent)
        {
            if (agent == null)
            {
                TextObject textObject = new TextObject("Cannot issue the order because the target is dead.");
                InformationManager.DisplayMessage(new InformationMessage(textObject.ToString()));
                return false;
            }
            return true;
        }

        public PreventCrashWhenOrderingFormationToFollowThePlayer(Harmony harmony) : base(harmony)
        {
        }
    }
}