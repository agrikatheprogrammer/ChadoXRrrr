#if PLAYMAKER

using STYLY;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    /// <summary>
    /// カメラ画像を取得する
    /// </summary>
    [ActionCategory("STYLY")]
    public class GetARCameraTexture : FsmStateAction
    {
        [RequiredField]
        [Tooltip("Prepare and set a RenderTexture instead of a regular Texture.")]
        public FsmTexture renderTexture;

        [Tooltip("Event to send if there's an error on process.")]
        public FsmEvent errorEvent;
        
        [UIHint(UIHint.Variable)]
        [Tooltip("Error message if there's an error on process.")]
        public FsmString errorString;

        [Tooltip("Repeat every frame while the state is active.")]
        public bool everyFrame;

        public override void OnEnter()
        {
            if (!everyFrame)
            {
                DoGetARCameraTexture();
                Finish();
            }
        }

        public override void OnUpdate()
        {
            if (everyFrame)
            {
                DoGetARCameraTexture();
            }
        }

        private void DoGetARCameraTexture()
        {
            if (renderTexture == null)
            {
                errorString.Value = "renderTexture is null.";
                Fsm.Event(errorEvent);
                return;
            }
            
            if (renderTexture.Value is RenderTexture tex)
            {
                StylyServiceForPlayMaker.Instance.GetARCameraTexture(tex, error =>
                {
                    if (error == null)
                    {
                        return;
                    }
                    if (errorString != null)
                    {
                        errorString.Value = error.Message;
                    }
                    Fsm.Event(errorEvent);
                });
            }
            else
            {
                errorString.Value = "renderTexture is not RenderTexture.";
                Fsm.Event(errorEvent);
            }
        }

        public override void Reset()
        {
            renderTexture = null;
            errorString = null;
            errorEvent = null;
            everyFrame = false;
        }
    }
}

#endif