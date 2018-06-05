#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class AnimatorData : ComponentData
    {
#if UNITY_EDITOR
        // 收集此组件的数据
        public override bool Collect(Component component, BinaryWriter writer, IAssetsExport mgr)
        {
            bool has = false;
            // 网格渲染器，需要保存下材质数据
            Animator anim = component as Animator;

            RuntimeAnimatorController rac = anim.runtimeAnimatorController;
            if (__CollectAnimatorController__(ref rac, writer, mgr))
            {
                anim.runtimeAnimatorController = null;
                has = true;
            }

            Avatar avatar = anim.avatar;
            if (__CollectAvatar__(ref avatar, writer, mgr))
            {
                anim.avatar = null;
                has = true;
            }

            return has;
        }
#endif

        // 动态时资源加载
        public override Data LoadResources(ParamData pd, BinaryReader reader)
        {
            Data data = CreateData(pd);

            __LoadAnimatorController__(data, reader, OnLoadEnd, data);
            __LoadAvatar__(data, reader, OnLoadEnd, data);

            return data;
        }

        static void OnLoadEnd(RuntimeAnimatorController rac, object p)
        {
            Data data = (Data)p;
            Animator animator = data.mComponent as Animator;
            animator.runtimeAnimatorController = rac;

            data.OnEnd();
        }

        static void OnLoadEnd(Avatar avatar, object p)
        {
            Data data = (Data)p;
            Animator animator = data.mComponent as Animator;
            animator.avatar = avatar;

            data.OnEnd();
        }
    }
}
#endif