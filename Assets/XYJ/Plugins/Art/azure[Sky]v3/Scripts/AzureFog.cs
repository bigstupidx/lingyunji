using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent (typeof(Camera))]
[AddComponentMenu("azure[Sky]/Fog Scattering")]
public class AzureFog : MonoBehaviour
{
	public Material fogMaterial;


	//=======================================================================================================
	//-------------------------------------------------------------------------------------------------------
//	[ImageEffectOpaque]
	void OnRenderImage(RenderTexture source, RenderTexture destination) 
	{
		//-------------------------------------------------------------------------------------------------------
		GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
		if (fogMaterial==null)
		{
			Graphics.Blit (source, destination);
			#if UNITY_EDITOR
			Debug.Log("Warning. Apply the <b>Fog Material</b> to ('Fog Scattering Image Effect') script in the Main Camera Inspector");
			#endif
			return;
		}
		//-------------------------------------------------------------------------------------------------------
		Camera cam = GetComponent<Camera>();
		float CAMERA_NEAR = cam.nearClipPlane;
		float CAMERA_FAR = cam.farClipPlane;
		float CAMERA_FOV = cam.fieldOfView;
		float CAMERA_ASPECT_RATIO = cam.aspect;

		Matrix4x4 frustumCorners = Matrix4x4.identity;		

		float fovWHalf = CAMERA_FOV * 0.5f;

		Vector3 toRight = GetComponent<Camera>().transform.right * CAMERA_NEAR * Mathf.Tan (fovWHalf * Mathf.Deg2Rad) * CAMERA_ASPECT_RATIO;
		Vector3 toTop = GetComponent<Camera>().transform.up * CAMERA_NEAR * Mathf.Tan (fovWHalf * Mathf.Deg2Rad);

		Vector3 topLeft = (GetComponent<Camera>().transform.forward * CAMERA_NEAR - toRight + toTop);
		float CAMERA_SCALE = topLeft.magnitude * CAMERA_FAR/CAMERA_NEAR;	

		topLeft.Normalize();
		topLeft *= CAMERA_SCALE;

		Vector3 topRight = (cam.transform.forward * CAMERA_NEAR + toRight + toTop);
		topRight.Normalize();
		topRight *= CAMERA_SCALE;

		Vector3 bottomRight = (cam.transform.forward * CAMERA_NEAR + toRight - toTop);
		bottomRight.Normalize();
		bottomRight *= CAMERA_SCALE;

		Vector3 bottomLeft = (cam.transform.forward * CAMERA_NEAR - toRight - toTop);
		bottomLeft.Normalize();
		bottomLeft *= CAMERA_SCALE;

		frustumCorners.SetRow (0, topLeft); 
		frustumCorners.SetRow (1, topRight);		
		frustumCorners.SetRow (2, bottomRight);
		frustumCorners.SetRow (3, bottomLeft);		

		fogMaterial.SetMatrix ("_FrustumCorners", frustumCorners);

		CustomGraphicsBlit(source, destination, fogMaterial, 0);
	}
	//-------------------------------------------------------------------------------------------------------
	static void CustomGraphicsBlit(RenderTexture source, RenderTexture dest, Material fxMaterial, int passNr) 
	{
		RenderTexture.active = dest;

		fxMaterial.SetTexture ("_MainTex", source);	        

		GL.PushMatrix ();
		GL.LoadOrtho ();

		fxMaterial.SetPass (passNr);	

		GL.Begin (GL.QUADS);

		GL.MultiTexCoord2 (0, 0.0f, 0.0f); 
		GL.Vertex3 (0.0f, 0.0f, 3.0f); // BL

		GL.MultiTexCoord2 (0, 1.0f, 0.0f); 
		GL.Vertex3 (1.0f, 0.0f, 2.0f); // BR

		GL.MultiTexCoord2 (0, 1.0f, 1.0f); 
		GL.Vertex3 (1.0f, 1.0f, 1.0f); // TR

		GL.MultiTexCoord2 (0, 0.0f, 1.0f); 
		GL.Vertex3 (0.0f, 1.0f, 0.0f); // TL

		GL.End ();
		GL.PopMatrix ();
	}	
}