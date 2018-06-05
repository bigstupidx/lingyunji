#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

/// <summary>
/// 关卡编辑器  路径
/// </summary>
[ExecuteInEditMode]
public class LevelDesignPath : MonoBehaviour
{
    public LevelDesignConfig.LevelPathData m_data;
    [SerializeField]
    public Color color = new Color(1, 0, 1, 0.5f);
    [SerializeField]
    public float radius = 0.1f;

    void OnEnable()
    {
        if (null == m_data)
            m_data = new LevelDesignConfig.LevelPathData();
    }

    public void SetData(LevelDesignConfig.LevelPathData data)
    {
        m_data = data;
    }

    public void AddPathToSet()
    {
        GameObject child = LevelDesignTool.CreateNode(transform);
        Selection.activeGameObject = gameObject;

        child.gameObject.name = transform.childCount.ToString();

        DestroyImmediate(child.GetComponent<Renderer>());
        DestroyImmediate(child.GetComponent<MeshFilter>());
        DestroyImmediate(child.GetComponent<SphereCollider>());

        //需要给对象增加对应的脚本
        LevelDesignPathItem item = child.GetOrAddComponent<LevelDesignPathItem>();
        item.m_event = "";
        item.m_stayTime = 0;
    }

    public void SetDefaultSpeed(float speed)
    {
        m_data.m_speed = speed;
    }

    public void SetEvent(GameObject go, string eventName)
    {
        go.GetComponent<LevelDesignPathItem>().m_event = eventName;
    }

    public void SetStayTime(GameObject go, float stayTime)
    {
        go.GetComponent<LevelDesignPathItem>().m_stayTime = stayTime;
    }

    public void SetSpeed(GameObject go, float speed)
    {
        go.GetComponent<LevelDesignPathItem>().m_speed = speed;
    }

    void OnDrawGizmos()
    {
        if (transform.childCount <= 0)
            return;

        if (!ShowLine())
        {
            return;
        }

        Gizmos.color = color;
        Gizmos.DrawWireCube(transform.GetChild(0).transform.position, Vector3.one * radius);
        Gizmos.DrawWireCube(transform.GetChild(transform.childCount - 1).transform.position, Vector3.one * radius);

        for (int i = 1; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawWireSphere(transform.GetChild(i).transform.position, radius);
        }

        for (int i = 1; i <= transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i - 1).transform.position, transform.GetChild(i).transform.position);
        }

        for (int i = 1; i < transform.childCount; i++)
        {
            Vector2 pos = HandleUtility.WorldToGUIPoint(transform.GetChild(i).transform.position);
            GUI.Box(new Rect(pos.x, pos.y, 100, 30), "Point:" + transform.GetChild(i).gameObject.name);
        }
    }

    bool ShowLine()
    {
        bool chooseChild = false;
        if (Selection.activeObject is GameObject)
        {
            GameObject go = Selection.activeObject as GameObject;
            if (go.transform.parent == transform)
            {
                chooseChild = true;
            }
        }
        if (Selection.activeObject == gameObject || chooseChild)
        {
            return true;
        }
        return false;
    }

    void Update()
    {
        CollectData();
    }

    public void CollectData()
    {
        m_data.m_dirs.Clear();
        m_data.m_events.Clear();
        m_data.m_names.Clear();
        m_data.m_postions.Clear();
        m_data.m_stayTimes.Clear();
        m_data.m_speeds.Clear();

        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            m_data.m_postions.Add(child.position);
            m_data.m_dirs.Add(child.eulerAngles);
            m_data.m_names.Add(child.name);

            //收集事件和延迟
            m_data.m_events.Add(child.GetComponent<LevelDesignPathItem>().m_event);
            m_data.m_stayTimes.Add(child.GetComponent<LevelDesignPathItem>().m_stayTime);
            m_data.m_speeds.Add(child.GetComponent<LevelDesignPathItem>().m_speed);
        }
    }
}
#endif