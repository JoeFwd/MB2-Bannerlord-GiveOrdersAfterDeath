using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using TaleWorlds.MountAndBlade.ViewModelCollection.Order;
using static HarmonyLib.AccessTools;

namespace GiveOrdersAfterDeath
{
    public class PreventCrashWhenOrderingFormationToFollowThePlayer : IPatch
    {
        private static readonly MethodInfo SetOrderWithAgentMethodInfo = 
            typeof(MissionOrderVM).GetMethod("OnOrder", all); // Prevent crash when Agent.Main is called
        
        private static readonly MethodInfo SetOrderWithNonNullAgentMethodInfo = 
            typeof(PreventCrashWhenOrderingFormationToFollowThePlayer).GetMethod(nameof(DisableFollowMeOrderWhenPlayerIsDead), all);
        
        public bool Applied { get; private set; }
        
        public void Apply(Game game)
        {
            if (Applied)
                return;
            
            GiveOrdersAfterDeathSubModule.Harmony.Patch(SetOrderWithAgentMethodInfo,
                prefix: new HarmonyMethod(SetOrderWithNonNullAgentMethodInfo));

            Applied = true;
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
        
    }
}