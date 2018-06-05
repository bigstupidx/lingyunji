using UnityEngine;
using System.Collections;
using ParticlePlayground;

public class FollowerEventExample : MonoBehaviour {

	public PlaygroundFollow followScript;

	void OnEnable () 
	{
		if (followScript != null)
		{
			followScript.sendEvents = true;
			followScript.followerEventBirth += OnFollowerBirth;
			followScript.followerEventDeath += OnFollowerDeath;
		}
	}

	void OnDisable ()
	{
		if (followScript != null)
		{
			followScript.followerEventBirth -= OnFollowerBirth;
			followScript.followerEventDeath -= OnFollowerDeath;
		}
	}

	void OnFollowerBirth (PlaygroundFollower follower)
	{
		Debug.Log ("Follower "+follower.gameObject.name+" is now tracking the particle id: "+follower.particleId);
	}

	void OnFollowerDeath (PlaygroundFollower follower)
	{
		Debug.Log ("Follower "+follower.gameObject.name+" died at position "+follower.transform.position);
	}
}
