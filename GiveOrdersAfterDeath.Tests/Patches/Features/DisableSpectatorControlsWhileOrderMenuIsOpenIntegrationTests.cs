using GiveOrdersAfterDeath.Patches.Features;
using HarmonyLib;
using NUnit.Framework;
using System.Linq;

namespace GiveOrdersAfterDeath.Tests.Patches.Features
{
    public class DisableSpectatorControlsWhileOrderMenuIsOpenIntegrationTests
    {
        private Harmony CreateDefaultHarmony()
        {
            return new Harmony(nameof (DisableSpectatorControlsWhileOrderMenuIsOpenIntegrationTests));
        }

        private DisableSpectatorControlsWhileOrderMenuIsOpen CreateDefaultPatch()
        {
            return new DisableSpectatorControlsWhileOrderMenuIsOpen(CreateDefaultHarmony());
        }

        [Test]
        public void MissionScreen_FindNextCameraAttachableAgent_Exists()
        {
            Assert.IsNotNull(CreateDefaultPatch().GetPatchMethodsInfo().ElementAt(0).PatchedMethod);
        }

        [Test]
        public void MissionSpectatorControl_OnMissionTick_Exists()
        {
            Assert.IsNotNull(CreateDefaultPatch().GetPatchMethodsInfo().ElementAt(1).PatchedMethod);
        }
    }
}