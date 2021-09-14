using SandBox;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace GiveOrdersAfterDeath
{
    public class DetectThievingBehaviour : AgentBehavior
    {

        public bool IsMainAgentThieving { private set; get; }
        
        public DetectThievingBehaviour(AgentBehaviorGroup behaviorGroup) : base(behaviorGroup)
        {
            IsMainAgentThieving = false;
        }

        public override void Tick(float dt, bool isSimulation)
        {
            base.Tick(dt, isSimulation);

            if (OwnerAgent.IsMainAgent || !Navigator.CanSeeAgent(Agent.Main))
            {
                IsMainAgentThieving = false;
                return;
            }

            IsMainAgentThieving = true;
            InformationManager.DisplayMessage(new InformationMessage(OwnerAgent.Character.Name + " Sees you !"));
        }

        public override string GetDebugInfo()
        {
            return "";
        }
    }
}