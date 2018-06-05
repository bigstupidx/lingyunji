using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine.UI;



/// <summary>
/// 外部可以使用TransformWrap来代替Transform，这样角色改变模型就可以对引用者透明
/// </summary>
public class TransformWrap
{
    public TransformWrap(Transform t)
    {
        Set(t);
    }

    Transform m_tran;

    //慎用，可能变了，除非变了无所谓
    public Transform transform { get { return m_tran; } }

    //慎用，可能变了，除非变了无所谓
    public GameObject gameObject { get { return m_tran != null ?m_tran.gameObject:null; } }

    public int childCount { get { return m_tran.childCount; } }
    
    public Vector3 eulerAngles { get { return m_tran.eulerAngles; } set { m_tran.eulerAngles = value; } }
    
    public Vector3 forward { get { return m_tran.forward; } set{ m_tran.forward = value; } }
    
    public Vector3 localEulerAngles { get { return m_tran.localEulerAngles; } set { m_tran.localEulerAngles = value; } }
    
    public Vector3 localPosition { get { return m_tran.localPosition; } set { m_tran.localPosition = value; } }

    public Quaternion localRotation { get { return m_tran.localRotation; } set { m_tran.localRotation = value; } }

    public Vector3 localScale { get { return m_tran.localScale; } set { m_tran.localScale = value; } }

    public Vector3 lossyScale { get { return m_tran.lossyScale; } }

    //慎用，可能变了，除非变了无所谓
    public Transform parent { get { return m_tran.parent; } }

    public Vector3 position { get { return m_tran.position; } set { m_tran.position = value; } }

    public Vector3 right { get { return m_tran.right; } set { m_tran.right = value; } }
    
    public Quaternion rotation { get { return m_tran.rotation; } set { m_tran.rotation = value; } }

    public Vector3 up { get { return m_tran.up; } set { m_tran.up = value; } }

    public Matrix4x4 localToWorldMatrix { get { return m_tran.localToWorldMatrix; } }

    public void Set(Transform t)
    {
        m_tran = t;
    }

    
    public void DetachChildren() { m_tran.DetachChildren(); }

    public Transform Find(string name) { return m_tran.Find(name); }

    public Transform GetChild(int index) { return m_tran.GetChild(index); }

    public int GetSiblingIndex() { return m_tran.GetSiblingIndex(); }

    public Vector3 InverseTransformDirection(Vector3 direction) { return m_tran.InverseTransformDirection(direction); }

    public Vector3 InverseTransformPoint(Vector3 position) { return m_tran.InverseTransformPoint(position); }

    public Vector3 InverseTransformVector(Vector3 vector) { return m_tran.InverseTransformVector(vector); }

    public bool IsChildOf(Transform parent) { return m_tran.IsChildOf(parent); }

    public void LookAt(Vector3 worldPosition, Vector3 worldUp) { m_tran.LookAt(worldPosition, worldUp); }

    public void Rotate(Vector3 eulerAngles) { m_tran.Rotate(eulerAngles); }

    public void RotateAround(Vector3 point, Vector3 axis, float angle) { m_tran.RotateAround(point, axis, angle); }

    public void SetAsFirstSibling() { m_tran.SetAsFirstSibling(); }

    public void SetAsLastSibling() { m_tran.SetAsLastSibling(); }

    public void SetSiblingIndex(int index) { m_tran.SetSiblingIndex(index); }

    public void SetParent(Transform parent) { m_tran.SetParent(parent); }

    public Vector3 TransformDirection(Vector3 direction) { return m_tran.TransformDirection(direction); }

    public Vector3 TransformPoint(Vector3 position) { return m_tran.TransformPoint(position); }

    public Vector3 TransformVector(Vector3 vector) { return m_tran.TransformVector(vector); }

    public void Translate(Vector3 translation) { m_tran.Translate(translation); }
}
