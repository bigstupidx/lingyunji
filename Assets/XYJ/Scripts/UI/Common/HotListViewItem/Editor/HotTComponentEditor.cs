namespace xys.UI
{
    using UnityEditor;

    [CustomEditor(typeof(HotTComponent), true)]
    class HotTComponentEditor : UIWidgets.ListViewItemEditor
    {
        protected IL.Editor.ILMonoEditor monoEditor = new IL.Editor.ILMonoEditor();

        protected override void OnEnable()
        {
            base.OnEnable();
            monoEditor.InitByBaseType(target, "xys.hot.UI.HotTComponentBase");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            HotTComponent item = target as HotTComponent;
            if (item.isShowTypeInfo = EditorGUILayout.Foldout(item.isShowTypeInfo, "映射类型数据"))
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
