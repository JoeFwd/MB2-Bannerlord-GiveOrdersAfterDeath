using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;
using static HarmonyLib.AccessTools;
using static System.Reflection.Emit.OpCodes;

namespace GiveOrdersAfterDeath
{
    public class PreventCrashWhenOrderingFormationToFaceEnemy : IPatch
    {
        private static readonly MethodInfo OnMissionScreenTickMethodInfo = 
            typeof(MissionOrderGauntletUIHandler).GetMethod("OnMissionScreenTick", all); // Prevent crash when Agent.Main is called
        
        private static readonly MethodInfo ReplaceMainAgentTeamByPlayerTeamTranspilerMethodInfo = 
            typeof(PreventCrashWhenOrderingFormationToFaceEnemy).GetMethod(nameof(ReplaceMainAgentTeamByPlayerTeamTranspiler), all);
        
        public bool Applied { get; private set; }
        
        public void Apply()
        {
            if (Applied)
                return;
            
            GiveOrdersAfterDeathSubModule.Harmony.Patch(OnMissionScreenTickMethodInfo,
                transpiler: new HarmonyMethod(ReplaceMainAgentTeamByPlayerTeamTranspilerMethodInfo));

            Applied = true;
        }
        
        public void Reset()
        {
            if (!Applied)
                return;

            GiveOrdersAfterDeathSubModule.Harmony.Unpatch(OnMissionScreenTickMethodInfo, ReplaceMainAgentTeamByPlayerTeamTranspilerMethodInfo);
            Applied = false;
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