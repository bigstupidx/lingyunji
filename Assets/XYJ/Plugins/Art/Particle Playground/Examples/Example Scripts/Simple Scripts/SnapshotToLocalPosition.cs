using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class SnapshotToLocalPosition : MonoBehaviour {

	public Transform transformPoint;			// The transform you wish to base new positions from (most likely your particle system, splines will need to be in Vector3.zero world space)
	public PlaygroundParticlesC particles;		// Your particle system
	public int mask = -1;						// Apply any mask during transition (optional, keep this below 0 if you don't want to alter the loading snapshot's particle mask)					
	Vector3[] positionCache;					// The current loading snapshot's position cache of your assigned particle system
	int snapshotToLoad = 1;						// The current snapshot to load when spacebar is pressed

	void Start () {
		if (particles==null)
			particles = GetComponent<PlaygroundParticlesC>();
	}

	void Update () {

		// Load a snapshot when spacebar is pressed
		if (Input.GetKeyDown (KeyCode.Space))
			StartCoroutine (LoadSnapshot());
	}

	IEnumerator LoadSnapshot () {

		// Make a backup clone of the snapshot's position cache
		positionCache = (Vector3[])particles.snapshots[snapshotToLoad].settings.snapshotData.position.Clone();

		// Alter the positions in the live snapshot's position data
		for (int i = 0; i<particles.snapshots[snapshotToLoad].settings.snapshotData.position.Length; i++) {
			particles.snapshots[snapshotToLoad].settings.snapshotData.position[i] = transformPoint.TransformPoint(particles.snapshots[snapshotToLoad].settings.snapshotData.position[i]);
		}

		// Make sure that you're not using the stored Transform (otherwise it will pop into place)
		particles.snapshots[snapshotToLoad].loadTransform = false;

		// Load the snapshot (this is where the transition kicks in)
		if (mask<0) {
			// Load regularly
			particles.Load (snapshotToLoad);
		} else {
			// Load and alter particle mask during transition
			particles.LoadAndApplyMask (snapshotToLoad, mask);
		}

		// Wait till loading is done (will return false once the transition is finished)
		while (particles.IsLoading())
			yield return null;

		// Put the position cache backup back so it will appear untouched till next load, otherwise we may get positions warped additionally next load
		particles.snapshots[snapshotToLoad].settings.snapshotData.position = (Vector3[])positionCache.Clone();

		// Increment the snapshot to load for next call (just for this example)
		snapshotToLoad = (snapshotToLoad+1)%particles.snapshots.Count;
	}
}
