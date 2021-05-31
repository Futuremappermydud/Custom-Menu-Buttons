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
    internal class ButtonSettings : BSMLResourceViewController
    {
        // For this method of setting the ResourceName, this class must be the first class in the file.
        public override string ResourceName => "CustomMenuButtons.Settings.ButtonSettings.bsml";

        [UIComponent("overrideName")]
        private TextMeshProUGUI text = null;

        [UIObject("Enabled")]
        private GameObject enabledToggle = null;

        [UIObject("OverrideImage")]
        private GameObject imgToggle = null;

        [UIObject("OverrideText")]
        private GameObject txtToggle = null;

        [UIComponent("ImagePair")]
        private StringSetting imagePair = null;

        private ButtonOverride currectOverride = null;

        private void EnabledToggleClicked(bool value)
        {
            Config.instance.ButtonOverrides.Remove(currectOverride);
            currectOverride.Enabled = value;
            Config.instance.ButtonOverrides.Add(currectOverride);
        }

        private void ImageToggleClicked(bool value)
        {
            Config.instance.ButtonOverrides.Remove(currectOverride);
            currectOverride.OverrideImage = value;
            Config.instance.ButtonOverrides.Add(currectOverride);
        }

        private void TextToggleClicked(bool value)
        {
            Config.instance.ButtonOverrides.Remove(currectOverride);
            currectOverride.OverrideText = value;
            Config.instance.ButtonOverrides.Add(currectOverride);
        }

        private void ImagePairChanged(string value)
        {
            Plugin.Log.Info("ImagePairChanged");
            Config.instance.ButtonOverrides.Remove(currectOverride);
            currectOverride.PairName = value;
            Config.instance.ButtonOverrides.Add(currectOverride);
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
            if(firstActivation)
            {
                enabledToggle.GetComponentInChildren<Toggle>().onValueChanged.AddListener(EnabledToggleClicked);
                imgToggle.GetComponentInChildren<Toggle>().onValueChanged.AddListener(ImageToggleClicked);
                txtToggle.GetComponentInChildren<Toggle>().onValueChanged.AddListener(TextToggleClicked);
                Action<string> mainDelegate = ImagePairChanged; //cringe
                MethodInfo mainMethod = mainDelegate.Method; //cringe
                imagePair.onChange = new BSMLAction(this, mainMethod); //cringe
            }
        }

        public void SetData(ButtonOverride Override)
        {
            currectOverride = Override;
            text.text = Override.ObjectName;    
            enabledToggle.GetComponentInChildren<Toggle>().isOn = Override.Enabled;
            imgToggle.GetComponentInChildren<Toggle>().isOn = Override.OverrideImage;
            txtToggle.GetComponentInChildren<Toggle>().isOn = Override.OverrideText;
            imagePair.Text = Override.PairName;
            
        }
    }
}
