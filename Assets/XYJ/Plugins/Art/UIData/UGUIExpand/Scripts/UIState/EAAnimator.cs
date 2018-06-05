using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace xys.UI.State
{
    public partial class PlayAnim : TTTEA<Animator>
    {
        public override void Init(Element element, ElementStateData sd)
        {
            sd.strValue = "";
        }

        public override void Set(Element element, int index)
        {
            if (element.target != null)
            {
                Animator anim = element.GetTarget<Animator>();
                anim.Play(element.stateData[index].strValue);
            }
        }

#if UNITY_EDITOR
        public override void ShowElementTarget(Element element)
        {
            //Object old = element.target;
            Animator newv = EditorGUILayout.ObjectField(element.Name, element.target, typeof(Animator), true) as Animator;
            if (element.target != newv)
            {
                RegisterUndo(() =>
                {
                    element.target = newv;
                });
            }
        }

        public override bool ShowState(Element element, ElementStateData sc, int index)
        {
            int old = sc.intValue;
            //Animator sr = element.GetTarget<Animator>();
            string v = EditorGUILayout.TextField("AnimName", sc.strValue);

            if (GUI.changed)
            {
                RegisterUndo(() =>
                {
                    sc.strValue = v;
                });
            }

            return sc.intValue != old;
        }
#endif
    }
}