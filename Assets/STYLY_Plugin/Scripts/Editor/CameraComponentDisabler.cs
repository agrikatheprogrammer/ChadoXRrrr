using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace STYLY.Uploader
{
    /// <summary>
    /// アップロードしようとしているアセット内のCameraコンポーネントをdisable、もしくは警告を出す処理を行うクラス
    /// prefabか、モデルデータかどうかによって処理を分ける
    /// ※ シーンアセットについてはここでは行わない (SceneProcessorクラスで実施）
    /// </summary>
    public class CameraComponentDisabler
    {
        private readonly IDialogService dialogService;

        public CameraComponentDisabler(IDialogService dialogService)
        {
            this.dialogService = dialogService;
        }
        
        /// <summary>
        /// 指定アセット内のCameraコンポーネントをチェックし、disableしたり警告を出したりする
        /// </summary>
        /// <param name="assetObjects"></param>
        /// <returns></returns>
        public bool CheckCameraComponentInAssets(List<UnityEngine.Object> assetObjects)
        {
            foreach (var obj in assetObjects)
            {
                if (!(obj is GameObject go))
                {
                    continue;
                }
                var assetPath = AssetDatabase.GetAssetPath(go);

                if (Path.GetExtension(assetPath) == ".prefab")
                {
                    // prefabの場合、カメラをdisableする
                    if (!DisableCamerasInPrefab(go))
                    {
                        return false;
                    }
                }
                else if (Config.HasModelDataExtension(assetPath))
                {
                    // 3Dモデルデータの場合、カメラがあったら警告する
                    if (!CheckCamerasInModelData(go))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 指定コンポーネントのパス文字列をメッセージ表示用に取得
        /// Transformの親子関係を文字列で表示する。
        /// 例: "/ GameObject1 / CameraRig / Camera1" のような文字列を取得
        /// </summary>
        /// <param name="targetTrans"></param>
        /// <returns>パス文字列</returns>
        private string GetComponentPathString(Transform targetTrans)
        {
            if (targetTrans.parent == null)
            {
                return "/ " + targetTrans.name;
            }
            return GetComponentPathString(targetTrans.parent) + " / " + targetTrans.name;
        }

        /// <summary>
        /// STYLYにおいて不要と思われるCameraコンポーネントを取得する
        /// RenderTargetがセットされておらず、enable状態のコンポーネントを取得する
        /// </summary>
        /// <param name="trans">対象GameObject</param>
        /// <returns>カメラコンポーネントのリスト</returns>
        private List<Camera> GetUnneededCameraComponents(Transform trans)
        {
            var cameras = trans.GetComponentsInChildren<Camera>(true);
            var unneededCameras = cameras
                .Where(camera =>
                {
                    // Render Texture用ではなく、enabledなカメラを対象にする
                    return camera.targetTexture == null
                           && camera.enabled == true;
                })
                .ToList();
            return unneededCameras;
        }

        /// <summary>
        /// prefab内のCameraコンポーネントをdisableして保存する
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        private bool DisableCamerasInPrefab(GameObject go)
        {
            var camerasToDisable = GetUnneededCameraComponents(go.transform);
            if (camerasToDisable.Count == 0)
            {
                return true;
            }
            
            // 対象Cameraコンポーネントのパス文字列のリストを作成
            List<string> pathNames = camerasToDisable.Select(camera => GetComponentPathString(camera.transform)).ToList();

            // ダイアログでdisableするかどうかを確認
            var message = "<< Warning >>\n\n"
                          + $"Asset: <{AssetDatabase.GetAssetPath(go)}>\n\n"
                          + "Camera components in the prefab may interfere with the STYLY system.\n"
                          + "These Camera components will be DISABLED.\n"
                          + "Do you proceed?\n\n"
                          + "Target Components:\n"
                          + " - " + string.Join("\n - ", pathNames);
            if (!dialogService.DisplayDialog("STYLY Plugin", message, "Proceed", "Abort"))
            {
                return false;
            }
            
            // Cameraコンポーネントをdisableし、アセット保存する
            foreach (var camera in camerasToDisable)
            {
                camera.enabled = false;
                EditorUtility.SetDirty(go);
                AssetDatabase.SaveAssets();
            }
            return true;
        }

        /// <summary>
        /// prefabではない3Dモデルデータ内のCameraコンポーネントを処理する
        /// 3Dモデルデータは変更して保存というのができないので、ダイアログを表示して終わりにする。
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        private bool CheckCamerasInModelData(GameObject go)
        {
            var camerasToDisable = GetUnneededCameraComponents(go.transform);
            if (camerasToDisable.Count == 0)
            {
                return true;
            }
            
            // 対象Cameraコンポーネントのパス文字列のリストを作成
            List<string> pathNames = camerasToDisable.Select(camera => GetComponentPathString(camera.transform)).ToList();

            // ダイアログでそのまま進めるかどうかを確認
            var message = "<< Warning >>\n\n"
                          + $"Asset: <{AssetDatabase.GetAssetPath(go)}>\n\n"
                          + "Camera Components may interfere with the STYLY system.\n"
                          + "Please consider ignoring cameras in import settings.\n"
                          + "Do you proceed? \n\n"
                          + "Target Components:\n"
                          + " - " + string.Join("\n - ", pathNames);
            if (!dialogService.DisplayDialog("STYLY Plugin", message, "Proceed", "Abort"))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// ダイアログを出すインタフェース
        /// テスト時のモック用に切り出しておく
        /// </summary>
        public interface IDialogService
        {
            /// <summary>
            /// EditorUtility.DisplayDialog をモックするメソッド
            /// </summary>
            bool DisplayDialog(string title, string message, string ok, string cancel);
        }

        /// <summary>
        /// 本番用の IDialogService 実装
        /// 実際にダイアログを出す。
        /// </summary>
        public class EditorDialogService : IDialogService
        {
            public bool DisplayDialog(string title, string message, string ok, string cancel)
            {
                return EditorUtility.DisplayDialog(title, message, ok, cancel);
            }
        }    
    }
}