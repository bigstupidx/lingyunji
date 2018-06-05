#if UNITY_EDITOR
using UnityEngine;
using System.Collections;

namespace PackTool
{
    // 资源类型
    public enum ResourceType
    {
        Null,
        Texture,
        //Shader,
        Fontlib,
        Sound,
        Material,// 材质
        Mesh,
        Animation,
        Prefab,
        Scene, // 场景
        Modle, // 模型资源
        Animator, // 新动画组件
        Avatar,
        LightProbes,
        TMPFont,
        T2dAsset,
        //Atlas,

        Max,
    }
}
#endif