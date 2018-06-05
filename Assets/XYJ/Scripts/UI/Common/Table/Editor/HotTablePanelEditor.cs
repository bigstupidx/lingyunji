using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;

namespace xys.UI
{
    [CustomEditor(typeof(HotTablePanel), true)]
    class HotTablePanelEditor : UIHotPanelEditor
    {
        protected override void OnEnable()
        {
            monoEditor.InitByBaseType(target, "xys.hot.UI.HotTablePanelBase");

            flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            OnShowPageList();
        }

        protected void RegisterUndo()
        {
            UITools.RegisterUndo(target, GetType().Name);
        }

        BindingFlags flags;

        protected virtual void OnShowPageList()
        {
            var hot_table_panel_type = typeof(HotTablePanel);
            var hot_table_page_type = typeof(HotTablePage);
            HotTablePanel Target = target as HotTablePanel;
            List<HotTablePage> pages = new List<HotTablePage>(Target.gameObject.GetComponentsInChildren<HotTablePage>(true));

            string current = hot_table_panel_type.GetField("CurrentShow", flags).GetValue(target) as string;
            string newCurrent = GUIEditor.GuiTools.StringPopupT(false, "初始页", current, pages, (HotTablePage page) => 
            {
                string type = page.pageType;
                return type == null ? "" : type;
            });

            if (!string.Equals(newCurrent, current))
            {
                if (Application.isPlaying)
                {
                    hot_table_panel_type.GetField("CurrentShow", flags).SetValue(target, newCurrent);
                    Target.ShowType(newCurrent, null);
                }
                else
                {
                    RegisterUndo();
                    hot_table_panel_type.GetField("CurrentShow", flags).SetValue(target, newCurrent);
                    return;
                }
            }

            Target.isShowPageList = EditorGUILayout.Foldout(Target.isShowPageList, "分页列表");
            if (Target.isShowPageList)
            {
                if (GUILayout.Button("添加分页"))
                {
                    RegisterUndo();
                    Target.GetPageList().Add(new HotPageData());
                    EditorUtility.SetDirty(target);
                    return;
                }

                EditorGUI.indentLevel++;
                List<HotPageData> PageList = Target.GetPageList();
                for (int i = 0; i < PageList.Count; ++i)
                {
                    HotPageData tpd = PageList[i];
                    EditorGUILayout.BeginHorizontal();
                    tpd.isFoldouts = EditorGUILayout.Foldout(tpd.isFoldouts, tpd.Get() == null ? "未选择" : tpd.Get().pageType.ToString());
                    if (GUILayout.Button("删除"))
                    {
                        RegisterUndo();
                        PageList.RemoveAt(i);
                        --i;
                        continue;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (!tpd.isFoldouts)
                        continue;

                    EditorGUI.indentLevel++;
                    GUI.changed = false;
                    HotTablePage page = (HotTablePage)EditorGUILayout.ObjectField("分页", tpd.Get(), typeof(HotTablePage), true);

                    if (GUI.changed)
                    {
                        RegisterUndo();
                        tpd.Set(page);
                    }

                    if (page != null)
                    {
                        State.StateRoot sr = (State.StateRoot)EditorGUILayout.ObjectField("按钮", page.ToggleBtn, typeof(State.StateRoot), true);
                        if (sr != page.ToggleBtn)
                        {
                            RegisterUndo();
                            page.ToggleBtn = sr;
                        }
                    }

                    EditorGUI.indentLevel--;
                }

                EditorGUI.indentLevel--;
            }

        }
    }
}