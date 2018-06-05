using System.IO;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using UnityEngine.Rendering;
using System.Collections.Generic;

namespace PackTool
{
    public class GraphicsSettingsRef
    {
        Object target;
        private SerializedObject serializedObject;
        private SerializedProperty m_FogKeepExp;
        private SerializedProperty m_FogKeepExp2;
        private SerializedProperty m_FogKeepLinear;
        private SerializedProperty m_FogStripping;
        private SerializedProperty m_LightmapKeepDirCombined;
        private SerializedProperty m_LightmapKeepDirSeparate;
        private SerializedProperty m_LightmapKeepDynamicDirCombined;
        private SerializedProperty m_LightmapKeepDynamicDirSeparate;
        private SerializedProperty m_LightmapKeepDynamicPlain;
        private SerializedProperty m_LightmapKeepPlain;
        private SerializedProperty m_LightmapStripping;

        public GraphicsSettingsRef()
        {
            target = (Object)(typeof(GraphicsSettings).GetMethod("GetGraphicsSettings", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, new object[] { }));

            serializedObject = new SerializedObject(target);
            m_LightmapStripping = serializedObject.FindProperty("m_LightmapStripping");
            m_LightmapKeepPlain = serializedObject.FindProperty("m_LightmapKeepPlain");
            m_LightmapKeepDirCombined = serializedObject.FindProperty("m_LightmapKeepDirCombined");
            m_LightmapKeepDirSeparate = serializedObject.FindProperty("m_LightmapKeepDirSeparate");
            m_LightmapKeepDynamicPlain = serializedObject.FindProperty("m_LightmapKeepDynamicPlain");
            m_LightmapKeepDynamicDirCombined = serializedObject.FindProperty("m_LightmapKeepDynamicDirCombined");
            m_LightmapKeepDynamicDirSeparate = serializedObject.FindProperty("m_LightmapKeepDynamicDirSeparate");
            m_FogStripping = serializedObject.FindProperty("m_FogStripping");
            m_FogKeepLinear = serializedObject.FindProperty("m_FogKeepLinear");
            m_FogKeepExp = serializedObject.FindProperty("m_FogKeepExp");
            m_FogKeepExp2 = serializedObject.FindProperty("m_FogKeepExp2");
        }

        public static void Default()
        {
            GraphicsSettingsRef gs = new GraphicsSettingsRef();
            gs.SetDefault();
        }

        void SetDefault()
        {
            bool isDirty = false;

            XTools.Pair<SerializedProperty, object>[] sps = new XTools.Pair<SerializedProperty, object>[]
            {
                // Lightmap modes
                new XTools.Pair<SerializedProperty, object>(m_LightmapStripping, 1),

                // Baked Non-Directional|Include support for baked non-directional lightmaps.
                new XTools.Pair<SerializedProperty, object>(m_LightmapKeepPlain, true),

                // Baked Directional|Include support for baked directional lightmaps.
                new XTools.Pair<SerializedProperty, object>(m_LightmapKeepDirCombined, true),

                // Baked Directional|Include support for baked directional lightmaps.
                new XTools.Pair<SerializedProperty, object>(m_LightmapKeepDirSeparate, true),

                //Realtime Non-Directional|Include support for realtime non-directional lightmaps.
                new XTools.Pair<SerializedProperty, object>(m_LightmapKeepDynamicPlain, false),

                //Realtime Directional|Include support for realtime directional lightmaps.
                new XTools.Pair<SerializedProperty, object>(m_LightmapKeepDynamicDirCombined, false),

                //Realtime Directional Specular|Include support for realtime directional specular lightmaps.
                new XTools.Pair<SerializedProperty, object>(m_LightmapKeepDynamicDirSeparate, false),

                // Fog modes
                new XTools.Pair<SerializedProperty, object>(m_FogStripping, 1),
                // Linear|Include support for Linear fog.
                new XTools.Pair<SerializedProperty, object>(m_FogKeepLinear, true),
                // Exponential|Include support for Exponential fog.
                new XTools.Pair<SerializedProperty, object>(m_FogKeepExp, false),
                // Exponential Squared|Include support for Exponential Squared fog.
                new XTools.Pair<SerializedProperty, object>(m_FogKeepExp2, false),
            };

            foreach (var itor in sps)
            {
                if (itor.first == null)
                    continue;

                if (itor.second is int)
                {
                    if (itor.first.intValue != (int)itor.second)
                    {
                        itor.first.intValue = (int)itor.second;
                        isDirty = true;
                    }
                }
                if (itor.second is bool)
                {
                    if (itor.first.boolValue != (bool)itor.second)
                    {
                        itor.first.boolValue = (bool)itor.second;
                        isDirty = true;
                    }
                }
            }

            if (isDirty)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}