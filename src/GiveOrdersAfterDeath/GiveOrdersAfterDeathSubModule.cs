using System;
using HarmonyLib;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace GiveOrdersAfterDeath
{
    public partial class GiveOrdersAfterDeathSubModule : MBSubModuleBase
    {
        internal static readonly Harmony Harmony = new Harmony(nameof(GiveOrdersAfterDeath));

        protected override void OnSubModuleLoad() {
            try {
                Harmony.PatchAll(typeof(GiveOrdersAfterDeathSubModule).Assembly);
            }
            catch (Exception ex) {
                Error(ex, "Could not apply all generic attribute-based harmony patches.");
            }

            base.OnSubModuleLoad();
        }
        
        public override void OnGameInitializationFinished(Game game) {
            ApplyPatches(game);

            base.OnGameInitializationFinished(game);
        }

        private static bool IsSiegeBattle(Mission mission) 
            => mission.GetMissionBehaviour<SiegeMissionController>() != null;

        private static bool IsCustomBattleGameType()
            => Game.Current.GameType == CustomGame.Current;
        
        public override void OnMissionBehaviourInitialize(Mission mission)
        {
            base.OnMissionBehaviourInitialize(mission);

            if (!mission.IsFieldBattle && !IsSiegeBattle(mission) && !IsCustomBattleGameType())
                return;
            
            mission.AddMissionBehaviour(new GiveOrdersWhileDeadLogic());

            Print($"{typeof(GiveOrdersAfterDeathSubModule)} : Added GiveOrdersWhileDeadLogic to mission");
        }
    }
}