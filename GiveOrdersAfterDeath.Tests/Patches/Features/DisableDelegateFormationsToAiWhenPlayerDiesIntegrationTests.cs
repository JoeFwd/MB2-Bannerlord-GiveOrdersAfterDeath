using GiveOrdersAfterDeath.Patches.Features;
using HarmonyLib;
using NUnit.Framework;
using System.Linq;
using System.Reflection;
using GiveOrdersAfterDeath.Tests.Patches.Utilities;

namespace GiveOrdersAfterDeath.Tests.Patches.Features
{
    public class DisableDelegateFormationsToAiWhenPlayerDiesIntegrationTests
    {
        private Harmony CreateDefaultHarmony()
        {
            return new Harmony(nameof (DisableDelegateFormationsToAiWhenPlayerDiesIntegrationTests));
        }

        private DisableDelegateFormationsToAiWhenPlayerDies CreateDefaultPatch()
        {
            Harmony harmony = CreateDefaultHarmony();
            return new DisableDelegateFormationsToAiWhenPlayerDies(harmony);
        }

        [Test]
        public void Mission_OnAgentRemoved_Exists()
        {
            Assert.IsNotNull(CreateDefaultPatch().GetPatchMethodsInfo().ElementAt(0).PatchedMethod);
        }

        [Test]
        public void TranspilerPatch_ShouldFindOnePatternOnly()
        {
            DisableDelegateFormationsToAiWhenPlayerDies defaultPatch = CreateDefaultPatch();
            MethodInfo patchedMethod = defaultPatch.GetPatchMethodsInfo().ElementAt(0).PatchedMethod;
            MethodInfo patchMethod = defaultPatch.GetPatchMethodsInfo().ElementAt(0).PatchMethod;
            
            CodeInstructionDifference codeInstructionDifference = TranspilerPatchHelper.CodeInstructionDifferencesBetweenOriginalAndTranspiledMethod(
                defaultPatch,
                patchedMethod,
                patchMethod
            );
            
            Assert.AreEqual(3, codeInstructionDifference.RemovedCodeInstructions.Count());
            Assert.AreEqual(0, codeInstructionDifference.InsertedCodeInstructions.Count());
        }
    }
}
