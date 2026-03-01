using HarmonyLib;
using TaleWorlds.MountAndBlade;

// enables civiliant equipment for arena spectators
// native uses battle equipment

namespace Anno_Domini_Calradia_1084
{

    [HarmonyPatch(typeof(Mission), "SpawnAgent")]
    public static class MissionSpawnAgentPatch
    {
        public static void Prefix(AgentBuildData agentBuildData)
        {
            // Check if this is an audience spawn by looking at call stack
            var stackTrace = new System.Diagnostics.StackTrace();
            foreach (var frame in stackTrace.GetFrames())
            {
                if (frame.GetMethod()?.DeclaringType?.Name == "MissionAudienceHandler")
                {
                    agentBuildData.CivilianEquipment(true);
                    break;
                }
            }
        }
    }

}