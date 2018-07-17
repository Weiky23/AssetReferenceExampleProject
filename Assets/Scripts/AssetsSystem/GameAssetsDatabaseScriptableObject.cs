using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AssetsNamespace
{
    [Serializable]
    public class GameAssetsDatabaseScriptableObject : ScriptableObject
    {
        public AssetInfo[] assetInfos;

        [NonSerialized]
        Dictionary<string, AssetInfo> assetTable;

        [NonSerialized]
        Dictionary<string, List<string>> assetIdsByLabel;

        [NonSerialized]
        Dictionary<string, string> uniqueAssetIdsByType;

        [NonSerialized]
        string[] assetLabels;

        const string all = "all";
        public const string AssetInfos = "assetInfos";

        void OnEnable()
        {
            Initialize();
        }

        public void Initialize()
        {
            CreateAssetTable();
            CreateAssetIdsByLabel();
            CreateUniqueAssetsTable();
            CreateAssetLabels();
        }

        public void CreateAssetTable()
        {
            if (assetInfos == null)
            {
                assetInfos = new AssetInfo[0];
            }
            assetTable = new Dictionary<string, AssetInfo>();
            for (int i = 0; i < assetInfos.Length; i++)
            {
                AssetInfo assetInfo = assetInfos[i];
                assetTable.Add(assetInfo.assetId, assetInfo);
            }
        }

        void CreateAssetIdsByLabel()
        {
            assetIdsByLabel = new Dictionary<string, List<string>>();
            assetIdsByLabel.Add(all, new List<string>());
            for (int i = 0; i < assetInfos.Length; i++)
            {
                AddAssetIdToLabel(assetInfos[i].assetId, all);
                string[] assetLabels = assetInfos[i].assetLabels;
                if (assetLabels != null)
                {
                    for (int k = 0; k < assetLabels.Length; k++)
                    {
                        AddAssetIdToLabel(assetInfos[i].assetId, assetLabels[k]);
                    }
                }
            }
        }

        void AddAssetIdToLabel(string assetId, string assetLabel)
        {
            List<string> idsByLabel;
            if (assetIdsByLabel.TryGetValue(assetLabel, out idsByLabel))
            {
                idsByLabel.Add(assetId);
                return;
            }
            idsByLabel = new List<string>();
            idsByLabel.Add(assetId);
            assetIdsByLabel.Add(assetLabel, idsByLabel);
        }
        
        void CreateUniqueAssetsTable()
        {
            uniqueAssetIdsByType = new Dictionary<string, string>();

            string[] uniqueAssetsIds = GetAssetIdsByLabel(AssetLabelNames.Unique);
            for (int i = 0; i < uniqueAssetsIds.Length; i++)
            {
                AssetInfo assetInfo = GetAssetInfo(uniqueAssetsIds[i]);
                uniqueAssetIdsByType.Add(assetInfo.type, uniqueAssetsIds[i]);
            }
        }

        void CreateAssetLabels()
        {
            assetLabels = assetIdsByLabel.Keys.ToArray();
        }

        public AssetInfo GetAssetInfo(string assetId)
        {
            AssetInfo assetInfo;
            assetTable.TryGetValue(assetId, out assetInfo);
            if (assetInfo == null)
            {
                Debug.LogError("Нет информации об ассете в базе " + assetId);
            }
            return assetInfo;
        }

        public string GetUniqueAssetId(Type type)
        {
            string assetId = string.Empty;
            if (!uniqueAssetIdsByType.TryGetValue(type.ToString(), out assetId))
            {
                Debug.LogError("Нет уникального ассета в базе для типа " + type.ToString() + " Нужно пометить ассет этого типа Ассет Лейблом " + AssetLabelNames.Unique + " и пересобрать базу ассетов");
            }
            return assetId;
        }

        public AssetInfo GetAssetInfo(Type type)
        {
            string assetId = string.Empty;
            if (uniqueAssetIdsByType.TryGetValue(type.ToString(), out assetId))
            {
                return GetAssetInfo(assetId);
            }
            Debug.LogError("Нет уникального ассета в базе для типа " + type.ToString() + " Нужно пометить ассет этого типа Ассет Лейблом " + AssetLabelNames.Unique + " и пересобрать базу ассетов");
            return null;
        }

        public string[] GetAssetIdsByLabel(string label)
        {
            List<string> assetsWithLabel;
            assetIdsByLabel.TryGetValue(label, out assetsWithLabel);
            if (assetsWithLabel == null)
            {
                return new string[0];
            }
            return assetsWithLabel.ToArray();
        }

        public string[] GetAssetLabels()
        {
            return assetLabels;
        }
    }
}
