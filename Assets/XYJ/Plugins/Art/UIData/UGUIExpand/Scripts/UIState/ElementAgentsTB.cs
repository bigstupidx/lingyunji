using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace xys.UI.State
{
    public abstract class TTTEA<T> : ElementAgent where T : Object
    {
#if UNITY_EDITOR
        public override void ShowElementTarget(Element element)
        {
            Object target = EditorGUILayout.ObjectField(element.Name, element.target, typeof(T), true);
            if (element.target == null && target != null)
            {
                RegisterUndo(() => 
                {
                    element.target = target;
                    for (int i = 0; i < element.stateData.Length; ++i)
                        element.Agent.Init(element, element.stateData[i]);
                });
            }
            else if (element.target != null && target != element.target)
            {
                RegisterUndo(() =>
                {
                    for (int i = 0; i < element.stateData.Length; ++i)
                        element.Agent.Set(element, i);
                });
            }
        }

        public override void ShowElementState(Element element, int stateid, bool iscanset)
        {
            if (!iscanset)
                EditorGUILayout.ObjectField(element.Name, element.target, typeof(T), true);
            else
            {
                ShowElementTarget(element);
            }
        }
#endif
    }

    public abstract class TTTEASmooth<T> : TTTEA<T> where T : Object
    {
        //  «∑Ò‘ –ÌΩ•±‰
        public override bool isSmooth { get { return true; } }

        public abstract void InitBySmooth(T target, SmoothData sd);

        public abstract void SetBySmooth(T target, SmoothData sd, ElementStateData esd, float progress);

        public override void Set(Element element, int index)
        {
            if (element.target != null)
            {
                T target = element.GetTarget<T>();
                ElementStateData esd = element.stateData[index];
                if (esd.isSmooth)
                {
                    SmoothData sd = new SmoothData();
                    InitBySmooth(target, sd);
                    esd.AddFrameUpdate((object p) =>
                    {
                        if (target == null)
                            return false;

#if UNITY_EDITOR
                        if (!Application.isPlaying)
                        {
//                             Debug.Log("deltaTime:" + XTools.GlobalCoroutine.deltaTime + " frameCount:" + XTools.GlobalCoroutine.frameCount);
//                             sd.esc_timer += XTools.GlobalCoroutine.deltaTime;
                            sd.esc_timer += 0.011f;
                        }
                        else
#endif
                        {
                            float deltaTime = Time.deltaTime;
                            //Debug.Log("deltaTime:" + deltaTime);
                            sd.esc_timer += deltaTime;
                        }

                        bool isend = false;
                        float progress = 1f;
                        if (sd.esc_timer >= esd.smoothTime)
                        {
                            isend = true;
                        }
                        else
                        {
                            progress = sd.esc_timer / esd.smoothTime;
                        }

                        SetBySmooth(target, sd, esd, progress);

#if UNITY_EDITOR
                        UnityEditor.EditorUtility.SetDirty(target);
#endif
                        return !isend;
                    }, null);
                }
                else
                {
                    SetBySmooth(target, null, esd, -1f);
                }

#if UNITY_EDITOR
                UnityEditor.EditorUtility.SetDirty(target);
#endif
            }
        }
    }
}