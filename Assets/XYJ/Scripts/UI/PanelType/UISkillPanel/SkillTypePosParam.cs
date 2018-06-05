using System.Collections.Generic;
using UnityEngine;

public class SkillTypePosParam : MonoBehaviour
{

    [Tooltip("选择天赋面板显示左右分界列")]
    public int m_boundLRByColu = 3;
    [Tooltip("选择天赋面板显示左坐标")]
    public Vector3 m_posLeft = new Vector3(-110, 0, 0);
    [Tooltip("选择天赋面板显示右坐标")]
    public Vector3 m_posRight = new Vector3(230, 0, 0);

    [Tooltip("X轴对应位置(左上角为（0，0))")]
    public List<float> posXList;
    [Tooltip("Y轴对应位置(左上角为（0，0))")]
    public List<float> posYList;

    public List<RectTransform> RectXList = new List<RectTransform>();
    public List<RectTransform> RectYList = new List<RectTransform>();

    [ContextMenu("更新位置")]
    void ChangePos()
    {
        posXList = new List<float>();
        posYList = new List<float>();
        foreach (RectTransform item in RectXList)
        {
            Debug.Log(item.anchoredPosition);
            posXList.Add(item.anchoredPosition.x);
        }
        foreach (RectTransform item in RectYList)
        {
            Debug.Log(item.anchoredPosition);
            posYList.Add(item.anchoredPosition.y);
        }
    }
}