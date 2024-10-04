using System;
using TaleWorlds.CampaignSystem.CharacterCreationContent;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Anno_Domini_Calradia_1084.CC
{
    // Token: 0x02000005 RID: 5
    internal class CreationContent : SandboxCharacterCreationContent
    {
        // Token: 0x06000008 RID: 8 RVA: 0x0000240C File Offset: 0x0000060C
        protected override void OnInitialized(CharacterCreation characterCreation)
        {
            this.AddParentsMenu(characterCreation);
            base.AddChildhoodMenu(characterCreation);
            base.AddEducationMenu(characterCreation);
            CustomAddYouthMenu(characterCreation); // Call the custom method here
            base.AddAdulthoodMenu(characterCreation);
            base.AddAgeSelectionMenu(characterCreation);
            this._startingPoints.Add("svadia", new Vec2(379.0639f, 347.452f));
            this._startingPoints.Add("nord", new Vec2(444.4771f, 516.6031f));
        }

        // Token: 0x06000009 RID: 9 RVA: 0x000024EC File Offset: 0x000006EC
        protected new void AddParentsMenu(CharacterCreation characterCreation)
        {
            base.AddParentsMenu(characterCreation);
            CharacterCreationMenu menu = characterCreation.GetCurrentMenu(0);
            CharacterCreationCategory svadiaCategory = menu.AddMenuCategory(new CharacterCreationOnCondition(this.SvadiaCondition));
            MBList<SkillObject> effectedSkills = new MBList<SkillObject>
            {
                DefaultSkills.Riding,
                DefaultSkills.OneHanded
            };
            CharacterAttribute effectedAttribute = DefaultCharacterAttributes.Vigor;
            svadiaCategory.AddCategoryOption(new TextObject("{=!}A landlord's retainers", null), effectedSkills, effectedAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, delegate (CharacterCreation CharacterCreation)
            {
                this.SetParentAndOccupationType(characterCreation, 1, SandboxCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
            }, delegate (CharacterCreation CharacterCreation)
            {
                this.FinalizeParents();
            }, new TextObject("{=!}Your father was a trusted lieutenant of the local landowning aristocrat. He rode with the lord's cavalry, fighting as an armored lancer.", null), null, 0, 0, 0, 0, 0);
            effectedSkills = new MBList<SkillObject>
            {
                DefaultSkills.Polearm,
                DefaultSkills.Athletics
            };
            effectedAttribute = DefaultCharacterAttributes.Endurance;
            svadiaCategory.AddCategoryOption(new TextObject("{=!}Sailors", null), effectedSkills, effectedAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, delegate (CharacterCreation CharacterCreation)
            {
                this.SetParentAndOccupationType(characterCreation, 2, SandboxCharacterCreationContent.OccupationTypes.Mercenary, "", "", true, true);
            }, delegate (CharacterCreation CharacterCreation)
            {
                this.FinalizeParents();
            }, new TextObject("{=!}Your family lived in a small house near one of the great ports of the Empire. Your father served as a sailor in the imperial navy, manning one of their mighty warships.", null), null, 0, 0, 0, 0, 0);
            effectedSkills = new MBList<SkillObject>
            {
                DefaultSkills.Charm,
                DefaultSkills.Trade
            };
            effectedAttribute = DefaultCharacterAttributes.Social;
            svadiaCategory.AddCategoryOption(new TextObject("{=!}Urban merchants", null), effectedSkills, effectedAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, delegate (CharacterCreation CharacterCreation)
            {
                this.SetParentAndOccupationType(characterCreation, 3, SandboxCharacterCreationContent.OccupationTypes.Merchant, "", "", true, true);
            }, delegate (CharacterCreation CharacterCreation)
            {
                this.FinalizeParents();
            }, new TextObject("{=!}Your family were respected traders in a seaside town. They ran caravans to nearby towns, and discussed issues in the town council.", null), null, 0, 0, 0, 0, 0);
            effectedSkills = new MBList<SkillObject>
            {
                DefaultSkills.Athletics,
                DefaultSkills.OneHanded
            };
            effectedAttribute = DefaultCharacterAttributes.Endurance;
            svadiaCategory.AddCategoryOption(new TextObject("{=!}Farmers", null), effectedSkills, effectedAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, delegate (CharacterCreation CharacterCreation)
            {
                this.SetParentAndOccupationType(characterCreation, 4, SandboxCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
            }, delegate (CharacterCreation CharacterCreation)
            {
                this.FinalizeParents();
            }, new TextObject("{=!}Your family tilled the soil in one of the coastal plains around the Perassic Sea. The grain they grew was sent to feed the vast cities of the Empire. On occasion, your father was levied to defend the village against bedouin raiders.", null), null, 0, 0, 0, 0, 0);
            effectedSkills = new MBList<SkillObject>
            {
                DefaultSkills.Crossbow,
                DefaultSkills.Crafting
            };
            effectedAttribute = DefaultCharacterAttributes.Intelligence;
            svadiaCategory.AddCategoryOption(new TextObject("{=!}Urban Artisans", null), effectedSkills, effectedAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, delegate (CharacterCreation CharacterCreation)
            {
                this.SetParentAndOccupationType(characterCreation, 5, SandboxCharacterCreationContent.OccupationTypes.Artisan, "", "", true, true);
            }, delegate (CharacterCreation CharacterCreation)
            {
                this.FinalizeParents();
            }, new TextObject("{=!}Your family owned their own workshop in the city, making goods from raw materials brought in from the countryside. Your father played an active if minor role in the town council, and also served in the militia.", null), null, 0, 0, 0, 0, 0);
            effectedSkills = new MBList<SkillObject>
            {
                DefaultSkills.Roguery,
                DefaultSkills.Throwing
            };
            effectedAttribute = DefaultCharacterAttributes.Cunning;
            svadiaCategory.AddCategoryOption(new TextObject("{=!}Vagabonds", null), effectedSkills, effectedAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, delegate (CharacterCreation CharacterCreation)
            {
                this.SetParentAndOccupationType(characterCreation, 6, SandboxCharacterCreationContent.OccupationTypes.Vagabond, "", "", true, true);
            }, delegate (CharacterCreation CharacterCreation)
            {
                this.FinalizeParents();
            }, new TextObject("{=!}Your family numbered among the many poor migrants living in the slums that grow up outside the walls of imperial cities, making whatever money they could from a variety of odd jobs. Sometimes they did service for one of the Empire's many criminal gangs, and you had an early look at the dark side of life.", null), null, 0, 0, 0, 0, 0);
            CharacterCreationCategory nordCategory = menu.AddMenuCategory(new CharacterCreationOnCondition(this.NordCondition));
            effectedSkills = new MBList<SkillObject>
            {
                DefaultSkills.Riding,
                DefaultSkills.OneHanded
            };
            effectedAttribute = DefaultCharacterAttributes.Social;
            nordCategory.AddCategoryOption(new TextObject("{=!}Retainers of a Valtias", null), effectedSkills, effectedAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, delegate (CharacterCreation CharacterCreation)
            {
                this.SetParentAndOccupationType(characterCreation, 1, SandboxCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
            }, delegate (CharacterCreation CharacterCreation)
            {
                this.FinalizeParents();
            }, new TextObject("{=!}Your father served as a retainer to a Valtias, one of the noble lords of the Nord. He sat at his lord's table in the great hall, oversaw his estates, and stood by his side in the center of the shield wall in battle.", null), null, 0, 0, 0, 0, 0);
            effectedSkills = new MBList<SkillObject>
            {
                DefaultSkills.Tactics,
                DefaultSkills.Trade
            };
            effectedAttribute = DefaultCharacterAttributes.Cunning;
            nordCategory.AddCategoryOption(new TextObject("{=!}Urban traders", null), effectedSkills, effectedAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, delegate (CharacterCreation CharacterCreation)
            {
                this.SetParentAndOccupationType(characterCreation, 2, SandboxCharacterCreationContent.OccupationTypes.Merchant, "", "", true, true);
            }, delegate (CharacterCreation CharacterCreation)
            {
                this.FinalizeParents();
            }, new TextObject("{=!}Your family were merchants who lived in one of the larger Nord settlements, organizing the trade of furs, honey and other forest products to faraway lands.", null), null, 0, 0, 0, 0, 0);
            effectedSkills = new MBList<SkillObject>
            {
                DefaultSkills.Charm,
                DefaultSkills.Medicine
            };
            effectedAttribute = DefaultCharacterAttributes.Intelligence;
            nordCategory.AddCategoryOption(new TextObject("{=!}Forest shamans", null), effectedSkills, effectedAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, delegate (CharacterCreation CharacterCreation)
            {
                this.SetParentAndOccupationType(characterCreation, 3, SandboxCharacterCreationContent.OccupationTypes.Healer, "", "", true, true);
            }, delegate (CharacterCreation CharacterCreation)
            {
                this.FinalizeParents();
            }, new TextObject("{=!}Your family were the keepers of the ancient knowledge of wood and spring, conveying the wisdom of the elder spirits to the Nord people. They tended the sick and counseled the powerful, resolving disputes and providing practical advice.", null), null, 0, 0, 0, 0, 0);
            effectedSkills = new MBList<SkillObject>
            {
                DefaultSkills.Athletics,
                DefaultSkills.Polearm
            };
            effectedAttribute = DefaultCharacterAttributes.Endurance;
            nordCategory.AddCategoryOption(new TextObject("{=!}Free farmers", null), effectedSkills, effectedAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, delegate (CharacterCreation CharacterCreation)
            {
                this.SetParentAndOccupationType(characterCreation, 4, SandboxCharacterCreationContent.OccupationTypes.Farmer, "", "", true, true);
            }, delegate (CharacterCreation CharacterCreation)
            {
                this.FinalizeParents();
            }, new TextObject("{=!}Your family dwelt in temporary camps on the edge of the forest, slashing and burning fields which they tended for a year or two before moving on. People like them were the pillars of the realm's economy, as well as the backbone of the levy.", null), null, 0, 0, 0, 0, 0);
            effectedSkills = new MBList<SkillObject>
            {
                DefaultSkills.Bow,
                DefaultSkills.Scouting
            };
            effectedAttribute = DefaultCharacterAttributes.Vigor;
            nordCategory.AddCategoryOption(new TextObject("{=!}Foresters", null), effectedSkills, effectedAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, delegate (CharacterCreation CharacterCreation)
            {
                this.SetParentAndOccupationType(characterCreation, 5, SandboxCharacterCreationContent.OccupationTypes.Hunter, "", "", true, true);
            }, delegate (CharacterCreation CharacterCreation)
            {
                this.FinalizeParents();
            }, new TextObject("{=!}Your family dwelt amidst the dark, snowy forests of the Nord heartland. They made their living deep in the woods, hunting and trapping fox, hare, ermine, and other fur-bearing animals.", null), null, 0, 0, 0, 0, 0);
            effectedSkills = new MBList<SkillObject>
            {
                DefaultSkills.Roguery,
                DefaultSkills.Throwing
            };
            effectedAttribute = DefaultCharacterAttributes.Cunning;
            nordCategory.AddCategoryOption(new TextObject("{=!}Vagabonds", null), effectedSkills, effectedAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, delegate (CharacterCreation CharacterCreation)
            {
                this.SetParentAndOccupationType(characterCreation, 6, SandboxCharacterCreationContent.OccupationTypes.Vagabond, "", "", true, true);
            }, delegate (CharacterCreation CharacterCreation)
            {
                this.FinalizeParents();
            }, new TextObject("{=!}Your family numbered among the many poor migrants living in the slums that grow up outside the walls of nord settlements, making whatever money they could from a variety of odd jobs. Sometimes they did service for one of the region's many criminal gangs.", null), null, 0, 0, 0, 0, 0);

        }

        // Token: 0x0600000A RID: 10 RVA: 0x00003377 File Offset: 0x00001577
        protected bool SvadiaCondition()
        {
            return base.GetSelectedCulture().StringId == "svadia";
        }

        // Token: 0x0600000B RID: 11 RVA: 0x0000338E File Offset: 0x0000158E
        protected bool NordCondition()
        {
            return base.GetSelectedCulture().StringId == "nord";
        }

        // The custom method for adding the Youth menu
        protected void CustomAddYouthMenu(CharacterCreation characterCreation)
        {
            CharacterCreationMenu characterCreationMenu = new CharacterCreationMenu(
                new TextObject("{=ok8lSW6M}Youth", null),
                new TextObject("{=youthIntro}As a youngster growing up in Calradia, war was never too far away. You...", null), // Replace with your actual introductory text
                new CharacterCreationOnInit(this.YouthOnInit),
                CharacterCreationMenu.MenuTypes.MultipleChoice
            );

            CharacterCreationCategory youthCategory = characterCreationMenu.AddMenuCategory(null);

            // Commander
            youthCategory.AddCategoryOption(new TextObject("{=CITG915d}joined a commander's staff.", null),
                new MBList<SkillObject>
                {
            DefaultSkills.Steward,
            DefaultSkills.Tactics
                },
                DefaultCharacterAttributes.Cunning,
                this.FocusToAdd,
                this.SkillLevelToAdd,
                this.AttributeLevelToAdd,
                new CharacterCreationOnCondition(this.YouthCommanderOnCondition),
                new CharacterCreationOnSelect(this.YouthCommanderOnConsequence),
                new CharacterCreationApplyFinalEffects(this.YouthCommanderOnApply),
                new TextObject("{=Ay0G3f7I}Your family arranged for you to be part of the staff of an imperial strategos. You were not given major responsibilities - mostly carrying messages and tending to his horse -- but it did give you a chance to see how campaigns were planned and men were deployed in battle.", null),
                null,
                0, 0, 0, 0, 0);
            // Groom
            youthCategory.AddCategoryOption(new TextObject("{=bhE2i6OU}served as a baron's groom.", null),
                new MBList<SkillObject>
                {
            DefaultSkills.Steward,
            DefaultSkills.Tactics
                },
                DefaultCharacterAttributes.Cunning,
                this.FocusToAdd,
                this.SkillLevelToAdd,
                this.AttributeLevelToAdd,
                new CharacterCreationOnCondition(this.YouthGroomOnCondition),
                new CharacterCreationOnSelect(this.YouthGroomOnConsequence),
                new CharacterCreationApplyFinalEffects(this.YouthGroomOnApply),
                new TextObject("{=iZKtGI6Y}Your family arranged for you to accompany a minor baron of the Vlandian kingdom. You were not given major responsibilities - mostly carrying messages and tending to his horse -- but it did give you a chance to see how campaigns were planned and men were deployed in battle.", null),
                null,
                0, 0, 0, 0, 0);
            // Chieftain
            youthCategory.AddCategoryOption(new TextObject("{=F2bgujPo}were a chieftain's servant.", null),
                new MBList<SkillObject>
                {
            DefaultSkills.Riding,
            DefaultSkills.Polearm
                },
                DefaultCharacterAttributes.Endurance,
                this.FocusToAdd,
                this.SkillLevelToAdd,
                this.AttributeLevelToAdd,
                new CharacterCreationOnCondition(this.YouthChieftainOnCondition),
                new CharacterCreationOnSelect(this.YouthChieftainOnConsequence),
                new CharacterCreationApplyFinalEffects(this.YouthChieftainOnApply),
                new TextObject("{=iZKtGI6Y}Your family arranged for you to accompany a minor baron of the Vlandian kingdom. You were not given major responsibilities - mostly carrying messages and tending to his horse -- but it did give you a chance to see how campaigns were planned and men were deployed in battle.", null),
                null,
                0, 0, 0, 0, 0);
            // Cavalry
            youthCategory.AddCategoryOption(new TextObject("{=h2KnarLL}trained with the cavalry.", null),
                new MBList<SkillObject>
                {
            DefaultSkills.Riding,
            DefaultSkills.Polearm
                },
                DefaultCharacterAttributes.Endurance,
                this.FocusToAdd,
                this.SkillLevelToAdd,
                this.AttributeLevelToAdd,
                new CharacterCreationOnCondition(this.YouthCavalryOnCondition),
                new CharacterCreationOnSelect(this.YouthCavalryOnConsequence),
                new CharacterCreationApplyFinalEffects(this.YouthCavalryOnApply),
                new TextObject("{=7AYJ3SjK}Your family arranged for you to accompany a chieftain of your people. You were not given major responsibilities - mostly carrying messages and tending to his horse -- but it did give you a chance to see how campaigns were planned and men were deployed in battle.", null),
                null,
                0, 0, 0, 0, 0);
            // HearthGuard
            youthCategory.AddCategoryOption(new TextObject("{=zsC2t5Hb}trained with the hearth guard.", null),
                new MBList<SkillObject>
                {
            DefaultSkills.Riding,
            DefaultSkills.Polearm
                },
                DefaultCharacterAttributes.Endurance,
                this.FocusToAdd,
                this.SkillLevelToAdd,
                this.AttributeLevelToAdd,
                new CharacterCreationOnCondition(this.YouthHearthGuardOnCondition),
                new CharacterCreationOnSelect(this.YouthHearthGuardOnConsequence),
                new CharacterCreationApplyFinalEffects(this.YouthHearthGuardOnApply),
                new TextObject("{=RmbWW6Bm}You were a big and imposing enough youth that the chief's guard allowed you to train alongside them, in preparation to join them some day.", null),
                null,
                0, 0, 0, 0, 0);
            // Garrison Crossbow
            youthCategory.AddCategoryOption(new TextObject("{=aTncHUfL}stood guard with the garrisons.", null),
                new MBList<SkillObject>
                {
            DefaultSkills.Crossbow,
            DefaultSkills.Engineering
                },
                DefaultCharacterAttributes.Intelligence,
                this.FocusToAdd,
                this.SkillLevelToAdd,
                this.AttributeLevelToAdd,
                new CharacterCreationOnCondition(this.YouthGarrisonOnCondition),
                new CharacterCreationOnSelect(this.YouthGarrisonOnConsequence),
                new CharacterCreationApplyFinalEffects(this.YouthGarrisonOnApply),
                new TextObject("{=63TAYbkx}Urban troops spend much of their time guarding the town walls. Most of their training was in missile weapons, especially useful during sieges.", null),
                null,
                0, 0, 0, 0, 0);
            // Garrison Bow
            youthCategory.AddCategoryOption(new TextObject("{=aTncHUfL}stood guard with the garrisons.", null),
                new MBList<SkillObject>
                {
            DefaultSkills.Bow,
            DefaultSkills.Engineering
                },
                DefaultCharacterAttributes.Intelligence,
                this.FocusToAdd,
                this.SkillLevelToAdd,
                this.AttributeLevelToAdd,
                new CharacterCreationOnCondition(this.YouthOtherGarrisonOnCondition),
                new CharacterCreationOnSelect(this.YouthOtherGarrisonOnConsequence),
                new CharacterCreationApplyFinalEffects(this.YouthOtherGarrisonOnApply),
                new TextObject("{=63TAYbkx}Urban troops spend much of their time guarding the town walls. Most of their training was in missile weapons, especially useful during sieges.", null),
                null,
                0, 0, 0, 0, 0);
            // Outriders
            youthCategory.AddCategoryOption(new TextObject("{=VlXOgIX6}rode with the scouts.", null),
                new MBList<SkillObject>
                {
            DefaultSkills.Riding,
            DefaultSkills.Bow
                },
                DefaultCharacterAttributes.Endurance,
                this.FocusToAdd,
                this.SkillLevelToAdd,
                this.AttributeLevelToAdd,
                new CharacterCreationOnCondition(this.YouthOutridersOnCondition),
                new CharacterCreationOnSelect(this.YouthOutridersOnConsequence),
                new CharacterCreationApplyFinalEffects(this.YouthOutridersOnApply),
                new TextObject("{=888lmJqs}All of Calradia's kingdoms recognize the value of good light cavalry and horse archers, and are sure to recruit nomads and borderers with the skills to fulfill those duties. You were a good enough rider that your neighbors pitched in to buy you a small pony and a good bow so that you could fulfill their levy obligations.", null),
                null,
                0, 0, 0, 0, 0);
            // OtherOutriders
            youthCategory.AddCategoryOption(new TextObject("{=VlXOgIX6}rode with the scouts.", null),
                new MBList<SkillObject>
                {
            DefaultSkills.Riding,
            DefaultSkills.Bow
                },
                DefaultCharacterAttributes.Endurance,
                this.FocusToAdd,
                this.SkillLevelToAdd,
                this.AttributeLevelToAdd,
                new CharacterCreationOnCondition(this.YouthOtherOutridersOnCondition),
                new CharacterCreationOnSelect(this.YouthOtherOutridersOnConsequence),
                new CharacterCreationApplyFinalEffects(this.YouthOtherOutridersOnApply),
                new TextObject("{=sYuN6hPD}All of Calradia's kingdoms recognize the value of good light cavalry, and are sure to recruit nomads and borderers with the skills to fulfill those duties. You were a good enough rider that your neighbors pitched in to buy you a small pony and a sheaf of javelins so that you could fulfill their levy obligations.", null),
                null,
                0, 0, 0, 0, 0);
            // Infantry
            youthCategory.AddCategoryOption(new TextObject("{=a8arFSra}trained with the infantryX.", null),
                new MBList<SkillObject>
                {
            DefaultSkills.Bow,
            DefaultSkills.Engineering
                },
                DefaultCharacterAttributes.Intelligence,
                this.FocusToAdd,
                this.SkillLevelToAdd,
                this.AttributeLevelToAdd,
                null, // Removed the OnCondition
                new CharacterCreationOnSelect(this.YouthInfantryOnConsequence), 
                new CharacterCreationApplyFinalEffects(this.YouthInfantryOnApply),
                new TextObject("{=sYuN6hPD}XAll of Calradia's kingdoms recognize the value of good light cavalry, and are sure to recruit nomads and borderers with the skills to fulfill those duties. You were a good enough rider that your neighbors pitched in to buy you a small pony and a sheaf of javelins so that you could fulfill their levy obligations.", null),
                null,
                0, 0, 0, 0, 0);
            // Skirmisher
            youthCategory.AddCategoryOption(new TextObject("{=oMbOIPc9}joined the skirmishers.", null),
                new MBList<SkillObject>
                {
            DefaultSkills.Throwing,
            DefaultSkills.OneHanded
                },
                DefaultCharacterAttributes.Control,
                this.FocusToAdd,
                this.SkillLevelToAdd,
                this.AttributeLevelToAdd,
                new CharacterCreationOnCondition(this.YouthSkirmisherOnCondition),
                new CharacterCreationOnSelect(this.YouthSkirmisherOnConsequence),
                new CharacterCreationApplyFinalEffects(this.YouthSkirmisherOnApply),
                new TextObject("{=afH90aNs}Levy armed with spear and shield, drawn from smallholding farmers, have always been the backbone of most armies of Calradia.", null),
                null,
                0, 0, 0, 0, 0);
            // Kern
            youthCategory.AddCategoryOption(new TextObject("{=cDWbwBwI}joined the kern.", null),
                new MBList<SkillObject>
                {
            DefaultSkills.Throwing,
            DefaultSkills.OneHanded
                },
                DefaultCharacterAttributes.Control,
                this.FocusToAdd,
                this.SkillLevelToAdd,
                this.AttributeLevelToAdd,
                new CharacterCreationOnCondition(this.YouthKernOnCondition),
                new CharacterCreationOnSelect(this.YouthKernOnConsequence),
                new CharacterCreationApplyFinalEffects(this.YouthKernOnApply),
                new TextObject("{=tTb28jyU}Many Battanians fight as kern, versatile troops who could both harass the enemy line with their javelins or join in the final screaming charge once it weakened.", null),
                null,
                0, 0, 0, 0, 0);
            // Camper
            youthCategory.AddCategoryOption(new TextObject("{=GFUggps8}marched with the camp followers.", null),
                new MBList<SkillObject>
                {
            DefaultSkills.Roguery,
            DefaultSkills.Throwing
                },
                DefaultCharacterAttributes.Cunning,
                this.FocusToAdd,
                this.SkillLevelToAdd,
                this.AttributeLevelToAdd,
                new CharacterCreationOnCondition(this.YouthCamperOnCondition),
                new CharacterCreationOnSelect(this.YouthCamperOnConsequence),
                new CharacterCreationApplyFinalEffects(this.YouthCamperOnApply),
                new TextObject("{=64rWqBLN}You avoided service with one of the main forces of your realm's armies, but followed instead in the train - the troops' wives, lovers and servants, and those who make their living by caring for, entertaining, or cheating the soldiery.", null),
                null,
                0, 0, 0, 0, 0);





            // Add more options for youth as needed...

            characterCreation.AddNewMenu(characterCreationMenu);
        }


        // Additional methods (YouthOnInit, YouthCommanderOnCondition, etc.) should be defined here...

    }
}
