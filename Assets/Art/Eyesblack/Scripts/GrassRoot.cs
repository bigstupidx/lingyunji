using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Eyesblack.Optimization;

namespace Eyesblack.Nature {
    [AddComponentMenu("")]
    public class GrassRoot : MonoBehaviour {
        public static GrassRoot Instance { get; private set; }


        void Awake() {
            Instance = this;
   
            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) {
                GrassOptimizer.Optimize(gameObject);
                foreach (GrassObject detail in gameObject.GetComponentsInChildren<GrassObject>()) {
                    // detail.gameObject.SetActive(false);
                    Destroy(detail.gameObject);
                }
            }
        }
        
        void Start() {
            //SetQualityLevel(VegetationQuality.QualityLevel);

            CombinedGrassMesh[] grassMeshes = GetComponentsInChildren<CombinedGrassMesh>(true);
            for (int i = 0; i < grassMeshes.Length; i++) {
                grassMeshes[i].gameObject.SetActive(true);
            }
        }

        /*
        public void SetQualityLevel(VegetationQuality.Quality quality) {
            switch (quality) {
                case VegetationQuality.Quality.High: {
                        CombinedGrassMesh[] grassMeshes = GetComponentsInChildren<CombinedGrassMesh>(true);
                        for (int i = 0; i < grassMeshes.Length; i++) {
                            grassMeshes[i].gameObject.SetActive(true);
                        }
                    }
                    break;

                case VegetationQuality.Quality.Low: {
                        CombinedGrassMesh[] grassMeshes = GetComponentsInChildren<CombinedGrassMesh>(true);
                        for (int i = 0; i < grassMeshes.Length; i++) {
                            grassMeshes[i].gameObject.SetActive(false);
                        }
                    }
                    break;
            }
        }
        */
    }
}
