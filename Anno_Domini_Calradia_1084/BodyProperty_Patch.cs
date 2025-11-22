using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.CharacterCreation;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Anno_Domini_Calradia_1084.Patches
{
    internal class BodyProperty_Patch
    {
        [HarmonyPatch(typeof(CharacterCreationCultureStageVM), "InitializePlayersFaceKeyAccordingToCultureSelection")]
        public class ADCCultureFaceKeyPatch
        {
            private static void Postfix(CharacterCreationCultureVM selectedCulture)
            {
                Dictionary<CultureObject, string> dic = new Dictionary<CultureObject, string>(15);
                MBReadOnlyList<CultureObject> cultures = Campaign.Current.ObjectManager.GetObjectTypeList<CultureObject>();
                dic.Add(cultures.First((CultureObject x) => x.StringId == "empire"), "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='001BB80D0000100D7C7664876753888A7577866254C69643C4B647398C95A0370077760307A7497300000000000000000000000000000000000000003AF47002'/>");
                dic.Add(cultures.First((CultureObject x) => x.StringId == "vlandia"), "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='0002AC08C000300D5976648E6774B8355398866295113236DCB177278CA47665007776030748B49500000000000000000000000000000000000000003EFC1002'/>");
                dic.Add(cultures.First((CultureObject x) => x.StringId == "sturgia"), "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='000500000000200D797664884754DCBA735E866295A09677744175498A45808100777603076367A500000000000000000000000000000000000000003CFC2002'/>");
                dic.Add(cultures.First((CultureObject x) => x.StringId == "aserai"), "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='0029C80FC000300DBA756445533377873CD1833B3101B44A2B5351327CA32C260077760307627764000000000000000000000000000000000000000042F49002'/>");
                dic.Add(cultures.First((CultureObject x) => x.StringId == "khuzait"), "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'   key='0013F80E4000200EB8708BD6CDC8522973678B3ABDFB144CD22D3DD7518933210077760307A7724B00000000000000000000000000000000000000003EF43042'/>");
                dic.Add(cultures.First((CultureObject x) => x.StringId == "battania"), "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='00000006C000200858766434475CDCBA834E84625590967A7441DA4983AB5646007776030754B76300000000000000000000000000000000000000003CFC3002'/>");
                dic.Add(cultures.First((CultureObject x) => x.StringId == "svadia"), "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='0005B80D8000000F7C7664876753888A574A866254C69643A45D951A48A7A72700777603073C7D4300000000000000000000000000000000000000003AF41082'/>");
                dic.Add(cultures.First((CultureObject x) => x.StringId == "nord"), "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='00000800800000045C472B462BBE28B7E9E68689B9C76DD674414A4939677C23007776030767697A00000000000000000000000000000000000000003CFC7002'/>");
                dic.Add(cultures.First((CultureObject x) => x.StringId == "balion"), "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='0002B808C0000008037664876753888A97658665B9C62DACA46D2B7748ACA797057776570775389800000000000000000000000000000000000000003AF43142'/>");
                BodyProperties properties;
                bool flag = BodyProperties.FromString(dic[selectedCulture.Culture], out properties);
                if (flag)
                {
                    CharacterObject.PlayerCharacter.UpdatePlayerCharacterBodyProperties(properties, CharacterObject.PlayerCharacter.Race, CharacterObject.PlayerCharacter.IsFemale);
                }
            }
        }
    }
}
