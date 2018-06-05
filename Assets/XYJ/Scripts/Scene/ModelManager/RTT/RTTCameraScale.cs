using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 控制摄像机缩放
/// </summary>
public class RTTCameraScale : MonoBehaviour {

    // 可缩放的状态
    List<ScaleState> m_states = new List<ScaleState>();
    int m_curState = 0;

    public Camera m_cam;// 镜头对象
    Transform m_camObject;

    public float m_sampleHeight = 1.48f;
    public float m_speed = 2;// 切换速度

    // 缩放配置元素
    public List<ScaleElement> m_elements = new List<ScaleElement>();
    [HideInInspector]
    public bool m_hasInitScale = false;

    // 相机的目标位置和fov
    float m_targetFOV = 60;
    Vector3 m_targetPosition = Vector3.zero;

    // 触摸镜头缩放
    float m_curTouchDis = 0.0f;
    float m_lastTouchDis = 0.0f;
    
    /// <summary>
    /// 设置脸部对象
    /// </summary>
    /// <param name="rootObject"></param>
    /// <param name="faceObj"></param>
    public void SetFaceObject(Transform rootObject, Transform faceObj)
    {
        float h = faceObj.transform.position.y - rootObject.transform.position.y;
        float rate = h / m_sampleHeight;
        Debuger.LogWarning("模型脖子高度：" + h);
        m_states.Clear();
        for (int i=0; i<m_elements.Count; ++i)
        {
            ScaleState item = new ScaleState ();
            item.m_position = new Vector3(0, h + m_elements[i].m_height, m_elements[i].m_distance * rate);
            item.m_fov = m_elements[i].m_fov;
            m_states.Add(item);
        }
        m_hasInitScale = true;
        m_curState = 0;
        SetState(0);
    }

    public void SetCamera(Camera cam)
    {
        m_cam = cam;
        m_camObject = m_cam.transform;

        m_targetPosition = m_camObject.localPosition;
        m_targetFOV = m_cam.fieldOfView;
    }

    public void Reset()
    {
        m_hasInitScale = false;
    }

    /// <summary>
    /// 设置目标状态数据
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="fov"></param>
    /// <param name="immediately"></param>
    public void SetTargetData(Vector3 pos, float fov, bool immediately)
    {
        m_targetPosition = pos;
        m_targetFOV = fov;
        if (immediately)
        {
            m_camObject.localPosition = pos;
            m_cam.fieldOfView = fov;
        }
    }

    /// <summary>
    /// 设置镜头状态
    /// </summary>
    /// <param name="index"></param>
    /// <param name="immediately"></param>
    public void SetState(int index, bool immediately=false)
    {
        if (m_states.Count == 0)
            return;
        m_curState = index;
        SetTargetData(m_states[index].m_position, m_states[index].m_fov, immediately);
    }

    public void AddState()
    {
        if (m_curState >= m_states.Count - 1)
        {
            m_curState = m_states.Count - 1;
            return;
        }
        SetState(m_curState+1);
    }

    public void ReduceState()
    {
        if (m_curState <= 0)
        {
            m_curState = 0;
            return;
        }
        SetState(m_curState-1);
    }
    

    // Use this for initialization
    void Start()
    {

        SetCamera(m_cam);
	}
	
	// Update is called once per frame
	void Update () {

        if (m_cam == null)
            return;

        // 鼠标滚轮缩放镜头
        if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
        {
            float mouseValue = Input.GetAxis("Mouse ScrollWheel");
            if (mouseValue != 0)
            {
                if (mouseValue < 0f)
                    ReduceState();
                else
                    AddState();
            }
        }
        else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount == 2)
            {
                if (Input.GetTouch(1).phase == TouchPhase.Began)
                {
                    m_curTouchDis = m_lastTouchDis = Vector2.Distance(Input.GetTouch(0).position, Input.GetTouch(1).position);
                }

                if (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved)
                {
                    Touch touch1 = Input.GetTouch(0);
                    Touch touch2 = Input.GetTouch(1);

                    m_curTouchDis = Vector2.Distance(touch1.position, touch2.position);
                    float moveDis = m_curTouchDis - m_lastTouchDis;
                    if (moveDis > 0f)
                    {
                        if (moveDis>100f)
                        {
                            m_lastTouchDis = m_curTouchDis;
                            AddState();
                        }
                    }
                    else if (moveDis < 0f)
                    {
                        if (moveDis<-100f)
                        {
                            m_lastTouchDis = m_curTouchDis;
                            ReduceState();
                        }
                    }
                }
            }
        }

        // 处理镜头缩放
        if (m_targetFOV != m_cam.fieldOfView)
        {
            m_cam.fieldOfView = m_targetFOV; //Mathf.Lerp(m_cam.fieldOfView, m_targetFOV, m_speed * Time.deltaTime);
        }

        if (m_targetPosition != m_camObject.localPosition)
        {
            m_camObject.localPosition = Vector3.LerpUnclamped(m_camObject.localPosition, m_targetPosition, m_speed * Time.deltaTime);
        }
	}

    [System.Serializable]
    public class ScaleState
    {
        public Vector3 m_position = Vector3.zero;
        public float m_fov = 60;
    }

    [System.Serializable]
    public class ScaleElement
    {
        public float m_distance;

        public float m_height;

        public float m_fov;
    }

}
