using UnityEngine;
using System;

namespace AssetsNamespace
{
    [Serializable]
    public class AssetInfoScriptableObject : ScriptableObject
    {
        public UnityEngine.Object asset;

        public const string Asset = "asset";
    }
}