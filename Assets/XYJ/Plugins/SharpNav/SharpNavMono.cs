#if UNITY_EDITOR
using SharpNav.Pathfinding;
using System.Collections.Generic;
using NavMeshQuery = SharpNav.NavMeshQuery;
using SVector3 = SharpNav.Geometry.Vector3;

public static class Nav
{
    static NavMeshQuery navMeshQuery_;
    public static NavMeshQuery navMeshQuery
    {
        get
        {
            if (navMeshQuery_ == null)
            {
                navMeshQuery_ = new NavMeshQuery(SharpNavAgent.CreateNavMeshByFile(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name), 2048);
                //crowd_ = new SharpNav.Crowds.Crowd(2048, 0.6f, navMeshQuery_.NavMesh);
            }

            return navMeshQuery_;
        }
    }

    public static void FindPath(UnityEngine.Vector3 start, UnityEngine.Vector3 end, List<UnityEngine.Vector3> smoothPath, int maxNode)
    {
        var startPt = navMeshQuery.FindNearestPoly(
            new SVector3(start.x, start.y, start.z),
            new SVector3(0.2f, 1f, 0.2f));

        var endPt = navMeshQuery.FindNearestPoly(
            new SVector3(end.x, end.y, end.z),
            new SVector3(0.2f, 1f, 0.2f));


    }
}

public class SharpNavMono : UnityEngine.MonoBehaviour
{
    [UnityEngine.SerializeField]
    bool isShow = false;

    SharpNavDrawer drawer;

    private void LateUpdate()
    {
        //Nav.Update();
        if (isShow)
        {
            if (drawer == null)
                drawer = gameObject.GetOrAddComponent<SharpNavDrawer>();
            drawer.enabled = true;
            drawer.SetNavMesh(Nav.navMeshQuery.NavMesh, SharpNav.NavMeshGenerationSettings.Default.VertsPerPoly);
        }
        else if (drawer != null)
        {
            drawer.enabled = false;
        }
        
    }
}
#endif