using System;
using HarmonyLib;
using SandBox;
using SandBox.Source.Missions;
using TaleWorlds.MountAndBlade;

namespace GiveOrdersAfterDeath
{
    public partial class GiveOrdersAfterDeathSubModule : MBSubModuleBase
    {
        internal static readonly Harmony Harmony = new Harmony(nameof(GiveOrdersAfterDeath));

        protected override void OnSubModuleLoad()
        {
            try {
                Harmony.PatchAll(typeof(GiveOrdersAfterDeathSubModule).Assembly);
            }
            catch (Exception ex) {
                Error(ex, "Could not apply all generic attribute-based harmony patches.");
            }
            
            base.OnSubModuleLoad();
        }
        
        private static bool IsSiegeBattle(Mission mission) 
            => mission.GetMissionBehaviour<SiegeMissionController>() != null;


        private static bool IsCustomBattleGameType(Mission mission)
            => mission.GetMissionBehaviour<CustomBattleAgentLogic>() != null;
        
        private static bool IsVisitingTownCenter(Mission mission)
            => mission.GetMissionBehaviour<TownCenterMissionController>() != null
            && mission.GetMissionBehaviour<WorkshopMissionHandler>() != null;

        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            base.OnMissionBehaviourInitialize(mission);

            // Mission.Current.Is
            
            if (IsVisitingTownCenter(mission))
            {
                Print("Detected visiting town center");
                mission.AddMissionBehaviour(new TownThievingLogic());
            }
            else
            {
                Print("Is not visiting town center mission");
            }
            
            
            if (!mission.IsFieldBattle && !IsSiegeBattle(mission) && !IsCustomBattleGameType(mission))
            {
                if (ArePatchesApplied())
                {
                    ResetPatches();
                    Print($"{typeof(GiveOrdersAfterDeathSubModule)} : All patches reset");            
                }
                return;
            }

            if (!ArePatchesApplied())
            {
                ApplyPatches();
                Print($"{typeof(GiveOrdersAfterDeathSubModule)} : All patches applied");
            }
            
            mission.AddMissionBehaviour(new GiveOrdersWhileDeadLogic());
            Print($"{typeof(GiveOrdersAfterDeathSubModule)} : Added GiveOrdersWhileDeadLogic to mission");
        }
    }
}