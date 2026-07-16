using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.Prefabs2;

namespace Anno_Domini_Calradia_1084
{
    [PrefabExtension("EncyclopediaUnitPage", "descendant::Widget[./Children/ButtonWidget[@Id='BookmarkButton']]/Children")]
    internal sealed class EncyclopediaUnitPagePrefabExtension : PrefabExtensionInsertPatch
    {
        [PrefabExtensionInsertPatch.PrefabExtensionFileNameAttribute(false)]
        public string FileName => "Encyclopedia_Troop_Description_Widget";

        public override InsertType Type => InsertType.Child;

        public override int Index => 99;
    }
}