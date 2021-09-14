using System.Collections.Generic;
using System.Reflection;
using GiveOrdersAfterDeath.Patches.Model;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using static HarmonyLib.AccessTools;
using static HarmonyLib.HarmonyPatchType; 

namespace GiveOrdersAfterDeath.Patches.Features
{
    public class EnableOrderMenuToBeOpened : Patch<EnableOrderMenuToBeOpened>
    {
        private static readonly MethodInfo CheckCanBeOpenedMethodInfo = typeof(MissionOrderVM).GetMethod("CheckCanBeOpened", all); // Enable order menu to be open
        
        private static readonly MethodInfo CheckCanBeOpenedPrefixMethodInfo = typeof(EnableOrderMenuToBeOpened).GetMethod(nameof(CheckCanBeOpenedPrefix), all);
        
        private static readonly MethodInfo CheckCanBeOpenedPostfixMethodInfo = typeof(EnableOrderMenuToBeOpened).GetMethod(nameof(CheckCanBeOpenedPostfix), all);
        
        public EnableOrderMenuToBeOpened(Harmony harmony) : base(harmony) { }
        
        public override IEnumerable<PatchMethodInfo> GetPatchMethodsInfo()
        {
            yield return new PatchMethodInfo(CheckCanBeOpenedMethodInfo, Prefix, CheckCanBeOpenedPrefixMethodInfo);
            yield return new PatchMethodInfo(CheckCanBeOpenedMethodInfo, Postfix, CheckCanBeOpenedPostfixMethodInfo);
        }

        private static void CheckCanBeOpenedPrefix(ref bool displayMessage)
        {
            if (Agent.Main != null)
                return;
            
            displayMessage = false;
        }
        
        private static void CheckCanBeOpenedPostfix(ref bool __result) {
            if (Agent.Main != null)
                return;
      
            __result = true;
        }
    }
}