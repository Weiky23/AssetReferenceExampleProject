using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetsNamespace
{
    [Serializable]
    public class AssetReference
    {
        public string label;
        public string assetId;

        public const string Label = "label";
        public const string AssetId = "assetId";

        // для ассетов – не инстанцируемых (звуки, ScriptableObject)
        public T GetObject<T>() where T : UnityEngine.Object
        {
            return AssetSystem.GetObject<T>(assetId);
        }

        // для ассетов – инстанцируемых, возвращается инстанцированный объект в выключенном состоянии
        public T GetAsset<T>() where T : Component
        {
            return AssetSystem.GetAsset<T>(assetId);
        }

        // для GameObject – возврается инстанцированный объект в выключенном состоянии
        public GameObject GetAsset()
        {
            return AssetSystem.GetAsset(assetId);
        }
    }
}
