using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace xys.UI
{
    public abstract class TablePanelEditor<T> : UIPanelBaseEditor
    {
        TablePanel<T> Target = null;

        void OnEnable()
        {
            Target = target as TablePanel<T>;
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

        protected virtual void OnShowPageList()
        {
            T type = (T)(object)GUIEditor.GuiTools.EnumPopup(false, "初始页", (System.Enum)(object)Target.CurrentPage);
            if (Target.CurrentPage.ToString() != type.ToString())
            {
                RegisterUndo();

                Target.CurrentPage = type;
                return;
            }

            Target.isShowPageList = EditorGUILayout.Foldout(Target.isShowPageList, "分页列表");
            if (Target.isShowPageList)
            {
                if (GUILayout.Button("添加分页"))
                {
                    RegisterUndo();
                    Target.GetPageList().Add(new PageData());
                    EditorUtility.SetDirty(target);
                    return;
                }

                EditorGUI.indentLevel++;
                List<PageData> PageList = Target.GetPageList();
                for (int i = 0; i < PageList.Count; ++i)
                {
                    PageData tpd = PageList[i];
                    EditorGUILayout.BeginHorizontal();
                    tpd.isFoldouts = EditorGUILayout.Foldout(tpd.isFoldouts, tpd.Get<T>() == null ? "未选择" : tpd.Get<T>().pageType.ToString());
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
                    TablePage<T> page = (TablePage<T>)EditorGUILayout.ObjectField("分页", tpd.Get<T>(), typeof(TablePage<T>), true);

                    if (GUI.changed)
                    {
                        RegisterUndo();
                        tpd.Set<T>(page);
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