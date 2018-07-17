using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace AssetsNamespace
{
    // инстанцирует ассеты в пул
    [RequireComponent(typeof(AssetSystemInitializer))]
    public class AssetPoolCreator : MonoBehaviour
    {
        public AssetToPoolInfo[] assetsToPool;

        public const string AssetsToPool = "assetsToPool";

        public void CreateAssetPool()
        {
            for (int i = 0; i < assetsToPool.Length; i++)
            {
                AssetSystem.Instance.InstantiateToPool(assetsToPool[i].assetId, assetsToPool[i].amount);
            }
        }
    }

    [Serializable]
    public class AssetToPoolInfo
    {
        public string label;
        public string assetId;
        public int amount;

        public const string Label = "label";
        public const string AssetId = "assetId";
        public const string Amount = "amount";
    }
}
