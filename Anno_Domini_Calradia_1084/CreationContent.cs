using System;
using System.Collections.Generic;
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
            }, new TextObject("{=!}Your father served as a loyal retainer to the local nobleman, riding with the lord's cavalry and engaging in battle as an armored rider.", null), null, 0, 0, 0, 0, 0);
            effectedSkills = new MBList<SkillObject>
            {
                DefaultSkills.Polearm,
                DefaultSkills.Athletics
            };
            effectedAttribute = DefaultCharacterAttributes.Endurance;
            svadiaCategory.AddCategoryOption(new TextObject("{=!}Town Wardens", null), effectedSkills, effectedAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, delegate (CharacterCreation CharacterCreation)
            {
                this.SetParentAndOccupationType(characterCreation, 2, SandboxCharacterCreationContent.OccupationTypes.Mercenary, "", "", true, true);
            }, delegate (CharacterCreation CharacterCreation)
            {
                this.FinalizeParents();
            }, new TextObject("{=!}Your family has a long-standing reputation for training local militia in combat techniques and physical skills. Ensuring the community remains prepared to defend itself and fostering pride and resilience among the people.", null), null, 0, 0, 0, 0, 0);
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
            }, new TextObject("{=!}Your family thrived in the vibrant marketplace, trading goods from afar. Renowned for their keen negotiation skills, they built a reputation that attracted many customers, contributing significantly to the city's economy.", null), null, 0, 0, 0, 0, 0);
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
            }, new TextObject("{=!}Your family worked the land, cultivating crops and raising livestock. Their days followed the rhythm of the seasons, tending to the fields and ensuring the harvest would sustain the village. Proficient in agriculture and safeguarding their land, they stood as vital members of the community.", null), null, 0, 0, 0, 0, 0);
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
            }, new TextObject("{=!}Your family were skilled artisans, crafting goods in a small workshop nestled within the bustling streets of the city. They were respected artisans, transforming raw materials into sought-after goods for local and trade markets.", null), null, 0, 0, 0, 0, 0);
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
            }, new TextObject("{=!}Your family was among the countless poor migrants living in the slums that sprung up outside the city walls, scraping by with whatever work they could find. At times, they fell into service for local criminal gangs, giving you an early glimpse into the harsh realities of life on the streets.", null), null, 0, 0, 0, 0, 0);
            CharacterCreationCategory nordCategory = menu.AddMenuCategory(new CharacterCreationOnCondition(this.NordCondition));
            effectedSkills = new MBList<SkillObject>
            {
                DefaultSkills.Riding,
                DefaultSkills.OneHanded
            };
            effectedAttribute = DefaultCharacterAttributes.Social;
            nordCategory.AddCategoryOption(new TextObject("{=!}Retainers of a Jarl", null), effectedSkills, effectedAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, delegate (CharacterCreation CharacterCreation)
            {
                this.SetParentAndOccupationType(characterCreation, 1, SandboxCharacterCreationContent.OccupationTypes.Retainer, "", "", true, true);
            }, delegate (CharacterCreation CharacterCreation)
            {
                this.FinalizeParents();
            }, new TextObject("{=!}Your father served as a huskarl, a loyal warrior in the retinue of a powerful Nord Jarl. He trained you in the ways of combat and the code of honor, instilling in you the values of bravery and loyalty that defined the bond between a huskarl and his lord.", null), null, 0, 0, 0, 0, 0);
            effectedSkills = new MBList<SkillObject>
            {
                DefaultSkills.Tactics,
                DefaultSkills.Trade
            };
            effectedAttribute = DefaultCharacterAttributes.Cunning;
            nordCategory.AddCategoryOption(new TextObject("{=!}Sailors", null), effectedSkills, effectedAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, delegate (CharacterCreation CharacterCreation)
            {
                this.SetParentAndOccupationType(characterCreation, 2, SandboxCharacterCreationContent.OccupationTypes.Merchant, "", "", true, true);
            }, delegate (CharacterCreation CharacterCreation)
            {
                this.FinalizeParents();
            }, new TextObject("{=!}Your family hailed from a line of skilled sailors who navigated the icy North Sea. Growing up by the docks, you learned the ways of the sea from your father, who often spoke of distant lands and fierce storms.", null), null, 0, 0, 0, 0, 0);
            effectedSkills = new MBList<SkillObject>
            {
                DefaultSkills.Charm,
                DefaultSkills.Medicine
            };
            effectedAttribute = DefaultCharacterAttributes.Intelligence;
            nordCategory.AddCategoryOption(new TextObject("{=!}Herbalists", null), effectedSkills, effectedAttribute, this.FocusToAdd, this.SkillLevelToAdd, this.AttributeLevelToAdd, null, delegate (CharacterCreation CharacterCreation)
            {
                this.SetParentAndOccupationType(characterCreation, 3, SandboxCharacterCreationContent.OccupationTypes.Healer, "", "", true, true);
            }, delegate (CharacterCreation CharacterCreation)
            {
                this.FinalizeParents();
            }, new TextObject("{=!}Your family upheld the ancient teachings of nature and the spirits of the springs, sharing their insights with the Nord people. They cared for the ill and advised the influential, mediating conflicts and offering sound guidance.", null), null, 0, 0, 0, 0, 0);
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
            }, new TextObject("{=!}As free farmers of the Nord lands, your parents tilled the fertile soil on the village's outskirts. Your life was intertwined with the land, as you grew grains and tended to livestock, ensuring your community was well-fed. You learned to defend your homestead from raiders, taking pride in your independence and resilience.", null), null, 0, 0, 0, 0, 0);
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
            }, new TextObject("{=!}Your family resided in the shadowy, snow-covered woodlands of the Nord heartland. They earned their livelihood deep within the forest, hunting down foxes and hares, as well as cutting wood for fuel and trade.", null), null, 0, 0, 0, 0, 0);
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
            }, new TextObject("{=!}Your family was one of the countless destitute migrants residing in the makeshift communities that sprang up beyond the boundaries of Nord settlements, scraping together a living through various odd jobs. At times, they found work for some of the numerous criminal organizations in the area.", null), null, 0, 0, 0, 0, 0);

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

            // BodyGuard
            youthCategory.AddCategoryOption(new TextObject("{=GFUggps9}served in a jarl's Household.", null),
                new MBList<SkillObject>
                {
            DefaultSkills.Steward,
            DefaultSkills.Tactics
                },
                DefaultCharacterAttributes.Cunning,
                this.FocusToAdd,
                this.SkillLevelToAdd,
                this.AttributeLevelToAdd,
                new CharacterCreationOnCondition(this.YouthBodyGuardOnCondition),
                new CharacterCreationOnSelect(this.YouthBodyGuardOnConsequence),
                new CharacterCreationApplyFinalEffects(this.YouthBodyGuardOnApply),
                new TextObject("{=JarlBodyguard}From a young age, you stood watch over the jarl, sworn to protect him with your life. You learned the ways of combat and loyalty. Through feasts and battles, you became an unwavering shield, ready to face any threat that would dare challenge your lord's honor.", null),
                null,
                0, 0, 0, 0, 0);
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
            youthCategory.AddCategoryOption(new TextObject("{=bhE2i6OU}served as a lord's groom.", null),
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
                new TextObject("{=iZKtGI6Y}Your family arranged for you to accompany a minor lord of the local land. You were not given major responsibilities - mostly carrying messages and tending to his horse -- but it did give you a chance to see how campaigns were planned and men were deployed in battle.", null),
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
                new TextObject("{=7AYJ3SjK}Your family arranged for you to accompany a chieftain of your people. You were not given major responsibilities - mostly carrying messages and tending to his horse -- but it did give you a chance to see how campaigns were planned and men were deployed in battle.", null),
                null,
                0, 0, 0, 0, 0);
            // Sailor
            youthCategory.AddCategoryOption(new TextObject("{=GFUggps8}sailed with the longships.", null),
                new MBList<SkillObject>
                {
            DefaultSkills.Trade,
            DefaultSkills.Athletics
                },
                DefaultCharacterAttributes.Social,
                this.FocusToAdd,
                this.SkillLevelToAdd,
                this.AttributeLevelToAdd,
                new CharacterCreationOnCondition(this.YouthSailorOnCondition),
                new CharacterCreationOnSelect(this.YouthSailorOnConsequence),
                new CharacterCreationApplyFinalEffects(this.YouthSailorOnApply),
                new TextObject("{=VikingSailing}From a young age, you were raised in the shadow of longships. You learned to navigate the treacherous waters of the North Sea, becoming familiar with the ways of the sea and the call of adventure. Whether raiding distant shores or trading with neighboring tribes, the thrill of the ocean became a part of your very soul.", null),
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
                new TextObject("{=7cHsIMLP}You could never have afforded the equipment on your own, but your skill as a rider earned you the favor of the local lord, who provided you with a horse and gear. You joined the cavalry, honing your skills with the spear.", null),
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
            youthCategory.AddCategoryOption(new TextObject("{=a8arFSra}trained with the infantry.", null),
                new MBList<SkillObject>
                {
            DefaultSkills.Polearm,
            DefaultSkills.OneHanded
                },
                DefaultCharacterAttributes.Vigor,
                this.FocusToAdd,
                this.SkillLevelToAdd,
                this.AttributeLevelToAdd,
                null, // Removed the OnCondition
                new CharacterCreationOnSelect(this.YouthInfantryOnConsequence), 
                new CharacterCreationApplyFinalEffects(this.YouthInfantryOnApply),
                new TextObject("{=sYuN6hPD}All of Calradia's kingdoms recognize the value of good light cavalry, and are sure to recruit nomads and borderers with the skills to fulfill those duties. You were a good enough rider that your neighbors pitched in to buy you a small pony and a sheaf of javelins so that you could fulfill their levy obligations.", null),
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
        // Check if the selected culture is Nord and if the YouthBodyGuard option can be selected
        protected bool YouthBodyGuardOnCondition()
        {
            return base.GetSelectedCulture().StringId == "nord" && this._familyOccupationType == SandboxCharacterCreationContent.OccupationTypes.Retainer;
        }

        // Execute consequences after selecting the YouthBodyguard option
        protected void YouthBodyGuardOnConsequence(CharacterCreation characterCreation)
        {
            base.SelectedTitleType = 10;
            this.RefreshPlayerAppearance(characterCreation);
            characterCreation.ChangeCharsAnimation(new List<string>
    {
        "act_childhood_sharp"
    });
        }

        // Apply final effects after the youth choice has been made
        protected void YouthBodyGuardOnApply(CharacterCreation characterCreation)
        {
        }
        // Check if the selected culture is Nord and if the YouthSailor option can be selected
        protected bool YouthSailorOnCondition()
        {
            return base.GetSelectedCulture().StringId == "nord";
        }

        // Execute consequences after selecting the YouthSailor option
        protected void YouthSailorOnConsequence(CharacterCreation characterCreation)
        {
            base.SelectedTitleType = 10;
            this.RefreshPlayerAppearance(characterCreation);
            characterCreation.ChangeCharsAnimation(new List<string>
    {
        "act_childhood_sharp"
    });
        }
        // Apply final effects after the youth choice has been made
        protected void YouthSailorOnApply(CharacterCreation characterCreation)
        {
        }
    }
}
