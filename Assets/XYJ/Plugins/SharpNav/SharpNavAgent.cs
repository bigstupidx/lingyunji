#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Triangle3 = SharpNav.Geometry.Triangle3;

public class SharpNavAgent
{
    public static readonly string pathRoot;

    static SharpNavAgent()
    {
        pathRoot = Application.dataPath + "/../Data/Config/Terrain/";

        Reg((BoxCollider box, List<Triangle3> triangles) => 
        {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
            Object.DestroyImmediate(go);

            GetTriangle3(mesh, box.transform.localToWorldMatrix, triangles);
        });

        Reg((MeshCollider collider, List<Triangle3> triangles) => { GetTriangle3(collider.sharedMesh, collider.transform.localToWorldMatrix, triangles); });

    }

    static void GetTriangle3(Mesh mesh, Matrix4x4 localToWorldMatrix, List<Triangle3> triangles)
    {
        if (mesh == null)
            return;

        Vector3[] vertices = mesh.vertices;
        for (int i = 0; i < mesh.subMeshCount; ++i)
        {
            var subMesh = mesh.GetIndices(i);
            for (int j = 0; j < subMesh.Length; j += 3)
            {
                Vector3 a = localToWorldMatrix.MultiplyPoint(vertices[subMesh[j]]);
                Vector3 b = localToWorldMatrix.MultiplyPoint(vertices[subMesh[j + 1]]);
                Vector3 c = localToWorldMatrix.MultiplyPoint(vertices[subMesh[j + 2]]);

                triangles.Add(new Triangle3(
                    new SharpNav.Geometry.Vector3(a.x, a.y, a.z),
                    new SharpNav.Geometry.Vector3(b.x, b.y, b.z),
                    new SharpNav.Geometry.Vector3(c.x, c.y, c.z)
                    ));
            }
        }
    }

    [MenuItem("Assets/ExportNav")]
    static void ExportNav()
    {
        var navMesh = ExportNavMesh();

        SharpNavDrawer snd = GameObject.FindObjectOfType<SharpNavDrawer>();
        if (snd != null)
            snd.SetNavMesh(navMesh, SharpNav.NavMeshGenerationSettings.Default.VertsPerPoly);
    }

    [MenuItem("Assets/ExportNavFile")]
    static void ExportNavFile()
    {
        string file = pathRoot + UnityEngine.SceneManagement.SceneManager.GetActiveScene().name + ".bin";

        System.IO.Directory.CreateDirectory(file.Substring(0, file.LastIndexOf('/')));
        var navMesh = GetColliderTriangle();
        if (navMesh == null)
        {
            Debug.LogErrorFormat("navMesh == null");
            return;
        }

        System.IO.File.WriteAllBytes(file, SharpNav.Serialization.Generate(navMesh, SharpNav.NavMeshGenerationSettings.Default));
    }

    public static SharpNav.NavMesh CreateNavMeshByFile(string file)
    {
        if (!file.Contains(":"))
            file = pathRoot + file + ".bin";

        return SharpNav.Serialization.Create(System.IO.File.ReadAllBytes(file));
    }

    public static SharpNav.NavMesh ExportNavMesh()
    {
        var triangles = GetColliderTriangle();
        var navMesh = SharpNav.Serialization.CreateNavMesh(triangles, SharpNav.NavMeshGenerationSettings.Default);
        return navMesh;
    }   

    static Dictionary<System.Type, System.Action<Collider, List<Triangle3>>> ColliderToMesh = new Dictionary<System.Type, System.Action<Collider, List<Triangle3>>>();
    static void Reg<T>(System.Action<T, List<Triangle3>> fun) where T : Collider
    {
        ColliderToMesh.Add(typeof(T), (Collider c, List<Triangle3> triangle) => { fun((T)c, triangle); });
    }

    // 得到所有阻挡的三角形列表
    static List<Triangle3> GetColliderTriangle()
    {
        List<Triangle3> triangles = new List<Triangle3>();

        HashSet<System.Type> NoTypes = new HashSet<System.Type>();
        Collider[] colliders = Object.FindObjectsOfType<Collider>();
        foreach (Collider collider in colliders)
        {
            int layer = collider.gameObject.layer;
            if (!(Layer.terrain == layer || Layer.wall == layer))
                continue;

            System.Action<Collider, List<Triangle3>> fun;
            if (ColliderToMesh.TryGetValue(collider.GetType(), out fun))
            {
                fun(collider, triangles);
            }
            else
            {
                NoTypes.Add(collider.GetType());
            }
        }

        if (NoTypes.Count != 0)
        {
            Debug.LogFormat("Count:{0}", NoTypes.Count);
            foreach (var type in NoTypes)
            {
                Debug.LogFormat("type:{0} not find!", type.Name);
            }
        }

        if (triangles.Count == 0)
        {
            Debug.LogErrorFormat("triangles.Count == 0没有地形数据");
        }

        return triangles;
    }
}
#endif