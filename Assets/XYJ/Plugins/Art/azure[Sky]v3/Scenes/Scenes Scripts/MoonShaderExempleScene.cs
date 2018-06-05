/*															   This script controls the sunlight direction of the
 * 																		moon in the "moon shader scene".
*/

using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class MoonShaderExempleScene : MonoBehaviour {
	public Transform camPivot;
	public Transform lightCaster;
	public Material moonMaterial;
	public float RotationSpeed = 5;

	// Update is called once per frame
	void Update ()
	{
		moonMaterial.SetVector ("_SunDir" ,   -lightCaster.transform.forward  );
		moonMaterial.SetFloat  ("_LightIntensity" ,  1.5f  );
		moonMaterial.SetColor  ("_MoonColor", Color.white);
		if (Application.isPlaying) {
			if (Input.GetMouseButton (0)) {
				lightCaster.Rotate (0, -(Input.GetAxis ("Mouse X") * RotationSpeed * Time.deltaTime), -(Input.GetAxis ("Mouse Y") * RotationSpeed * Time.deltaTime), Space.World);
			}
			camPivot.Rotate (0, Input.GetAxis ("Horizontal") * (RotationSpeed * 0.5f) * Time.deltaTime, 0);
		}
	}
}
