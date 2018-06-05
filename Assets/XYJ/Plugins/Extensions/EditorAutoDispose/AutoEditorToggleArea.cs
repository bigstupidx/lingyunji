#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace EditorExtensions
{

    public class GUIToggleTween
    {
        public bool open = true;
        public float value = 1;

        [System.NonSerialized]
        public Rect lastRect;

        [System.NonSerialized]
        public float lastUpdate;

        /** Update the visibility in Layout to avoid complications with different events not drawing the same thing */
        bool isNeedShow = true;
        public bool IsNeedShow
        {
            get
            {
                if (Event.current.type == EventType.Layout)
                {
                    isNeedShow = open || value > 0F;
                }

                return isNeedShow;

            }
        }

        public static implicit operator bool(GUIToggleTween o)
        {
            return o.IsNeedShow;
        }
    }

    public class AutoEditorToggleArea : IDisposable
    {

        const float TweenDuration = 0.2f;

        public AutoEditorToggleArea(GUIToggleTween toggle, string text) : this(toggle, text, EditorStylesEx.BoxArea) { }

        public AutoEditorToggleArea(GUIToggleTween toggle, string text, GUIStyle areaStyle)
        {
            //计算要显示的大小
            Rect lastRect = toggle.lastRect;
            lastRect.height = lastRect.height < 20 ? 20 : lastRect.height;
            lastRect.height -= 20;
            float faded = Hermite(0F, 1F, toggle.value);
            lastRect.height *= faded;
            lastRect.height += 20;
            lastRect.height = Mathf.Round(lastRect.height);

            //显示区域布局、开始Area、BeginVertical计算真实大小
            Rect gotLastRect = GUILayoutUtility.GetRect(new GUIContent(), areaStyle, GUILayout.Height(lastRect.height));
            GUILayout.BeginArea(lastRect, areaStyle);
            Rect newRect = EditorGUILayout.BeginVertical();

            if (Event.current.type == EventType.Repaint || Event.current.type == EventType.ScrollWheel)
            {
                //计算真实大小
                newRect.x = gotLastRect.x;
                newRect.y = gotLastRect.y;
                newRect.width = gotLastRect.width;
                newRect.height += areaStyle.padding.top + areaStyle.padding.bottom;
                toggle.lastRect = newRect;

                //计算插值
                if (Event.current.type == EventType.Repaint)
                {
                    float deltaTime = Time.realtimeSinceStartup - toggle.lastUpdate;
                    toggle.lastUpdate = Time.realtimeSinceStartup;
                    toggle.value = Mathf.Clamp01(toggle.open ? toggle.value + deltaTime / TweenDuration : toggle.value - deltaTime / TweenDuration);
                }
            }


            if (GUILayout.Button(text, EditorStylesEx.LabelAreaHeader))
                toggle.open = !toggle.open;
        }

        public void Dispose()
        {
            EditorGUILayout.EndVertical();
            GUILayout.EndArea();
        }

        //两端平缓中间陡峭
        static float Hermite(float start, float end, float value)
        {
            return Mathf.Lerp(start, end, value * value * (3.0f - 2.0f * value));
        }

    }
}
#endif