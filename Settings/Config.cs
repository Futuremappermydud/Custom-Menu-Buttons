using CustomMenuButtons.Utils;
using IPA.Config.Stores;
using IPA.Config.Stores.Attributes;
using IPA.Config.Stores.Converters;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace CustomMenuButtons.Settings
{
    internal class Config
    {
        public static Config instance = null;
        public virtual bool UseAtlas { get; set; } = true;
        public virtual bool HideOverlay { get; set; } = true;
        public virtual string AtlasPath { get; set; } = "Atlas.png";

        [NonNullable, UseConverter(typeof(ListConverter<ImagePair>))]
        public virtual List<ImagePair> ImagePairs { get; set; } = new List<ImagePair>()
        {
            new ImagePair("SoloPair", "SoloTemp1.png", "SoloTemp2.png")
        };
        [NonNullable, UseConverter(typeof(ListConverter<ButtonOverride>))]
        public virtual List<ButtonOverride> ButtonOverrides { get; set; } = new List<ButtonOverride>()
        {
            new ButtonOverride("SoloButton", false,  false, false, "Single Player", "SoloPair")
        };
        
        public virtual void Changed()
        {

        }
    }
}
