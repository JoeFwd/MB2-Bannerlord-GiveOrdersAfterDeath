using System.Linq;
using System.Reflection;
using GiveOrdersAfterDeath.Patches.PreventCrashes;
using GiveOrdersAfterDeath.Tests.Patches.Utilities;
using HarmonyLib;
using NUnit.Framework;

namespace GiveOrdersAfterDeath.Tests.Patches.PreventCrashes
{
    public class PreventCrashWhenOrderingFormationToFaceEnemyIntegrationTests
    {
        private Harmony CreateDefaultHarmony()
        {
            return new Harmony(nameof (PreventCrashWhenOrderingFormationToFaceEnemyIntegrationTests));
        }

        private PreventCrashWhenOrderingFormationToFaceEnemy CreateDefaultPatch()
        {
            return new PreventCrashWhenOrderingFormationToFaceEnemy(CreateDefaultHarmony());
        }

        [Test]
        public void OrderController_AfterSetOrder_Exists()
        {
            Assert.IsNotNull(CreateDefaultPatch().GetPatchMethodsInfo().ElementAt(0).PatchedMethod);
        }

        [Test]
        public void TranspilerPatch_ShouldFindOnePatternOnly()
        {
            PreventCrashWhenOrderingFormationToFaceEnemy defaultPatch = CreateDefaultPatch();
            MethodInfo patchedMethod = defaultPatch.GetPatchMethodsInfo().ElementAt(0).PatchedMethod;
            MethodInfo patchMethod = defaultPatch.GetPatchMethodsInfo().ElementAt(0).PatchMethod;
      
            CodeInstructionDifference codeInstructionDifference = TranspilerPatchHelper.CodeInstructionDifferencesBetweenOriginalAndTranspiledMethod(
                defaultPatch,
                patchedMethod,
                patchMethod
            );
      
            Assert.AreEqual(2, codeInstructionDifference.InsertedCodeInstructions.Count());
            Assert.AreEqual(4, codeInstructionDifference.RemovedCodeInstructions.Count());
        }
    }
}