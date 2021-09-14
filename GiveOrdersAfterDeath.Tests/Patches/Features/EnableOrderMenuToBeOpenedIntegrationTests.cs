using GiveOrdersAfterDeath.Patches.Features;
using HarmonyLib;
using NUnit.Framework;
using System.Linq;

namespace GiveOrdersAfterDeath.Tests.Patches.Features
{
    public class EnableOrderMenuToBeOpenedIntegrationTests
    {
        private Harmony CreateDefaultHarmony()
        {
            return new Harmony(nameof (EnableOrderMenuToBeOpenedIntegrationTests));
        }

        private EnableOrderMenuToBeOpened CreateDefaultPatch()
        {
            return new EnableOrderMenuToBeOpened(CreateDefaultHarmony());
        }

        [Test]
        public void MissionOrderVM_CheckCanBeOpened_Exists()
        {
            Assert.IsNotNull(CreateDefaultPatch().GetPatchMethodsInfo().ElementAt(0).PatchedMethod);
        }
    }
}