using BS_Utils.Utilities;
using IPA;
using IPALogger = IPA.Logging.Logger;
using Config = CustomMenuButtons.Settings.Config;
using IPA.Config.Stores;
using CustomMenuButtons.Utils;
using UnityEngine;
using HMUI;
using System.IO;
using BeatSaberMarkupLanguage;
using System.Linq;
using Polyglot;
using BeatSaberMarkupLanguage.MenuButtons;
using System.Reflection;
using BeatSaberMarkupLanguage.FloatingScreen;
using CustomMenuButtons.UI;
using System.Collections.Generic;
using IPA.Utilities;
using BeatSaberMarkupLanguage.Animations;
using Newtonsoft.Json;

namespace CustomMenuButtons
{
    
    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        private PosInfo pos;

        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        internal static SettingsFlowCoordinator _SettingsFlowCoordinator { get; private set; }

        [Init]
        public Plugin(IPALogger logger, IPA.Config.Config cfg)
        {
            pos = new SpritePosLoader().Load();
            Config.instance = cfg.Generated<Config>();
            Instance = this;
            Log = logger;
            MenuButtons.instance.RegisterButton(new MenuButton("Custom Menu Buttons", "", LoadSettingsUI));
            
        }
        void LoadSettingsUI()
        {
            var _SettingsFlowCoordinator = BeatSaberUI.CreateFlowCoordinator<Settings.SettingsFlowCoordinator>();
            BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(_SettingsFlowCoordinator);
        }

        [OnStart]
        public void OnApplicationStart()
        {
            BSEvents.lateMenuSceneLoadedFresh += OnMenuActive;
        }

        [OnExit]
        public void OnApplicationQuit()
        {
            BSEvents.lateMenuSceneLoadedFresh -= OnMenuActive;
        }

        //Takes a Sprite and an replacement atlas and trys to replace the Texture
        Sprite GetReplacementSprite(Sprite OldSprite, Texture2D NewAtlas)
        {
            if (OldSprite.name.Contains("Overlay")) return OldSprite;
            float multiplierX = (NewAtlas.width / OldSprite.texture.width);
            float multiplierY = (NewAtlas.height / OldSprite.texture.height);
            Rect r = pos.FuckYouBeatgamesWhyCantSpritePositionsBeConsistantDearLordIHateMyLife[OldSprite.name];
            Log.Debug(r.ToString());
            Vector2 p = OldSprite.pivot;
            Sprite New = Sprite.Create(NewAtlas, new Rect(r.x * multiplierX, r.y * multiplierY, r.width * multiplierX, r.height * multiplierY), p, 100.0f);
            return New; 
        }
        public static byte[] GetResource(Assembly asm, string ResourceName)
        {
            Stream stream = asm.GetManifestResourceStream(ResourceName);
            byte[] data = new byte[stream.Length];
            stream.Read(data, 0, (int)stream.Length);
            return data;
        }

        Texture2D DuplicateTexture(Texture2D source)
        {
            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);
            return readableText;
        }
        internal void OnMenuActive(ScenesTransitionSetupDataSO transitionSetupDataSO)
        {
            /*
            var result = SpriteAnimationUtil.Loaders.Loader.Load(Path.Combine(UnityGame.InstallPath, "Sparling"), "Normal", 60f);

            var result2 = SpriteAnimationUtil.Loaders.Loader.Load(Path.Combine(UnityGame.InstallPath, "Sparling"), "Highlight", 60f);

            List<SequenceLoadData> loadDatas = new List<SequenceLoadData>();
            loadDatas.Add(new SequenceLoadData(result.atlas, Path.Combine(UnityGame.InstallPath, "Sparling", "Unhover.ogg"), result.uvs, result.delays, NoTransitionsButton.SelectionState.Normal));
            loadDatas.Add(new SequenceLoadData(result2.atlas, Path.Combine(UnityGame.InstallPath, "Sparling", "Hover.ogg"), result2.uvs, result2.delays, NoTransitionsButton.SelectionState.Highlighted));

            var swaps = Resources.FindObjectsOfTypeAll<ButtonSpriteSwap>();
            GameObject the = swaps.First().gameObject;
            Object.Destroy(the.GetComponent<ButtonSpriteSwap>());
            CustomMenuButtonSwap swap = the.AddComponent<CustomMenuButtonSwap>();
            swap.Init(the.GetComponentsInChildren<ImageView>(), loadDatas.ToArray(), new StaticData()); ;*/

            if (Config.instance.UseAtlas) {
                var Sprites = Resources.FindObjectsOfTypeAll<ButtonSpriteSwap>();
                string atlasPath = Path.Combine(UnityGame.UserDataPath, "Buttons", Config.instance.AtlasPath);
                if(!Directory.Exists(Path.Combine(UnityGame.UserDataPath, "Buttons")))
                {
                    Directory.CreateDirectory(Path.Combine(UnityGame.UserDataPath, "Buttons"));
                }
                bool atlasExists = File.Exists(atlasPath);
                if(!atlasExists)
                {
                    Log.Error("Atlas does not exist!");
                    return;
                }
                Texture2D newAtlas = Utils.Utils.LoadTexture(atlasPath);
                if((newAtlas.width / newAtlas.height)!=1)
                {
                    for (int i = 0; i < 15; i++)
                    {
                        Log.Critical("ATLAS SIZE IS NOT A 1:1 RATIO PLEASE RESIZE!");
                        
                    }
                    return;
                }
                for (int i = 0; i < Sprites.Length; i++)
                {
                    ButtonSpriteSwap btn = Sprites[i];
                    ImageView OverlaySprite = btn.transform.Find("Image")?.Find("ImageOverlay")?.GetComponent<ImageView>();
                    if(OverlaySprite)
                    {
                        OverlaySprite.sprite = GetReplacementSprite(OverlaySprite.sprite, newAtlas);
                        OverlaySprite.gameObject.SetActive(!Config.instance.HideOverlay && atlasExists);
                    }
                        
                    
                    FieldInfo[] fields = typeof(ButtonSpriteSwap).GetFields(
                          BindingFlags.NonPublic |
                          BindingFlags.Instance);
                    FieldInfo normInfo = fields.First((FieldInfo l) => { return l.Name == "_normalStateSprite"; });
                    FieldInfo highlightInfo = fields.First((FieldInfo l) => { return l.Name == "_highlightStateSprite"; });
                    Sprite normal = (Sprite)normInfo.GetValue(btn);
                    Sprite hover = (Sprite)highlightInfo.GetValue(btn);
                    Sprite newNormal = GetReplacementSprite(normal, newAtlas);
                    Sprite newHover =  GetReplacementSprite(hover, newAtlas);
                    normInfo.SetValue(btn, newNormal);
                    highlightInfo.SetValue(btn, newHover);
                }
            }
            else {
                var infos = ImageLoader.LoadAll();
                var objs = Resources.FindObjectsOfTypeAll<NoTransitionsButton>(); //I'm so sorry but Gameobject.Find Barely works
                Log.Debug("Menu Loaded!");
                for (int index = 0; index < infos.Count(); index++)
                {
                    ImageInfo item = infos[index];
                    if (!item.overrideInfo.Enabled) continue;
                    var obj = objs.First((NoTransitionsButton g) => g.gameObject.name == item.overrideInfo.ObjectName);
                    if (!obj)
                    {
                        Log.Error(item.overrideInfo.ObjectName + " Does Not Exist, Skipping!");
                        continue;
                    }
                    if (item.overrideInfo.OverrideImage)
                    {
                        Sprite unSelected = item.unSelectedSprite;
                        if (!unSelected)
                        {
                            Log.Error("Unselected Sprite is null! Are there previous errors?");
                            continue;
                        }
                        else
                        {
                            Sprite selected = item.selectedSprite;
                            if (!selected)
                            {
                                Log.Error("Selected Sprite is null! Are there previous errors?");
                                continue;
                            }
                            else
                            {
                                BeatSaberUI.SetButtonStates(obj, unSelected, selected);
                                Log.Debug("Successfully replace " + item.overrideInfo.ObjectName);
                            }
                        }
                    }
                    if (item.overrideInfo.OverrideText)
                    {
                        obj.GetComponentInChildren<LocalizedTextMeshProUGUI>().Key = item.overrideInfo.Text;
                    }
                }
            }
        }
    }
}
