using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using static HarmonyLib.AccessTools;
using static System.Reflection.Emit.OpCodes;


namespace GiveOrdersAfterDeath
{
    public class DisableDelegateFormationsToAiWhenPlayerDies : IPatch
    {
        private static readonly MethodInfo OnAgentRemovedMethodInfo = typeof(Mission).GetMethod("OnAgentRemoved", all);
        
        private static readonly MethodInfo OnAgentRemovedTranspilerMethodInfo = typeof(DisableDelegateFormationsToAiWhenPlayerDies).GetMethod(nameof(DisableDelegateFormationsToAiTranspiler), all);
        
        public bool Applied { get; private set; }
        
        public void Apply(Game game)
        {
            if (Applied)
                return;

            GiveOrdersAfterDeathSubModule.Harmony.Patch(OnAgentRemovedMethodInfo,
                transpiler: new HarmonyMethod(OnAgentRemovedTranspilerMethodInfo));

            Applied = true;
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

                foreach (var code in codes)
                {
                    GiveOrdersAfterDeathSubModule.Print($"opcode : {code.opcode} operand : {code.operand}");
                }
                
                return codes.AsEnumerable();
            }

            return codes.AsEnumerable();
        }
        
    }
    
}