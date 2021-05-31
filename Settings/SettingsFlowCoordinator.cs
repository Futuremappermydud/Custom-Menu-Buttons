using BeatSaberMarkupLanguage;
using HMUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CustomMenuButtons.Settings
{
	internal class SettingsFlowCoordinator : FlowCoordinator
	{
		internal SettingsUI _settingsViewController;
		internal PairSettings _pairViewController;
		internal ButtonSettings _overrideViewController;

		protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
		{	
			Plugin.Log.Debug("M");
			_settingsViewController = BeatSaberUI.CreateViewController<SettingsUI>();
			_pairViewController = BeatSaberUI.CreateViewController<PairSettings>();
			_overrideViewController = BeatSaberUI.CreateViewController<ButtonSettings>();
			try
			{
				if (firstActivation)
				{
					SetTitle("Custom Menu Buttons");
					showBackButton = true;
					_settingsViewController.flow = this;
					ProvideInitialViewControllers(_settingsViewController);
				}
			}
			catch (Exception ex)
			{
				Plugin.Log.Error(ex);
			}
		}

		protected override void BackButtonWasPressed(ViewController _)
		{
			BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
			if (Config.instance.UseAtlas) return;
			// Dismiss ourselves
			
			Config.instance.Changed();
		}

		public void ShowPairSettings()
		{
			SetLeftScreenViewController(_pairViewController, ViewController.AnimationType.In);
		}
		public void ShowOverrideSettings()
		{
			SetRightScreenViewController(_overrideViewController, ViewController.AnimationType.In);
		}
	}
}
