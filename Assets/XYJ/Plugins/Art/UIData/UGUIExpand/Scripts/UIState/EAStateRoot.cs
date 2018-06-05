using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace xys.UI.State
{
    public partial class StateRootEA : TTTEA<StateRoot>
    {
        public override void Init(Element element, ElementStateData sd)
        {
            if (element.target != null)
            {
                StateRoot sr = element.GetTarget<StateRoot>();
                if (sr != null)
                    sd.intValue = sr.CurrentState;
            }
        }

        public override void Set(Element element, int index)
        {
            if (element.target != null)
            {
                StateRoot sr = element.GetTarget<StateRoot>();
                sr.CurrentState = element.stateData[index].intValue;

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(sr);
#endif
            }
        }

#if UNITY_EDITOR
        public override void ShowElementTarget(Element element)
        {
            Object old = element.target;
            StateRoot newv = EditorGUILayout.ObjectField(element.Name, element.target, typeof(StateRoot), true) as StateRoot;
            if (newv != null && newv == GetStateRoot())
            {
                Debug.LogError("不能自身改成自身!");
                return;
            }

            if (element.target != newv)
            {
                RegisterUndo(() => 
                {
                    element.target = newv;
                    if (old == null && element.target != null)
                    {
                        for (int i = 0; i < element.stateData.Length; ++i)
                            element.Agent.Init(element, element.stateData[i]);
                    }
                    else if (element.target != null && old != element.target)
                    {
                        for (int i = 0; i < element.stateData.Length; ++i)
                            element.Agent.Set(element, i);
                    }
                });
            }
        }

        public override bool ShowState(Element element, ElementStateData sc, int index)
        {
            int old = sc.intValue;
            StateRoot sr = element.GetTarget<StateRoot>();
            if (sr == null)
            {
                GUI.changed = false;
                int v = EditorGUILayout.IntField("状态", sc.intValue);
                if (GUI.changed)
                {
                    RegisterUndo(() => 
                    {
                        sc.intValue = v;
                    });

                    return true;
                }
                return false;
            }
            else
            {
                List<string> olds = new List<string>();
                List<int> values = new List<int>();
                for (int i = 0; i < sr.States.Length; ++i)
                {
                    olds.Add(sr.States[i].Name);
                    values.Add(i);
                }

                GUI.changed = false;
                int v = EditorGUILayout.IntPopup("状态", sc.intValue, olds.ToArray(), values.ToArray());
                if (GUI.changed)
                {
                    RegisterUndo(() => 
                    {
                        sc.intValue = v;
                    });
                }
                return sc.intValue != old;
            }
        }
#endif
    }
}