using UnityEditor;

namespace xys.UI
{
    [CustomEditor(typeof(UIHotPanel), true)]
    class UIHotPanelEditor : UIPanelBaseEditor
    {
        protected IL.Editor.ILMonoEditor monoEditor = new IL.Editor.ILMonoEditor();

        protected virtual void OnEnable()
        {
            monoEditor.InitByBaseType(target, "xys.hot.UI.HotPanelBase");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            UIHotPanel panel = target as UIHotPanel;
            if (panel.isShowTypeInfo = EditorGUILayout.Foldout(panel.isShowTypeInfo, "映射类型数据"))
            {
                OnChildGUI();
            }
        }

        protected virtual void OnChildGUI()
        {
            System.Type type = target.GetType();
            var attributes = type.GetCustomAttributes(typeof(SinglePanelType), true);
            if (attributes != null && attributes.Length != 0)
            {
                monoEditor.OnGUI(((SinglePanelType)attributes[0]).type);
                return;
            }

            monoEditor.OnGUI();
        }
    }
}
