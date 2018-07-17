using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetsNamespace
{
    [Serializable]
    public class AssetInfo
    {
        public string assetId;
        public AssetLoadType assetLoadType;
        public string location;
        public UnityEngine.Object asset;
        public string[] assetLabels;
        public string type;

        //public string assetProjectPath;

        public const string Asset = "asset";
    }

    public enum AssetLoadType
    {
        // ассет лежит в бандле
        AssetBundle,
        // ассет лежит в папке ресурсы
        Resources,
        // ассет лежит в исходном проекте - есть ссылка на него в поле asset
        Project
    }
}
