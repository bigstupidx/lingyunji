#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.AnimatedValues;

namespace xys.UI.State
{
    public class StateElementDraw
    {
        public static int DrawType()
        {
            //增加按钮
            return EditorGUILayout.Popup("增加", -1, Factory.Names);
        }

        static public void DrawElements(ref Element[] elements)
        {
            for (int i = 0; i < elements.Length; )
            {
                if (State.StateElementDraw.DrawElement(elements[i]))
                {
                    XTools.Utility.ArrayRemove<Element>(ref elements, i);
                }
                else
                {
                    ++i;
                }
            }
        }

        public static System.Func<Element, int, bool, bool, bool> CurrentDrawStateInfo { get; set; }

        static public void DrawStateInfos(ref Element[] elements, int stateid, bool isShowRecord, bool iscanset)
        {
            for (int i = 0; i < elements.Length;)
            {
                if (CurrentDrawStateInfo(elements[i], stateid, isShowRecord, iscanset))
                {
                    XTools.Utility.ArrayRemove(ref elements, i);
                }
                else
                {
                    ++i;
                }
            }
        }        

        static public bool DrawElement(Element element)
        {
            bool isdel = false;
            EditorGUILayout.BeginHorizontal();

            element.Agent.ShowElementTarget(element);
            if (GUILayout.Button("删除", GUILayout.Width(50)))
            {
                isdel = true;
            }

            EditorGUILayout.EndHorizontal();
            return isdel;
        }
    }
}
#endif