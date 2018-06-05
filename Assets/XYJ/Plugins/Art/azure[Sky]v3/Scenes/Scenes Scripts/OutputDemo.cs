/*																	Controls the parameters of Light Component
 * 																		besed on Outputs of AzureInspector
*/


using UnityEngine;
using System.Collections;

[ExecuteInEditMode] // To execute the script in the Editor.
public class OutputDemo : MonoBehaviour
{
	//Drag in the Inspector the GameObject that contain AzureSky_Controller script.
	public AzureSky_Controller getOutput;
	private Light thisLight;//To get the Light component

	void Start()
	{
		//Getting the Light component and save in this variable to use later.
		thisLight = GetComponent<Light> ();
	}

	// Update is called once per frame
	void Update ()
	{
		if(getOutput)
		{
			//Getting element 0 of "Curve Output" in "Azure Inspector" and applying to the Light.
			thisLight.intensity = getOutput.AzureSkyGetCurveOutput (0);


			//Getting element 0 of "Color Output" in "Azure Inspector" and applying to the Light.
			thisLight.color     = getOutput.AzureSkyGetGradientOutput (0);
		}
	}
}
