using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Eyesblack.Utility;

namespace Eyesblack.Nature {
    public class GrassPrototypeEditor : EditorWindow {
        public GrassEditorInspector _parent;
        public GrassPrototype _prototype;


        void OnEnable() {
            titleContent.text = "Prototype";
        }



        void OnGUI() {
            EditorGUILayout.Space();

            Material newMaterial = EditorGUILayout.ObjectField("Material", _prototype._material, typeof(Material), false) as Material;
            _parent.SetPrototypeMaterial(_prototype, newMaterial);


            EditorGUI.BeginChangeCheck();
            _prototype._minWidth = EditorGUILayout.FloatField("Min Width", _prototype._minWidth);
            _prototype._maxWidth = EditorGUILayout.FloatField("Max Width", _prototype._maxWidth);
            _prototype._minHeight = EditorGUILayout.FloatField("Min Height", _prototype._minHeight);
            _prototype._maxHeight = EditorGUILayout.FloatField("Max Height", _prototype._maxHeight);
            if (EditorGUI.EndChangeCheck()) {
                _parent.UpdateDetailsSize(_prototype);
                UtilityFuncs.MarkCurrentSceneIsDirty();
            }
        }
    }







    [CustomEditor(typeof(GrassEditor))]
    public class GrassEditorInspector : Editor {
        public GrassEditor _editor;
        private int _currentDetailSel = -1;

        private static bool _showAdvanced = false;

        private static GUIStyle _previewImageStyle;

        private const float IMAGE_SIZE = 80;
        private const int IMAGES_PER_ROW = 4;

        private int _currentMenuBar = 0;
        private string[] _menuBars = { "Detail", "Setting" };


        private List<GrassPrototype> _prototypes;

        private GameObject _grassRootGo;
        private GameObject GrassRootGo {
            get {
                if (_grassRootGo == null) {
                    GrassRoot gr = GameObject.FindObjectOfType<GrassRoot>();
                    _grassRootGo = gr != null ? gr.gameObject : null;
                    if (_grassRootGo == null) {
                        _grassRootGo = new GameObject("__GrassRoot__");
                        //_vegetationRoot.hideFlags = HideFlags.HideInHierarchy;
                        _grassRootGo.hideFlags = HideFlags.NotEditable;
                        _grassRootGo.AddComponent<GrassRoot>();
                        _grassRootGo.transform.SetParent(_editor.transform.parent);

                    }
                }
                return _grassRootGo;
            }
        }



        private Tool _lastTool = Tool.None;

        private Projector _brushProjector;
        public Projector BrushProjector {
            get {
                if (_brushProjector == null) {
                    GameObject go = GameObject.Find("__GrassEditorBrushProjector__");
                    if (go == null) {
                        go = GameObject.Instantiate(_editor._burshProjectorPrefab);
                        go.hideFlags = HideFlags.HideAndDontSave;
                        go.name = "__GrassEditorBrushProjector__";
                    }

                    _brushProjector = go.GetComponent<Projector>();
                }

                return _brushProjector;
            }
        }




        private GrassPrototype CurrentPrototype {
            get {
                if (_currentDetailSel >= 0 && _currentDetailSel < _editor._prototypes.Count)
                    return _editor._prototypes[_currentDetailSel];
                return null;
            }
        }

        private Texture2D GetImagePreView(GrassPrototype prototype) {
            Texture2D image = null;
            if (prototype != null) {
                Material m = prototype._material;
                if (m != null)
                    image = m.mainTexture as Texture2D;
            }

            return image;
        }




        void OnEnable() {
            _editor = target as GrassEditor;
            if (_editor == null)
                return;


            _lastTool = Tools.current;
            Tools.current = Tool.None;

            _prototypes = _editor._prototypes;
            BrushProjector.ignoreLayers = ~_editor._layerMask;
        }



        void OnDisable() {
            if (_brushProjector != null) {
                _brushProjector.enabled = false;
                _brushProjector = null;
            }


            Tools.current = _lastTool;
        }

        private void CheckGUIRes() {
            if (_previewImageStyle == null) {
                _previewImageStyle = new GUIStyle();
                _previewImageStyle.padding = new RectOffset(3, 3, 3, 3);
                _previewImageStyle.margin = new RectOffset(0, 2, 0, 2);
                _previewImageStyle.border = new RectOffset(3, 3, 3, 3);
                //_previewImageStyle.normal.background = _currentDetailSel == i ? _selBackground : _unselBackground;
            }
        }


        public override void OnInspectorGUI() {
            CheckGUIRes();

            EditorGUILayout.Space();

            _currentMenuBar = GUILayout.Toolbar(_currentMenuBar, _menuBars);
            EditorGUILayout.Space();

            if (_currentMenuBar == 0) {
                Rect boxRectBegin = GUILayoutUtility.GetLastRect();

                if (_editor._prototypes.Count > 0) {
                    GUILayoutOption preViewImageWidth = GUILayout.Width(IMAGE_SIZE);
                    GUILayoutOption preViewImageHeight = GUILayout.Height(IMAGE_SIZE);

                    int rows = Mathf.CeilToInt(_editor._prototypes.Count / (float)IMAGES_PER_ROW);
                    EditorGUILayout.BeginVertical();
                    for (int r = 0; r < rows; r++) {
                        EditorGUILayout.BeginHorizontal();
                        for (int i = r * IMAGES_PER_ROW; i < (r + 1) * IMAGES_PER_ROW && i < _editor._prototypes.Count; i++) {
                            _previewImageStyle.normal.background = _currentDetailSel == i ? _editor._buttonOnTex : _editor._buttonOffTex;

                            EditorGUILayout.BeginVertical();
                            Texture2D imgPreview = GetImagePreView(_editor._prototypes[i]);
                            if (GUILayout.Button(imgPreview, _previewImageStyle, preViewImageWidth, preViewImageHeight)) {
                                if (_currentDetailSel == i)
                                    _currentDetailSel = -1;
                                else
                                    _currentDetailSel = i;
                            }

                            EditorGUI.BeginChangeCheck();
                            Material newMaterial = EditorGUILayout.ObjectField(_editor._prototypes[i]._material, typeof(Material), false, preViewImageWidth) as Material;
                            if (EditorGUI.EndChangeCheck())
                                SetPrototypeMaterial(_editor._prototypes[i], newMaterial);
                            EditorGUILayout.EndVertical();
                        }
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("New")) {
                    Undo.RecordObject(_editor, "Add New Prototype");
                    NewPrototype();
                }

                if (_currentDetailSel != -1) {
                    if (GUILayout.Button("Edit")) {
                        GrassPrototypeEditor editWindow = ScriptableObject.CreateInstance<GrassPrototypeEditor>();
                        editWindow._parent = this;
                        editWindow._prototype = _editor._prototypes[_currentDetailSel];
                        editWindow.Show();
                    }

                    if (GUILayout.Button("Delete")) {
                        bool remove = true;
                        if (_editor._prototypes[_currentDetailSel]._material != null) {
                            if (!EditorUtility.DisplayDialog("确认删除吗？", "这将删除所有使用当前材质的草！！！", "确认", "取消"))
                                remove = false;
                        }
                        if (remove) {
                            Undo.RecordObject(_editor, "Delete Prototype");
                            if (RemovePrototype(_currentDetailSel)) {
                                if (_editor._prototypes.Count == 0)
                                    _currentDetailSel = -1;
                                else if (_currentDetailSel >= _editor._prototypes.Count) {
                                    _currentDetailSel = _editor._prototypes.Count - 1;
                                }
                            }
                        }
                    }
                }

                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();

                Rect boxRectEnd = GUILayoutUtility.GetLastRect();

                GUI.Box(new Rect(boxRectBegin.xMin - 3, boxRectBegin.yMin, (IMAGE_SIZE + 6) * IMAGES_PER_ROW + 6, boxRectEnd.yMax - boxRectBegin.yMin + 5), GUIContent.none);


                GUILayout.Space(10);


                EditorGUI.BeginChangeCheck();
                int brushSize = EditorGUILayout.IntSlider("Brush Size", _editor._brushSize, 1, 20);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(_editor, "Set Brush Size");
                    _editor._brushSize = brushSize;
                }

                EditorGUI.BeginChangeCheck();
                int density = EditorGUILayout.IntSlider("Density", _editor._density, 1, 20);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(_editor, "Set Brush Density");
                    _editor._density = density;
                }

                EditorGUI.BeginChangeCheck();
                bool isKeepInDeep = EditorGUILayout.Toggle("Keep In Deep", _editor._isKeepInDeep);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(_editor, "Set Keep In Deep");
                    _editor._isKeepInDeep = isKeepInDeep;            
                }

                EditorGUILayout.Space();
                _showAdvanced = EditorGUILayout.Toggle("Advanced", _showAdvanced);
                if (_showAdvanced) {
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("Delete Grass Root")) {
                        if (EditorUtility.DisplayDialog("确认删除", "确认要删除跟对象吗？这样会删除场景中所有的草！", "确认", "取消"))
                            GameObject.DestroyImmediate(GrassRootGo);
                    }
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }


            } else if (_currentMenuBar == 1) {  //Setting
                EditorGUI.BeginChangeCheck();
                int layerMask = LayerMaskField("Culling Mask", _editor._layerMask);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(_editor, "Set Bursh Culling Mask");
                    _editor._layerMask = layerMask;
                    BrushProjector.ignoreLayers = ~_editor._layerMask;
                }

                float scaleInLightmap = EditorGUILayout.FloatField("Scale In Lightmap", _editor._scaleInLightmap);
                if (scaleInLightmap < 0)
                    scaleInLightmap = 0;
                if (scaleInLightmap != _editor._scaleInLightmap) {
                    Undo.RecordObject(_editor, "Set Detail Scale In Lightmap");
                    _editor._scaleInLightmap = scaleInLightmap;
                    UpdateAllScaleInLightmap();
                }
            }
        }


        static LayerMask LayerMaskField(string label, LayerMask layerMask) {
            List<int> layerNumbers = new List<int>();
            var layers = UnityEditorInternal.InternalEditorUtility.layers;
            layerNumbers.Clear();

            for (int i = 0; i < layers.Length; i++)
                layerNumbers.Add(LayerMask.NameToLayer(layers[i]));

            int maskWithoutEmpty = 0;
            for (int i = 0; i < layerNumbers.Count; i++) {
                if (((1 << layerNumbers[i]) & layerMask.value) != 0)
                    maskWithoutEmpty |= (1 << i);
            }

            maskWithoutEmpty = UnityEditor.EditorGUILayout.MaskField(label, maskWithoutEmpty, layers);

            int mask = 0;
            for (int i = 0; i < layerNumbers.Count; i++) {
                if ((maskWithoutEmpty & (1 << i)) != 0)
                    mask |= (1 << layerNumbers[i]);
            }
            layerMask.value = mask;

            return layerMask;
        }


        public void SetPrototypeMaterial(GrassPrototype prototype, Material newMaterial) {
            if (prototype._material == null && newMaterial != null) {
                if (!newMaterial.shader.name.Contains("Grass")) {
                    Debug.LogError("Must use the specfied \"Grass\" shader!");
                } else {
                    if (!IsExistMaterail(newMaterial))
                        prototype._material = newMaterial;
                }
            }
        }



        public void NewPrototype() {
            _prototypes.Add(new GrassPrototype());
        }

        public bool RemovePrototype(int idx) {
            if (idx >= 0 && idx < _prototypes.Count) {
                if (_prototypes[idx] != null) {
                    RemoveDetailObjs(_prototypes[idx]);
                }

                //Undo.RecordObject(_editor, "Delete Prototype");
                _prototypes.RemoveAt(idx);
                return true;
            }
            return false;
        }

        public bool IsExistMaterail(Material material) {
            foreach (GrassPrototype prototype in _prototypes) {
                if (prototype._material == material) {
                    return true;
                }
            }

            return false;
        }

        public bool UnsetPrototype(int idx) {
            if (idx < 0 || idx >= _prototypes.Count || _prototypes[idx] == null)
                return false;

            _prototypes[idx] = null;
            return true;
        }

        public void SetDetailObjs(GrassPrototype prototype, int x, int z, int amount) {
            if (prototype != null && amount > 0) {
                List<GameObject> objs = new List<GameObject>();
                GetDetailObjs(prototype, x, z, ref objs);
                if (objs.Count > 0) {
                    if (amount > objs.Count) {
                        //Undo.RecordObject(this, "Set Detail");
                        AddDetailObjs(prototype, x, z, amount - objs.Count);
                    } else if (amount < objs.Count) {
                        //Undo.RecordObject(this, "Set Detail");
                        RemoveObjs(objs.Count - amount, ref objs);
                    }
                } else {
                    AddDetailObjs(prototype, x, z, amount);
                }
            }
        }

        private void DestoryObj(GameObject obj) {
            DestroyImmediate(obj);
            UtilityFuncs.MarkCurrentSceneIsDirty();
            //Undo.DestroyObjectImmediate(obj);
        }


        public void RemoveDetailObjs(GrassPrototype prototype) {
            GrassObject[] infos = GrassRootGo.GetComponentsInChildren<GrassObject>();
            foreach (GrassObject info in infos) {
                if (info._material == prototype._material) {
                    //DestroyImmediate(info.gameObject);
                    DestoryObj(info.gameObject);
                }
            }
        }

        public void ClearDetail(GrassPrototype prototype, int x, int z) {
            GrassObject[] infos = GrassRootGo.GetComponentsInChildren<GrassObject>();
            foreach (GrassObject info in infos) {
                if (info._material == prototype._material && info._x == x && info._z == z) {
                    //DestroyImmediate(info.gameObject);
                    DestoryObj(info.gameObject);
                }
            }
        }

        public void ClearAllDetails(int x, int z) {
            GrassObject[] infos = GrassRootGo.GetComponentsInChildren<GrassObject>();
            foreach (GrassObject info in infos) {
                if (info._x == x && info._z == z) {
                    //DestroyImmediate(info.gameObject);
                    DestoryObj(info.gameObject);
                }
            }
        }




        private bool AddDetailObjs(GrassPrototype prototype, int x, int z, int amount) {
            for (int i = 0; i < amount; i++) {
                Vector3 pos = new Vector3(x + Random.Range(0.0f, 1.0f), 0, z + Random.Range(0.0f, 1.0f));
                Ray ray = new Ray(new Vector3(pos.x, 100000, pos.z), Vector3.down);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, _editor._layerMask)) {
                    pos.y = hit.point.y;

   
                    GameObject go = new GameObject(prototype._material.name);
                    //Undo.RegisterCreatedObjectUndo(go, go.name);                 
                    //go.layer = LayerMask.NameToLayer("Vegetation");
                    //go.hideFlags = HideFlags.HideInHierarchy;
                    go.tag = "EditorOnly";
                    GameObjectUtility.SetStaticEditorFlags(go, StaticEditorFlags.LightmapStatic | StaticEditorFlags.BatchingStatic);
                    go.hideFlags = HideFlags.NotEditable;

                    go.transform.position = pos;
                    go.transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
                    float width = Random.Range(prototype._minWidth, prototype._maxWidth);
                    float height = Random.Range(prototype._minHeight, prototype._maxHeight);
                    go.transform.localScale = new Vector3(width, height, 1);
                    go.transform.SetParent(GrassRootGo.transform);

                    MeshFilter meshFilter = go.AddComponent<MeshFilter>();
                    Mesh mesh = _editor._grassMesh;
                    meshFilter.mesh = mesh;

                    MeshRenderer meshRenderer = go.AddComponent<MeshRenderer>();
                    meshRenderer.sharedMaterial = prototype._material;
                    SerializedObject so = new SerializedObject(meshRenderer);
                    SerializedProperty goScaleProperty = so.FindProperty("m_ScaleInLightmap");
                    goScaleProperty.floatValue = _editor._scaleInLightmap;
                    so.ApplyModifiedProperties();

                    GrassObject info = go.AddComponent<GrassObject>();
                    info._material = prototype._material;
                    //info._x = x;
                    //info._z = z;


                    //fix y
                    if (_editor._isKeepInDeep) {
                        float minY = pos.y;
                        for (int v = 0; v < mesh.vertexCount; v++) {
                            if (mesh.vertices[v].y == 0) {
                                Vector3 posWorld = go.transform.TransformPoint(mesh.vertices[v]);
                                if (Physics.Raycast(posWorld, Vector3.down, out hit, Mathf.Infinity, _editor._layerMask)) {
                                    if (hit.point.y < minY)
                                        minY = hit.point.y;
                                }
                            }
                        }
                        if (minY < pos.y) {
                            pos.y = minY;
                            go.transform.position = pos;
                        }
                    }

                    UtilityFuncs.MarkCurrentSceneIsDirty();
                }

            }

            return true;
        }


        private bool RemoveObjs(int amount, ref List<GameObject> objs) {
            for (int i = 0; i < amount; i++) {
                int ridx = Random.Range(0, objs.Count);
                GameObject go = objs[ridx];
                //DestroyImmediate(go);
                DestoryObj(go);
                objs.RemoveAt(ridx);
            }
            return true;
        }


        public void GetDetailObjs(GrassPrototype prototype, int x, int z, ref List<GameObject> objs) {
            //GrassEditor.Detail detail = null;
            GrassObject[] infos = GrassRootGo.GetComponentsInChildren<GrassObject>();
            foreach (GrassObject info in infos) {
                if (info._material == prototype._material && info._x == x && info._z == z) {
                    /*
                    if (detail == null) {
                        detail = new GrassEditor.Detail();
                        detail.objs = new List<GameObject>();
                    }
                    detail.objs.Add(info.gameObject);
                    */
                    objs.Add(info.gameObject);
                }
            }
            // return detail;
        }

        public void UpdateAllScaleInLightmap() {
            GrassObject[] infos = GrassRootGo.GetComponentsInChildren<GrassObject>();
            foreach (GrassObject info in infos) {
                GameObject go = info.gameObject;
                SerializedObject so = new SerializedObject(go.GetComponent<MeshRenderer>());
                SerializedProperty goScaleProperty = so.FindProperty("m_ScaleInLightmap");

                if (_editor._scaleInLightmap != goScaleProperty.floatValue) {
                    Undo.RecordObject(go, "Set ScaleInLightmap");

                    goScaleProperty.floatValue = _editor._scaleInLightmap;
                    so.ApplyModifiedProperties();
                }
            }
        }


        public void UpdateDetailsSize(GrassPrototype prototype) {
            GrassObject[] grassObjs = GrassRootGo.GetComponentsInChildren<GrassObject>();
            foreach (GrassObject grassObj in grassObjs) {
                if (grassObj._material == prototype._material) {
                    float width = Random.Range(prototype._minWidth, prototype._maxWidth);
                    float height = Random.Range(prototype._minHeight, prototype._maxHeight);

                    grassObj.transform.localScale = new Vector3(width, height, 1);
                }
            }
        }


        void OnSceneGUI() {
            //if (!_editor._editMode)
            //   return;

            HandleUtility.AddDefaultControl(0);

            BrushProjector.enabled = _currentMenuBar == 0 && CurrentPrototype != null;
            BrushProjector.orthographicSize = _editor._brushSize;

            Event e = Event.current;

            int brushSize = _editor._brushSize;
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.KeypadPlus) {
                brushSize += 1;
            } else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.KeypadMinus) {
                brushSize -= 1;
            }
            brushSize = Mathf.Clamp(brushSize, 1, 20);
            if (_editor._brushSize != brushSize) {
                Undo.RecordObject(_editor, "Set Brush Size");
                _editor._brushSize = brushSize;
            }

            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _editor._layerMask)) {
                int cx = Mathf.FloorToInt(hit.point.x);
                int cz = Mathf.FloorToInt(hit.point.z);
                Transform brushTr = BrushProjector.transform;
                Vector3 brushPos = brushTr.position;
                brushPos.x = cx + 0.5f;
                brushPos.y = hit.point.y + 10;
                brushPos.z = cz + 0.5f;
                brushTr.position = brushPos;

                int range = _editor._brushSize - 1;
                if (e.button == 0 && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)) {
                    if (_currentMenuBar == 0) { //detail
                        for (int x = cx - range; x <= cx + range; x++) {
                            for (int z = cz - range; z <= cz + range; z++) {
                                if ((x - cx) * (x - cx) + (z - cz) * (z - cz) <= range * range) {
                                    if (e.shift) {
                                        ClearAllDetails(x, z);
                                    } else if (e.control) {
                                        if (CurrentPrototype != null)
                                            ClearDetail(CurrentPrototype, x, z);
                                    } else {
                                        if (CurrentPrototype != null)
                                            SetDetailObjs(CurrentPrototype, x, z, _editor._density);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            SceneView.RepaintAll();
        }
    }




}

