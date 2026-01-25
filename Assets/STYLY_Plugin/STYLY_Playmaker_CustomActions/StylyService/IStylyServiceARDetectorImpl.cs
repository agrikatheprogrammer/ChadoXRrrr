using System;
using UnityEngine;

/// <summary>
/// interface to handle AR Detector
/// </summary>
public interface IStylyServiceARDetectorImpl
{
    /// <summary>
    /// Immersalによる位置合わせを行う
    /// </summary>
    /// <param name="localizeInterval">ローカライズのインターバル</param>
    /// <param name="burstMode">バーストモードを利用するかどうか</param>
    /// <param name="mapFile">Immersalのマップ作成アプリで作成したbytes形式のファイル</param>
    /// <param name="onFinished">終了イベント</param>
    void ImmersalDetect(float localizeInterval, bool burstMode, TextAsset mapFile,
        Action<Exception> onFinished);
    
    /// <summary>
    /// Immersal初期化処理
    /// 初期化とローカライズの処理を分けたいという要望から生まれたカスタムアクション
    /// </summary>
    /// <param name="mapFile">Immersalのマップ作成アプリで作成したbytes形式のファイル</param>
    /// <param name="onFinished">終了イベント</param>
    void ImmersalInit(TextAsset mapFile, Action<Exception> onFinished);
    
    /// <summary>
    /// Immersalのローカライズ呼び出し
    /// 初期化とローカライズの処理を分けたいという要望から生まれたカスタムアクション
    /// </summary>
    /// <param name="onFinished">終了イベント</param>
    void ImmersalLocalize(Action<Exception> onFinished);
}
