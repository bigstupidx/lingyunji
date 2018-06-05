#if UNITY_EDITOR && USE_RESOURCESEXPORT
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XTools;
using System.IO;
using System.Reflection;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace PackTool
{
    public interface IAssetsExport
    {
        Component current { get; set; }

        void CollectAnimation(AnimationClip clip);

        void CollectTMPFont(TMPro.TMP_FontAsset fontasset);

        void CollectTexture2DAsset(Texture2DAsset asset);

        void CollectBuiltinResource(Object obj);

        void CollectMesh(Mesh obj);

        void CollectSprite(Sprite sprite);

        void CollectMaterial(Material mat);

        void CollectAnimator(RuntimeAnimatorController rac);
        void CollectLightProbes(LightProbes lightProbes);

        void CollectAvatar(Avatar avatar);
        void CollectSound(AudioClip clip);
        void CollectTexture(Texture texture);
        void CollectFontlib(Font font);
        void CollectPrefab(GameObject prefab);
    }

    public class AEDelegation : IAssetsExport
    {
        public Component current { get; set; }

        public System.Action<AnimationClip> _CollectAnimation;
        public System.Action<TMPro.TMP_FontAsset> _CollectTMPFont;
        public System.Action<Object> _CollectBuiltinResource;
        public System.Action<Mesh> _CollectMesh;
        public System.Action<Sprite> _CollectSprite;
        public System.Action<Material> _CollectMaterial;
        public System.Action<RuntimeAnimatorController> _CollectAnimator;
        public System.Action<Avatar> _CollectAvatar;
        public System.Action<AudioClip> _CollectSound;
        public System.Action<Texture> _CollectTexture;
        public System.Action<Font> _CollectFontlib;
        public System.Action<GameObject> _CollectPrefab;
        public System.Action<LightProbes> _CollectLightProbes;
        public System.Action<Texture2DAsset> _CollectTexture2DAsset;
        public void CollectLightProbes(LightProbes lightProbes)
        {
            if (_CollectLightProbes != null)
                _CollectLightProbes(lightProbes);
        }

        public void CollectAnimation(AnimationClip clip)
        {
            if (_CollectAnimation != null)
                _CollectAnimation(clip);
        }

        public void CollectTMPFont(TMPro.TMP_FontAsset fontasset)
        {
            if (_CollectTMPFont != null)
                _CollectTMPFont(fontasset);
        }

        public void CollectTexture2DAsset(Texture2DAsset asset)
        {
            if (_CollectTexture2DAsset != null)
                _CollectTexture2DAsset(asset);
        }


        public void CollectBuiltinResource(Object obj)
        {
            if (_CollectBuiltinResource != null)
                _CollectBuiltinResource(obj);
        }

        public void CollectMesh(Mesh obj)
        {
            if (_CollectMesh != null)
                _CollectMesh(obj);
        }

        public void CollectSprite(Sprite sprite)
        {
            if (_CollectSprite != null)
                _CollectSprite(sprite);
        }

        public void CollectMaterial(Material mat)
        {
            if (_CollectMaterial != null)
                _CollectMaterial(mat);
        }

        public void CollectAnimator(RuntimeAnimatorController rac)
        {
            if (_CollectAnimator != null)
                _CollectAnimator(rac);
        }

        public void CollectAvatar(Avatar avatar)
        {
            if (_CollectAvatar != null)
                _CollectAvatar(avatar);
        }
        public void CollectSound(AudioClip clip)
        {
            if (_CollectSound != null)
                _CollectSound(clip);
        }
        public  void CollectTexture(Texture texture)
        {
            if (_CollectTexture != null)
                _CollectTexture(texture);
        }
        public void CollectFontlib(Font font)
        {
            if (_CollectFontlib != null)
                _CollectFontlib(font);
        }
        public void CollectPrefab(GameObject prefab)
        {
            if (_CollectPrefab != null)
                _CollectPrefab(prefab);
        }
    }
}

#endif