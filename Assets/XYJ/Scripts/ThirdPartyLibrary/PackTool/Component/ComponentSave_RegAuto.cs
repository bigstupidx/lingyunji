#if USE_RESOURCESEXPORT
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public partial class ComponentSave
    {
        // 注册自动生成的组件
        static void RegAuto()
        {

            Register<AfterimageEffect>(new AfterimageEffectPackData());

            Register<AmplifyColorBase>(new AmplifyColorBasePackData());

            Register<AmplifyColorEffect>(new AmplifyColorEffectPackData());

            Register<UnityStandardAssets.ImageEffects.BloomBase>(new BloomBasePackData());

            Register<UnityStandardAssets.ImageEffects.Bloom>(new BloomPackData());

            Register<CacheAssets>(new CacheAssetsPackData());

            Register<UnityStandardAssets.ImageEffects.CameraBloom>(new CameraBloomPackData());

            Register<UnityStandardAssets.ImageEffects.CameraDepthOfFieldDeprecated>(new CameraDepthOfFieldDeprecatedPackData());

            Register<CGAnimatorMainPlayer>(new CGAnimatorMainPlayerPackData());

            Register<CGAnimatorPrefabs>(new CGAnimatorPrefabsPackData());

            Register<CGAnimatorPrefabsPart>(new CGAnimatorPrefabsPartPackData());

            Register<UnityStandardAssets.ImageEffects.DepthOfFieldDeprecatedBase>(new DepthOfFieldDeprecatedBasePackData());

            Register<UnityStandardAssets.ImageEffects.DepthOfFieldDeprecated>(new DepthOfFieldDeprecatedPackData());

            Register<Eyesblack.FX.GhostEffect>(new GhostEffectPackData());

            Register<Eyesblack.FX.GhostEffectSimple>(new GhostEffectSimplePackData());

            Register<UI.LineImage>(new LineImagePackData());

            Register<MaterialAnimation1>(new MaterialAnimation1PackData());

            Register<MaterialAnimation2>(new MaterialAnimation2PackData());

            Register<MaterialAnimation>(new MaterialAnimationPackData());

            Register<MaterialData>(new MaterialDataPackData());

            Register<ModelPartSetting1>(new ModelPartSetting1PackData());

            Register<ProFlareAtlas>(new ProFlareAtlasPackData());

            Register<ProFlareBatch>(new ProFlareBatchPackData());

            Register<ProFlare>(new ProFlarePackData());

            Register<UIWidgets.Resizable>(new ResizablePackData());

            Register<PackTool.SceneRoot>(new SceneRootPackData());

            Register<DynamicShadowProjector.ShadowTextureRenderer>(new ShadowTextureRendererPackData());

            Register<T4MObjSC>(new T4MObjSCPackData());

            Register<UnityStandardAssets.CinematicEffects.TonemappingColorGrading>(new TonemappingColorGradingPackData());

            Register<UnityStandardAssets.ImageEffects.Tonemapping>(new TonemappingPackData());

            Register<touchBendingCollisionGS>(new touchBendingCollisionGSPackData());

            Register<WellFired.USCreatePrefabs>(new USCreatePrefabsPackData());

            Register<Xft.XftEventComponent>(new XftEventComponentPackData());

            Register<XyjColorAdjust>(new XyjColorAdjustPackData());

        }
    }
}
#endif