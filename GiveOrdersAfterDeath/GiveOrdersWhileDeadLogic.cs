using TaleWorlds.MountAndBlade;

namespace GiveOrdersAfterDeath
{
    public class GiveOrdersWhileDeadLogic : MissionLogic
    {
        public override bool IsOrderShoutingAllowed()
        {
            return Mission.MainAgent != null;
        }
    }
}