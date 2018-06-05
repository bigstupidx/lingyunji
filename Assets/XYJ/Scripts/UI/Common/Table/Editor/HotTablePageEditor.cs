using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace xys.UI
{
    [CustomEditor(typeof(HotTablePage))]
    class HotTablePageEditor : UnityEditor.Editor
    {
        IL.Editor.ILMonoEditor monoEditor = new IL.Editor.ILMonoEditor();

        protected void OnEnable()
        {
            monoEditor.InitByBaseType(target, "xys.hot.UI.HotTablePageBase");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            monoEditor.OnGUI();
            CheckStateRoot();
        }

        void CheckStateRoot()
        {
            var stateRoot = (State.StateRoot)typeof(HotTablePage).GetField("stateRoot", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(target);
            if (stateRoot == null)
                return;

            var s = stateRoot.States;
            if (s == null || s.Length == 0)
            {
                stateRoot.AddState();
                stateRoot.AddState();
            }
            if (s.Length == 1)
            {
                stateRoot.AddState();
            }

            s = stateRoot.States;
            if (s[1].Name != "选中")
            {
                s[1].Name = "选中";
            }

            if (s[0].Name != "未选中")
            {
                s[0].Name = "未选中";
            }
        }

        protected void RegisterUndo()
        {
            UITools.RegisterUndo(target, GetType().Name);
        }
    }
}