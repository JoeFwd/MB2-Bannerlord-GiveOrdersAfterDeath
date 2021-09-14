using System;
using SandBox;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace GiveOrdersAfterDeath
{
    public class TownThievingLogic : MissionLogic
    {
        /*public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);

            foreach (var VARIABLE in Mission.)
            {
                
            }
            
            if (agent.IsMainAgent || !agent.IsHuman)
            {
                return;
            }
            
            if (agent.GetComponent<CampaignAgentComponent>().AgentNavigator != null)
            {
                GiveOrdersAfterDeathSubModule.Print($"{agent.Name} : Setting DetectThievingBehaviour");
                AgentNavigator agentNavigator = agent.GetComponent<CampaignAgentComponent>().AgentNavigator;
                agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<DetectThievingBehaviour>();
            }
            else
            {
                GiveOrdersAfterDeathSubModule.Print($"{agent.Name} : AgentNavigator is null");
            }
        }*/

        public override void AfterStart()
        {
            base.AfterStart();
            
            foreach (Agent agent in Mission.Agents)
            {
                if (agent.IsMainAgent || !agent.IsHuman)
                {
                    continue;
                }
            
                if (agent.GetComponent<CampaignAgentComponent>().AgentNavigator != null)
                {
                    AgentNavigator agentNavigator = agent.GetComponent<CampaignAgentComponent>().AgentNavigator;
                    agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>().AddBehavior<DetectThievingBehaviour>();
                    GiveOrdersAfterDeathSubModule.Print($"{agent.Name} : Setting DetectThievingBehaviour");
                }
                else
                {
                    GiveOrdersAfterDeathSubModule.Print($"{agent.Name} : AgentNavigator is null");
                }
            }
        }

        public override void OnMissionTick(float dt) {
            base.OnMissionTick(dt);

            foreach (Agent agent in Mission.Agents)
            {
                if (agent.IsMainAgent || !agent.IsHuman)
                {
                    continue;
                }

                if (IsAgentSeeableByOtherAgent(Agent.Main, agent, 50f))
                {
                    GiveOrdersAfterDeathSubModule.Print($"You can be seen by {agent.Name}");
                }

                /*DetectThievingBehaviour detectThievingBehaviour = agentNavigator.GetBehaviorGroup<DailyBehaviorGroup>()?.GetBehavior<DetectThievingBehaviour>();
                if (detectThievingBehaviour == null)
                {
                    GiveOrdersAfterDeathSubModule.Print("detectThievingBehaviour is null");
                    return;
                }

                if (detectThievingBehaviour.IsMainAgentThieving)
                {
                    // InformationManager.DisplayMessage(new InformationMessage(agent.Character.Name + " Sees you !"));
                    GiveOrdersAfterDeathSubModule.Print($"{agent.Name} can see u");
                }*/
            }

        }
        
        private bool IsAgentSeeableByOtherAgent(Agent targetAgent, Agent agent, float distance)
        {
            float distanceFromTarget = (agent.Position - targetAgent.Position).Length;
            if (distanceFromTarget > distance)
            {
                return false;
            }
            
            if (IsObjectOrTerrainBetweenAgents(agent, targetAgent))
            {
                return false;
            }

            float eyeAngle = getEyeAngleBetweenAgentsFromLeftAgentPerspective(agent, targetAgent);
            GiveOrdersAfterDeathSubModule.Print($"$Angle for {agent.Name} is {eyeAngle} and distance {distanceFromTarget}");

            // If the target agent is less than X meters away from the agent, then the target agent can be seen
            // by agent, if the target agent is in front or less than Y° from the agent's eye sight.
            return eyeAngle <= 1.4f && (
                distanceFromTarget <= 5f
                || distanceFromTarget <= 10f && eyeAngle <= 1.3f
                || distanceFromTarget <= 12.5f && eyeAngle <= 1.15f
                || distanceFromTarget <= 15f && eyeAngle <= 1f
                || distanceFromTarget <= 17.5f && eyeAngle <= 0.85f
                || distanceFromTarget <= 20f && eyeAngle <= 0.75f
                || distanceFromTarget <= 30f && eyeAngle <= 0.60f
                || eyeAngle <= 0.5f);
        }

        private bool IsObjectOrTerrainBetweenAgents(Agent leftAgent, Agent rightAgent)
        {
            return Mission.Scene.RayCastForClosestEntityOrTerrain(leftAgent.GetEyeGlobalPosition(), rightAgent.GetEyeGlobalPosition(), out float _);   
        }

        // 1.5 -> right agent is completely next to left agent (ignoring distance)
        // 0 -> right agent is completely in front of left agent (ignoring distance)
        // 3 -> right agent is completely behind of left agent (ignoring distance)
        private float getEyeAngleBetweenAgentsFromLeftAgentPerspective(Agent leftAgent, Agent rightAgent)
        {
            return Math.Abs(Vec3.AngleBetweenTwoVectors(rightAgent.Position - leftAgent.Position, leftAgent.LookDirection));
        }

    }
    
    
}