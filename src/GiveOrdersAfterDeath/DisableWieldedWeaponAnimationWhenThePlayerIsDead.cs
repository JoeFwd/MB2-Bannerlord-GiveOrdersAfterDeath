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
    public class DisableWieldedWeaponAnimationWhenThePlayerIsDead : IPatch
    {
        private static readonly MethodInfo AfterSetOrderMethodInfo = typeof(OrderController).GetMethod("AfterSetOrder", all);
        
        private static readonly MethodInfo AfterSetOrderTranspilerMethodInfo = typeof(DisableWieldedWeaponAnimationWhenThePlayerIsDead).GetMethod(nameof(DisableWieldedWeaponAnimationTranspiler), all);
        
        public bool Applied { get; private set; }
        
        public void Apply(Game game)
        {
            if (Applied)
                return;

            GiveOrdersAfterDeathSubModule.Harmony.Patch(AfterSetOrderMethodInfo,
                transpiler: new HarmonyMethod(AfterSetOrderTranspilerMethodInfo));

            Applied = true;
        }

        private static bool IsMoreThanOneSelectedFormationCheckCalled(List<CodeInstruction> instructions, int index)
            => instructions[index].Calls(typeof(OrderController).GetMethod("AfterSetOrderMakeVoice", all))
               && instructions[index + 1].IsLdarg(0)
               && instructions[index + 2].LoadsField(typeof(OrderController).GetField("_selectedFormations", all))
               && instructions[index + 3].Calls(typeof(List<Formation>).GetMethod("get_Count", all))
               && instructions[index + 4].opcode == Ldc_I4_0
               && instructions[index + 5].opcode == Ble;

        private static bool IsMainAgentDead()
            => Agent.Main == null;

        private static readonly MethodInfo IsMainAgentDeadMethodInfo =
            typeof(DisableWieldedWeaponAnimationWhenThePlayerIsDead).GetMethod("IsMainAgentDead", all);

        private static List<CodeInstruction> ReturnIfMainAgentIsDead(CodeInstruction branchToFinalReturn)
        {
            var branchToFinalReturnIfTrue = new CodeInstruction(branchToFinalReturn);
            branchToFinalReturnIfTrue.opcode = Brtrue;
            
            var instructions = new List<CodeInstruction>(2)
            {
                new CodeInstruction(Call, IsMainAgentDeadMethodInfo),
                branchToFinalReturnIfTrue
            };

            return instructions;
        } 
        
        private static IEnumerable<CodeInstruction> DisableWieldedWeaponAnimationTranspiler(IEnumerable<CodeInstruction> instructions) 
        {
            var codes = instructions.ToList();
      
            for (var index = 0; index < codes.Count - 5; index++)
            {
                if (!IsMoreThanOneSelectedFormationCheckCalled(codes, index))
                    continue;

                var branchToFinalReturn = codes[index + 5];
                codes.InsertRange(index + 1, ReturnIfMainAgentIsDead(branchToFinalReturn));
                
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