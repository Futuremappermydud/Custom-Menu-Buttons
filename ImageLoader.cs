using CustomMenuButtons.Utils;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace CustomMenuButtons
{
    public class ImageInfo
    {
        public ButtonOverride overrideInfo;
        public ImagePair pair;
        public Sprite unSelectedSprite;
        public Sprite selectedSprite;
    }
    class ImageLoader
    {
        public static ImageInfo[] LoadAll()
        {
            List<ImageInfo> infos = new List<ImageInfo>();
            for (int i = 0; i < Settings.Config.instance.ButtonOverrides.Count; i++)
            {
                ButtonOverride Override = Settings.Config.instance.ButtonOverrides[i];
                ImageInfo info = Load(Override);
                infos.Add(info);
            }
            return infos.ToArray();
        }
        public static ImageInfo Load(ButtonOverride Override)
        {
            ImageInfo info = new ImageInfo();
            info.overrideInfo = Override;
            ImagePair pair = Settings.Config.instance.ImagePairs.First((ImagePair p) => { return p.PairName == Override.PairName; });
            info.pair = pair;
            info.selectedSprite = Utils.Utils.LoadNewSprite(Path.Combine(IPA.Utilities.UnityGame.UserDataPath, "Buttons", pair.SelectedImagePath));
            info.unSelectedSprite = Utils.Utils.LoadNewSprite(Path.Combine(IPA.Utilities.UnityGame.UserDataPath, "Buttons", pair.UnselectedImagePath));
            return info;
        }
    }
}
