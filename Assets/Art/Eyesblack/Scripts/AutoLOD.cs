using UnityEngine;
using System.Collections;

namespace Eyesblack.Optimizations {
    [AddComponentMenu("Eyesblack/Optimizations/Auto LOD")]
    public class AutoLOD : MonoBehaviour {
        void Awake() {
            AddLODs(gameObject);
            LODBasedDistance.Enabled = true;
        }

        /*
        void OnGUI() {
            if (GUILayout.Button("Off")) {
                LODBasedDistance.Enabled = false;
            }

            if (GUILayout.Button("LOD 0")) {
                GraphicsDetails.ObjectDetailLevel = 0;
                LODBasedDistance.Enabled = true;
            }

            if (GUILayout.Button("LOD 1")) {
                GraphicsDetails.ObjectDetailLevel = 1;
                LODBasedDistance.Enabled = true;
            }

            if (GUILayout.Button("LOD 2")) {
                GraphicsDetails.ObjectDetailLevel = 2;
                LODBasedDistance.Enabled = true;
            }

            if (GUILayout.Button("LOD 3")) {
                GraphicsDetails.ObjectDetailLevel = 3;
                LODBasedDistance.Enabled = true;
            }
        }
        */

        void AddLODs(GameObject rootGo) {
            foreach (Renderer renderer in rootGo.GetComponentsInChildren<Renderer>()) {
                if (renderer.enabled) {
                    LODBasedDistance lod = renderer.GetComponent<LODBasedDistance>();
                    if (lod == null) {
                        lod = renderer.gameObject.AddComponent<LODBasedDistance>();

                        float sizeSq = Vector3.SqrMagnitude(renderer.bounds.max - renderer.bounds.min);
                        if (sizeSq > 64 * 64)
                            lod._distanceLevel = LODBasedDistance.DistanceLevel.High;
                        else if (sizeSq > 16 * 16)
                            lod._distanceLevel = LODBasedDistance.DistanceLevel.Medium;
                        else if (sizeSq > 8 * 8)
                            lod._distanceLevel = LODBasedDistance.DistanceLevel.Low;
                        else
                            lod._distanceLevel = LODBasedDistance.DistanceLevel.Lowest;
                    }
                }
            }
        }
    }
}