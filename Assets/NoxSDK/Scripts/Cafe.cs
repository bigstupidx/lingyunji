using UnityEngine;
using System.Collections;

public class Cafe : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if (GLink.sharedInstance() != null)
		{
			GLink.sharedInstance().startWidget();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
