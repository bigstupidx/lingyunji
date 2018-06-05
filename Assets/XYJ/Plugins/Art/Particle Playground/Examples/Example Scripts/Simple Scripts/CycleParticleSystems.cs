using UnityEngine;
using System.Collections;
using ParticlePlayground;

/// <summary>
/// Cycle through particle systems and apply a particle mask transitioning effect.
/// </summary>
public class CycleParticleSystems : MonoBehaviour {

	public PlaygroundParticlesC[] particleSystems;
	public float transitionTime = .5f;
	private int _currentIndex = 0;
	private int _previousIndex = 0;

	void Start () 
	{
		// Disable all particle systems
		DisableAll ();

		// Load first particle system
		GoTo (0);
	}

	void Update ()
	{
		// Example of calling previous/next
		if (Input.GetKeyDown (KeyCode.LeftArrow))
			Previous ();
		if (Input.GetKeyDown (KeyCode.RightArrow))
			Next ();
	}

	/// <summary>
	/// Disable all particle systems at once.
	/// </summary>
	public void DisableAll ()
	{
		for (int i = 0; i<particleSystems.Length; i++)
			particleSystems[i].particleSystemGameObject.SetActive(false);
	}

	/// <summary>
	/// Enables next particle system in the particleSystems array.
	/// </summary>
	public void Next () 
	{
		GoTo (_currentIndex+1);
	}

	/// <summary>
	/// Enables previous particle system in the particleSystems array.
	/// </summary>
	public void Previous ()
	{
		GoTo (_currentIndex-1);
	}

	/// <summary>
	/// Enables particle system at index.
	/// </summary>
	/// <param name="index">Particle system index you wish to enable.</param>
	public void GoTo (int index) 
	{
		// Set previous and current index
		_previousIndex = _currentIndex;
		_currentIndex = index%particleSystems.Length;
		if (_currentIndex<0)
			_currentIndex = particleSystems.Length-1;

		// Start the transitioning routine
		StartCoroutine (GoToRoutine ());
	}
	
	IEnumerator GoToRoutine ()
	{
		// Mask out previous system
		particleSystems[_previousIndex].applyParticleMask = true;
		particleSystems[_previousIndex].particleMaskTime = transitionTime;
		particleSystems[_previousIndex].particleMask = particleSystems[_previousIndex].particleCount;

		// Unmask current system
		particleSystems[_currentIndex].particleSystemGameObject.SetActive (true);
		particleSystems[_currentIndex].applyParticleMask = true;
		particleSystems[_currentIndex].particleMaskTime = transitionTime;
		particleSystems[_currentIndex].particleMask = 0;

		// Wait for previous system to mask out
		yield return new WaitForSeconds(transitionTime);

		// Disable the previous particle system
		if (_previousIndex != _currentIndex)
			particleSystems[_previousIndex].particleSystemGameObject.SetActive (false);
	}
}
