using UnityEngine;
using System.Collections;

[AddComponentMenu("azure[Sky]/Cloud Animation")]
[ExecuteInEditMode]
public class AzureSkyCloudAnimation : MonoBehaviour
{
	public  Texture2D[] clouds;
	private Texture2D   c1;
	private Texture2D   c2;
	public  int         iniCloud;
	private int 		currentCloud;
	public  float 	    animationSpeed;
	private float       lerp;
	
	private AzureSky_Controller skyController;
	//=======================================================================================================
	//-------------------------------------------------------------------------------------------------------
	// Use this for initialization
	void Start()
	{
		skyController = GetComponent<AzureSky_Controller> ();
		if (skyController != null) {
			currentCloud = iniCloud;
			if (clouds.Length > 1) {
				skyController.Sky_Material.SetTexture ("_Cloud1", clouds [currentCloud]);
				skyController.Sky_Material.SetTexture ("_Cloud2", clouds [currentCloud + 1]);
			}
		}
	}
	//-------------------------------------------------------------------------------------------------------
	// Update is called once per frame
	void Update()
	{
		if (skyController != null) {
			if (clouds.Length == 120) {
				lerp += animationSpeed * Time.deltaTime;
				if (lerp >= 1.0f) {
					if (currentCloud < 119) {
						currentCloud += 1;
					} else {
						currentCloud = 0;
					}
				
				
					if (currentCloud <= 119) {
						skyController.Sky_Material.SetTexture ("_Cloud1", clouds [currentCloud]);
					} else {
						skyController.Sky_Material.SetTexture ("_Cloud1", clouds [0]);
					}
				
					if (currentCloud <= 118) {
						skyController.Sky_Material.SetTexture ("_Cloud2", clouds [currentCloud + 1]);
					} else {
						skyController.Sky_Material.SetTexture ("_Cloud2", clouds [0]);
					}
				
					lerp = 0.0f;
				}
				skyController.Sky_Material.SetFloat ("_CloudLerp", lerp);
			}
		}
		//-------------------------------------------------------------------------------------------------------
		// No animation in the editor
		#if UNITY_EDITOR
		skyController = GetComponent<AzureSky_Controller> ();
		if (skyController != null){
			if (clouds.Length > 0)
			{
				if (!Application.isPlaying) {
					skyController.Sky_Material.SetTexture ("_Cloud1", clouds [iniCloud]);
					skyController.Sky_Material.SetTexture ("_Cloud2", clouds [iniCloud]);
				}
			}
		}
		#endif
	}
	//=======================================================================================================
	//-------------------------------------------------------------------------------------------------------
	public void setCloudSpeed(float speed)
	{
		animationSpeed = speed;
	}
}