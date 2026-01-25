using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace STYLY.MaintenanceTool
{
    //STYLYアセットデータとして保持する内容
    [System.Serializable]
    public class stylyAssetData
    {
        public string prefabName;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public string title;
        public string description;
        public string exclusiveCategory;
        public string[] vals;
        public string itemURL;
        public string visible = true.ToString();

        // 同一アセットチェック用キー生成
        public string GetSameCheckKey()
        {
            string valStr = "";
            if (vals != null)
            {
                valStr = string.Join(",", vals);
            }
            return prefabName + exclusiveCategory + valStr + itemURL + visible;
        }
    }

    [System.Serializable]
    [XmlRoot("stylyAssetDataSet")]
    public class stylyAssetDataSet
    {
        public SceneSettings SceneSettings = new SceneSettings();

        public stylyAssetData[] AssetDataSet;

        public bool IsEmpty
        {
            get { return (AssetDataSet.Length == 0); }
        }
    }

    [System.Serializable]
    public class SceneSettings
    {
        Vector3? startPosition;

        /// <summary>
        /// XMLシリアライズにおいて、"StartPosition" 項目の保存・再生用に利用されます
        /// </summary>
        public Vector3 StartPosition
        {
            get => startPosition ?? new Vector3(0, 0, -5f);
            set => startPosition = value;
        }

        Vector3? startRotation;
        /// <summary>
        /// XMLシリアライズにおいて、"StartRotation" 項目の保存・再生用に利用されます
        /// </summary>
        public Vector3 StartRotation
        {
            get => startRotation ?? Vector3.zero;
            set => startRotation = value;
        }
    }
}