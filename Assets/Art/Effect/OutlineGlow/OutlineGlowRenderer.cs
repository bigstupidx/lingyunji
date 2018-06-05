using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[AddComponentMenu("Chickenlord/Outline Glow Renderer")]
public class OutlineGlowRenderer : MonoBehaviour
{
    public GameObject m_go;
    public bool IncludeChildMeshes = true;
    public Color OutlineColor = Color.cyan;
    public float ObjectOutlineStrength = 3f;

    private bool ICMT = false;
    private int myID = -1;
    private int previousLayer;
    public int childCounter = 0;

    public float scaleValue = 1f;

    //影响的对象
    GameObject effectGo
    {
        get 
        {
            if (m_go == null)
                return this.gameObject;
            else
                return m_go;
        }
    }

    Transform trans
    {
        get
        {
            if (m_go == null)
                return this.gameObject.transform;
            else
                return m_go.transform;
        }
    }

    class Data
    {
        public Data(int first, GameObject second)
        {
            this.first = first;
            this.second = second;
        }

        public int first;
        public GameObject second;
    }

    private List<Data> childLayers;
	// Update is called once per frame
	void Update () 
    {
        //Grabbing id here, as it doesn't work in Start without modifying the script execution order. Shouldn't cost too much performance.
        if (myID == -1)
        {
            OutlineGlowEffectScript es = OutlineGlowEffectScript.Instance;
            if (es != null)
                myID = es.AddRenderer(this);
        }
	}

    void OnEnable()
    {
        if (this.myID == -1)
        {
            try
            {
                myID = OutlineGlowEffectScript.Instance.AddRenderer(this);
            }
            catch
            {
            }
        }
        else
        {
            Debug.LogWarning("OutlineGlowRenderer enabled, although id is already/still assigned. Shouldn't happen.");
        }
    }

    void OnDisable()
    {
        if (this.myID != -1)
        {
            OutlineGlowEffectScript.Instance.RemoveRenderer(this.myID);
            this.myID = -1;
            this.childLayers = null;
        }
    }

    Vector3 oldScaleValue = Vector3.one;
    //避免同时多个描边相互影响
    bool m_hadSetLayer = false;
    public void SetLayer(int layer)
    {
        if (!m_hadSetLayer)
        {
            m_hadSetLayer = true;
            previousLayer = effectGo.layer;
            oldScaleValue = trans.localScale;
        }

        ICMT = this.IncludeChildMeshes;
        trans.localScale = oldScaleValue * scaleValue;
        if (this.enabled)
        {
            if (ICMT)
            {
                if (childLayers == null)
                {
                    childLayers = new List<Data>();
                }
                else
                {
                    childLayers.Clear();
                }

                CollectChild(effectGo);

                SetLayerRecursive(layer);
            }
            else
            {
                effectGo.layer = layer;
            }
        }
    }

    public void ResetLayer()
    {
        if (!m_hadSetLayer)
            return;

        m_hadSetLayer = false;
        trans.localScale = oldScaleValue;
        childCounter = 0;
        effectGo.layer = previousLayer;
        if (ICMT)
        {
            ResetLayerRecursive();
        }
    }

    // 收集子结点
    void CollectChild(GameObject obj)
    {
        //隐藏的就不需要加了
        if (!obj.activeInHierarchy)
            return;

        SkinnedMeshRenderer sk = obj.GetComponent<SkinnedMeshRenderer>();
        if (sk == null)
        {
            childLayers.Add(new Data(obj.layer, obj));
        }
        else
        {
            SyncSkinned.Begin(sk, this);
        }

        Transform tran = obj.transform;
        for (int i = 0; i < tran.childCount; ++i)
            CollectChild(tran.GetChild(i).gameObject);
    }

    private void SetLayerRecursive(int layer)
    {
        for (int i = 0; i < childLayers.Count; ++i)
            childLayers[i].second.layer = layer;
    }

    private void ResetLayerRecursive()
    {
        for (int i = 0; i < childLayers.Count; ++i)
            childLayers[i].second.layer = childLayers[i].first;
    }

}
