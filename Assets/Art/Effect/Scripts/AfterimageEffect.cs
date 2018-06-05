using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class AfterimageEffect : MonoBehaviour
{
    [PackTool.Pack]
    public Mesh shaderMesh; // 共享的Mesh
    [PackTool.Pack]
    public Material shaderMaterial; // 共享的材质
    public float speed; // 淡出速度
    public float distance; // 距离差多少时发射
    public float scale = 1.0f; // 缩放值
    public float alphaDiff = 0.0f; // 相邻两个模型之间的透明差

    Vector3 lastPosition; // 最后的速度

    public string nameColor; // 颜色属性

    void Start()
    {
        lastPosition = transform.position;
    }

    // 当前生成的模型列表
    List<AfterimageMono> afterimageList = new List<AfterimageMono>();

    // 创建一个特效
    void CreateEffect(Vector3 currentPosition, Vector3 eulerAngles)
    {
        // 需要创建一个残影
        Material material = new Material(shaderMaterial);

        GameObject obj = new GameObject(shaderMesh.name);
        obj.transform.localScale = Vector3.one * scale;
        obj.transform.position = currentPosition;
        obj.transform.eulerAngles = eulerAngles;
        obj.layer = gameObject.layer;

#if UNITY_EDITOR
        if (!Application.isPlaying)
            obj.hideFlags = HideFlags.DontSave;
#endif
        MeshFilter filter = obj.AddComponent<MeshFilter>();
        MeshRenderer renderer = obj.AddComponent<MeshRenderer>();
        filter.sharedMesh = shaderMesh;
        renderer.sharedMaterial = material;

        AfterimageMono aim = obj.AddComponent<AfterimageMono>();
        aim.Begin(speed, material, nameColor);
        afterimageList.Add(aim); // 添加到最后
    }

    void CheckAlphaDiff()
    {
        afterimageList.Remove(null);
        if (afterimageList.Count < 2 || alphaDiff <= 0.0f)
            return;

        float currentv = afterimageList[afterimageList.Count - 1].alpha;
        float newv = 0;
        for (int i = afterimageList.Count - 2; i >= 0; --i)
        {
            if (afterimageList[i] == null)
                break;

            newv = afterimageList[i].alpha;
            if ((newv - currentv) < alphaDiff)
            {
                afterimageList[i].alpha = currentv - alphaDiff;
                currentv = newv;
            }
        }
    }

//     bool isrenew = false;
// 
//     void OnDisable()
//     {
//         isrenew = true;
//     }

    void OnDisable()
    {
        RemoveAll();
    }

    void RemoveAll()
    {
        for (int i = 0; i < afterimageList.Count; ++i)
        {
            if (afterimageList[i] != null && afterimageList[i].gameObject != null)
                Object.Destroy(afterimageList[i].gameObject);
        }

        afterimageList.Clear();
    }

    void OnDestroy()
    {
        RemoveAll();
    }

	// Update is called once per frame
	void LateUpdate()
    {
        if (shaderMesh == null || shaderMaterial == null)
            return;

        //if (isrenew)
        //{
        //    lastPosition = transform.position;
        //    isrenew = false;
        //}

        Vector3 currentPosition = transform.position;
        float curdis =(currentPosition - lastPosition).sqrMagnitude;
        if (curdis >= distance * distance)
        {
            curdis = Mathf.Sqrt(curdis);

            {
//                 int num = (int)(curdis / distance);
//                 if (num >= 2)
//                 {
//                     Debug.Log("bnum:" + num);
//                     num = Mathf.Clamp(num, 1, (int)(alphaDiff <= 0 ? 5 : (1.0f / alphaDiff)));
//                     Debug.Log("enum:" + num);
//                     Vector3 pos = (currentPosition - lastPosition).normalized * (curdis / num);
//                     for (int i = 1; i < num; ++i)
//                     {
//                         CreateEffect(lastPosition + (pos * i), transform.eulerAngles);
//                     }
//                 }
            }

            CreateEffect(currentPosition, transform.eulerAngles);
            CheckAlphaDiff();
            lastPosition = currentPosition;
        }
    }
}
