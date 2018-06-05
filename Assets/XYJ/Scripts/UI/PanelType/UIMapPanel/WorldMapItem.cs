using UnityEngine;
using System.Collections;

public class WorldMapItem : MonoBehaviour
{
    public int m_sceneId;
    public GameObject m_locateArrow;

    void Awake()
    {
        if(null == m_locateArrow)
        {
            m_locateArrow = transform.Find("LocateArrow").gameObject;
        }
    }
}
