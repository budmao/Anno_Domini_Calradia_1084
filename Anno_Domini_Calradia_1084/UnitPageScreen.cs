using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;

namespace Anno_Domini_Calradia_1084
{
    [ViewModelMixin]
    public sealed class UnitPageScreen : BaseViewModelMixin<EncyclopediaUnitPageVM>
    {
        public UnitPageScreen(EncyclopediaUnitPageVM vm) : base(vm) { }
    }
}