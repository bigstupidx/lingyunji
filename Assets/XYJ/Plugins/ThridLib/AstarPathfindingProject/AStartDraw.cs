using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class AStartDraw : MonoBehaviour
{
    [SerializeField]
    Transform startPos;
    [SerializeField]
    Transform endPos;
    [SerializeField]
    bool isSet = false;

    [SerializeField]
    List<Vector3> points;

    private void OnDrawGizmos()
    {
        if (points == null || points.Count == 0)
            return;

        Gizmos.DrawLine(startPos.position, points[0]);
        for (int i = 1; i < points.Count; ++i)
            Gizmos.DrawLine(points[i - 1], points[i]);
        Gizmos.DrawLine(points[points.Count - 1], endPos.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSet)
            return;
        isSet = false;
        AstarPath.StartPath(startPos.position, endPos.position, (p) => { points = p; });
    }
}
