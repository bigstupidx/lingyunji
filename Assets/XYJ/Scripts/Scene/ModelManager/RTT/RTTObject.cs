using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// RTT站台对象，对象基本包含下面几个东西：
/// 1.Camera
/// 2.RTT Root Object
/// 3.Light（可选）
/// </summary>
public class RTTObject : MonoBehaviour
{
    // 自身带有Render Camera
    public Camera m_renderCamera;
    protected RenderTexture m_renderTexture;

    //外部RawImage
    [HideInInspector]
    public RectTransform m_uiObject;
    protected RawImage m_uiImage;
    public int m_imageWidth = 512;
    public int m_imageHeight = 512;

    // rtt root object
    public Transform m_objectRoot;// 需预制体指定对象
    protected Quaternion m_rootOgrRotate;// 默认转角
    protected string m_objectName;
    protected System.Action<GameObject> m_loadModelEndEvent;

    // Animation

    // auto rotate
    protected bool m_IsAutoRotate = false;
    protected float m_rotateSpeed = 20f;

    // drag
    protected bool m_canDrag = false;
    protected RTTDrag m_drag;

    public virtual void Init(RectTransform uiObject, bool canDrag = false)
    {
        SetRenderLayer();
        SetRenderTexture(uiObject);
        SetModelDrag(canDrag);
    }

    /// <summary>
    /// 加载对象
    /// </summary>
    /// <param name="modelName"></param>
    public virtual void LoadModel(string modelName, System.Action<GameObject> loadEnd)
    {
        SetRenderActive(false);
        m_objectName = modelName;
        m_loadModelEndEvent = loadEnd;
    }

    /// <summary>
    /// 销毁对象
    /// </summary>
    public virtual void DestroyModel()
    {
        SetRenderActive(false);
    }

    /// <summary>
    /// 销毁对象，render texture等
    /// </summary>
    public virtual void Release()
    {
        DestroyModel();

        if (m_drag != null)
        {
            GameObject.Destroy(m_drag);
        }

        if (m_uiImage != null)
        {
            m_uiImage.texture = null;
            m_uiImage = null;
        }

        if (m_renderTexture != null)
        {
            m_renderCamera.targetTexture = null;
            RenderTexture.ReleaseTemporary(m_renderTexture);
            m_renderTexture = null;
        }
    }

    #region Animation Methods

    // 播放动画
    protected bool m_isPlayIdle = false;
    protected float m_aniTimeLen = 0.0f;
    protected float m_lastPlayTime = 0.0f;
    protected string m_lastAnim = string.Empty;

    protected string m_idleAnim;
    protected List<string> m_relaxAnims = new List<string>();// once

    /// <summary>
    /// 根据角色类型设置代码动画和休闲动画
    /// </summary>
    /// <param name="idleName"></param>
    /// <param name="relaxs"></param>
    public void SetAnims(string idleName, string[] relaxs = null)
    {
        m_idleAnim = idleName;
        m_relaxAnims.Clear();
        if (relaxs!=null && relaxs.Length>0)
        {
            for (int i=0; i<relaxs.Length; ++i)
            {
                if (!string.IsNullOrEmpty(relaxs[i]))
                    m_relaxAnims.Add(relaxs[i]);
            }
        }

        // 播放默认待机动作
        if (string.IsNullOrEmpty(m_idleAnim))
        {
            m_isPlayIdle = false;
            m_aniTimeLen = 0.0f;
            m_lastPlayTime = 0.0f;
            m_lastAnim = string.Empty;
        }
        else
        {
            m_isPlayIdle = true;
            PlayAnim(m_idleAnim, 1.0f, true);
            m_lastAnim = m_idleAnim;
            m_aniTimeLen = Random.Range(3.0f, 6.0f);
            m_lastPlayTime = Time.timeSinceLevelLoad;
        }
    }

    // 播放动画
    public virtual void PlayAnim(string name, float speed = 1.0f, bool isLoop = true)
    {

    }

    public virtual float GetAnimLen (string name)
    {
        return 0.0f;
    }

    /// <summary>
    /// 休闲动作的更新切换
    /// </summary>
    protected virtual void UdatePlayAnims()
    {
        if (string.IsNullOrEmpty(m_idleAnim) || m_relaxAnims.Count == 0)
            return;
        
        if (Time.timeSinceLevelLoad >= (m_lastPlayTime + m_aniTimeLen))
        {
            string animName = null;
            if (m_isPlayIdle)
            {
                animName = m_idleAnim;
                m_aniTimeLen = Random.Range(3.0f, 6.0f);
            }
            else
            {
                if (m_relaxAnims.Count == 1)
                    animName = m_relaxAnims[0];
                else
                    animName = m_relaxAnims[Random.Range(0, m_relaxAnims.Count)];
                m_aniTimeLen = GetAnimLen(name);
            }
            if (m_aniTimeLen == 0)
            {
                // 没有找到动画，就等待一秒
                m_aniTimeLen = 1.0f;
            }
            else
            {
                if (!m_isPlayIdle || animName != m_lastAnim)
                    PlayAnim(animName, 1.0f, m_isPlayIdle);
            }
            m_isPlayIdle = !m_isPlayIdle;
            m_lastPlayTime = Time.timeSinceLevelLoad;
            
        }
    }

    #endregion

    // Internal Methods

    /// <summary>
    /// 用来初始化Image和render texture
    /// </summary>
    /// <param name="uiObject"></param>
    protected void SetRenderTexture(RectTransform uiObject)
    {
        m_uiObject = uiObject;

        ResetRenderTexture((int)uiObject.rect.width, (int)uiObject.rect.height);
    }

    /// <summary>
    /// 重设render texture
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void ResetRenderTexture(int width, int height)
    {
        m_imageWidth = width;
        m_imageHeight = height;

        // Camera
        if (m_renderCamera == null)
            m_renderCamera = this.GetComponentInChildren<Camera>();

        if (m_renderTexture != null)
        {
            m_renderCamera.targetTexture = null;
            RenderTexture.ReleaseTemporary(m_renderTexture);
            m_renderTexture = null;
        }

        // Set render texture
        if (m_renderTexture == null)
        {
            m_renderTexture = RenderTexture.GetTemporary(m_imageWidth, m_imageHeight, 14, RenderTextureFormat.ARGB32);
            m_renderTexture.Create();
            m_renderCamera.targetTexture = m_renderTexture;
        }

        // UI Image
        if (m_uiImage == null)
            m_uiImage = m_uiObject.GetComponent<RawImage>();
        if (m_uiImage == null)
            m_uiImage = m_uiObject.gameObject.AddComponent<RawImage>();

        if (m_uiImage != null)
        {
            m_uiImage.texture = m_renderTexture;
            m_uiImage.raycastTarget = m_canDrag;

            // 检查
            if (m_uiImage.material == null)
                Debuger.LogError("RawImage没有指定材质，建议使用：RttParticle");
        }
    }

    /// <summary>
    /// Set Render Layer
    /// </summary>
    protected void SetRenderLayer ()
    {
        // 处理渲染模型
        SetObjectLayer(this.gameObject);

        // 处理摄像头
        if (m_renderCamera == null)
            m_renderCamera = this.GetComponentInChildren<Camera>();
        m_renderCamera.cullingMask = (1 << ComLayer.Layer_RTT);

        // 处理灯光
        Light[] lights = this.GetComponentsInChildren<Light>();
        if (lights!=null && lights.Length>0)
        {
            for (int i = 0; i < lights.Length; ++i)
            {
                lights[i].cullingMask = (1 << ComLayer.Layer_RTT);
            }
        }
    }

    /// <summary>
    /// 渲染激活
    /// </summary>
    /// <param name="active"></param>
    public void SetRenderActive(bool active)
    {
        if (m_uiObject != null)
        {
            m_uiObject.gameObject.SetActive(active);
        }
        //if (m_objectRoot != null)
        //{
        //    m_objectRoot.gameObject.SetActive(active);
        //}
        if (m_renderCamera != null)
        {
            m_renderCamera.gameObject.SetActive(active);
        }
    }

    public bool IsRenderActive()
    {
        if (m_uiObject!=null)
        {
            return m_uiObject.gameObject.activeSelf;
        }
        else if (m_renderCamera != null)
        {
            return m_renderCamera.gameObject.activeSelf;
        }
        else
            return false;
    }

    /// <summary>
    /// 设置rtt整体对象位置偏移
    /// </summary>
    /// <param name="pos"></param>
    public void SetPosition(Vector3 pos)
    {
        this.transform.position = pos;
    }

    /// <summary>
    ///  把对象设置到根目录
    /// </summary>
    /// <param name="go"></param>
    public void SetObjectParent(Transform go)
    {
        m_objectRoot.localRotation = m_rootOgrRotate;// 每次执行时候把根对象转角恢复
        go.parent = m_objectRoot;
        go.localPosition = Vector3.zero;
        go.localRotation = Quaternion.identity;
        //go.localScale = Vector3.one;
    }

    /// <summary>
    /// 设置展台对象的默认旋转角度
    /// </summary>
    /// <param name="rotate"></param>
    public void SetObjectRotation(Vector3 rotate)
    {
        m_objectRoot.localEulerAngles = rotate;
        m_rootOgrRotate = m_objectRoot.localRotation;// 每次执行时候把根对象转角恢复
    }

    // ============根据模型需要调整摄像机的位置和fov=============

    /// <summary>
    /// 直接调整摄像机
    /// </summary>
    /// <param name="pos"></param>
    public void SetCameraPositon(Vector3 pos)
    {
        if (m_renderCamera!=null)
            m_renderCamera.transform.localPosition = pos;
    }

    /// <summary>
    /// 直接设置相机fov
    /// </summary>
    /// <param name="fov"></param>
    public void SetCameraFov (float fov)
    {
        if (m_renderCamera != null)
            m_renderCamera.fieldOfView = fov;

       

    }
    public void SetCameraClipPlane(float far, float near)
    {
        if (m_renderCamera != null)
        {
            m_renderCamera.farClipPlane = far;
            m_renderCamera.nearClipPlane = near;
        }
            
    }

    /// <summary>
    /// Set Object render layer
    /// </summary>
    /// <param name="go"></param>
    public void SetObjectLayer(GameObject go)
    {
        // 处理渲染模型
        ComLayer.SetRenderLayer(go, ComLayer.Layer_RTT);
    }

    protected void SetModelDrag(bool active)
    {
        m_canDrag = active;
        if (m_uiImage!=null)
        {
            m_uiImage.raycastTarget = m_canDrag;

            m_drag = m_uiObject.GetComponent<RTTDrag>();
            if (m_drag == null)
                m_drag = m_uiObject.gameObject.AddComponent<RTTDrag>();

            if (m_drag != null)
                m_drag.SetDragObject(m_objectRoot);
        }
    }

    protected void SetModelAutoRotate(bool active)
    {
        m_IsAutoRotate = active;
    }

    // Use this for initialization
    protected virtual void Start () {

        if (m_objectRoot != null)
            m_rootOgrRotate = m_objectRoot.localRotation;
        else
        {
            Debuger.LogError("m_objectRoot不能为空");
        }
    }
	
	// Update is called once per frame
    protected virtual void Update()
    {

        if (m_IsAutoRotate && m_objectRoot != null)
        {
            m_objectRoot.localRotation = m_objectRoot.localRotation * Quaternion.Euler(0f, m_rotateSpeed * Time.deltaTime, 0f);
        }

        UdatePlayAnims();
    }

}
