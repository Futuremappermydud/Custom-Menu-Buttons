using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomMenuButtons
{
    //Used To AutoGen Pos
    [Serializable]
    public class M
    {
        [SerializeField] public float x;
        [SerializeField] public float y;
        [SerializeField] public float width;
        [SerializeField] public float height;

        public M(float x, float y, float width, float height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public static implicit operator Rect(M m) => new Rect(m.x, m.y, m.width, m.height);
    }
    [Serializable]
    public class PosInfo
    {
        [SerializeField] public Dictionary<string, M> FuckYouBeatgamesWhyCantSpritePositionsBeConsistantDearLordIHateMyLife = new Dictionary<string, M>();
    }
    class SpritePosLoader
    {
        public PosInfo Load()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "CustomMenuButtons.pos.json";


            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                string result = reader.ReadToEnd();
                return Newtonsoft.Json.JsonConvert.DeserializeObject<PosInfo>(result);
            }
        }
    }
}
