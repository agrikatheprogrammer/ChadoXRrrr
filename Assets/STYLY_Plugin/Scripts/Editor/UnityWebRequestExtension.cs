using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace STYLY.Uploader
{
    public static class UnityWebRequestExtension
    {
        private static int timeout = 10;
        /// <summary>
        /// Editor上でUnityWebRequestで通信をする
        /// </summary>
        public static void EditorSendWebRequest(this UnityWebRequest self, Action onSuccess = null, Action<Exception> onError = null)
        {
            self.timeout = timeout;
            self.SendWebRequest();

            EditorApplication.CallbackFunction updateFunc = null;
            updateFunc = () =>
            {
                if (self.isDone)
                {
#if UNITY_2020_2_OR_NEWER
                    if (self.result != UnityWebRequest.Result.Success)
#else
                    if (self.isHttpError || self.isNetworkError || !string.IsNullOrEmpty(self.error))
#endif
                    {
                        if (onError != null) onError.Invoke(new Exception(self.error));
                    }
                    else
                    {
                        if (onSuccess != null) onSuccess.Invoke();
                    }

                    EditorApplication.update -= updateFunc;
                    return;
                }
            };

            EditorApplication.update += updateFunc;
        }
    }
}
