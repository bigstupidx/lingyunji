using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using EditorExtensions;

namespace xys.UI.State
{
    [InitializeOnLoad()]
    class InitStateRoot
    {
        static InitStateRoot()
        {
            if (ElementAgent.GetCurrentStateRoot == null)
                ElementAgent.GetCurrentStateRoot = () => { return StateRootEditor.instance == null ? null : StateRootEditor.instance.Get(); };

            if (StateElementDraw.CurrentDrawStateInfo == null)
            {
                StateElementDraw.CurrentDrawStateInfo = StateRootEditor.DrawStateInfo;
            }
        }
    }

    [CustomEditor(typeof(StateRoot), true)]
    public class StateRootEditor : UnityEditor.Editor
    {
        bool showElement = true;

        bool isShowRecord = false; // 记录当前数据

        public static StateRootEditor instance = null;

        public StateRoot Get()
        {
            if (target == null)
                return null;

            return target as StateRoot;
        }

        SerializedProperty onStateChangeProperty;
        SerializedProperty onClickProperty;

        void OnEnable()
        {
            instance = this;
            onStateChangeProperty = serializedObject.FindProperty("onStateChange");
            onClickProperty = serializedObject.FindProperty("onClick");
        }

        public static bool DrawStateInfo(Element element, int stateid, bool isShowRecord, bool iscanset)
        {
            StateRoot target = (instance == null ? null : instance.Get());
            using (new AutoEditorHorizontal())
            {
                element.Agent.ShowElementState(element, stateid, iscanset);
                GUILayout.Space(-5);
                using (new AutoEditorLabelWidth(40))//控件有限，只能填两个字
                {
                    ElementStateData esd = element.stateData[stateid];
                    if (element.Agent.ShowState(element, esd, stateid))
                    {
                        if (target == null || target.CurrentState == stateid)
                        {
                            element.Agent.Set(element, stateid);
                        }
                    }

                    if (element.Agent.isSmooth)
                    {
                        GUI.changed = false;
                        bool issmooth = GUILayout.Toggle(esd.isSmooth, "渐变", GUILayout.ExpandWidth(false));
                        float smoothTime = esd.smoothTime;
                        if (issmooth)
                        {
                            smoothTime = EditorGUILayout.FloatField("时长", smoothTime);
                        }

                        if (GUI.changed)
                        {
                            ElementAgent.RegisterUndo(() =>
                            {
                                esd.isSmooth = issmooth;
                                esd.smoothTime = smoothTime;
                            });
                        }
                    }

                    if (isShowRecord)
                    {
                        if (GUILayout.Button("记录"))
                        {
                            ElementAgent.RegisterUndo(() =>
                            {
                                element.Agent.Init(element, element.stateData[stateid]);
                            });
                        }
                    }

                    if (iscanset)
                    {
                        if (GUILayout.Button("移除"))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        void RegisterUndo()
        {
            UITools.RegisterUndo(target, "State Root Change");
        }

        public override void OnInspectorGUI()
        {
            StateRoot target = this.target as StateRoot;

            EditorGUIUtility.labelWidth = 80;

            GUI.changed = false;
            isShowRecord = EditorGUILayout.Toggle("记录按钮", isShowRecord);
            bool isClickSwitchState = EditorGUILayout.Toggle("点击切换状态", target.isClickSwitchState);

            //当前状态
            int currentState = EditorGUILayout.Popup("初始状态", target.CurrentState, target.StateNames);

            if (GUI.changed)
            {
                RegisterUndo();
                target.isClickSwitchState = isClickSwitchState;
                target.CurrentState = currentState;
            }

            showElement = EditorGUILayout.Foldout(showElement, "元素");
            if (showElement)
            {
                EditorGUI.indentLevel++;
                if (target.elements != null)
                {
                    for (int i = 0; i < target.elements.Count; )
                    {
                        if (StateElementDraw.DrawElement(target.elements[i]))
                        {
                            RegisterUndo();
                            target.RemoveElement(target.elements[i]);
                        }
                        else
                        {
                            ++i;
                        }
                    }
                }

                //增加按钮
                int idx = StateElementDraw.DrawType();
                if (idx != -1)
                {
                    RegisterUndo();
                    target.AddElement((Type)idx);
                }
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Separator();//华丽的分割线
            for (int i = 0; i < target.States.Length; ++i)
            {
                StateConfig sc = target.States[i];
                EditorGUILayout.BeginHorizontal();
                sc.isFoldouts = EditorGUILayout.Foldout(sc.isFoldouts, string.Format("状态({0})", sc.Name));
                if (GUILayout.Button("删除", GUILayout.Width(50)))//删除按钮
                {
                    RegisterUndo();
                    target.RemoveState(i);
                    return;
                }
                EditorGUILayout.EndHorizontal();

                if (sc.isFoldouts)
                {
                    EditorGUI.indentLevel++;
                    sc.Name = EditorGUILayout.TextField("状态名", sc.Name);

                    if (target.elements != null)
                    {
                        foreach (Element element in target.elements)
                        {
                            DrawStateInfo(element, i, isShowRecord, false);
                        }
                    }

                    EditorGUI.indentLevel--;
                }
            }

            //增加按钮
            if (GUILayout.Button("增加状态", GUILayout.Height(40)))
            {
                RegisterUndo();
                target.AddState();
            }

            Button ubutton = EditorGUILayout.ObjectField("按钮", target.uCurrentButton, typeof(Button), true) as Button;
            if (ubutton != target.uCurrentButton)
            {
                RegisterUndo();
                target.uCurrentButton = ubutton;
            }

            EditorGUILayout.PropertyField(onStateChangeProperty);
            EditorGUILayout.PropertyField(onClickProperty);

            serializedObject.ApplyModifiedProperties();

            //             NGUIEditorTools.DrawEvents("On Click", target, target.onClick, false);
            //             NGUIEditorTools.DrawEvents("On State Change", target, target.onStateChange, false);
        }
    }
}