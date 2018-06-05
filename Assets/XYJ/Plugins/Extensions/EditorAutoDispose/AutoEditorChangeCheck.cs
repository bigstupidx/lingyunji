#if UNITY_EDITOR
using UnityEditor;
using System;

namespace EditorExtensions
{
    // Check if any control was changed inside a block of code.
    // When needing to check if GUI.changed is set to true inside a block of code, wrap the code inside BeginChangeCheck () and EndChangeCheck () like this AutoEditorChangeCheck
    public class AutoEditorChangeCheck : IDisposable
    {
        // Call this action while gui changed
        private Action OnGUIChange;

        // Block of code with controls that may set GUI.changed to true.
        public AutoEditorChangeCheck(Action changeAction)
        {
            OnGUIChange = changeAction;
            EditorGUI.BeginChangeCheck();
        }

        public void Dispose()
        {
            if (EditorGUI.EndChangeCheck())
            {
                if (OnGUIChange != null)
                    OnGUIChange();
            }
        }

    }
}
#endif