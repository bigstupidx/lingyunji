using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Eyesblack.ImageEffects;

namespace Eyesblack {
    public static class PostEffectsQuality {
        public enum Quality {
            Low = 0,
            Medium,
            High
        };


        private static Quality _quality = Quality.High;
        public static Quality QualityLevel {
            get {
                return _quality;
            }

            set {
                if (_quality != value) {
                    _quality = value;

                    OnChangeQualityLevel();
                }
            }
        }

        private static void OnChangeQualityLevel() {
            PostEffectProxy[] proxies = Object.FindObjectsOfType<PostEffectProxy>();
            for (int i = 0; i < proxies.Length; i++) {
                proxies[i].OnChangeQualityLevel();
            }         
        }

    }
}
