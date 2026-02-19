using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml.Linq;
using Bannerlord.UIExtenderEx.Attributes;
using Bannerlord.UIExtenderEx.ViewModels;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.ViewModelCollection.Encyclopedia.Pages;
using TaleWorlds.Core.ViewModelCollection.Information;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace Anno_Domini_Calradia_1084
{
    [ViewModelMixin]
    internal class EncyclopediaUnitPageVMMixin : BaseViewModelMixin<EncyclopediaUnitPageVM>
    {
        private readonly EncyclopediaUnitPageVM _pageVM;
        private bool _exampleSpriteVisible;
        private HintViewModel _exampleSpriteHint;
        private string _lastCharacterId;

        public EncyclopediaUnitPageVMMixin(EncyclopediaUnitPageVM vm) : base(vm)
        {
            _pageVM = vm;
            _exampleSpriteVisible = false;
            _exampleSpriteHint = new HintViewModel();
        }

        private CharacterObject GetCurrentCharacter()
        {
            try
            {
                if (_pageVM == null || _pageVM.Obj == null)
                    return null;

                return _pageVM.Obj as CharacterObject;
            }
            catch
            {
                return null;
            }
        }

        private string GetDescriptionForCurrentCharacter()
        {
            var character = GetCurrentCharacter();
            if (character == null)
                return "Empty description";

            string description = TroopDescriptionStrings.GetDescriptionForTroop(character.StringId);
            if (string.IsNullOrEmpty(description))
                return "Empty description";

            return description.Replace(";", "\n");
        }

        [DataSourceProperty]
        public bool ExampleSpriteVisible
        {
            get
            {
                // Handle input and update hint lazily when UI queries this property
                try
                {
                    if (Campaign.Current == null)
                        return false;

                    var character = GetCurrentCharacter();
                    if (character == null)
                        return false;

                    // Update hint when character changes
                    if (_lastCharacterId != character.StringId)
                    {
                        _lastCharacterId = character.StringId;
                        string description = GetDescriptionForCurrentCharacter();
                        _exampleSpriteHint = new HintViewModel(new TextObject(description), null);
                        ViewModel.OnPropertyChangedWithValue<HintViewModel>(_exampleSpriteHint, "TroopsDescriptionSpriteHint");
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Debug.Print($"TroopsDescription: ExampleSpriteVisible error: {ex.Message}");
                    return false;
                }
            }
            set
            {
                if (value != _exampleSpriteVisible)
                {
                    _exampleSpriteVisible = value;
                    ViewModel.OnPropertyChangedWithValue(value);
                }
            }
        }

        [DataSourceProperty]
        public HintViewModel TroopsDescriptionSpriteHint
        {
            get
            {
                // Lazy load description when UI reads the hint
                try
                {
                    if (Campaign.Current == null)
                        return _exampleSpriteHint;

                    var character = GetCurrentCharacter();
                    if (character == null)
                        return _exampleSpriteHint;

                    if (_lastCharacterId != character.StringId)
                    {
                        _lastCharacterId = character.StringId;
                        string description = GetDescriptionForCurrentCharacter();
                        _exampleSpriteHint = new HintViewModel(new TextObject(description), null);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print($"TroopsDescription: TroopsDescriptionSpriteHint error: {ex.Message}");
                }

                return _exampleSpriteHint;
            }
            set
            {
                if (value != _exampleSpriteHint)
                {
                    _exampleSpriteHint = value;
                    ViewModel.OnPropertyChangedWithValue<HintViewModel>(value, "TroopsDescriptionSpriteHint");
                }
            }
        }

        public static class TroopDescriptionStrings
        {
            private static Dictionary<string, string> _strings = new Dictionary<string, string>();
            private static bool _isLoaded = false;

            public static void Load(string xmlPath)
            {
                if (_isLoaded)
                    return;

                if (!File.Exists(xmlPath))
                {
                    Debug.Print($"TroopsDescription: XML file not found: {xmlPath}");
                    return;
                }

                try
                {
                    XDocument doc = XDocument.Load(xmlPath);
                    foreach (var elem in doc.Descendants("string"))
                    {
                        string id = elem.Attribute("id")?.Value;
                        string text = elem.Attribute("text")?.Value;
                        if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(text))
                        {
                            _strings[id] = text;
                            Debug.Print($"TroopsDescription: Loaded {id}");
                        }
                    }

                    _isLoaded = true;
                    Debug.Print($"TroopsDescription: Total strings loaded: {_strings.Count}");
                }
                catch (Exception ex)
                {
                    Debug.Print($"TroopsDescription: Error loading XML: {ex.Message}");
                }
            }

            public static void Reload(string xmlPath)
            {
                _isLoaded = false;
                _strings.Clear();
                Load(xmlPath);
            }

            public static string GetDescriptionForTroop(string troopId)
            {
                if (!_isLoaded)
                {
                    string modFolder = Path.Combine(BasePath.Name, "Modules", "DAC_SHIELDWALL", "ModuleData");
                    string xmlPath = Path.Combine(modFolder, "troops_description_mod_strings.xml");
                    Load(xmlPath);
                }

                string key = $"troopsdescription.{troopId}_description";
                return _strings.ContainsKey(key) ? _strings[key] : null;
            }
        }
    }
}