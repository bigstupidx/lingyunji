using UnityEngine;
using System.Collections;
using Eyesblack.Nature;


namespace Eyesblack.Optimization {

    public class OptimizeGrassOnSaveLevel : UnityEditor.AssetModificationProcessor {
        public static string[] OnWillSaveAssets(string[] paths) {
            bool saveingLevel = false;
            foreach (string path in paths) {
                if (path.Contains(".unity")) {
                    saveingLevel = true;
                    break;
                }
            }


            if (saveingLevel) {
                GrassRoot grassRoot = GameObject.FindObjectOfType<GrassRoot>();
                if (grassRoot)
                    GrassOptimizer.Optimize(grassRoot.gameObject);
            }

            return paths;
        }
    }
}

