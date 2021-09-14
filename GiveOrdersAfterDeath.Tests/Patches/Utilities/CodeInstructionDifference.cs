using HarmonyLib;
using System.Collections.Generic;
using System.Linq;

namespace GiveOrdersAfterDeath.Tests.Patches.Utilities
{
    public class CodeInstructionDifference
    {
        public CodeInstructionDifference(
            IEnumerable<CodeInstruction> originalCodeInstructions,
            IEnumerable<CodeInstruction> transpiledCodeInstructions)
        {
            this.InsertedCodeInstructions = transpiledCodeInstructions.ToList<CodeInstruction>().Except<CodeInstruction>(originalCodeInstructions);
            this.RemovedCodeInstructions = originalCodeInstructions.ToList<CodeInstruction>().Except<CodeInstruction>(transpiledCodeInstructions);
        }

        public IEnumerable<CodeInstruction> RemovedCodeInstructions { get; }

        public IEnumerable<CodeInstruction> InsertedCodeInstructions { get; }
    }
}