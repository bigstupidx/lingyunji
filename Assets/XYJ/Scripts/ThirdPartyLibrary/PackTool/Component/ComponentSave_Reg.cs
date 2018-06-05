using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if USE_RESOURCESEXPORT

namespace PackTool
{
    public partial class ComponentSave
    {
        class ComData
        {
            public System.Type type; // 类型
            public ComponentData comSave; // 组件保存数据
        }

        static Dictionary<System.Type, ComData> sComDataList = null;

        static bool isInit = false;

        static Dictionary<System.Type, ComData> ComDataList
        {
            get
            {
                if (!isInit)
                {
                    sComDataList = new Dictionary<System.Type, ComData>();
                    RegisterDefault();
                    RegAuto();
                    isInit = true;
                }

                return sComDataList;
            }
        }

        static void Register<T>(ComponentData com) where T : Component
        {
            ComData comData = new ComData();
            comData.type = typeof(T);
            if (sComDataList.ContainsKey(comData.type))
                return;

            comData.comSave = com;
            sComDataList.Add(comData.type, comData);
        }

        static void RegisterDefault()
        {
            Register<MeshFilter>(new MeshFilterData());
            Register<MeshCollider>(new MeshColliderData());
            Register<Renderer>(new RendererData());
            Register<MeshRenderer>(new RendererData());
            Register<LineRenderer>(new RendererData());
            Register<SkinnedMeshRenderer>(new SkinnedMeshRendererData());
            Register<ParticleRenderer>(new RendererData());
            Register<ParticleSystemRenderer>(new ParticleSystemRendererData());
            Register<ParticleSystem>(new ParticleSystemData());
            Register<TrailRenderer>(new RendererData());
            Register<Projector>(new ProjectorData());
            Register<Animator>(new AnimatorData());
            Register<ReflectionProbe>(new ReflectionProbePackData());
            
            Register<Animation>(new AnimationData());
            Register<AudioSource>(new AudioSourceData());
            
            // NGUI组件
            Register<DynamicShadowProjector.DrawTargetObject>(new DrawTargetObjectPackData());
            //Register<AnimationEffectManage>(new AnimationEffectManagePackData());

            Register<CS_Cloud>(new CS_CloudPackData());

            Register<UnityStandardAssets.ImageEffects.Blur>(new BlurPackData());

            // uGUI依赖收集
            Register<UnityEngine.UI.Image>(new ImagePackData());
            Register<UnityEngine.UI.Text>(new TextPackData());
            Register<UI.Label>(new TextPackData());
            Register<WXB.SymbolText>(new TextPackData());
            Register<WXB.SymbolLabel>(new TextPackData());
            Register<UnityEngine.UI.RawImage>(new RawImagePackData());
            Register<UnityEngine.UI.Button>(new SelectablePackData());
            Register<UnityEngine.UI.Selectable>(new SelectablePackData());
            Register<UIWidgets.ImageAdvanced>(new ImagePackData());
            Register<UnityEngine.UI.MirrorImage>(new ImagePackData());
            Register<xys.UI.State.StateRoot>(new StateRootPackData());

            // TMP插件
            Register<TMPro.TextMeshPro>(new TMTextPackData());
            Register<TMPro.TextMeshProUGUI>(new TMTextPackData());

            Register<PigeonCoopToolkit.Effects.Trails.SmokePlume>(new TrailRenderer_BasePackData());
            Register<PigeonCoopToolkit.Effects.Trails.SmokeTrail>(new TrailRenderer_BasePackData());
            Register<PigeonCoopToolkit.Effects.Trails.SmoothTrail>(new TrailRenderer_BasePackData());
            Register<PigeonCoopToolkit.Effects.Trails.Trail>(new TrailRenderer_BasePackData());

            Register<xys.ILMonoBehaviour>(new ILSerializedPackData());
            Register<xys.UI.UIHotPanel>(new ILSerializedPackData());
            Register<xys.UI.HotTablePanel>(new ILSerializedPackData());
            Register<xys.UI.HotTablePage>(new ILSerializedPackData());
            Register<xys.UI.UIPackagePanel>(new ILSerializedPackData());
            Register<xys.UI.UIItemTipsPanel>(new ILSerializedPackData());
            Register<xys.UI.UIMainPanel>(new ILSerializedPackData());
            Register<xys.UI.UIFunctionPanel>(new ILSerializedPackData());

            Register<RoleSkinConfig>(new RoleSkinConfigPackData());
        }
    }
}
#endif