using System;
using HarmonyLib;
using TaleWorlds.CampaignSystem.MapEvents;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace Anno_Domini_Calradia_1084.Patches
{
    /// <summary>
    /// Postfix on Mission.AfterStart — spawns livestock at existing scene
    /// markers (sp_sheep, sp_cow, sp_hog, sp_goose, sp_chicken) during
    /// battles/raids that take place in village scenes.
    ///
    /// Self-limiting: only fires when MapEvent exists (battle, not visit)
    /// and scene has markers (village scene, not open field). Peaceful
    /// visits are unaffected — vanilla handles those separately.
    /// </summary>
    [HarmonyPatch(typeof(Mission), "AfterStart")]
    public static class VillageBattleLivestockPatch
    {
        private static readonly (string tag, string itemId)[] AnimalSpawnTags =
        {
            ("sp_sheep", "sheep"),
            ("sp_cow", "cow"),
            ("sp_hog", "hog"),
            ("sp_goose", "goose"),
            ("sp_chicken", "chicken")
        };

        [HarmonyPostfix]
        public static void Postfix()
        {
            try
            {
                // Only in battles — peaceful visits are handled by vanilla
                if (MapEvent.PlayerMapEvent == null)
                    return;

                Mission mission = Mission.Current;
                if (mission == null)
                    return;

                Scene scene = mission.Scene;
                if (scene == null)
                    return;

                foreach (var (tag, itemId) in AnimalSpawnTags)
                {
                    ItemObject animalItem = Game.Current.ObjectManager.GetObject<ItemObject>(itemId);
                    if (animalItem == null)
                        continue;

                    foreach (GameEntity marker in scene.FindEntitiesWithTag(tag))
                    {
                        SpawnAnimalAtMarker(mission, marker, animalItem);
                    }
                }

                // Horses/mules/camels — item type comes from second tag on each marker
                foreach (GameEntity marker in scene.FindEntitiesWithTag("sp_horse"))
                {
                    if (marker.Tags.Length < 2)
                        continue;

                    ItemObject horseItem = Game.Current.ObjectManager.GetObject<ItemObject>(marker.Tags[1]);
                    if (horseItem == null || !horseItem.HasHorseComponent)
                        continue;

                    SpawnAnimalAtMarker(mission, marker, horseItem);
                }
            }
            catch (Exception ex)
            {
                Main.DebugLog("ERROR in VillageBattleLivestockPatch: " + ex.Message);
            }
        }

        private static void SpawnAnimalAtMarker(Mission mission, GameEntity marker, ItemObject animalItem)
        {
            MatrixFrame frame = marker.GetGlobalFrame();
            frame.rotation.OrthonormalizeAccordingToForwardAndKeepUpAsZAxis();

            ItemRosterElement mountElement = new ItemRosterElement(animalItem, 0, null);
            ItemRosterElement harnessElement = default(ItemRosterElement);

            Agent agent = mission.SpawnMonster(mountElement, harnessElement,
                frame.origin, frame.rotation.f.AsVec2, -1);

            agent.SetAgentFlags(agent.GetAgentFlags() | AgentFlag.CanWander);
            SimulateAnimalAnimations(agent);
        }

        /// <summary>
        /// Mirrors SandBoxHelpers.MissionHelper.SimulateAnimalAnimations (private).
        /// Ticks the animation forward 10-100 frames so animals don't all
        /// start in the same pose.
        /// </summary>
        private static void SimulateAnimalAnimations(Agent agent)
        {
            int ticks = 10 + MBRandom.RandomInt(90);
            for (int i = 0; i < ticks; i++)
            {
                agent.TickActionChannels(0.1f);
                Vec3 displacement = agent.ComputeAnimationDisplacement(0.1f);
                if (displacement.LengthSquared > 0f)
                    agent.TeleportToPosition(agent.Position + displacement);
                agent.AgentVisuals.GetSkeleton().TickAnimations(0.1f,
                    agent.AgentVisuals.GetGlobalFrame(), true);
            }
        }
    }
}