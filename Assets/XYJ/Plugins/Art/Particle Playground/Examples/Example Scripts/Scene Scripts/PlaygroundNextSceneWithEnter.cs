using UnityEngine;
using System.Collections;

public class PlaygroundNextSceneWithEnter : MonoBehaviour {
	
	void Update () {
		if (Input.GetKeyDown (KeyCode.Return))
			LoadNext ();
	}

	void LoadNext ()
	{
		#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
		Application.LoadLevel ((Application.loadedLevel+1)%Application.levelCount);
		#else
		UnityEngine.SceneManagement.SceneManager.LoadScene((UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex+1)%UnityEngine.SceneManagement.SceneManager.sceneCount);
		#endif
	}
}
