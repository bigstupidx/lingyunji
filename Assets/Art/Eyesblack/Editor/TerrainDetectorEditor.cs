using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Eyesblack.FX {
    [CustomEditor(typeof(TerrainDetector))]
    public class TerrainDetectorEditor : Editor {
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            if (GUILayout.Button("自动添加子对象组件")) {
                foreach (CloseToTerrain ct in (target as TerrainDetector).GetComponentsInChildren<CloseToTerrain>(true)) {
                    DestroyImmediate(ct);
                }

                foreach (Renderer renderer in (target as TerrainDetector).GetComponentsInChildren<Renderer>()) {
                    if (CheckValidRenderer(renderer)) {
                        CloseToTerrain ct = renderer.gameObject.GetComponent<CloseToTerrain>();
                        if (ct == null)
                            ct = renderer.gameObject.AddComponent<CloseToTerrain>();
                    }
                }
            }
        }


        bool CheckValidRenderer(Renderer renderer) {
            ParticleSystem particleSystem = renderer.GetComponent<ParticleSystem>();
            if (particleSystem != null) {
                ParticleSystemRenderer particleRenderer = renderer.GetComponent<ParticleSystemRenderer>();
                if (particleRenderer != null) {
                    if (particleRenderer.renderMode == ParticleSystemRenderMode.Mesh) {
                        if (!particleRenderer.mesh.isReadable)
                            Debug.LogWarning(target.name + ": 无法贴地表，因为Mesh数据不可读写！");
                        else
                            return true;
                    }
                }
            } else {
                MeshRenderer meshRenderer = renderer.GetComponent<MeshRenderer>();
                if (meshRenderer != null && meshRenderer.enabled) {
                    MeshFilter meshFilter = renderer.GetComponent<MeshFilter>();
                    if (meshFilter != null) {
                        if (meshFilter.sharedMesh.isReadable) {
                            return true;
                        } else {
                            Debug.LogWarning(target.name + ": 无法贴地表，因为Mesh数据不可读写！");
                        }
                    }
                }
            }

            return false;
        }
    }
}