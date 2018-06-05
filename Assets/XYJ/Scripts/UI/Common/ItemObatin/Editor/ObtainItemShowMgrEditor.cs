using UnityEditor;

namespace xys.UI
{
    [CustomEditor(typeof(ObtainItemShowMgr), true)]
    class ObtainItemShowMgrEditor : UnityEditor.Editor
    {
        protected IL.Editor.ILMonoEditor monoEditor = new IL.Editor.ILMonoEditor();

        protected virtual void OnEnable()
        {
            monoEditor.Init(target);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            ObtainItemShowMgr panel = target as ObtainItemShowMgr;
            if (panel.isShowTypeInfo = EditorGUILayout.Foldout(panel.isShowTypeInfo, "映射类型数据"))
            {
                OnChildGUI();
            }
        }

        protected virtual void OnChildGUI()
        {
            monoEditor.OnGUI("xys.hot.UI.HotObtainItemShowMgr");
        }
    }
}
