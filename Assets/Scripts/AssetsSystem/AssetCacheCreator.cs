using System;
using UnityEngine;

namespace AssetsNamespace
{
    // создает в системе кэш ссылок на ассеты - не инстанцирует их
    [RequireComponent(typeof(AssetSystemInitializer))]
    public class AssetCacheCreator : MonoBehaviour
    {
        public AssetReference[] assetsToLoad;

        public const string AssetsToLoad = "assetsToLoad";

        public void CreateAssetCache()
        {
            for (int i = 0; i < assetsToLoad.Length; i++)
            {
                AssetSystem.Instance.CashAsset(assetsToLoad[i].assetId);
            }
        }
    }
}
