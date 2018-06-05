using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
[ExecuteInEditMode]
public class ModelPartTempObject : MonoBehaviour {

    public ModelPartManage m_manager = new ModelPartManage();
    
#if SCENE_DEBUG

    // Use this for initialization
    void Start () {

        UpdatePosition();
        
        // 处理父子对象
        if (Application.isPlaying)
        {
            if (m_manager.m_modelObject != null)
                this.transform.parent = m_manager.m_modelObject.transform;
        }
        else
        {
            this.transform.parent = ModelPartSetting1.TempModelsRoot;
        }
    }
	
	// Update is called once per frame
	void Update () {

        UpdatePosition();

        CheckModelObject();
    }

    void CheckModelObject()
    {
        if (m_manager.m_modelObject == null || m_manager.m_setting.RootObject == null)
        {
            GameObject.DestroyImmediate(this.gameObject);
            return;
        }

        if (m_manager.m_setting.RootObject != null && m_manager.m_setting.RootObject != this.transform)
        {
            GameObject.DestroyImmediate(this.gameObject);
        }
    }

    void UpdatePosition()
    {
        if (m_manager.m_modelObject != null)
        {
            this.transform.position = m_manager.m_modelObject.transform.position;
            this.transform.rotation = m_manager.m_modelObject.transform.rotation;
        }
    }

#endif

    // 传入加载好的模型
    public static void Create(GameObject go)
    {
        if (go==null || go.GetComponent<ModelPartSetting1>() == null)
            return;

        ModelPartTempObject tmpObject = go.GetComponent<ModelPartTempObject>();
        if (tmpObject == null)
            tmpObject = go.AddComponent<ModelPartTempObject>();

        tmpObject.m_manager.InitSettingModel(go);
    }



}
