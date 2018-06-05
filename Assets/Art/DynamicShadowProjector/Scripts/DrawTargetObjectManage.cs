//#define USE_DRAW_POOL

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DynamicShadowProjector;
using xys;




public class DrawTargetObjectManage : MonoBehaviour
{
    #region 静态接口
    static DrawTargetObjectManage instance;
    static public void Create()
    {
        ////实时阴影预制体创建 
        //if (!ComSetting.IsIOS())
        //    return;
        XYJObjectPool.LoadPrefab("DynamicShadowProjector", OnFinishLoad);
    }

    static public void AddShadow(Transform trans)
    {
        if (instance == null)
            return;
        instance.AddDrawTarget(trans);
    }

    static public void RemoveShadow(Transform trans)
    {
        if (instance == null)
            return;
        instance.RemoveDrawTarget(trans);
    }

    static public void ShowShadow(Transform trans, bool isShow)
    {
        if (instance == null)
            return;
        instance.ShowObject(trans, isShow);
    }

    static void OnFinishLoad(GameObject go, object para)
    {
        if (App.my.localPlayer != null)
            instance.SetFollow(App.my.localPlayer.root);

        foreach (var r in App.my.sceneMgr.GetObjs())
        {
            if(r.Value.battle.actor.m_partManager.IsNormal)
                instance.AddDrawTarget(r.Value.root);
        }

    }
    #endregion

    Transform m_followTrans;

    List<DrawTargetObject> m_drawObjects;
    DrawTargetObject m_defaultDrawObject;
    Camera m_camera;
    ShadowTextureRenderer m_shadowTextureRenderer;

    Transform selfTrans;
    GameObject m_sceneLight;
    Vector3 m_posOff;

    //场景唯一平行光名称
    const string LIGHTNAME = "Directional Light_jiaose";
    const float DEFAULTANGLE = 30F;
    void Awake()
    {
        if (instance != null)
        {
            GameObject.Destroy(this.gameObject);
            return;
        }

        selfTrans = transform;

        m_camera = ArtWrap.mainCamera;
        m_shadowTextureRenderer = GetComponent<ShadowTextureRenderer>();
        if (m_shadowTextureRenderer != null)
            m_shadowTextureRenderer.camerasForViewClipTest = new Camera[] { m_camera };

        //添加默认的，因为要根据默认的设置参数
        m_defaultDrawObject = GetComponent<DrawTargetObject>();
        m_defaultDrawObject.enabled = false;
        m_drawObjects = new List<DrawTargetObject>();
        m_drawObjects.Add(m_defaultDrawObject);

        //设置阴影跟随平行光
        SetLight();

        instance = this;
    }

    void OnDestroy()
    {
        instance = null;
    }

    void Update()
    {
        if (m_followTrans != null)
        {
            //设置坐标偏移
            m_posOff = -this.transform.forward * 20;
            selfTrans.parent.position = m_followTrans.position + m_posOff;
        }


        for (int i = m_drawObjects.Count - 1; i >= 0; i--)
        {
            if (m_drawObjects[i].target == null)
            {
                m_drawObjects.RemoveAt(i);
                return;
            }
        }
    }

    void SetLight()
    {
        //设置默认值
        selfTrans.localPosition = Vector3.zero;
        selfTrans.localRotation = Quaternion.identity;
        selfTrans.parent.rotation = Quaternion.Euler(DEFAULTANGLE, 0, 0);

        //根据场景平行光，设置阴影的方向
        GameObject MapScene = GameObject.Find("MapScene");
        if (MapScene == null)
            return;
        Light[] lights = MapScene.GetComponentsInChildren<Light>();
        int lightCount = 0;
        for (int i = 0; i < lights.Length; ++i)
        {
            if (lights[i].gameObject.name.Equals(LIGHTNAME))
            {
                lightCount++;
                m_sceneLight = lights[i].gameObject;
            }
        }

        if (lightCount > 1)
            Debuger.LogError("场景中有多个角色平行光");

        //设置灯光方向
        if (m_sceneLight != null)
            m_sceneLight.AddComponentIfNoExist<TargetDirection>();
    }

    void ShowObject(Transform trans, bool isShow)
    {
#if USE_DRAW_POOL
        //关闭影子
        for (int i = 0; i < m_drawObjects.Count; i++)
        {
            if (m_drawObjects[i].target == m_role.m_go.transform)
            {
                m_drawObjects[i].enabled = isShow;
            }
        }
#else
        if (isShow)
            AddDrawTarget(trans);
        else
            RemoveDrawTarget(trans);
#endif
    }

    void AddDrawTarget(Transform trans)
    {
        //如果已经添加过了，就不要再添加了
        for (int i = 0; i < m_drawObjects.Count; i++)
        {
            if (m_drawObjects[i].target == trans)
            {
                m_drawObjects[i].enabled = true;
                m_drawObjects[i].target = trans;
                Debuger.LogError("重复添加阴影");
                return;
            }
        }

        DrawTargetObject drawTarget = null;
#if USE_DRAW_POOL
        for (int i = 0; i < m_drawObjects.Count; i++)
        {
            if (s_instce.m_drawObjects[i].target == null)
            {
                drawTarget = m_drawObjects[i];
                m_drawObjects[i].enabled = true;
                break;
            }
        }
#endif

        //重新创建一个新的
        if (drawTarget == null)
        {
            drawTarget = gameObject.AddComponent<DrawTargetObject>();
            drawTarget.followTarget = false;
            drawTarget.shadowShader = m_defaultDrawObject.shadowShader;
            drawTarget.replacementShaders = m_defaultDrawObject.replacementShaders;
            drawTarget.layerMask = m_defaultDrawObject.layerMask;
            m_drawObjects.Add(drawTarget);
        }
        drawTarget.target = trans;
    }

    void SetFollow(Transform trans)
    {
        m_followTrans = trans;
    }


    void RemoveDrawTarget(Transform trans)
    {
        for (int i = m_drawObjects.Count - 1; i >= 0; i--)
        {
            if (m_drawObjects[i].target == trans)
            {
#if USE_DRAW_POOL
                s_instance.m_drawObjects[i].target = null;
                s_instance.m_drawObjects[i].enabled = false;
                s_instance.m_drawObjects[i].Clear();
#else
                Object.Destroy(m_drawObjects[i]);
                m_drawObjects.RemoveAt(i);
#endif
            }
        }
    }
}
