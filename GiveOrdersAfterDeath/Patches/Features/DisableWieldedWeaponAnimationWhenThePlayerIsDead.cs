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
    public class DisableWieldedWeaponAnimationWhenThePlayerIsDead : Patch<DisableWieldedWeaponAnimationWhenThePlayerIsDead>
    {
        private static readonly MethodInfo AfterSetOrderMethodInfo = typeof(OrderController).GetMethod("AfterSetOrder", all);
        
        private static readonly MethodInfo AfterSetOrderTranspilerMethodInfo = typeof(DisableWieldedWeaponAnimationWhenThePlayerIsDead).GetMethod(nameof(DisableWieldedWeaponAnimationTranspiler), all);
        
        public DisableWieldedWeaponAnimationWhenThePlayerIsDead(Harmony harmony) : base(harmony) { }
        
        public override IEnumerable<PatchMethodInfo> GetPatchMethodsInfo()
        {
            yield return new PatchMethodInfo(AfterSetOrderMethodInfo, Transpiler, AfterSetOrderTranspilerMethodInfo);
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
                
                return codes.AsEnumerable();
            }

            return codes.AsEnumerable();
        }
    }
    
}