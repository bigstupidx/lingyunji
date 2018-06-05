using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;
using System.Collections.Generic;
using UnityEditor.AI;

public class BuildNavigation : EditorWindow
{
    [System.Runtime.InteropServices.DllImport("ExportPlugin")]
    static extern bool InitPlugin(int tilesize);

    [System.Runtime.InteropServices.DllImport("ExportPlugin")]
    static extern bool ExportServerBinFile(string path, string scenename);

    [MenuItem("Tools/场景工具/烘焙导航网格并导出bin文件(慎用,会导致场景对象改变)", false, 0)]
    private static void Init()
    {
        EditorWindow.GetWindow(typeof(BuildNavigation));
    }

    void OnGUI()
    {
		//GUILayout.Label("旧版烘焙：转化后场景对象会改变，美术慎用");
		//if (GUILayout.Button("旧版烘焙寻路(美术不要使用)"))
		//{
		//    BeginBuildNavigation();
		//}

        GUILayout.Label("新烘焙：转化后场景对象会改变，美术慎用");
        if (GUILayout.Button("新烘焙寻路"))
        {
            BeginBuild();
            Export();
        }
    }

    static void Export()
    {
        string name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        EditorMapExporter.ExportWholeSelectionToSingle();
        InitPlugin(64);
        if (ExportServerBinFile("ExportedMap", name))
        {
            if (!System.IO.Directory.Exists("NavMesh/"))
                System.IO.Directory.CreateDirectory("NavMesh/");
			
			string outPath = "NavMesh/" + name + ".bin";
			System.IO.File.Copy("ExportedMap/" + name + ".bin", outPath, true);
			EditorUtility.DisplayDialog("提示", "bin文件保存到" + outPath, "Ok");
        }
    }

    #region 新的烘焙
    static void BeginBuild()
    {
        GameObject go = GameObject.Find("MapScene") as GameObject;
        if (go == null)
            return;

        //如果renderer没有collider的，不要烘焙
        foreach (Renderer r in go.transform.GetComponentsInChildren<Renderer>())
        {
            Collider c = r.gameObject.GetComponent<Collider>();
            {
                if(c==null)
                    GameObjectUtility.SetStaticEditorFlags(r.gameObject, GameObjectUtility.GetStaticEditorFlags(r.gameObject) & ~StaticEditorFlags.NavigationStatic);
            }
        }

        //每一个Collider都必须要有Renderer
        Collider[] cols = go.transform.GetComponentsInChildren<Collider>();
        //记录改变的render
        List<Renderer> colRenderListRecord = new List<Renderer>();
        foreach (Collider col in cols)
        {
            bool isCanWalk = (ComLayer.IsGroundLayer(col.gameObject.layer));
            //不区分可走不可走，全部障碍都可以走
            isCanWalk = true;
            //要吧renderer打开才能烘焙寻路
            if (col is TerrainCollider)
            {
                Terrain mesh = col.GetComponent<Terrain>();
                if (mesh == null)
                {
                    Debuger.LogError(col.name + " 找不到Terrain");
                    continue;
                }
                Debuger.LogError("碰撞含有Terrain组件，导出的obj后端无法使用");
            }
            else
            {
                Renderer mesh = col.GetComponent<Renderer>();
                if (mesh == null)
                {
                    Debuger.LogError(col.name + " 找不到Renderer");
                    continue;
                }
                if (mesh.enabled == false)
                    colRenderListRecord.Add(mesh);
                mesh.enabled = true;
            }
            GameObjectUtility.SetStaticEditorFlags(col.gameObject, StaticEditorFlags.NavigationStatic);
        }

        //设置stepheight
        SerializedObject settingsObject = new SerializedObject(NavMeshBuilder.navMeshSettingsObject);
        SerializedProperty agentRadius = settingsObject.FindProperty("m_BuildSettings.agentClimb");
        agentRadius.floatValue = 0.5f;
        settingsObject.ApplyModifiedProperties();

        NavMeshBuilder.BuildNavMesh();

        //吧render不激活
        foreach (Renderer r in colRenderListRecord)
            r.enabled = false;
    }
    #endregion

    #region 旧的烘焙
    static void BeginBuildNavigation()
    {
        CopyColToTemp("MapScene");

        GameObject mapScene = GameObject.Find("MapScene") as GameObject;
        if (mapScene != null)
            mapScene.SetActive(false);
        //创建导航网格
        NavMeshBuilder.BuildNavMesh();
        Export();

    }

    //临时文件夹
    static GameObject s_tempTerrain;
    static GameObject s_tempObstnuct;
    static Transform GetParent(bool isCanWalk)
    {
        if (isCanWalk)
        {
            if (s_tempTerrain == null)
            {
                s_tempTerrain = GameObject.Find("[tempTerrain]") as GameObject;
                if (s_tempTerrain == null)
                    s_tempTerrain = new GameObject("[tempTerrain]");
            }
            return s_tempTerrain.transform;
        }
        else
        {
            if (s_tempObstnuct == null)
            {
                s_tempObstnuct = GameObject.Find("[tempObstnuct]") as GameObject;
                if (s_tempObstnuct == null)
                    s_tempObstnuct = new GameObject("[tempObstnuct]");
            }
            return s_tempObstnuct.transform;
        }
    }

    //吧碰撞归类，放到对应文件夹
    static void CopyColToTemp(string findGo)
    {
        GameObject go = GameObject.Find(findGo) as GameObject;
        if (go == null)
            return;
        //每一个Collider都必须要有Renderer,并且把碰撞复制到toParentTemp
        Collider[] cols = go.transform.GetComponentsInChildren<Collider>();
        foreach (Collider col in cols)
        {
            bool isCanWalk = (ComLayer.IsGroundLayer(col.gameObject.layer));
            //不区分可走不可走，全部障碍都可以走
            isCanWalk = true;
            //要吧renderer打开才能烘焙寻路
            if (col is TerrainCollider)
            {
                Terrain mesh = col.GetComponent<Terrain>();
                if (mesh == null)
                {
                    Debuger.LogError(col.name + " 找不到Terrain");
                    continue;
                }
                mesh.enabled = true;
                Debuger.LogError("碰撞含有Terrain组件，导出的obj后端无法使用");
            }
            else
            {
                Renderer mesh = col.GetComponent<Renderer>();
                if (mesh == null)
                {
                    Debuger.LogError(col.name + " 找不到Renderer");
                    continue;
                }
                mesh.enabled = true;
            }

            col.transform.parent = GetParent(isCanWalk).transform;
            GameObjectUtility.SetStaticEditorFlags(col.gameObject, StaticEditorFlags.NavigationStatic);
            //设置能否行走
            GameObjectUtility.SetNavMeshArea(col.gameObject, isCanWalk ? 0 : 1);
        }
    }
    #endregion
}
