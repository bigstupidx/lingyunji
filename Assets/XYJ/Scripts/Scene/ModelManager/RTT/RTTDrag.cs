using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RTTDrag : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    Transform m_dragObject;

    public float m_rotateSmoothTime = 20f;

    public void SetDragObject(Transform go)
    {
        m_dragObject = go;
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        //Debuger.Log("OnBeginDrag");
    }

    //拖动中
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (m_dragObject != null)
        {
            float rotY = 0.0f;
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                rotY = Input.GetAxis("Mouse X");
            }
            else if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                if (Input.touchCount == 1)
                {
                    rotY = Input.GetAxis("Mouse X");
                }
            }
            if (rotY != 0.0f)
            {
                m_dragObject.localRotation = m_dragObject.localRotation * Quaternion.Euler(0f, -rotY * m_rotateSmoothTime, 0f);
            }
        }
    }

    //结束拖动
    public virtual void OnEndDrag(PointerEventData eventData)
    {
        //Debuger.Log("OnEndDrag");
    }

}
