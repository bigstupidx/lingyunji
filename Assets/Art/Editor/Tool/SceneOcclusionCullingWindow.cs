using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using GUIEditor;
using Object = UnityEngine.Object;
using GUI = UnityEngine.GUI;
using System.Reflection;

namespace SM
{
    class SceneOcclusionCullingWindow : SceneMergeBaseEditor
    {
        [MenuItem("Tools/SceneOcclusionCullingWindow")]
        static void Open()
        {
            GetWindow<SceneOcclusionCullingWindow>().Show();
        }

        protected override void OnCommonBtn(RenderData rd)
        {

        }

        protected override void OnCommonBtn(RenderDataList rdl, params GUILayoutOption[] objs)
        {

        }

        protected override void OnCombine(RenderData rd) { }
        protected override void OnCombine(RenderDataList rdl) { }

        protected override void OnCombineRemove(RenderData rd) { }
        protected override void OnCombineRemove(RenderDataList rdl) { }

        List<string> ollcusion_items = new List<string>();

        protected List<string> Ollcusion_items
        {
            get
            {
                if (ollcusion_items.Count == 0)
                {
                    ollcusion_items.Add("无遮挡");
                    ollcusion_items.Add("双遮");
                    ollcusion_items.Add("小物件");
                    ollcusion_items.Add("大物件");
                }

                return ollcusion_items;
            }
        }

        static void SetStaticEditorFlags(GameObject go, string text)
        {
            switch (text)
            {
            case "无遮挡":
                GF.SetStaticEditorFlags(go, StaticEditorFlags.OccludeeStatic, false);
                GF.SetStaticEditorFlags(go, StaticEditorFlags.OccluderStatic, false);
                break;
            case "双遮":
                GF.SetStaticEditorFlags(go, StaticEditorFlags.OccludeeStatic, true);
                GF.SetStaticEditorFlags(go, StaticEditorFlags.OccluderStatic, true);
                break;
            case "小物件":
                GF.SetStaticEditorFlags(go, StaticEditorFlags.OccludeeStatic, true);
                GF.SetStaticEditorFlags(go, StaticEditorFlags.OccluderStatic, false);
                break;
            case "大物件":
                GF.SetStaticEditorFlags(go, StaticEditorFlags.OccludeeStatic, false);
                GF.SetStaticEditorFlags(go, StaticEditorFlags.OccluderStatic, true);
                break;
            }
        }

        protected override void OnMeshGUI(Mesh mesh, RenderDataList rdl)
        {
            StaticEditorFlags flags = 0;
            foreach (var item in rdl.dataList)
            {
                if (GF.GetStaticEditorFlags(item.renderer.gameObject, StaticEditorFlags.OccludeeStatic))
                    flags |= StaticEditorFlags.OccludeeStatic;

                if (GF.GetStaticEditorFlags(item.renderer.gameObject, StaticEditorFlags.OccluderStatic))
                    flags |= StaticEditorFlags.OccluderStatic;
            }

            string text = "无遮挡";
            if (flags == (StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic))
                text = "双遮";
            else if (flags == StaticEditorFlags.OccludeeStatic)
                text = "小物件";
            else if (flags == StaticEditorFlags.OccluderStatic)
                text = "大物件";

            var text1 = GuiTools.StringPopup(false, text, Ollcusion_items, GUILayout.Width(120));
            if (text1 != text)
            {
                foreach (var item in rdl.dataList)
                    SetStaticEditorFlags(item.renderer.gameObject, text1);
            }
        }

        // 此对象当前是否处于合并列表当中
        protected override bool IsCombine(RenderData rd) { return false; }

        RenderDataList OnFilterMinMax(RenderDataList rdl, ParamList pl)
        {
            float minV = pl.Get<float>("min", 0f);
            float maxV = pl.Get<float>("max", 0f);

            minV = EditorGUILayout.FloatField("最小:", minV, GUILayout.ExpandWidth(false));
            maxV = EditorGUILayout.FloatField("最大:", maxV, GUILayout.ExpandWidth(false));
            pl.Set("min", minV);
            pl.Set("max", maxV);
            if (minV == 0 && maxV == 0)
                return rdl;

            Predicate<RenderData> fun = null;
            if (minV != 0f && maxV != 0f)
            {
                fun = (RenderData x) => { return x.colliderArea >= minV && x.colliderArea <= maxV; };
            }
            else if (minV != 0)
            {
                fun = (RenderData x) => { return x.colliderArea <= minV; };
            }
            else if (maxV != 0)
            {
                fun = (RenderData x) => { return x.colliderArea >= maxV; };
            }

            return new RenderDataList(rdl.FindAll(fun));
        }

        protected override RenderDataList OnFilterRenderDataList(RenderDataList rdl, ParamList pl)
        {
            rdl = OnFilterMinMax(rdl, pl);
            return rdl;
        }

        protected override void OnFilterRenderDataListEnd(RenderDataList rdl)
        {
            GuiTools.HorizontalField(false, () =>
            {
                if (GUILayout.Button("设置为小物件，不会遮挡别人", GUILayout.Width(200f), GUILayout.Height(50f)))
                {
                    foreach (var data in rdl.dataList)
                    {
                        GF.SetStaticEditorFlags(data.renderer.gameObject, StaticEditorFlags.OccludeeStatic, true);
                        GF.SetStaticEditorFlags(data.renderer.gameObject, StaticEditorFlags.OccluderStatic, false);
                    }
                }

                if (GUILayout.Button("设置为大物件，遮挡别人，不会被别人遮挡", GUILayout.Width(200f), GUILayout.Height(50f)))
                {
                    foreach (var data in rdl.dataList)
                    {
                        GF.SetStaticEditorFlags(data.renderer.gameObject, StaticEditorFlags.OccludeeStatic, false);
                        GF.SetStaticEditorFlags(data.renderer.gameObject, StaticEditorFlags.OccluderStatic, true);
                    }
                }

                if (GUILayout.Button("即会被遮挡也会摭挡别人", GUILayout.Width(200f), GUILayout.Height(50f)))
                {
                    foreach (var data in rdl.dataList)
                    {
                        GF.SetStaticEditorFlags(data.renderer.gameObject, StaticEditorFlags.OccludeeStatic, false);
                        GF.SetStaticEditorFlags(data.renderer.gameObject, StaticEditorFlags.OccluderStatic, true);
                    }
                }
            });
        }

        bool findDisable = false;
        protected override void OnGUI()
        {
            OnShowInitBtn(ref findDisable);
            base.OnGUI();
        }

        //enum SortType
        //{
        //    Null,
        //    Size,
        //}

        //SortType sortType = SortType.Null;

        //public void OnGUI()
        //{
        //    if (GUILayout.Button("初始化", GUILayout.Width(100f), GUILayout.Height(50f)))
        //    {
        //        OnInit();
        //    }

        //    System.Func<Renderer, Object> fun = (Renderer r) => { return r; };
        //    GuiTools.ObjectFieldList<Renderer>(
        //        paramList,
        //        Renderers,
        //        fun,
        //        false,
        //        (List<Renderer> rs, ParamList pl) =>
        //        {

        //        },
        //        null,
        //        (Renderer r) =>
        //        {
        //            var size = Size(r);
        //            GUILayout.Label(string.Format("大小:{0}({1})", size, Area(size)), GUILayout.Width(300f));
        //        });
        //}
    }
}