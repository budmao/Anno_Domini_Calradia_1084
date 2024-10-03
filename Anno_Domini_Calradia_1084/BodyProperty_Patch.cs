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
    // Token: 0x02000003 RID: 3
    internal class BodyProperty_Patch
    {
        // Token: 0x02000008 RID: 8
        [HarmonyPatch(typeof(CharacterCreationCultureStageVM), "InitializePlayersFaceKeyAccordingToCultureSelection")]
        public class ADCCultureFaceKeyPatch
        {
            // Token: 0x0600001A RID: 26 RVA: 0x000036B8 File Offset: 0x000018B8
            private static void Postfix(CharacterCreationCultureVM selectedCulture)
            {
                Dictionary<CultureObject, string> dic = new Dictionary<CultureObject, string>(15);
                MBReadOnlyList<CultureObject> cultures = Campaign.Current.ObjectManager.GetObjectTypeList<CultureObject>();
                dic.Add(cultures.First((CultureObject x) => x.StringId == "empire"), "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='001CB80CC000300D7C7664876753888A7577866254C69643C4B647398C95A0370077760307A7497300000000000000000000000000000000000000003AF47002'/>");
                dic.Add(cultures.First((CultureObject x) => x.StringId == "vlandia"), "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='000BAC088000100DB976648E6774B835537D86629511323BDCB177278A84F667017776140748B49500000000000000000000000000000000000000003EFC5002'/>");
                dic.Add(cultures.First((CultureObject x) => x.StringId == "sturgia"), "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='000500000000000D797664884754DCBAA35E866295A0967774414A498C8336860F7776F20BA7B7A500000000000000000000000000000000000000003CFC2002'/>");
                dic.Add(cultures.First((CultureObject x) => x.StringId == "aserai"), "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='0028C80FC000100DBA756445533377873CD1833B3101B44A21C3C5347CA32C260F7776F20BBC35E8000000000000000000000000000000000000000042F41002'/>");
                dic.Add(cultures.First((CultureObject x) => x.StringId == "khuzait"), "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'   key='0016F80E4000200EB8708BD6CDC85229D3698B3ABDFE344CD22D3DD5388988680F7776F20B96723B00000000000000000000000000000000000000003EF41002'/>");
                dic.Add(cultures.First((CultureObject x) => x.StringId == "battania"), "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='000000058000200D79766434475CDCBAC34E866255A096777441DA49838BF6A50F7776F20BA7B7A500000000000000000000000000000000000000003CFC0002'/>");
                dic.Add(cultures.First((CultureObject x) => x.StringId == "svadia"), "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='0026B80E4000300F7C7664876753888A574A866254C69643A45D951A48A7A72700777603073C7D4300000000000000000000000000000000000000003AF43082'/>");
                dic.Add(cultures.First((CultureObject x) => x.StringId == "nord"), "<BodyProperties version='4' age='25.84' weight='0.5000' build='0.5000'  key='00006802800010045C472B462BBE28B7E9E68689B9C76DD674414A4934693BA70077760307A7B7A500000000000000000000000000000000000000003CFC6002'/>");
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
