using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using HMUI;
using UnityEngine;
using UnityEngine.UI;
using IPA.Utilities;
using System.Collections.Generic;
using CustomMenuButtons.Utils;

namespace CustomMenuButtons.Settings
{
    internal class SettingsUI : BSMLResourceViewController
    {
        public override string ResourceName => Config.instance.UseAtlas ? ResourceNameAtlas : ResourceNameNormal;
        public string ResourceNameNormal => "CustomMenuButtons.Settings.SettingsUI.bsml";
        public string ResourceNameAtlas => "CustomMenuButtons.Settings.SettingsUIAtlas.bsml";
        public SettingsFlowCoordinator flow = null;
        //Pair Settings
        [UIComponent("pairList")]
        public CustomListTableData pairCustomListTableData = null;
        List<ImagePair> pairsInTable = new List<ImagePair>();
        [UIAction("pairSelect")]
        public void PairSelect(TableView table, int row)
        {
            if (pairsInTable.Count <= row)
            {
                Plugin.Log.Info("Add New pair");
                ImagePair newPair = new ImagePair("UnsetName", "Image.png", "Image.png");
                Config.instance.ImagePairs.Add(newPair);
                pairsInTable.Add(newPair);
                var customCellInfo = new CustomListTableData.CustomCellInfo(newPair.PairName, "", null);
                pairCustomListTableData.data.Insert(pairsInTable.Count-1, customCellInfo);
                pairCustomListTableData.tableView.ReloadData();
                flow.ShowPairSettings();
                flow._pairViewController.SetData(newPair);
                return;
            }
            flow.ShowPairSettings();
            flow._pairViewController.SetData(pairsInTable[row]);
        }
        //End Pair Settings
        //Override Settings
        [UIComponent("overrideList")]
        public CustomListTableData overrideCustomListTableData = null;
        List<ButtonOverride> overridesInTable = new List<ButtonOverride>();
        [UIAction("overrideSelect")]
        public void OverrideSelect(TableView table, int row)
        {
            if(overridesInTable.Count <= row)
            {
                Plugin.Log.Info("Add New Override");
                return;
            }
            flow.ShowOverrideSettings();
            flow._overrideViewController.SetData(overridesInTable[row]);
        }
        //End Override Settings
        [UIAction("#post-parse")]
        public void SetupList()
        {
            if (Config.instance.UseAtlas) return;
            //Pairs
            pairCustomListTableData.tableView.ClearSelection();
            pairCustomListTableData.data.Clear();
            foreach (var Pair in Settings.Config.instance.ImagePairs)
            {   
                pairsInTable.Add(Pair);
                var customCellInfo = new CustomListTableData.CustomCellInfo(Pair.PairName, "", null);
                pairCustomListTableData.data.Add(customCellInfo);
            }
            var NewPairCell = new CustomListTableData.CustomCellInfo("Add New Pair", "Adds a new pair", null);
            pairCustomListTableData.data.Add(NewPairCell);
            pairCustomListTableData.tableView.ReloadData();

            //Overrides
            overrideCustomListTableData.tableView.ClearSelection();
            overrideCustomListTableData.data.Clear();
            foreach (var Override in Settings.Config.instance.ButtonOverrides)
            {
                overridesInTable.Add(Override);
                var customCellInfo = new CustomListTableData.CustomCellInfo(Override.ObjectName, "", null);
                overrideCustomListTableData.data.Add(customCellInfo);
            }
            //var NewOverrideCell = new CustomListTableData.CustomCellInfo("Add New Override", "Adds a new override", null);
            //overrideCustomListTableData.data.Add(NewOverrideCell);
            overrideCustomListTableData.tableView.ReloadData();
        }

        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            base.DidActivate(firstActivation, addedToHierarchy, screenSystemEnabling);
        }
    }
}