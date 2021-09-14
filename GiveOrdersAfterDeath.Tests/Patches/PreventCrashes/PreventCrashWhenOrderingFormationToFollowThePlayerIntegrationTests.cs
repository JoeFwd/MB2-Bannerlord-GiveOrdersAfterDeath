using GiveOrdersAfterDeath.Patches.PreventCrashes;
using HarmonyLib;
using NUnit.Framework;
using System.Linq;

namespace GiveOrdersAfterDeath.Tests.Patches.PreventCrashes
{
    public class PreventCrashWhenOrderingFormationToFollowThePlayerIntegrationTests
    {
        private Harmony CreateDefaultHarmony()
        {
            return new Harmony(nameof (PreventCrashWhenOrderingFormationToFollowThePlayerIntegrationTests));
        }

        private PreventCrashWhenOrderingFormationToFollowThePlayer CreateDefaultPatch()
        {
            return new PreventCrashWhenOrderingFormationToFollowThePlayer(CreateDefaultHarmony());
        }

        [Test]
        public void MissionOrderVM_OnOrder_Exists()
        {
            Assert.IsNotNull(CreateDefaultPatch().GetPatchMethodsInfo().ElementAt(0).PatchedMethod);
        }
    }
}