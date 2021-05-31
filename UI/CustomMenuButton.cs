using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HMUI;
using UnityEngine;

namespace CustomMenuButtons.UI
{
    public class StaticData
    {
        public Sprite normalSprite;
        public Sprite pressedSprite;
        public Sprite hoverSprite;
    }
    public class SequenceData
    {
        public List<Sprite> sprites = new List<Sprite>();
        public Rect[] uvs;
        public float[] delays;
        public int frame;
    }

    public class SequenceLoadData
    {
        public Texture2D atlas;
        public string clip;
        public Rect[] rects;
        public float[] delays;
        public NoTransitionsButton.SelectionState selectionState;

        public SequenceLoadData(Texture2D atlas, string clip, Rect[] rects, float[] delays, NoTransitionsButton.SelectionState selectionState)
        {
            this.atlas = atlas;
            this.clip = clip;
            this.rects = rects;
            this.delays = delays;
            this.selectionState = selectionState;
        }
    }
    public class CustomMenuButtonSwap : ButtonSpriteSwap
    {
        public Dictionary<NoTransitionsButton.SelectionState, AudioClip> clips = new Dictionary<NoTransitionsButton.SelectionState, AudioClip>();
        public Dictionary<NoTransitionsButton.SelectionState, SequenceData> sequenceData = new Dictionary<NoTransitionsButton.SelectionState, SequenceData>();

        public bool isStatic;

        public NoTransitionsButton.SelectionState currentState = NoTransitionsButton.SelectionState.Normal;
        public AudioSource source;

        public int uvIndex = 0;
        public DateTime lastSwitch = DateTime.UtcNow;
        public bool IsPlaying { get; set; } = false;
        public Material animMaterial;
        private bool _isDelayConsistent = true;
        public override void Awake()
        {
            _button = GetComponent<NoTransitionsButton>();
            base.Awake();
        }
        public Rect MultRect(Rect r, float mult)
        {
            r.x *= mult;
            r.y *= mult;
            r.width *= mult;
            r.height *= mult;
            return r;
        }

        IEnumerator LoadSongCoroutine(string path, NoTransitionsButton.SelectionState selectionState)
        {
            string url = string.Format("file:///{0}", path);
            WWW www = new WWW(url);
            yield return www;
            Plugin.Log.Info("Loaded");
            clips.Add(selectionState, www.GetAudioClip());
        }
        public void Init(ImageView[] images, SequenceLoadData[] loadDatas, StaticData staticData)
        {
            _button = GetComponent<NoTransitionsButton>();
            _button.selectionStateDidChangeEvent -= base.HandleButtonSelectionStateDidChange;
            _button.selectionStateDidChangeEvent += this.HandleButtonSelectionStateDidChange;
            _images = images;
            source = gameObject.AddComponent<AudioSource>();
            source.loop = false;
            if (!isStatic)
            {
                for (int i = 0; i < loadDatas.Length; i++)
                {
                    SequenceData loadedSequence = new SequenceData();
                    for (int x = 0; x < loadDatas[i].rects.Length; x++)
                    {
                        loadedSequence.sprites.Add(Sprite.Create(loadDatas[i].atlas, new Rect(loadDatas[i].rects[x].x, loadDatas[i].rects[x].y, loadDatas[i].rects[x].width, loadDatas[i].rects[x].height), new Vector2(0f, 0f), 100f));
                    }
                    loadedSequence.uvs = loadDatas[i].rects;
                    loadedSequence.delays = loadDatas[i].delays;
                    sequenceData.Add(loadDatas[i].selectionState, loadedSequence);
                        SharedCoroutineStarter.instance.StartCoroutine(LoadSongCoroutine(loadDatas[i].clip, loadDatas[i].selectionState));
                }
            }
            else
            {
                if(staticData != null)
                {
                    _normalStateSprite = staticData.normalSprite;
                    _pressedStateSprite = staticData.normalSprite;
                    _highlightStateSprite = staticData.hoverSprite;
                }
            }
            IsPlaying = true;
        }
        public override void HandleButtonSelectionStateDidChange(NoTransitionsButton.SelectionState state)
        {
            if (!IsPlaying) return;
            currentState = state;
            if (clips.ContainsKey(currentState))
            {
                source.clip = clips[currentState];
                source.Play();
            }
            if(isStatic)
            {
                base.HandleButtonSelectionStateDidChange(state);
            }
        }
        public override void OnDestroy()
        {
            if (_button != null)
            {
                _button.selectionStateDidChangeEvent -= HandleButtonSelectionStateDidChange;
            }
        }
        public void FixedUpdate()
        {
            DateTime now = DateTime.UtcNow;
            if (IsPlaying == true)
                CheckFrame(now);
        }

        internal void CheckFrame(DateTime now)
        {
            if (_images.Length == 0)
                return;
            uvIndex = Mathf.Clamp(uvIndex, 0, sequenceData[currentState].uvs.Length);
            SequenceData data = sequenceData[currentState];
            double differenceMs = (now - lastSwitch).TotalMilliseconds;
            if (differenceMs < data.delays[uvIndex])
                return;

            if (_isDelayConsistent && data.delays[uvIndex] <= 10 && differenceMs < 100)
            {
                // Bump animations with consistently 10ms or lower frame timings to 100ms
                return;
            }

            lastSwitch = now;
            do
            {
                uvIndex++;
                if (uvIndex >= data.uvs.Length)
                    uvIndex = 0;
            }
            while (!_isDelayConsistent && data.delays[uvIndex] == 0);

            foreach (ImageView image in _images)
            {
                image.sprite = data.sprites[uvIndex];
            }
        }
    }
}
