using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace GiveOrdersAfterDeath.Tests.Patches.Utilities
{
    public static class TranspilerPatchHelper
    {
        public static CodeInstructionDifference CodeInstructionDifferencesBetweenOriginalAndTranspiledMethod(
            IPatch patch,
            MethodInfo patchedMethod,
            MethodInfo transpilerMethod)
        {
            List<CodeInstruction> originalInstructions = PatchProcessor.GetOriginalInstructions(patchedMethod);
            List<CodeInstruction> codeInstructionList;
            
            patch.Apply();
            codeInstructionList = (List<CodeInstruction>) transpilerMethod.Invoke(null, new object[] { originalInstructions });
            
            return new CodeInstructionDifference(originalInstructions, codeInstructionList);
        }
    }
}