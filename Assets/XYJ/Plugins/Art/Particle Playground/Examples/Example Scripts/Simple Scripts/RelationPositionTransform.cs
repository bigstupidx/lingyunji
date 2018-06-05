using UnityEngine;
using System.Collections;

/// <summary>
/// Positions a transform in relation to an anchor transform towards a target transform. Use slack to ease the effect over time.
/// This can effectively be used on a Playground Spline's nodes and bezier handles if set to transform where the anchor and/or target is moving.
/// Note that the effect will make a handle move as set to Bezier Mode: Free.
/// </summary>
public class RelationPositionTransform : MonoBehaviour {

	public Transform anchor;						// The anchor to base positioning from (usually a bezier handle's node)
	public Transform target;						// The target to position towards (usually the next or previous transform node from the anchor)
	public float anchorToTargetDistance = .2f;		// The distance from the anchor towards the target (normalized value)
	public float slack = 0;							// The amount of easing over time (0 will apply the effect immediately)
	public bool rotateTowardsTarget;

	Transform thisTransform;

	void Start () {
		thisTransform = transform;
	}
	
	void Update () {
		if (anchor==null || target==null) 
			return;
		if (slack>0) 
			thisTransform.position = Vector3.Lerp (thisTransform.position, Vector3.Lerp (anchor.position, target.position, anchorToTargetDistance), Time.deltaTime/slack);
		else 
			thisTransform.position = Vector3.Lerp (anchor.position, target.position, anchorToTargetDistance);
		if (rotateTowardsTarget) {
			if (slack>0)
				thisTransform.rotation = Quaternion.RotateTowards (thisTransform.rotation, Quaternion.LookRotation(thisTransform.position-target.position, Vector3.down)*new Quaternion(-90f,0,90f,1f), Time.deltaTime/slack);
			else
				thisTransform.rotation = Quaternion.LookRotation(thisTransform.position-target.position, Vector3.down)*new Quaternion(-90f,0,90f,1f);
		}
	}
}
