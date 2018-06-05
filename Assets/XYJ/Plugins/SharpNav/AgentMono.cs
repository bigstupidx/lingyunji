#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NavMeshQuery = SharpNav.NavMeshQuery;
using NavPoint = SharpNav.Pathfinding.NavPoint;
using SVector3 = SharpNav.Geometry.Vector3;
using SharpNav.Pathfinding;

public class AgentMono : MonoBehaviour
{
    [SerializeField]
    GameObject dst;

    private void Start()
    {

    }

    Vector3 targetPosition;

    List<Vector3> pathPoint = new List<Vector3>();
    int startIndex = 1;
    bool isEnd = false;

    Vector3 CurrentTargetPosition { get { return pathPoint[startIndex]; } }

    void Update()
    {
        if (dst == null)
            return;

        if (!isEnd)
        {
            if (pathPoint.Count < 2)
            {
                isEnd = true;
                return;
            }

            float dt = Time.deltaTime; // 间隔
            float speed = 6f; // 速度
            Vector3 dstPos = CurrentTargetPosition;
            Vector3 srcPos = transform.position;
            while (true)
            {
                float dis = (dstPos - srcPos).magnitude;
                if (dis < (speed * dt))
                {
                    // 当前移动的距离超过和下一个点的距离
                    ++startIndex;
                    if (startIndex >= pathPoint.Count)
                    {
                        // 已经到达终于了
                        dstPos = pathPoint[pathPoint.Count - 1];
                        isEnd = true;
                        break;
                    }
                    else
                    {
                        srcPos = dstPos;
                        dstPos = CurrentTargetPosition;
                        dt -= dis / speed;
                    }
                }
                else
                {
                    dstPos = srcPos + (dstPos - srcPos).normalized * (speed * dt);
                    break;
                }
            }

            transform.position = dstPos;
        }

        if ((targetPosition - dst.transform.position).magnitude > 0.1f)
        {
            targetPosition = dst.transform.position;
            pathPoint.Clear();
            Nav.FindPath(transform.position, targetPosition, pathPoint, 1024);
            startIndex = 1;
            isEnd = false;
        }
    }
}
#endif