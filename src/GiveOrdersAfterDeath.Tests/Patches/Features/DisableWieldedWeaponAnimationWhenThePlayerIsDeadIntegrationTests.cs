using GiveOrdersAfterDeath.Patches.Features;
using GiveOrdersAfterDeath.Tests.Patches.Utilities;
using HarmonyLib;
using NUnit.Framework;
using System.Linq;
using System.Reflection;

namespace GiveOrdersAfterDeath.Tests.Patches.Features
{
  public class DisableWieldedWeaponAnimationWhenThePlayerIsDeadIntegrationTests
  {
    private Harmony CreateDefaultHarmony()
    {
      return new Harmony(nameof (DisableWieldedWeaponAnimationWhenThePlayerIsDeadIntegrationTests));
    }

    private DisableWieldedWeaponAnimationWhenThePlayerIsDead CreateDefaultPatch()
    {
      return new DisableWieldedWeaponAnimationWhenThePlayerIsDead(CreateDefaultHarmony());
    }

    [Test]
    public void OrderController_AfterSetOrder_Exists()
    {
      Assert.IsNotNull(CreateDefaultPatch().GetPatchMethodsInfo().ElementAt(0).PatchedMethod);
    }

    [Test]
    public void TranspilerPatch_ShouldFindOnePatternOnly()
    {
      DisableWieldedWeaponAnimationWhenThePlayerIsDead defaultPatch = CreateDefaultPatch();
      MethodInfo patchedMethod = defaultPatch.GetPatchMethodsInfo().ElementAt(0).PatchedMethod;
      MethodInfo patchMethod = defaultPatch.GetPatchMethodsInfo().ElementAt(0).PatchMethod;
      
      CodeInstructionDifference codeInstructionDifference = TranspilerPatchHelper.CodeInstructionDifferencesBetweenOriginalAndTranspiledMethod(
        defaultPatch,
        patchedMethod,
        patchMethod
      );
      
      Assert.AreEqual(2, codeInstructionDifference.InsertedCodeInstructions.Count());
      Assert.AreEqual(0, codeInstructionDifference.RemovedCodeInstructions.Count());
    }
  }
}
