using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GiveOrdersAfterDeath.Patches.Model;
using HarmonyLib;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;
using static HarmonyLib.AccessTools;
using static System.Reflection.Emit.OpCodes;
using static HarmonyLib.HarmonyPatchType;

namespace GiveOrdersAfterDeath.Patches.PreventCrashes
{
    public class PreventCrashWhenOrderingFormationToFaceEnemy : Patch<PreventCrashWhenOrderingFormationToFaceEnemy>
    {
        private static readonly MethodInfo OnMissionScreenTickMethodInfo = 
            typeof(MissionOrderGauntletUIHandler).GetMethod("OnMissionScreenTick", all); // Prevent crash when Agent.Main is called
        
        private static readonly MethodInfo ReplaceMainAgentTeamByPlayerTeamTranspilerMethodInfo = 
            typeof(PreventCrashWhenOrderingFormationToFaceEnemy).GetMethod(nameof(ReplaceMainAgentTeamByPlayerTeamTranspiler), all);
        
        public PreventCrashWhenOrderingFormationToFaceEnemy(Harmony harmony) : base(harmony) { }
        
        public override IEnumerable<PatchMethodInfo> GetPatchMethodsInfo()
        {
            yield return new PatchMethodInfo(OnMissionScreenTickMethodInfo, Transpiler, ReplaceMainAgentTeamByPlayerTeamTranspilerMethodInfo);
        }
        
        private static bool IsMissionMainAgentTeamCalled(List<CodeInstruction> instructions, int index)
            => instructions[index].opcode == Ldarg_0
               && instructions[index + 1].Calls(typeof(MissionBehaviour).GetMethod("get_Mission", all))
               && instructions[index + 2].Calls(typeof(Mission).GetMethod("get_MainAgent", all))
               && instructions[index + 3].Calls(typeof(Agent).GetMethod("get_Team", all));

        private static CodeInstruction MissionPlayerTeamInstruction()
            => new CodeInstruction(Callvirt, typeof(Mission).GetMethod("get_PlayerTeam", all));
        
        private static void ReplaceMissionMainAgentByMissionPlayerTeamInstruction(
            List<CodeInstruction> instructions, int index)
            => instructions[index] = MissionPlayerTeamInstruction();
        
        private static void RemoveAgentTeamInstruction(
            List<CodeInstruction> instructions, int index)
            => instructions.RemoveAt(index);
    
        private static IEnumerable<CodeInstruction> ReplaceMainAgentTeamByPlayerTeamTranspiler(IEnumerable<CodeInstruction> instructions) 
        {
            var codes = instructions.ToList();
      
            for (var index = 0; index < codes.Count - 4; index++) {
                if (IsMissionMainAgentTeamCalled(codes, index))
                {
                    ReplaceMissionMainAgentByMissionPlayerTeamInstruction(codes, index + 2);
                    RemoveAgentTeamInstruction(codes, index + 3);
                }
            }

            return codes.AsEnumerable();
        }
    }
}