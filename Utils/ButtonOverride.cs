using System;
using UnityEngine;

namespace CustomMenuButtons.Utils
{
    [Serializable]
    public class ButtonOverride
    {
        public virtual string ObjectName { get; set; } = "SoloButton";
        public virtual bool Enabled { get; set; } = false;
        public virtual bool OverrideText { get; set; } = false;
        public virtual bool OverrideImage { get; set; } = false;
        public virtual string Text { get; set; } = "Single Player";
        public virtual string PairName { get; set; } = "SoloPair";

        public ButtonOverride()
        {
        }

        public ButtonOverride(string objectName, bool enabled, bool overrideText, bool overrideImage, string text, string pairName)
        {
            ObjectName = objectName;
            Enabled = enabled;
            OverrideText = overrideText;
            OverrideImage = overrideImage;
            Text = text;
            PairName = pairName;
        }
    }

    [Serializable]
    public class ImagePair
    {
        public virtual string PairName { get; set; } = "SoloPair";
        public virtual string UnselectedImagePath { get; set; } = "SoloTemp1.png";
        public virtual string SelectedImagePath { get; set; } = "SoloTemp2.png";

        public ImagePair()
        {
        }

        public ImagePair(string pairName, string unselectedImagePath, string selectedImagePath)
        {
            PairName = pairName;
            UnselectedImagePath = unselectedImagePath;
            SelectedImagePath = selectedImagePath;
        }
    }
}
