using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Eyesblack.Nature;

namespace Eyesblack.Optimization {
    public static class GrassOptimizer {
        /*
        public static void StaticBatchingCombine(GameObject rootGo) {
            MeshRenderer[] renderers = rootGo.GetComponentsInChildren<MeshRenderer>();
            if (renderers.Length > 0) {
                GameObject[] gos = new GameObject[renderers.Length];
                for (int i = 0; i < renderers.Length; i++) {
                    gos[i] = renderers[i].gameObject;
                }

                GameObject batchingGo = new GameObject();
                batchingGo.hideFlags = HideFlags.HideAndDontSave;
                StaticBatchingUtility.Combine(gos, batchingGo);
            }
        }
        */




        public class CombineInfo {
            public Mesh _mesh;
            public Matrix4x4 _matrix;
            public Vector4 _lightmapTilingOffset;
        }

        public struct CombinePart {
            public int _startIndex;
            public int _length;
            public int _maxVertices;
        }

        public static void Optimize(GameObject root) {

            foreach (CombinedGrassMesh combinedMesh in root.GetComponentsInChildren<CombinedGrassMesh>(true)) {
                GameObject.DestroyImmediate(combinedMesh.GetComponent<MeshFilter>().sharedMesh);
               //GameObject.DestroyImmediate(combinedMesh.GetComponent<Renderer>().sharedMaterial);
                GameObject.DestroyImmediate(combinedMesh.gameObject);
            }

            CombineGrassMeshes(root);
        }

        static void CombineGrassMeshes(GameObject root) {
            /*
            if (LightmapSettings.lightmaps.Length == 0) {
                Debug.LogWarning("Grass Combing need the baked lightmaps!");
                return;
            }
            */



            Dictionary<Material, Dictionary<int, List<MeshFilter>>> combinedMesheDatas = new Dictionary<Material, Dictionary<int, List<MeshFilter>>>();
            {
                GrassObject[] details = root.GetComponentsInChildren<GrassObject>();
                for (int i = 0; i < details.Length; i++) {
                    MeshFilter meshFilter = details[i].GetComponent<MeshFilter>();
                    Renderer renderer = details[i].GetComponent<MeshRenderer>();
                    Material material = renderer.sharedMaterial;
                    int lightmapIndex = renderer.lightmapIndex;

                    if (material == null)
                        continue;

                    /*
                    if (lightmapIndex < 0) {
                        Debug.LogError("Invalid lightmap index: " + meshFilter.name);
                        return;
                    }
                    */

                    if (meshFilter.sharedMesh.vertexCount != 4 && meshFilter.sharedMesh.vertexCount != 8) {
                        Debug.LogError("Invalid grass mesh: " + meshFilter.name);
                        continue;
                    }

                    Dictionary<int, List<MeshFilter>> lightmap2MeshFilters;
                    if (!combinedMesheDatas.TryGetValue(material, out lightmap2MeshFilters)) {
                        List<MeshFilter> filters = new List<MeshFilter>();
                        filters.Add(meshFilter);
                        lightmap2MeshFilters = new Dictionary<int, List<MeshFilter>>();
                        lightmap2MeshFilters.Add(lightmapIndex, filters);

                        combinedMesheDatas.Add(material, lightmap2MeshFilters);
                    } else {
                        List<MeshFilter> filters;
                        if (!lightmap2MeshFilters.TryGetValue(lightmapIndex, out filters)) {
                            filters = new List<MeshFilter>();
                            filters.Add(meshFilter);
                            lightmap2MeshFilters.Add(lightmapIndex, filters);
                        } else {
                            filters.Add(meshFilter);
                        }
                    }
                }
            }



            foreach (KeyValuePair<Material, Dictionary<int, List<MeshFilter>>> combinedInfo in combinedMesheDatas) {
                Material materal = combinedInfo.Key;
                foreach (KeyValuePair<int, List<MeshFilter>> kvp in combinedInfo.Value) {
                    int lightmapIndex = kvp.Key;
                    List<MeshFilter> meshFilters = kvp.Value;
                    CombineInfo[] combine = new CombineInfo[meshFilters.Count];
                    List<CombinePart> combineParts = new List<CombinePart>();

                    int maxVertices = 0;
                    int startIndex = 0;
                    for (int i = 0; i < meshFilters.Count; i++) {
                        combine[i] = new CombineInfo();
                        Mesh mesh = meshFilters[i].sharedMesh;
                        combine[i]._mesh = mesh;
                        combine[i]._matrix = meshFilters[i].transform.localToWorldMatrix;
                        combine[i]._lightmapTilingOffset = meshFilters[i].GetComponent<Renderer>().lightmapScaleOffset;
                        maxVertices += mesh.vertexCount;
                        if (maxVertices > 20000) {  // 21.9k vertices limit
                            //Debug.Log(maxVertices);
                            CombinePart part = new CombinePart();
                            part._startIndex = startIndex;
                            part._length = i - startIndex;
                            part._maxVertices = maxVertices - mesh.vertexCount;
                            combineParts.Add(part);

                            startIndex = i;
                            maxVertices = mesh.vertexCount;
                        }
                    }
                    CombinePart lastPart = new CombinePart();
                    lastPart._startIndex = startIndex;
                    lastPart._length = meshFilters.Count - startIndex;
                    lastPart._maxVertices = maxVertices;
                    combineParts.Add(lastPart);


                    for (int i = 0; i < combineParts.Count; i++) {
                        CombinePart combinePart = combineParts[i];

                        GameObject go = new GameObject("__CombinedMeshes__");
                        go.hideFlags = HideFlags.NotEditable;
                        go.AddComponent<CombinedGrassMesh>();
                        // go.layer = LayerMask.NameToLayer("Vegetation");
                        go.transform.SetParent(root.transform);

                        MeshFilter meshfilter = go.AddComponent<MeshFilter>();
                        Mesh newMesh = GetCombinedMeshes(combine, combinePart._startIndex, combinePart._length, combinePart._maxVertices);
                        newMesh.name = "CombinedMesh";
                        meshfilter.mesh = newMesh;
                        MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
                        meshRenderer.lightmapIndex = lightmapIndex;

                        /*
                        Shader replacementShader = GetReplacementShader(materal.shader.name);
                        Material newMaterial = new Material(replacementShader);
                        newMaterial.name = materal.name;
                        newMaterial.mainTexture = materal.mainTexture;
                        newMaterial.SetTexture("_LightMap", LightmapSettings.lightmaps[lightmapIndex].lightmapFar);
                        newMaterial.SetFloat("_Cutoff", materal.GetFloat("_Cutoff"));
                        if (materal.shader.name.Contains("Self-Illumin")) {
                            newMaterial.SetFloat("_IllumStrength", materal.GetFloat("_IllumStrength"));
                            newMaterial.SetTexture("_SpecIllumReflTex", materal.GetTexture("_SpecIllumReflTex"));
                        }
                        meshRenderer.material = newMaterial;
                        */
                        meshRenderer.sharedMaterial = materal;
                        go.SetActive(false);
                    }
                }
            }
        }


        private static Mesh GetCombinedMeshes(CombineInfo[] combine, int startIndex, int length, int maxVertices) {
            Mesh newMesh = new Mesh();

            Vector3[] vertices = new Vector3[maxVertices];
            Vector3[] normals = new Vector3[maxVertices];
            Vector2[] uv = new Vector2[maxVertices];
            Vector2[] uv2 = new Vector2[maxVertices];
            int[] triangles = new int[maxVertices / 2 * 3];

            int handledTriangles = 0;
            int handledVertices = 0;
            for (int i = startIndex; i < startIndex + length; i++) {
                for (int j = 0; j < combine[i]._mesh.triangles.Length; j++) {
                    triangles[handledTriangles] = handledVertices + combine[i]._mesh.triangles[j];
                    handledTriangles++;
                }


                for (int j = 0; j < combine[i]._mesh.vertices.Length; j++) {
                    vertices[handledVertices] = combine[i]._matrix.MultiplyPoint(combine[i]._mesh.vertices[j]);
                    normals[handledVertices] = combine[i]._mesh.normals[j];
                    uv[handledVertices] = combine[i]._mesh.uv[j];
                    Vector2 lightmapTiling = new Vector2(combine[i]._lightmapTilingOffset.x, combine[i]._lightmapTilingOffset.y);
                    Vector2 lightmapOffset = new Vector2(combine[i]._lightmapTilingOffset.z, combine[i]._lightmapTilingOffset.w);
                    uv2[handledVertices].x = uv[handledVertices].x * lightmapTiling.x + lightmapOffset.x;
                    uv2[handledVertices].y = uv[handledVertices].x * lightmapTiling.y + lightmapOffset.y;

                    handledVertices++;
                }
            }


            newMesh.vertices = vertices;
            newMesh.normals = normals;
            newMesh.uv = uv;
            newMesh.uv2 = uv2;
            newMesh.triangles = triangles;

            return newMesh;
        }


    }
}
