// using UnityEngine;
// using UnityEditor;
// using System.Collections;
// using System.Collections.Generic;
// using System.IO;
// 
// namespace PackTool
// {
//     public class UIEditorWindow : EditorWindow
//     {
//         [MenuItem("PackTool/UIResources", false, 9)]
//         [MenuItem("Assets/PackTool/UIResources", false, 0)]
//         static public void OpenAtlasMaker()
//         {
//             EditorWindow.GetWindow<UIEditorWindow>(false, "UIResources", true);
//         }
// 
//         bool PackAllResources; // 打包所有资源
//         Vector3 GlobalPosition;
//         Vector3 FontViewPos;
//         Vector3 AtlasViewPos;
//         Vector3 PanelPos;
//         List<UIFont> fontsList;
//         List<UIAtlas> atlasList;
//         List<UIPanelBase> panelList;
//         List<Texture> textureList;
// 
//         bool bFontBtn = false;
//         bool bPanelBtn = false;
//         bool bAtlasBtn = false;
//         bool bTextureBtn = false;
//         void OnGUI()
//         {
//             PackAllResources = false;
//             if (GUILayout.Button("是否打包所有资源!", GUILayout.Height(50f)))
//             {
//                 PackAllResources = true;
//                 Debug.Log("清除缓存成功!");
//             }
// 
//             GlobalPosition = GUILayout.BeginScrollView(GlobalPosition);
//             List<GameObject> objList = null;
//             if (bFontBtn = GUILayout.Toggle(bFontBtn, "显示字体资源!"))
//             {
//                 objList = GetDirectoryPrefabs("UIData");
//                 fontsList = GetComponent<UIFont>(objList);
//             }
// 
//             if (bFontBtn)
//             {
//                 EditorGUILayout.LabelField("字体数量:" + (fontsList != null ? fontsList.Count : 0));
//                 if (fontsList != null && fontsList.Count != 0)
//                 {
//                     foreach (UIFont font in fontsList)
//                     {
//                         EditorGUILayout.ObjectField(font, typeof(UIFont));
//                     }
//                 }
//             }
// 
//             if (bAtlasBtn = GUILayout.Toggle(bAtlasBtn, "显示图集!"))
//             {
//                 if (objList == null)
//                     objList = GetDirectoryPrefabs("UIData");
//                 atlasList = GetComponent<UIAtlas>(objList);
//             }
// 
//             if (bTextureBtn = GUILayout.Toggle(bTextureBtn, "显示贴图!"))
//             {
//                 if (textureList == null)
//                     textureList = new List<Texture>();
// 
//                 if (textureList.Count == 0)
//                 {
//                     textureList = GetDirectoryTexture();
//                 }
//             }
//             else
//             {
//                 if (textureList != null)
//                     textureList.Clear();
//             }
// 
//             if (bTextureBtn)
//             {
//                 EditorGUILayout.LabelField("贴图数量:" + (textureList != null ? textureList.Count : 0));
//                 foreach (Texture texture in textureList)
//                 {
//                     EditorGUILayout.ObjectField(texture, typeof(Texture));
//                 }
//             }
// 
//             if (bAtlasBtn == true)
//             {
//                 EditorGUILayout.LabelField("图集数量:" + (atlasList == null ? 0 : atlasList.Count));
//                 foreach (UIAtlas atlas in atlasList)
//                 {
//                     EditorGUILayout.ObjectField(atlas, typeof(UIAtlas));
//                 }
//             }
// 
//             if (bPanelBtn = GUILayout.Toggle(bPanelBtn, "显示面板!"))
//             {
//                 if (objList == null)
//                     objList = GetDirectoryPrefabs("UIData");
//                 panelList = GetComponent<UIPanelBase>(objList);
//             }
// 
//             if (bPanelBtn)
//             {
//                 EditorGUILayout.LabelField("面板数量:" + (panelList != null ? panelList.Count : 0));
//                 foreach (UIPanelBase panel in panelList)
//                 {
//                     EditorGUILayout.ObjectField(panel, typeof(UIPanelBase));
//                 }
//             }
// 
//             GUILayout.EndScrollView();
// 
//             if (PackAllResources)
//             {
//                 PackEditorWindow.CreateTmpList();
// 
//                 // 开始打包所有资源
//                 if (objList == null)
//                 {
//                     objList = GetDirectoryPrefabs("UIData");
//                 }
// 
//                 List<UIFont> fonts = GetComponent<UIFont>(objList);
//                 foreach (UIFont font in fonts)
//                 {
//                     NGUIResourcesExport.ExportNGUIFont(font, false);
//                 }
// 
//                 List<UIAtlas> atlas = GetComponent<UIAtlas>(objList);
//                 foreach (UIAtlas at in atlas)
//                 {
//                     ComponentData.AddObject(at);
//                 }
// 
//                 List<UIPanelBase> panelBases = GetComponent<UIPanelBase>(objList);
//                 List<GameObject> deleteList = new List<GameObject>();
//                 foreach (UIPanelBase panelBase in panelBases)
//                 {
//                     deleteList.Add(ResourcesExport.ExportPrefab(panelBase.gameObject, false));
//                 }
// 
//                 // 开始打包贴图
//                 if (textureList == null || textureList.Count == 0)
//                     textureList = GetDirectoryTexture();
//                 foreach (Texture texture in textureList)
//                     ResourcesExport.ExportTexture(texture);
// 
//                 PackEditorWindow.SaveTmpList();
// 
//                 foreach (GameObject del in deleteList)
//                     ResourcesExport.DeleteCopyPrefab(del);
//             }
//         }
// 
//         List<T> GetComponent<T>(List<GameObject> objs) where T : Component
//         {
//             List<T> l = new List<T>();
//             T t = null;
//             foreach (GameObject obj in objs)
//             {
//                 if ((t = obj.GetComponent<T>()) != null)
//                 {
//                     l.Add(t);
//                 }
//             }
// 
//             return l;
//         }
// 
//         // 得到某个目录下的所有预设
//         List<GameObject> GetDirectoryPrefabs(string path)
//         {
//             int startIndex = Application.dataPath.Length - "Assets".Length;
//             path = Application.dataPath + "/" + path;
//             //Debug.Log(path);
//             string[] files = Directory.GetFiles(path, "*.prefab", SearchOption.AllDirectories);
//             List<GameObject> objList = new List<GameObject>();
//             foreach (string file in files)
//             {
//                 if (!(file.Contains("/NGUIResources/") || file.Contains("\\NGUIResources\\")))
//                     continue;
// 
//                 string s = file.Substring(startIndex);
//                 GameObject o = (GameObject)UnityEditor.AssetDatabase.LoadAssetAtPath(s, typeof(GameObject));
//                 if (o != null)
//                 {
//                     objList.Add(o);
//                 }
//             }
// 
//             return objList;
//         }
// 
//         // 得到某个目录下的所有
//         List<Texture> GetDirectoryTexture()
//         {
//             string targetPath = "/UIData";
//             int startIndex = Application.dataPath.Length - 6;
//             if (!Directory.Exists(Application.dataPath + targetPath))
//                 return null;
// 
//             List<Texture> textureList = new List<Texture>();
//             string[] files = Directory.GetFiles(Application.dataPath + targetPath, "*", SearchOption.AllDirectories);
//             //Debug.Log(files.Length);
//             foreach (string file in files)
//             {
//                 if (file.EndsWith(".meta"))
//                     continue;
// 
//                 if (!file.Replace('\\', '/').Contains("/NGUIResources/Textures/"))
//                     continue;
// 
//                 string s = file.Substring(startIndex);
//                 Texture asset = (Texture)UnityEditor.AssetDatabase.LoadAssetAtPath(s, typeof(Texture));
//                 if (asset != null)
//                 {
//                     s = s.Substring(targetPath.Length + 7);
//                     s = s.Replace('\\', '/');
//                     textureList.Add(asset);
//                 }
//             }
// 
//             return textureList;
//         }
//     }
// }
