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
    // Token: 0x02000006 RID: 6
    internal class GameManager_AD : SandBoxGameManager
    {
        // Token: 0x06000010 RID: 16 RVA: 0x000033F3 File Offset: 0x000015F3
        public GameManager_AD()
        {
            this._loadingSavedGame = false;
        }

        // Token: 0x06000011 RID: 17 RVA: 0x00003404 File Offset: 0x00001604
        public GameManager_AD(LoadResult loadedGameResult)
        {
            this._loadingSavedGame = true;
            this._loadedGameResult = loadedGameResult;
        }

        // Token: 0x06000012 RID: 18 RVA: 0x0000341C File Offset: 0x0000161C
        public override void OnLoadFinished()
        {
            bool flag = !this._loadingSavedGame;
            if (flag)
            {
                bool flag2 = !Game.Current.IsDevelopmentMode;
                if (flag2)
                {
                    VideoPlaybackState videoPlaybackState = Game.Current.GameStateManager.CreateState<VideoPlaybackState>();
                    string str = ModuleHelper.GetModuleFullPath("SandBox") + "Videos/CampaignIntro/";
                    string subtitleFileBasePath = str + "campaign_intro";
                    string videoPath = str + "campaign_intro.ivf";
                    string audioPath = str + "campaign_intro.ogg";
                    videoPlaybackState.SetStartingParameters(videoPath, audioPath, subtitleFileBasePath, 30f, true);
                    videoPlaybackState.SetOnVideoFinisedDelegate(new Action(this.LaunchSandboxCharacterCreation));
                    Game.Current.GameStateManager.CleanAndPushState(videoPlaybackState, 0);
                }
                else
                {
                    this.LaunchSandboxCharacterCreation();
                }
            }
            else
            {
                Game.Current.GameStateManager.OnSavedGameLoadFinished();
                Game.Current.GameStateManager.CleanAndPushState(Game.Current.GameStateManager.CreateState<MapState>(), 0);
                MapState mapState = Game.Current.GameStateManager.ActiveState as MapState;
                string text = (mapState != null) ? mapState.GameMenuId : null;
                bool flag3 = !string.IsNullOrEmpty(text);
                if (flag3)
                {
                    PlayerEncounter playerEncounter = PlayerEncounter.Current;
                    bool flag4 = playerEncounter != null;
                    if (flag4)
                    {
                        playerEncounter.OnLoad();
                    }
                    Campaign.Current.GameMenuManager.SetNextMenu(text);
                }
                PartyBase.MainParty.SetVisualAsDirty();
                Campaign.Current.CampaignInformationManager.OnGameLoaded();
                foreach (Settlement settlement in Settlement.All)
                {
                    settlement.Party.SetLevelMaskIsDirty();
                }
                CampaignEventDispatcher.Instance.OnGameLoadFinished();
                bool flag5 = mapState != null;
                if (flag5)
                {
                    mapState.OnLoadingFinished();
                }
            }
            base.IsLoaded = true;
        }

        // Token: 0x06000013 RID: 19 RVA: 0x00003614 File Offset: 0x00001814
        private void LaunchSandboxCharacterCreation()
        {
            CharacterCreationState gameState = Game.Current.GameStateManager.CreateState<CharacterCreationState>(new object[]
            {
                new CharacterCreation_AD()
            });
            Game.Current.GameStateManager.CleanAndPushState(gameState, 0);
        }

        // Token: 0x04000001 RID: 1
        private bool _loadingSavedGame;

        // Token: 0x04000002 RID: 2
        private LoadResult _loadedGameResult;
    }
}
