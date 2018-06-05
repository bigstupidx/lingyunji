using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
#if USE_HOT
using ILRuntime.Runtime.Intepreter;
#endif

namespace xys.Editor
{
    [CustomEditor(typeof(ILMonoBehaviour), true)]
    class ILMonoBehaviourEditor : UnityEditor.Editor
    {
        IL.Editor.ILMonoEditor monoEditor = new IL.Editor.ILMonoEditor();

        private void OnEnable()
        {
            monoEditor.InitByAttribute(target, typeof(AutoILMono));
        }

        public override void OnInspectorGUI()
        {
            ILMonoBehaviour il = target as ILMonoBehaviour;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour(il), typeof(MonoScript), true);

            monoEditor.OnGUI();
        }
    }
}
