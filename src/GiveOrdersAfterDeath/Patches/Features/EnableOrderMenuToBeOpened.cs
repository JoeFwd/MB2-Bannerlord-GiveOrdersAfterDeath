using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using static HarmonyLib.AccessTools;
namespace GiveOrdersAfterDeath
{
    public class EnableOrderMenuToBeOpened : IPatch
    {
        private static readonly MethodInfo CheckCanBeOpenedMethodInfo = typeof(MissionOrderVM).GetMethod("CheckCanBeOpened", all); // Enable order menu to be open
        
        private static readonly MethodInfo CheckCanBeOpenedPrefixMethodInfo = typeof(EnableOrderMenuToBeOpened).GetMethod(nameof(CheckCanBeOpenedPrefix), all);
        
        private static readonly MethodInfo CheckCanBeOpenedPostfixMethodInfo = typeof(EnableOrderMenuToBeOpened).GetMethod(nameof(CheckCanBeOpenedPostfix), all);
        
        public bool Applied { get; private set; }
        
        public void Apply()
        {
            if (Applied)
                return;

            GiveOrdersAfterDeathSubModule.Harmony.Patch(CheckCanBeOpenedMethodInfo,
                prefix: new HarmonyMethod(CheckCanBeOpenedPrefixMethodInfo),
                postfix: new HarmonyMethod(CheckCanBeOpenedPostfixMethodInfo));

            Applied = true;
        }
        
        public void Reset()
        {
            if (!Applied)
                return;

            GiveOrdersAfterDeathSubModule.Harmony.Unpatch(CheckCanBeOpenedMethodInfo, CheckCanBeOpenedPrefixMethodInfo);
            GiveOrdersAfterDeathSubModule.Harmony.Unpatch(CheckCanBeOpenedMethodInfo, CheckCanBeOpenedPostfixMethodInfo);
            Applied = false;
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