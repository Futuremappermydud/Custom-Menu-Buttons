using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Animations;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomMenuButtons.Utils
{
    class Utils
    {
        public static Sprite LoadNewSprite(string FilePath, float PixelsPerUnit = 100.0f)
        {
            string dirPath = Path.GetDirectoryName(FilePath);
            if(!Directory.Exists(dirPath))
            {
                Plugin.Log.Error(dirPath + " (Directory) Does Not Exist!");
                Directory.CreateDirectory(dirPath);  
                return null;
            }
            if (!File.Exists(FilePath))
            {
                Plugin.Log.Error(FilePath + " (File) Does Not Exist!");
                return null;
            }
            if (!FilePath.EndsWith(".gif"))
            {
                Sprite NewSprite;
                Texture2D SpriteTexture = LoadTexture(FilePath);
                NewSprite = Sprite.Create(SpriteTexture, new Rect(0, 0, SpriteTexture.width, SpriteTexture.height), new Vector2(0, 0), PixelsPerUnit);

                return NewSprite;
            }
            //gif support?
            return null;
        }

        public static Texture2D LoadTexture(string FilePath)
        {
            return LoadTexture(FilePath, 2, 2);
        }
        public static Texture2D LoadTexture(string FilePath, int w, int h)
        {
            Texture2D Tex2D;
            byte[] FileData;

            if (File.Exists(FilePath))
            {
                FileData = File.ReadAllBytes(FilePath);
                Tex2D = new Texture2D(w, h, TextureFormat.ARGB32, false);//No Weird Artifacts
                
                if (Tex2D.LoadImage(FileData))
                    return Tex2D;
            }
            return null;
        }
    }
}
