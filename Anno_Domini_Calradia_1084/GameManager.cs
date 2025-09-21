using System;
using SandBox;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.CampaignSystem.Encounters;
using TaleWorlds.CampaignSystem.GameState;
using TaleWorlds.CampaignSystem.Party;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.ModuleManager;
using TaleWorlds.MountAndBlade;
using TaleWorlds.SaveSystem.Load;

namespace Anno_Domini_Calradia_1084.CC
{
    internal class GameManager_AD : SandBoxGameManager
    {
        private bool _loadingSavedGame;
        private LoadResult _loadedGameResult;

        public GameManager_AD()
        {
            this._loadingSavedGame = false;
        }

        public GameManager_AD(LoadResult loadedGameResult)
        {
            this._loadingSavedGame = true;
            this._loadedGameResult = loadedGameResult;
        }

        public override void OnLoadFinished()
        {
            if (!this._loadingSavedGame)
            {
                if (!Game.IsDevelopmentMode)
                {
                    var videoPlaybackState = Game.GameStateManager.CreateState<VideoPlaybackState>();
                    string path = ModuleHelper.GetModuleFullPath("SandBox") + "Videos/CampaignIntro/";
                    string subtitleFileBasePath = path + "campaign_intro";
                    string videoPath = path + "campaign_intro.ivf";
                    string audioPath = path + "campaign_intro.ogg";
                    videoPlaybackState.SetStartingParameters(videoPath, audioPath, subtitleFileBasePath, 30f, true);
                    videoPlaybackState.SetOnVideoFinisedDelegate(new Action(this.LaunchSandboxCharacterCreation));
                    Game.GameStateManager.CleanAndPushState(videoPlaybackState, 0);
                }
                else
                {
                    this.LaunchSandboxCharacterCreation();
                }
            }
            else
            {
                Game.GameStateManager.OnSavedGameLoadFinished();
                Game.GameStateManager.CleanAndPushState(Game.GameStateManager.CreateState<MapState>(), 0);

                var mapState = Game.GameStateManager.ActiveState as MapState;
                string menuId = mapState?.GameMenuId;

                if (!string.IsNullOrEmpty(menuId))
                {
                    var playerEncounter = PlayerEncounter.Current;
                    playerEncounter?.OnLoad();
                    Campaign.Current.GameMenuManager.SetNextMenu(menuId);
                }

                PartyBase.MainParty.SetVisualAsDirty();
                Campaign.Current.CampaignInformationManager.OnGameLoaded();
                foreach (var settlement in Settlement.All)
                {
                    settlement.Party.SetLevelMaskIsDirty();
                }

                CampaignEventDispatcher.Instance.OnGameLoadFinished();

                if (mapState != null)
                {
                    mapState.OnLoadingFinished();
                }
            }

            base.IsLoaded = true;
        }

        private void LaunchSandboxCharacterCreation()
        {
            var gameState = Game.GameStateManager.CreateState<CharacterCreationState>(new object[]
            {
                new CharacterCreation_AD()
            });
            Game.GameStateManager.CleanAndPushState(gameState, 0);
        }
    }
}
