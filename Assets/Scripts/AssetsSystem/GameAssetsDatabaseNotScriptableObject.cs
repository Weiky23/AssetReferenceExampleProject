using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;

namespace AssetsNamespace
{
    // попытка первой реализации - не работает, потому что поле Object в AssetInfo сериализуется как InstanceID и при перезапуске ссылка ссылается на другой объект
    // если сохранять базу как ScriptableObject, поле Object в AssetInfo сериализуется как надо
    [Serializable]
    public class GameAssetsDatabase
    {
        public AssetInfo[] assetInfos;

        [NonSerialized]
        Dictionary<string, AssetInfo> assetTable;

        public GameAssetsDatabase(AssetInfo[] assetInfos)
        {
            this.assetInfos = assetInfos;
        }

        [OnDeserialized]
        void CreateAssetTable(StreamingContext context)
        {
            CreateAssetTable();
        }

        public void CreateAssetTable()
        {
            assetTable = new Dictionary<string, AssetInfo>();
            for (int i = 0; i < assetInfos.Length; i++)
            {
                AssetInfo assetInfo = assetInfos[i];
                assetTable.Add(assetInfo.assetId, assetInfo);
            }
        }

        public AssetInfo GetAssetInfo(string assetId)
        {
            AssetInfo assetInfo;
            assetTable.TryGetValue(assetId, out assetInfo);
            return assetInfo;
        }
    }
}
