using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AssetsNamespace
{
    //[CreateAssetMenu(fileName = "Asset Labels", menuName = "1")]
    public class AssetLabels : ScriptableObject
    {
        public string[] assetLabels;
        public const string AssetLabelsName = "assetLabels";
    }
}
