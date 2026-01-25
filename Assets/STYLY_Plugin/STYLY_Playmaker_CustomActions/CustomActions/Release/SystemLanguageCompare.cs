#if PLAYMAKER

using UnityEngine;

namespace HutongGames.PlayMaker.Actions.STYLY
{
    [ActionCategory("STYLY")]
    public class SystemLanguageCompare : FsmStateAction
    {
        [UIHint(UIHint.Variable)]
        public SystemLanguage language;

        [Tooltip("Event to send if the specified language is the same as the system language.")]
        public FsmEvent trueEvent;

        [Tooltip("Event to send if the specified language is NOT the same as the system language.")]
        public FsmEvent falseEvent;

        [UIHint(UIHint.Variable)]
        [Tooltip("Store the result in a bool variable.")]
        public FsmBool store;

        public override void Reset()
        {
            language = SystemLanguage.Japanese;
            trueEvent = null;
            falseEvent = null;
            store = null;
        }

        public override void OnEnter()
        {
            var result = Application.systemLanguage == language;
            
            if (store != null)
            {
                store.Value = result;
            }
            Fsm.Event(result ? trueEvent : falseEvent);
            
            Finish();
        }
    }
}

#endif