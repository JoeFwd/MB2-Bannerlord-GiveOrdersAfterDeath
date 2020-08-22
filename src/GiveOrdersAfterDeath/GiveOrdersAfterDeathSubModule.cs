using System;
using HarmonyLib;
using TaleWorlds.Core;
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

#if AFTER_E1_4_3
        private static bool IsCustomBattleGameType(Mission mission)
            => mission.GetMissionBehaviour<CustomBattleAgentLogic>() != null;
#else
        private static bool IsCustomBattleGameType()
            => Game.Current.GameType == CustomGame.Current;
#endif
        
        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            base.OnMissionBehaviourInitialize(mission);
            
#if AFTER_E1_4_3
            if (!mission.IsFieldBattle && !IsSiegeBattle(mission) && !IsCustomBattleGameType(mission))
#else
            if (!mission.IsFieldBattle && !IsSiegeBattle(mission) && !IsCustomBattleGameType())
#endif
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