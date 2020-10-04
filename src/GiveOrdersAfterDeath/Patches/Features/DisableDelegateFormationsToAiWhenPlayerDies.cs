using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GiveOrdersAfterDeath.Patches.Model;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using static HarmonyLib.AccessTools;
using static System.Reflection.Emit.OpCodes;
using static HarmonyLib.HarmonyPatchType;

namespace GiveOrdersAfterDeath.Patches.Features
{
    public class DisableDelegateFormationsToAiWhenPlayerDies : Patch<DisableDelegateFormationsToAiWhenPlayerDies>
    {
        private static readonly MethodInfo OnAgentRemovedMethodInfo = typeof(Mission).GetMethod("OnAgentRemoved", all);
        
        private static readonly MethodInfo OnAgentRemovedTranspilerMethodInfo = typeof(DisableDelegateFormationsToAiWhenPlayerDies).GetMethod(nameof(DisableDelegateFormationsToAiTranspiler), all);
        
        public DisableDelegateFormationsToAiWhenPlayerDies(Harmony harmony) : base(harmony) { }
        
        public override IEnumerable<PatchMethodInfo> GetPatchMethodsInfo()
        {
            yield return new PatchMethodInfo(OnAgentRemovedMethodInfo, Transpiler, OnAgentRemovedTranspilerMethodInfo);
        }

        private static bool IsDelegateToAICalled(List<CodeInstruction> instructions, int index)
            => instructions[index].opcode == Ldarg_1
               && instructions[index + 1].Calls(typeof(Agent).GetMethod("get_Team", all))
               && instructions[index + 2].Calls(typeof(Team).GetMethod("DelegateCommandToAI", all));

        private static IEnumerable<CodeInstruction> DisableDelegateFormationsToAiTranspiler(IEnumerable<CodeInstruction> instructions) 
        {
            var codes = instructions.ToList();
      
            for (var index = 0; index < codes.Count - 2; index++)
            {
                if (!IsDelegateToAICalled(codes, index))
                    continue;
                
                codes.RemoveRange(index, 3);

                return codes.AsEnumerable();
            }

            return codes.AsEnumerable();
        }
    }
    
}