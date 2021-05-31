using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Components.Settings;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using CustomMenuButtons.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace CustomMenuButtons.Settings
{
    internal class PairSettings : BSMLResourceViewController
    {
        // For this method of setting the ResourceName, this class must be the first class in the file.
        public override string ResourceName => "CustomMenuButtons.Settings.PairSettings.bsml";

        [UIComponent("pairName")]
        private TextMeshProUGUI text = null;

        [UIComponent("SelectedPath")]
        private StringSetting selectedPath = null;

        [UIComponent("UnselectedPath")]
        private StringSetting unselectedPath = null;

        private ImagePair currentPair = null;

        private void SelectedPathChanged(string value)
        {
            Plugin.Log.Info("SelectedPathChanged");
            Config.instance.ImagePairs.Remove(currentPair);
            currentPair.SelectedImagePath = value;
            Config.instance.ImagePairs.Add(currentPair);
        }

        private void UnselectedPathChanged(string value)
        {
            Plugin.Log.Info("UnselectedPathChanged");
            Config.instance.ImagePairs.Remove(currentPair);
            currentPair.UnselectedImagePath = value;
            Config.instance.ImagePairs.Add(currentPair);
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            if (firstActivation)
            {
                Action<string> mainDelegate = SelectedPathChanged; //cringe
                MethodInfo mainMethod = mainDelegate.Method; //cringe
                selectedPath.onChange = new BSMLAction(this, mainMethod); //cringe

                mainDelegate = UnselectedPathChanged; //cringe
                mainMethod = mainDelegate.Method; //cringe
                unselectedPath.onChange = new BSMLAction(this, mainMethod); //cringe
            }
        }

        public void SetData(ImagePair Pair)
        {
            currentPair = Pair;
            text.text = Pair.PairName;
            selectedPath.Text = Pair.SelectedImagePath;
            unselectedPath.Text = Pair.UnselectedImagePath;
        }
    }
}
