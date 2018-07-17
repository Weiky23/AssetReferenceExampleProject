using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

namespace AssetsNamespace
{
    public class RuntimeAssetLoader : AssetLoader
    {
        public RuntimeAssetLoader(string assetBundlesFolder)
            : base(assetBundlesFolder)
        {
        }
    }
}
