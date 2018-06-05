namespace xys.UI
{
    using UIWidgets;
    using UnityEditor;

    [CustomEditor(typeof(HotTileView), true)]
    class HotTileViewEditor : ListViewCustomBaseEditor
    {
        protected IL.Editor.ILMonoEditor monoEditor = new IL.Editor.ILMonoEditor();

        protected override void OnEnable()
        {
            base.OnEnable();
            monoEditor.InitByAttribute(target, typeof(HotTileViewAttribute));
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            HotTileView view = target as HotTileView;
            if (view.isShowTypeInfo = EditorGUILayout.Foldout(view.isShowTypeInfo, "映射类型数据"))
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
