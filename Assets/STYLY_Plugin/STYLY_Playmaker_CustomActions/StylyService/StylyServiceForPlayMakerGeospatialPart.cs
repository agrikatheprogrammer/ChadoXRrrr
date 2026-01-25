using System;
using UnityEngine;

namespace STYLY
{
    /// <summary>
    /// Geospatial of StylyServiceForPlayMaker (partial class)
    /// </summary>
    public partial class StylyServiceForPlayMaker
    {
        /// <summary>
        /// Geospatial用のインターフェース
        /// </summary>
        private IStylyServiceGeospatialImpl geospatialImpl;

        /// <summary>
        /// インターフェースをセット
        /// </summary>
        /// <param name="impl">実際の機能の中身</param>
        public void SetGeospatialImpl(IStylyServiceGeospatialImpl impl)
        {
            this.geospatialImpl = impl;
        }
        
        /// <summary>
        /// StreetscapeGeometryの初期化処理
        /// </summary>
        /// <param name="onFinished">終了イベント</param>
        public void StreetscapeGeometryInit(Action<Exception> onFinished)
        {
            if (geospatialImpl != null)
            {
                geospatialImpl.StreetscapeGeometryInit(onFinished);
            }
            else
            {
                // 未実装ならエラー通知
                var msg = "StreetscapeGeometryInit called, but the IStylyServiceGeospatialImpl implementation is not available.";
                Debug.LogError(msg);
                onFinished(new Exception("StylyServiceForPlayMaker implementation not available."));
            }
        }
    }
}
