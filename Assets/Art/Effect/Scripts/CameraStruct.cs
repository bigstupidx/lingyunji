using System;
using UnityEngine;

[Serializable]
public class CameraStruct
{
    public enum CameraType
    {
        Main, // �����
        UI, // ��ǰ��Ч��Ӧ��UI���
        Fix, // �̶����
    }

    public CameraType cameraType;

    public Camera camera;

    public Camera GetCurrent(GameObject obj)
    {
        switch (cameraType)
        {
        case CameraType.Main: return Camera.main;
        case CameraType.UI:
            {
                Canvas canvas = obj.GetComponentInParent<Canvas>();
                if (canvas != null)
                    return canvas.worldCamera;
            }
            break;
        case CameraType.Fix: return camera;
        }

        return null;
    }
}