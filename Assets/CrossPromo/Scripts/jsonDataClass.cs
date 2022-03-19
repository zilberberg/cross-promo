using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.CrossPromo.Scripts
{
    [System.Serializable]
    public class jsonDataClass
    {
        public List<videoData> results;
    }

    [System.Serializable]
    public class videoData
    {
        public string id;
        public string video_url;
        public string click_url;
        public string tracking_url;
        public string videoPath;
        public bool isTracked;
    }
}