#define FBXEXPORT_DEBUG


using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Eyesblack.Utility;

namespace Eyesblack.EditorTools {

    public class MeshExportWnd : EditorWindow {
        public enum ExportFormat {
            UnityAsset = 0,
            FBX
        }

        static ExportFormat _exportFormat;
        static bool _includeAllChildren = true;
        static bool _includeInactive = false;
#if FBXEXPORT_DEBUG
        static bool _autoAssignMesh = false;
#else
        static bool _autoAssignMesh = true;
#endif


        UnityMeshAssetExporter _unityAssetExporter;
        FBXExporter _fbxExporter;


        [MenuItem("Eyesblack/导出Mesh数据")]
        private static void Init() {
            EditorWindow.GetWindow(typeof(MeshExportWnd));
        }

        void OnEnable() {
            titleContent = new GUIContent("Export Mesh");

            _unityAssetExporter = new UnityMeshAssetExporter();
            _fbxExporter = new FBXExporter();
        }


        void OnGUI() {
            EditorGUILayout.Space();
            _exportFormat = (ExportFormat)EditorGUILayout.EnumPopup("Format", _exportFormat);
            _includeAllChildren = EditorGUILayout.Toggle("Include Children", _includeAllChildren);
            _includeInactive = EditorGUILayout.Toggle("Include Inactive", _includeInactive);
            if (_exportFormat == ExportFormat.UnityAsset)
                _autoAssignMesh = EditorGUILayout.Toggle("Auto Assign", _autoAssignMesh);

            if (GUILayout.Button("Export...")) {
                GameObject go = Selection.activeGameObject;
                if (go) {
                    if (_exportFormat == ExportFormat.FBX) {
                        _fbxExporter._includeAllChildren = _includeAllChildren;
                        _fbxExporter._includeInactive = _includeInactive;
                        _fbxExporter.ExportMeshToFile(go);
                    } else if (_exportFormat == ExportFormat.UnityAsset) {
                        _unityAssetExporter._includeAllChildren = _includeAllChildren;
                        _unityAssetExporter._includeInactive = _includeInactive;
                        _unityAssetExporter._autoAssignMesh = _autoAssignMesh;
                        _unityAssetExporter.ExportMeshToFile(go);
                    }
                } else
                    EditorUtility.DisplayDialog("提示", "必须选中一个对象才能导出！", "OK");
            }
        }
    }




    public class UnityMeshAssetExporter {
        public bool _includeAllChildren = true;
        public bool _includeInactive = false;
        public bool _autoAssignMesh = false;

        public void ExportMeshToFile(GameObject go) {

            string path = EditorUtility.SaveFilePanelInProject("Save Mesh", go.name, "asset", "Please enter a file name to save the mesh(es) to");
            if (!string.IsNullOrEmpty(path)) {

                Dictionary<int, MeshFilter> index2MeshFilter = new Dictionary<int, MeshFilter>();
                Dictionary<int, SkinnedMeshRenderer> index2SkinnedRenderer = new Dictionary<int, SkinnedMeshRenderer>();

                List<Mesh> exportMeshes = new List<Mesh>();

                {
                    MeshFilter[] meshFilters;
                    if (_includeAllChildren)
                        meshFilters = go.GetComponentsInChildren<MeshFilter>(_includeInactive);
                    else {
                        meshFilters = new MeshFilter[1];
                        meshFilters[0] = go.GetComponent<MeshFilter>();
                    }

                    foreach (MeshFilter meshFilter in meshFilters) {
                        if (meshFilter != null && meshFilter.sharedMesh != null) {
                            Mesh mesh = CopyMesh(meshFilter.sharedMesh.name, meshFilter.sharedMesh, false);
                            if (mesh != null) {
                                if (_autoAssignMesh)
                                    index2MeshFilter.Add(exportMeshes.Count, meshFilter);

                                exportMeshes.Add(mesh);
                            }
                        }
                    }
                }


                {
                    SkinnedMeshRenderer[] skinnedRenderers;
                    if (_includeAllChildren)
                        skinnedRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>(_includeInactive);
                    else {
                        skinnedRenderers = new SkinnedMeshRenderer[1];
                        skinnedRenderers[0] = go.GetComponent<SkinnedMeshRenderer>();
                    }

                    foreach (SkinnedMeshRenderer skinnedRenderer in skinnedRenderers) {
                        if (skinnedRenderer != null && skinnedRenderer.sharedMesh != null) {
                            Mesh mesh = CopyMesh(skinnedRenderer.sharedMesh.name, skinnedRenderer.sharedMesh, true);
                            if (mesh != null) {
                                if (_autoAssignMesh)
                                    index2SkinnedRenderer.Add(exportMeshes.Count, skinnedRenderer);

                                exportMeshes.Add(mesh);
                            }
                        }

                    }
                }


                if (exportMeshes.Count > 0) {
                    SaveToFile(path, exportMeshes);
                    AssetDatabase.ImportAsset(path);

                    MeshExportData meshExportData = AssetDatabase.LoadAssetAtPath(path, typeof(MeshExportData)) as MeshExportData;
                    if (_autoAssignMesh) {
                        for (int i = 0; i < meshExportData._meshes.Length; i++) {
                            MeshFilter meshFilterOrgin;
                            if (index2MeshFilter.TryGetValue(i, out meshFilterOrgin)) {
                                meshFilterOrgin.sharedMesh = meshExportData._meshes[i];
                            } else {
                                SkinnedMeshRenderer skinnedRendererOrgin;
                                if (index2SkinnedRenderer.TryGetValue(i, out skinnedRendererOrgin)) {
                                    skinnedRendererOrgin.sharedMesh = meshExportData._meshes[i];
                                }
                            }
                        }

                        UtilityFuncs.MarkCurrentSceneIsDirty();
                    }

                    Debug.Log("成功导出Mesh数据到" + path, meshExportData);

                } else {
                    EditorUtility.DisplayDialog("提示", "没有导出任何Mesh数据！", "OK");
                }
            }

        }




        Mesh CopyMesh(string meshName, Mesh mesh, bool isSkin) {
#if !FBXEXPORT_DEBUG
            if (IsImportedMeshResource(mesh))
                return null;
#endif

            Mesh newMesh = new Mesh();
            newMesh.name = meshName;

            newMesh.vertices = mesh.vertices;
            newMesh.normals = mesh.normals;
            newMesh.tangents = mesh.tangents;
            newMesh.colors = mesh.colors;
            newMesh.uv = mesh.uv;
            newMesh.uv2 = mesh.uv2;
            newMesh.triangles = mesh.triangles;

            newMesh.subMeshCount = mesh.subMeshCount;
            if (mesh.subMeshCount > 1) {
                for (int i = 0; i < mesh.subMeshCount; i++) {
                    newMesh.SetTriangles(mesh.GetTriangles(i), i);
                }
            }


            if (isSkin) {
                newMesh.bindposes = mesh.bindposes;
                newMesh.boneWeights = mesh.boneWeights;
            }

            return newMesh;
        }

        void SaveToFile(string path, List<Mesh> exportMeshes) {
            MeshExportData data = ScriptableObject.CreateInstance<MeshExportData>();
            data._meshes = exportMeshes.ToArray();

            AssetDatabase.CreateAsset(data, path);
            foreach (Mesh mesh in exportMeshes) {
                AssetDatabase.AddObjectToAsset(mesh, data);
            }
        }


        bool IsImportedMeshResource(Mesh mesh) {
            return !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(mesh));
        }

    }



    public class FBXExporter {
        public struct BoneInfo {
            public int parentIdx;
            public Vector3 pos;
            public Vector3 rot;
            public Vector3 scale;
            public string name;
        };

        public struct BoneWeight {
            public int boneIndex0;
            public int boneIndex1;
            public int boneIndex2;
            public int boneIndex3;
            public float weight0;
            public float weight1;
            public float weight2;
            public float weight3;
        };


        [DllImport("FBXExporter")]
        private static extern int ExportMesh(string meshName, Vector3[] vertices, Vector3[] normals, Vector3[] tangents, Color[] colors, Vector2[] uv, int vertCount, int[] indices, int indicesCount, int[] texIds, string[] materialNames, int materialCount);

        [DllImport("FBXExporter")]
        private static extern int ExportSkinMesh(string meshName, Vector3[] vertices, Vector3[] normals, Vector3[] tangents, Color[] colors, Vector2[] uv, BoneWeight[] boneWeights, int vertCount, int[] indices, int indicesCount, int[] texIds, string[] materialNames, int materialCount, BoneInfo[] boneInfos, int boneCount);


        [DllImport("FBXExporter")]
        private static extern int SaveScene(string path);



        public bool _includeAllChildren = false;
        public bool _includeInactive = false;


        public void ExportMeshToFile(GameObject go) {
            string path = EditorUtility.SaveFilePanelInProject("Save Mesh", go.name, "FBX", "Please enter a file name to save the mesh(es) to");
            if (!string.IsNullOrEmpty(path)) {

                bool hasDataExport = false;
                {

                    MeshFilter[] meshFilters;
                    if (_includeAllChildren)
                        meshFilters = go.GetComponentsInChildren<MeshFilter>(_includeInactive);
                    else {
                        meshFilters = new MeshFilter[1];
                        meshFilters[0] = go.GetComponent<MeshFilter>();
                    }

                    foreach (MeshFilter meshFilter in meshFilters) {
                        if (meshFilter) {
                            if (ExportMesh(meshFilter.transform, meshFilter.sharedMesh, meshFilter.sharedMesh.name) == 0) {
                                hasDataExport = true;
                            }
                        }
                    }
                }

                {
                    SkinnedMeshRenderer[] skinnedRenderers;
                    if (_includeAllChildren)
                        skinnedRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>(_includeInactive);
                    else {
                        skinnedRenderers = new SkinnedMeshRenderer[1];
                        skinnedRenderers[0] = go.GetComponent<SkinnedMeshRenderer>();
                    }

                    foreach (SkinnedMeshRenderer skinnedRenderer in skinnedRenderers) {
                        if (skinnedRenderer != null) {
                            if (ExportMesh(skinnedRenderer.transform, skinnedRenderer.sharedMesh, skinnedRenderer.sharedMesh.name, true, skinnedRenderer.bones) == 0) {
                                hasDataExport = true;
                            }
                        }

                    }
                }


                if (hasDataExport) {
                    Save2FbxFile(path);
                    ImportMesh(path);
                } else {
                    EditorUtility.DisplayDialog("提示", "没有导出任何Mesh数据！", "OK");
                }
            }
        }


        int ExportMesh(Transform tr, Mesh mesh, string meshName, bool isSkin = false, Transform[] bones = null) {
#if !FBXEXPORT_DEBUG
            if (IsImportedMeshResource(mesh))
                return -1;
#endif
            int[] submeshIdxs = new int[mesh.triangles.Length / 3];
            for (int i = 0; i < submeshIdxs.Length; i++) {
                int a = mesh.triangles[i * 3 + 0];
                int b = mesh.triangles[i * 3 + 1];
                int c = mesh.triangles[i * 3 + 2];
                submeshIdxs[i] = FindSubMeshIndex(mesh, a, b, c);
            }

            List<string> materialNames = new List<string>();
            for (int i = 0; i < submeshIdxs.Length; i++)
                materialNames.Add("SubMaterial0" + i);



            Vector3[] positions = new Vector3[mesh.vertexCount];
            for (int i = 0; i < mesh.vertexCount; i++) {
                positions[i] = mesh.vertices[i] * 100;
                positions[i].x = -positions[i].x;
            }

            Vector3[] normals = null;
            if (mesh.normals.Length > 0) {
                normals = new Vector3[mesh.normals.Length];
                for (int i = 0; i < mesh.normals.Length; i++) {
                    normals[i] = mesh.normals[i];
                    normals[i].x = -normals[i].x;
                }
            }

            Vector3[] tangents = null;
            if (mesh.tangents.Length > 0) {
                tangents = new Vector3[mesh.tangents.Length];
                for (int i = 0; i < mesh.tangents.Length; i++) {
                    tangents[i] = mesh.tangents[i];
                    tangents[i].x = -tangents[i].x;
                }
            }

            Color[] colors = mesh.colors.Length > 0 ? mesh.colors : null;

            if (isSkin) {
                if (bones.Length != mesh.bindposes.Length) {
                    Debug.LogError("bone data is error!", tr);
                    return -1;
                }

                BoneWeight[] boneWeights = new BoneWeight[mesh.vertexCount];
                for (int i = 0; i < mesh.vertexCount; i++) {
                    boneWeights[i].boneIndex0 = mesh.boneWeights[i].boneIndex0;
                    boneWeights[i].boneIndex1 = mesh.boneWeights[i].boneIndex1;
                    boneWeights[i].boneIndex2 = mesh.boneWeights[i].boneIndex2;
                    boneWeights[i].boneIndex3 = mesh.boneWeights[i].boneIndex3;

                    boneWeights[i].weight0 = mesh.boneWeights[i].weight0;
                    boneWeights[i].weight1 = mesh.boneWeights[i].weight1;
                    boneWeights[i].weight2 = mesh.boneWeights[i].weight2;
                    boneWeights[i].weight3 = mesh.boneWeights[i].weight3;
                }
                BoneInfo[] boneInfos = new BoneInfo[bones.Length];
                for (int i = 0; i < bones.Length; i++) {
                    Matrix4x4 boneMatrix = mesh.bindposes[i].inverse;
                    boneInfos[i].parentIdx = FindBoneParentIdx(bones[i], bones);
                    if (boneInfos[i].parentIdx != -1) {
                        boneMatrix = mesh.bindposes[boneInfos[i].parentIdx] * boneMatrix;
                    }

                    Quaternion rot;
                    DecomposeMatrix(ref boneMatrix, out boneInfos[i].pos, out rot, out boneInfos[i].scale);
                    boneInfos[i].rot = quaternion2Euler(rot, RotSeq.zyx) * Mathf.Rad2Deg;

                    boneInfos[i].pos *= 100;
                    boneInfos[i].pos.x = -boneInfos[i].pos.x;
                    boneInfos[i].rot.y = -boneInfos[i].rot.y;
                    boneInfos[i].rot.z = -boneInfos[i].rot.z;
                    //boneInfos[i].scale.x = -boneInfos[i].scale.x;

                    boneInfos[i].name = bones[i].name;
                }

                return ExportSkinMesh(meshName, positions, normals, tangents, colors, mesh.uv, boneWeights, mesh.vertexCount, mesh.triangles, mesh.triangles.Length, submeshIdxs, materialNames.ToArray(), materialNames.Count, boneInfos, boneInfos.Length);
            } else {
                return ExportMesh(meshName, positions, normals, tangents, colors, mesh.uv, mesh.vertexCount, mesh.triangles, mesh.triangles.Length, submeshIdxs, materialNames.ToArray(), materialNames.Count);
            }
        }

        int Save2FbxFile(string path) {
            return SaveScene(path);
        }

        void ImportMesh(string path) {
            AssetDatabase.ImportAsset(path);
            ModelImporter importer = ModelImporter.GetAtPath(path) as ModelImporter;
            // importer.globalScale = 1;
            importer.importMaterials = false;
            importer.importAnimation = false;
            importer.importNormals = ModelImporterNormals.Import;
            importer.importTangents = ModelImporterTangents.Import;
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
        }

        bool IsImportedMeshResource(Mesh mesh) {
            return !string.IsNullOrEmpty(AssetDatabase.GetAssetPath(mesh));
        }

        int FindBoneParentIdx(Transform bone, Transform[] bones) {
            Transform parent = bone.parent;
            if (parent == null)
                return -1;

            for (int i = 0; i < bones.Length; i++) {
                if (bones[i] == parent)
                    return i;
            }

            return -1;
        }

        int FindSubMeshIndex(Mesh mesh, int a, int b, int c) {
            int index = -1;
            for (int i = 0; i < mesh.subMeshCount; i++) {
                int[] subMeshTriangles = mesh.GetTriangles(i);
                for (int j = 0; j < subMeshTriangles.Length / 3; j++) {
                    if (a == subMeshTriangles[j * 3 + 0] && b == subMeshTriangles[j * 3 + 1] && c == subMeshTriangles[j * 3 + 2]) {
                        return i;
                    }
                }
            }

            return index;
        }

        void CreateNewBones(SkinnedMeshRenderer skinnedRenderer, out Transform[] newBones, ref Dictionary<int, int> boneIndexMapping) {
            Transform rootBone = skinnedRenderer.rootBone;
            newBones = rootBone.GetComponentsInChildren<Transform>(true);

            for (int i = 0; i < skinnedRenderer.bones.Length; i++) {
                for (int j = 0; j < newBones.Length; j++) {
                    if (skinnedRenderer.bones[i] == newBones[j]) {
                        boneIndexMapping.Add(i, j);
                        break;
                    }
                }
            }
        }




        public enum RotSeq {
            zyx, zyz, zxy, zxz, yxz, yxy, yzx, yzy, xyz, xyx, xzy, xzx
        };

        static Vector3 twoaxisrot(float r11, float r12, float r21, float r31, float r32) {
            r21 = Mathf.Clamp(r21, -1, 1);
            Vector3 ret = new Vector3();
            ret.x = Mathf.Atan2(r11, r12);
            ret.y = Mathf.Acos(r21);
            ret.z = Mathf.Atan2(r31, r32);
            return ret;
        }

        static Vector3 threeaxisrot(float r11, float r12, float r21, float r31, float r32) {
            r21 = Mathf.Clamp(r21, -1, 1);
            Vector3 ret = new Vector3();
            ret.x = Mathf.Atan2(r31, r32);
            ret.y = Mathf.Asin(r21);
            ret.z = Mathf.Atan2(r11, r12);
            return ret;
        }

        public static Vector3 quaternion2Euler(Quaternion q, RotSeq rotSeq) {
            switch (rotSeq) {
                case RotSeq.zyx:
                    return threeaxisrot(2 * (q.x * q.y + q.w * q.z),
                        q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                        -2 * (q.x * q.z - q.w * q.y),
                        2 * (q.y * q.z + q.w * q.x),
                        q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z);


                case RotSeq.zyz:
                    return twoaxisrot(2 * (q.y * q.z - q.w * q.x),
                        2 * (q.x * q.z + q.w * q.y),
                        q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                        2 * (q.y * q.z + q.w * q.x),
                        -2 * (q.x * q.z - q.w * q.y));


                case RotSeq.zxy:
                    return threeaxisrot(-2 * (q.x * q.y - q.w * q.z),
                        q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                        2 * (q.y * q.z + q.w * q.x),
                        -2 * (q.x * q.z - q.w * q.y),
                        q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z);


                case RotSeq.zxz:
                    return twoaxisrot(2 * (q.x * q.z + q.w * q.y),
                        -2 * (q.y * q.z - q.w * q.x),
                        q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                        2 * (q.x * q.z - q.w * q.y),
                        2 * (q.y * q.z + q.w * q.x));


                case RotSeq.yxz:
                    return threeaxisrot(2 * (q.x * q.z + q.w * q.y),
                        q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                        -2 * (q.y * q.z - q.w * q.x),
                        2 * (q.x * q.y + q.w * q.z),
                        q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z);

                case RotSeq.yxy:
                    return twoaxisrot(2 * (q.x * q.y - q.w * q.z),
                        2 * (q.y * q.z + q.w * q.x),
                        q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                        2 * (q.x * q.y + q.w * q.z),
                        -2 * (q.y * q.z - q.w * q.x));


                case RotSeq.yzx:
                    return threeaxisrot(-2 * (q.x * q.z - q.w * q.y),
                        q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                        2 * (q.x * q.y + q.w * q.z),
                        -2 * (q.y * q.z - q.w * q.x),
                        q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z);


                case RotSeq.yzy:
                    return twoaxisrot(2 * (q.y * q.z + q.w * q.x),
                        -2 * (q.x * q.y - q.w * q.z),
                        q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                        2 * (q.y * q.z - q.w * q.x),
                        2 * (q.x * q.y + q.w * q.z));


                case RotSeq.xyz:
                    return threeaxisrot(-2 * (q.y * q.z - q.w * q.x),
                        q.w * q.w - q.x * q.x - q.y * q.y + q.z * q.z,
                        2 * (q.x * q.z + q.w * q.y),
                        -2 * (q.x * q.y - q.w * q.z),
                        q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z);


                case RotSeq.xyx:
                    return twoaxisrot(2 * (q.x * q.y + q.w * q.z),
                        -2 * (q.x * q.z - q.w * q.y),
                        q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                        2 * (q.x * q.y - q.w * q.z),
                        2 * (q.x * q.z + q.w * q.y));


                case RotSeq.xzy:
                    return threeaxisrot(2 * (q.y * q.z + q.w * q.x),
                        q.w * q.w - q.x * q.x + q.y * q.y - q.z * q.z,
                        -2 * (q.x * q.y - q.w * q.z),
                        2 * (q.x * q.z + q.w * q.y),
                        q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z);


                case RotSeq.xzx:
                    return twoaxisrot(2 * (q.x * q.z - q.w * q.y),
                        2 * (q.x * q.y + q.w * q.z),
                        q.w * q.w + q.x * q.x - q.y * q.y - q.z * q.z,
                        2 * (q.x * q.z + q.w * q.y),
                        -2 * (q.x * q.y - q.w * q.z));

                default:
                    Debug.LogError("No good sequence");
                    return Vector3.zero;

            }

        }

        /// <summary>
        /// Extract translation from transform matrix.
        /// </summary>
        /// <param name="matrix">Transform matrix. This parameter is passed by reference
        /// to improve performance; no changes will be made to it.</param>
        /// <returns>
        /// Translation offset.
        /// </returns>
        public static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix) {
            Vector3 translate;
            translate.x = matrix.m03;
            translate.y = matrix.m13;
            translate.z = matrix.m23;
            return translate;
        }

        /// <summary>
        /// Extract rotation quaternion from transform matrix.
        /// </summary>
        /// <param name="matrix">Transform matrix. This parameter is passed by reference
        /// to improve performance; no changes will be made to it.</param>
        /// <returns>
        /// Quaternion representation of rotation transform.
        /// </returns>
        public static Quaternion ExtractRotationFromMatrix(ref Matrix4x4 matrix) {
            Vector3 forward;
            forward.x = matrix.m02;
            forward.y = matrix.m12;
            forward.z = matrix.m22;

            Vector3 upwards;
            upwards.x = matrix.m01;
            upwards.y = matrix.m11;
            upwards.z = matrix.m21;

            return Quaternion.LookRotation(forward, upwards);
        }

        /// <summary>
        /// Extract scale from transform matrix.
        /// </summary>
        /// <param name="matrix">Transform matrix. This parameter is passed by reference
        /// to improve performance; no changes will be made to it.</param>
        /// <returns>
        /// Scale vector.
        /// </returns>
        public static Vector3 ExtractScaleFromMatrix(ref Matrix4x4 matrix) {
            Vector3 scale;
            scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
            scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
            scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
            return scale;
        }

        /// <summary>
        /// Extract position, rotation and scale from TRS matrix.
        /// </summary>
        /// <param name="matrix">Transform matrix. This parameter is passed by reference
        /// to improve performance; no changes will be made to it.</param>
        /// <param name="localPosition">Output position.</param>
        /// <param name="localRotation">Output rotation.</param>
        /// <param name="localScale">Output scale.</param>
        public static void DecomposeMatrix(ref Matrix4x4 matrix, out Vector3 localPosition, out Quaternion localRotation, out Vector3 localScale) {
            localPosition = ExtractTranslationFromMatrix(ref matrix);
            localRotation = ExtractRotationFromMatrix(ref matrix);
            localScale = ExtractScaleFromMatrix(ref matrix);
        }

        /// <summary>
        /// Set transform component from TRS matrix.
        /// </summary>
        /// <param name="transform">Transform component.</param>
        /// <param name="matrix">Transform matrix. This parameter is passed by reference
        /// to improve performance; no changes will be made to it.</param>
        public static void SetTransformFromMatrix(Transform transform, ref Matrix4x4 matrix) {
            transform.localPosition = ExtractTranslationFromMatrix(ref matrix);
            transform.localRotation = ExtractRotationFromMatrix(ref matrix);
            transform.localScale = ExtractScaleFromMatrix(ref matrix);
        }


        // EXTRAS!

        /// <summary>
        /// Identity quaternion.
        /// </summary>
        /// <remarks>
        /// <para>It is faster to access this variation than <c>Quaternion.identity</c>.</para>
        /// </remarks>
        public static readonly Quaternion IdentityQuaternion = Quaternion.identity;
        /// <summary>
        /// Identity matrix.
        /// </summary>
        /// <remarks>
        /// <para>It is faster to access this variation than <c>Matrix4x4.identity</c>.</para>
        /// </remarks>
        public static readonly Matrix4x4 IdentityMatrix = Matrix4x4.identity;

        /// <summary>
        /// Get translation matrix.
        /// </summary>
        /// <param name="offset">Translation offset.</param>
        /// <returns>
        /// The translation transform matrix.
        /// </returns>
        public static Matrix4x4 TranslationMatrix(Vector3 offset) {
            Matrix4x4 matrix = IdentityMatrix;
            matrix.m03 = offset.x;
            matrix.m13 = offset.y;
            matrix.m23 = offset.z;
            return matrix;
        }


    }
}
