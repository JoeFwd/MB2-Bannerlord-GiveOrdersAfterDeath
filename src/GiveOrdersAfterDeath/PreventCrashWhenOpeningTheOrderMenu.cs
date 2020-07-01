using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.ViewModelCollection;
using static HarmonyLib.AccessTools;
using static System.Reflection.Emit.OpCodes;

namespace GiveOrdersAfterDeath
{
    public class PreventCrashWhenOpeningTheOrderMenu : IPatch
    {
        private static readonly MethodInfo OpenToggleOrderMethodInfo = 
            typeof(MissionOrderVM).GetMethod("OpenToggleOrder", all); // Prevent crash when Agent.Main is called

        private static readonly MethodInfo OpenToggleOrderTranspilerMethodInfo = 
            typeof(PreventCrashWhenOpeningTheOrderMenu).GetMethod(nameof(OpenToggleOrderTranspiler), all);
        
        public bool Applied { get; private set; }
        
        public void Apply(Game game)
        {
            if (Applied)
                return;
            
            GiveOrdersAfterDeathSubModule.Harmony.Patch(OpenToggleOrderMethodInfo,
                transpiler: new HarmonyMethod(OpenToggleOrderTranspilerMethodInfo));

            Applied = true;
        }
        
        private static void SetIsCombatActionsDisabledIfAgentMainNotNull() {
            if (Agent.Main == null)
                return;
      
            Agent.Main.SetIsCombatActionsDisabled(true);
        }
        
        public static IEnumerable<CodeInstruction> OpenToggleOrderTranspiler(IEnumerable<CodeInstruction> instructions) 
        {
            foreach (var instruction in instructions) {
                if (instruction.Calls( typeof(Agent).GetMethod("get_Main", all))) {
                    yield return new CodeInstruction(Call, typeof(PreventCrashWhenOpeningTheOrderMenu).GetMethod(nameof(SetIsCombatActionsDisabledIfAgentMainNotNull), all));
                    yield return instructions.Last();
                    break;
                }
                yield return instruction;
            }
        }

    }
}