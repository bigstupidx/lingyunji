using UnityEngine;
using UnityEditor;
using System.Collections;

namespace Eyesblack.Utility {
    public static class UtilityFuncs {
        public static void MarkCurrentSceneIsDirty() {
#if UNITY_5_2
            UnityEditor.EditorApplication.MarkSceneDirty();
#elif UNITY_5_3
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
#endif
        }

    }

}
