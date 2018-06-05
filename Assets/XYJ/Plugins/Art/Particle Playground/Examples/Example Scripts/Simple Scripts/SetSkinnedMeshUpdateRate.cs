using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class SetSkinnedMeshUpdateRate : MonoBehaviour {
	void Start () {
		PlaygroundC.skinnedUpdateRate = 2;
	}
}
