/// ProFlares - v1.05 - Copyright 2013-2014 All rights reserved - ProFlares.com

/// <summary>
/// ProFlareBatch.cs
/// Processes all the ProFlares in a scene, converts them into geometry that can be rendered.
/// </summary>

using UnityEngine;
using System.Collections;
using System.Collections.Generic;



[System.Serializable]
public class FlareOcclusion {
	public bool occluded = false;
	public float occlusionScale = 1;
	
	public CullingState _CullingState;
	public float CullTimer = 0;
	public float cullFader = 1;
	
	public enum CullingState{
		Visible,
		CullCountDown,
		CanCull,
		Culled,
		NeverCull
	}
}

[ExecuteInEditMode]
[RequireComponent (typeof (MeshRenderer))]
[RequireComponent (typeof (MeshFilter))]
public class ProFlareBatch : MonoBehaviour {
	
	public enum Mode{
		Standard = 0,
		SingleCamera = 1,
		VR = 2
	}
	
	public Mode mode = Mode.Standard;
	
    [PackTool.Pack]
	public ProFlareAtlas _atlas;
	
	//List of flares
	public List<ProFlare> Flares = new List<ProFlare>();
	//List of FlareElements
	
	public List<ProFlareElement> FlareElements = new List<ProFlareElement>();
	public ProFlareElement[] FlareElementsArray;
	
	public Camera GameCamera;
	public Transform GameCameraTrans;
	
	//Camera that the flare geometry will be rendered from.
	public Camera FlareCamera;
	public Transform FlareCameraTrans;
	//Cached Components
	public MeshFilter meshFilter;
	public Transform thisTransform;
	
	public MeshRenderer meshRender;
	
	public float zPos;
	
	//Multiple meshes used for double buffering technique
	public Mesh bufferMesh;
	public Mesh meshA;
	public Mesh meshB;
	
	//Ping pong value for double Buffering
	bool PingPong;
	
	//Material used for the Flares, this is automatically created.
	public Material mat;
	
	//Geometry Arrays
	Vector3[] vertices;
	Vector2[] uv;
	Color32[] colors;
	
	int[] triangles;
	
	public FlareOcclusion[] FlareOcclusionData;
	
	//Debug Propertys
	public bool useBrightnessThreshold = true;
	public int BrightnessThreshold = 1;
	public bool overdrawDebug;
	
	//When set to true the Flarebatches' geomerty will be rebuilt.
	public bool dirty = false;
	
	public bool useCulling = true;
	public int cullFlaresAfterTime = 5;
	public int cullFlaresAfterCount = 5;
	
	public bool culledFlaresNowVisiable = false;
	private float reshowCulledFlaresTimer = 0;
	public float reshowCulledFlaresAfter = 0.3f;
	
	//HelperTransform used for FlarePositions calculations.
	public Transform helperTransform;
	
	public bool showAllConnectedFlares;
		
	public bool VR_Mode = false;
	public float VR_Depth = 0.2f;
	public bool SingleCamera_Mode = false;
	
	void Reset(){
		if(helperTransform == null)
			CreateHelperTransform();
 
		
		mat = new Material(Shader.Find("ProFlares/Textured Flare Shader"));
		
		if(meshFilter == null)
			meshFilter = GetComponent<MeshFilter>();
		
        if(meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();
		
		meshRender = gameObject.GetComponent<MeshRenderer>();
		
		if(meshRender == null)
            meshRender = gameObject.AddComponent<MeshRenderer>();
		
		
		if(FlareCamera == null){
			FlareCamera = transform.root.GetComponentInChildren<Camera>();
		}
        
		meshRender.material = mat;
		
	 	/*
		meshA = SetupSingleMesh();
		meshB = SetupSingleMesh();
		
		meshA.MarkDynamic();
		meshB.MarkDynamic();*/
		
		SetupMeshes();
		
		dirty = true;
	}
	
	void Awake(){
		PI_Div180 = Mathf.PI / 180;
		Div180_PI = 180 / Mathf.PI;
	}

    public static System.Func<Camera> get_default_camera = null;

    void Start () {
		//Turn off overdraw debug mode.
		if(Application.isPlaying){
			overdrawDebug = false;
			dirty = true;
		}
		
		if(GameCamera == null)
        {
            GameObject GameCameraGo = GameObject.FindWithTag("MainCamera");
            if (GameCameraGo)
                if (GameCameraGo.GetComponent<Camera>())
                    GameCamera = GameCameraGo.GetComponent<Camera>();
#if !SCENE_DEBUG
            if (null != get_default_camera)
                GameCamera = get_default_camera();
#endif
        }
        
		if(GameCamera)
			GameCameraTrans = GameCamera.transform;
		//Make sure we have the transform cached
		thisTransform = transform;
		
		/*
	 	meshA = SetupSingleMesh();
		meshA.name = "MeshA";
		meshB = SetupSingleMesh();
		meshB.name = "MeshB";
		meshA.MarkDynamic();
		meshB.MarkDynamic();*/
		
		SetupMeshes();
 			
		for(int i = 0; i < Flares.Count; i++){
			 
			if(Flares[i].neverCull)
				FlareOcclusionData[i]._CullingState = FlareOcclusion.CullingState.NeverCull;
			
		}

	}
	
	void CreateMat(){
		//Debug.LogError("CreateMat");
		mat = new Material(Shader.Find("ProFlares/Textured Flare Shader"));
		meshRender.material = mat;
		if(_atlas)
			if(_atlas.texture)
				mat.mainTexture = _atlas.texture;
	}
	
	
	
	//Call when you switch your main render.
	public void SwitchCamera(Camera newCamera){
		GameCamera = newCamera;
		GameCameraTrans = newCamera.transform; 
		
		
		//Update Occlusion data on changing camera new in v1.03
		FixedUpdate();

		for(int F = 0; F < Flares.Count; F++){
			if(FlareOcclusionData[F] != null){
			 if(FlareOcclusionData[F].occluded)
				FlareOcclusionData[F].occlusionScale = 0;
				
			}
		}
	}
	
	void OnDestroy(){
		//Remove the helper transform.
		if(Application.isPlaying){
			Destroy(helperTransform.gameObject);
			Destroy(mat);
		}
		else{
			DestroyImmediate(helperTransform.gameObject);
			DestroyImmediate(mat);
		}
	}
	
    
	
	//Called from ProFlare.cs
	//First checks if the flare is already in the list, if not adds it and rebuils the Flarebatch Geometry
	public void AddFlare(ProFlare _flare){
		bool found = false;
        
		for(int i = 0; i < Flares.Count; i++){
			if(_flare == Flares[i]){
				found = true;
				break;
			}
		}
		
		if(!found){
			
			Flares.Add(_flare);
			
//			Debug.LogError("addFlare");
			
			FlareOcclusionData = new FlareOcclusion[Flares.Count];
			
			for(int i = 0; i < FlareOcclusionData.Length; i++){
				FlareOcclusionData[i] = new FlareOcclusion();
				if(_flare.neverCull)
					FlareOcclusionData[i]._CullingState = FlareOcclusion.CullingState.NeverCull;
			}
			dirty = true;
		}
	}
	
	void CreateHelperTransform(){
		
		GameObject HelpTransformGo =  new GameObject("_HelperTransform");
		
		helperTransform = HelpTransformGo.transform;
		helperTransform.parent =  transform;
		helperTransform.localScale = Vector3.one;
		helperTransform.localPosition = Vector3.zero;
	}
	
	void Update(){
		
		if(thisTransform)
				thisTransform.localPosition = Vector3.forward*zPos;
		
		
		
		//Checks if you have deleted the helper transform. If its missing recreate it.....
		if(helperTransform == null)
			CreateHelperTransform();
		
        if(meshRender){
			if(meshRender.sharedMaterial == null)
				CreateMat();
		}else
			meshRender = gameObject.GetComponent<MeshRenderer>();
		
		bool meshMissing = false;
		if(meshA == null){
			meshMissing = true;
		//	meshA = SetupSingleMesh();
		//	meshA.name = "MeshA";
		//	meshA.MarkDynamic();
		}
		
		if(meshB == null){
			meshMissing = true;
			
		//	meshB = SetupSingleMesh();
		//	meshB.name = "MeshB";
		//	meshB.MarkDynamic();
		}
		
		if(meshMissing)
			SetupMeshes(); 
		
		if(dirty)
			ReBuildGeometry();
	}
	
	//Late update
	void LateUpdate () {
 		
		if(_atlas == null)
			return;
		
		
		if(!VR_Mode)
 			UpdateFlares();
	
	}
	
	public void UpdateFlares(){
		
		bufferMesh = PingPong ? meshA : meshB;
		
		PingPong = !PingPong;
		
		UpdateGeometry();
		
		//Profiler.BeginSample("Set Arrays");

		bufferMesh.vertices = vertices;
		
		bufferMesh.colors32 = colors;

	  	meshFilter.sharedMesh = bufferMesh;
		//Profiler.EndSample();
	}
	
	public void ForceRefresh(){
		
		Flares.Clear();
		
		ProFlare[] flares = GameObject.FindObjectsOfType(typeof(ProFlare)) as ProFlare[];
		
		for(int i = 0; i < flares.Length; i++){
			if(flares[i]._Atlas == _atlas)
				Flares.Add(flares[i]);
		}
		
		dirty = true;
	}
	
	void ReBuildGeometry(){
#if UNITY_EDITOR
		//See when the geometry is built, try and avoid triggering this in the middle of your game.
		//This can be triggered by the new culling system, if your only using a few flares you may want to turn it off as the speed increase from culling will be more limited.
		Debug.Log("ProFlares - Rebuilding Geometry : "+ gameObject.name,gameObject); //Commented out if gets annoying
#endif
		
		FlareElements.Clear();
		
		int flareElementsCount = 0;
		
		for(int i = 0; i < Flares.Count; i++){
			for(int i2 = 0; i2 < Flares[i].Elements.Count; i2++){
		//		FlareElements.Add(Flares[i].Elements[i2]);
				
				if(FlareOcclusionData[i]._CullingState ==  FlareOcclusion.CullingState.CanCull){
					FlareOcclusionData[i]._CullingState = FlareOcclusion.CullingState.Culled;
					FlareOcclusionData[i].cullFader = 0;
				}
				
				
				if(FlareOcclusionData[i]._CullingState != FlareOcclusion.CullingState.Culled){
					flareElementsCount++;
				}
				
			}
		}
		
		FlareElementsArray = new ProFlareElement[flareElementsCount];
		
		flareElementsCount = 0;
		for(int i = 0; i < Flares.Count; i++){
			for(int i2 = 0; i2 < Flares[i].Elements.Count; i2++){
				if(FlareOcclusionData[i]._CullingState != FlareOcclusion.CullingState.Culled){

					FlareElementsArray[flareElementsCount] = (Flares[i].Elements[i2]);
					flareElementsCount++;
				}
			}
		}
		
		/*
		FlareOcclusionData = new FlareOcclusion[Flares.Count];
		
		for(int i = 0; i < FlareOcclusionData.Length; i++){
			FlareOcclusionData[i] = new FlareOcclusion();
		}*/
		
		//DestroyImmediate(meshA);
		//DestroyImmediate(meshB);
		//DestroyImmediate(bufferMesh);
		
		meshA = null;
		meshB = null;
		bufferMesh = null;
		
		SetupMeshes();
		dirty = false;
	}
	
	void SetupMeshes()
    {
		
		if(_atlas == null)
			return;

		if(FlareElementsArray == null)
			return;
        
		meshA = new Mesh();
		meshB = new Mesh();

		
		int vertSize = 0;
		int uvSize = 0;
		int colSize = 0;
		int triCount = 0;



		//Calculate how big each array needs to be based on the Flares
	 	for(int i = 0; i < FlareElementsArray.Length; i++){
			switch(FlareElementsArray[i].type){
				case(ProFlareElement.Type.Single):
				{
					vertSize = vertSize+4;
					uvSize = uvSize+4;
					colSize = colSize+4;
					triCount = triCount+6;
				}
                    break;
				case(ProFlareElement.Type.Multi):
				{
					int subCount = FlareElementsArray[i].subElements.Count;
					vertSize = vertSize+(4*subCount);
					uvSize = uvSize+(4*subCount);
					colSize = colSize+(4*subCount);
					triCount = triCount+(6*subCount);
				}
                    break;
			}
		}
		
		
		//Create Built in arrays
		vertices = new Vector3[vertSize];
    	uv = new Vector2[uvSize];
		colors = new Color32[colSize];
		triangles = new int[triCount];
        
		
		//Set Inital valuse for each vertex
		for(int i = 0; i < vertices.Length/4; i++){
			int extra = i * 4;
			vertices[0+extra] = new Vector3(1,1,0); //((Vector3.right)+(Vector3.up));
			vertices[1+extra] = new Vector3(1,-1,0);//((Vector3.right)+(Vector3.down));
			vertices[2+extra] = new Vector3(-1,1,0);//((Vector3.left)+(Vector3.up));
			vertices[3+extra] = new Vector3(-1,-1,0);//((Vector3.left)+(Vector3.down));
		}
		
		//Set UV coordinates for each vertex, this only needs to be done in the mesh rebuild.
		int count = 0;
		for(int i = 0; i < FlareElementsArray.Length; i++){
			switch(FlareElementsArray[i].type){
				case(ProFlareElement.Type.Single):
				{
					int extra = (count) * 4;
					Rect final = _atlas.elementsList[FlareElementsArray[i].elementTextureID].UV;
					uv[0+extra] = new Vector2(final.xMax,final.yMax);
					uv[1+extra] = new Vector2(final.xMax,final.yMin);
					uv[2+extra] = new Vector2(final.xMin,final.yMax);
					uv[3+extra] = new Vector2(final.xMin,final.yMin);
					count++;
				}break;
				case(ProFlareElement.Type.Multi):
				{
					for(int i2 = 0; i2 < FlareElementsArray[i].subElements.Count; i2++){
						
						int extra2 = (count+i2) * 4;
						
						Rect final = _atlas.elementsList[FlareElementsArray[i].elementTextureID].UV;
						
						uv[0+extra2] = new Vector2(final.xMax,final.yMax);
						uv[1+extra2] = new Vector2(final.xMax,final.yMin);
						uv[2+extra2] = new Vector2(final.xMin,final.yMax);
						uv[3+extra2] = new Vector2(final.xMin,final.yMin);
                        
					}
					count = count+FlareElementsArray[i].subElements.Count;
				}break;
			}
		}
		Color32 newColor = new Color32(255,255,255,255);
		//Set inital vertex colors.
		for(int i = 0; i < colors.Length/4; i++){
			int extra = i * 4;
			colors[0+extra] = newColor;
			colors[1+extra] = newColor;
			colors[2+extra] = newColor;
			colors[3+extra] = newColor;
		}
		
        
		
		//Set triangle array, this is only done in the mesh rebuild.
		for(int i = 0; i < triangles.Length/6; i++){
			int extra = i * 4;
			int extra2 = i * 6;
			triangles[0+extra2] = 0+extra;
			triangles[1+extra2] = 1+extra;
			triangles[2+extra2] = 2+extra;
			triangles[3+extra2] = 2+extra;
			triangles[4+extra2] = 1+extra;
			triangles[5+extra2] = 3+extra;
		}
		
        meshA.vertices = vertices;
        meshA.uv = uv;
        meshA.triangles = triangles;
  		meshA.colors32 = colors;
		
		meshA.bounds = new Bounds(Vector3.zero,Vector3.one*1000);
		
        meshB.vertices = vertices;
        meshB.uv = uv;
        meshB.triangles = triangles;
  		meshB.colors32 = colors;
		meshB.bounds = new Bounds(Vector3.zero,Vector3.one*1000);
	 	
		//return mesh;
    }
	/*
	Mesh SetupSingleMesh()
    {
        Mesh mesh = new Mesh();
		return mesh;
		if(_atlas == null)
			return mesh;
		
		int vertSize = 0;
		int uvSize = 0;
		int colSize = 0;
		int triCount = 0;
		
        
		//Calculate how big each array needs to be based on the Flares
	 	for(int i = 0; i < FlareElementsArray.Length; i++){
			switch(FlareElementsArray[i].type){
				case(ProFlareElement.Type.Single):
				{
					vertSize = vertSize+4;
					uvSize = uvSize+4;
					colSize = colSize+4;
					triCount = triCount+6;
				}
                    break;
				case(ProFlareElement.Type.Multi):
				{
					int subCount = FlareElementsArray[i].subElements.Count;
					vertSize = vertSize+(4*subCount);
					uvSize = uvSize+(4*subCount);
					colSize = colSize+(4*subCount);
					triCount = triCount+(6*subCount);
				}
                    break;
			}
		}
		
		
		//Create Built in arrays
		vertices = new Vector3[vertSize];
    	uv = new Vector2[uvSize];
		colors = new Color32[colSize];
		triangles = new int[triCount];
        
		
		//Set Inital valuse for each vertex
		for(int i = 0; i < vertices.Length/4; i++){
			int extra = i * 4;
			vertices[0+extra] = ((Vector3.right)+(Vector3.up));
			vertices[1+extra] = ((Vector3.right)+(Vector3.down));
			vertices[2+extra] = ((Vector3.left)+(Vector3.up));
			vertices[3+extra] = ((Vector3.left)+(Vector3.down));
		}
		
		//Set UV coordinates for each vertex, this only needs to be done in the mesh rebuild.
		int count = 0;
		for(int i = 0; i < FlareElementsArray.Length; i++){
			switch(FlareElementsArray[i].type){
				case(ProFlareElement.Type.Single):
				{
					int extra = (count) * 4;
					Rect final = _atlas.elementsList[FlareElementsArray[i].elementTextureID].UV;
					uv[0+extra] = new Vector2(final.xMax,final.yMax);
					uv[1+extra] = new Vector2(final.xMax,final.yMin);
					uv[2+extra] = new Vector2(final.xMin,final.yMax);
					uv[3+extra] = new Vector2(final.xMin,final.yMin);
					count++;
				}break;
				case(ProFlareElement.Type.Multi):
				{
					for(int i2 = 0; i2 < FlareElementsArray[i].subElements.Count; i2++){
						
						int extra2 = (count+i2) * 4;
						
						Rect final = _atlas.elementsList[FlareElementsArray[i].elementTextureID].UV;
						
						uv[0+extra2] = new Vector2(final.xMax,final.yMax);
						uv[1+extra2] = new Vector2(final.xMax,final.yMin);
						uv[2+extra2] = new Vector2(final.xMin,final.yMax);
						uv[3+extra2] = new Vector2(final.xMin,final.yMin);
                        
					}
					count = count+FlareElementsArray[i].subElements.Count;
				}break;
			}
		}
		
		//Set inital vertex colors.
		for(int i = 0; i < colors.Length/4; i++){
			int extra = i * 4;
			colors[0+extra] = new Color32(0,255,0,255);
			colors[1+extra] = new Color32(255,0,0,255);
			colors[2+extra] = new Color32(0,255,255,255);
			colors[3+extra] = new Color32(255,0,255,255);
		}
		
        
		
		//Set triangle array, this is only done in the mesh rebuild.
		for(int i = 0; i < triangles.Length/6; i++){
			int extra = i * 4;
			int extra2 = i * 6;
			triangles[0+extra2] = 0+extra;
			triangles[1+extra2] = 1+extra;
			triangles[2+extra2] = 2+extra;
			triangles[3+extra2] = 2+extra;
			triangles[4+extra2] = 1+extra;
			triangles[5+extra2] = 3+extra;
		}
		
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
  		mesh.colors32 = colors;
		
		
		mesh.bounds = new Bounds(Vector3.zero,Vector3.one*1000);
	 	
        return mesh;
    }*/
	
	Vector3[] verts;
	Vector2 _scale;
	Color32 _color;
    
	float PI_Div180;
	float Div180_PI;
	
	int visibleFlares = 0;
	
	/*
	public Vector3 RotatePoint(Vector3 p, float d,float ct,float st) {
		//float r = d * (Mathf.PI / 180);
		//float ct = Mathf.Cos(r);
		//float st = Mathf.Sin(r);
		float x = (ct * p.x - st * p.y);
		float y = (st * p.x + ct * p.y);
		return new Vector3(x,y,0);
	}*/
	
	void FixedUpdate(){
		
	
		if(!dirty)
		for(int F = 0; F < Flares.Count; F++){
			
			 FlareOcclusionData[F].occluded = false;
			
			//Flares[F].Occluded = false;
			
			if(Flares[F].RaycastPhysics){
				RaycastHit hit;
				
				Vector3 direction = GameCameraTrans.position-Flares[F].thisTransform.position;
				
				float distanceRay = Vector3.Distance(GameCameraTrans.position,Flares[F].thisTransform.position);
		        
				//Flares[F].Occluded = false;
				
				if(Flares[F].isVisible){
				
					Ray ray = new Ray(Flares[F].thisTransform.position,direction);
					
					if (Physics.Raycast(ray,  out hit,distanceRay,Flares[F].mask)){
						
						//Flares[F].Occluded = true;
						
						FlareOcclusionData[F].occluded = true;
	
			#if UNITY_EDITOR					
						Debug.DrawRay(Flares[F].thisTransform.position,direction,Color.red);
						
						Flares[F].OccludingObject = hit.collider.gameObject;
			#endif
					}else{
			#if UNITY_EDITOR
			            Flares[F].OccludingObject = null;
						
						Debug.DrawRay(Flares[F].thisTransform.position,direction);
			#endif
					}
				}
			}
		}
	}
    
	
 	
	void UpdateGeometry(){
		
		if(GameCamera == null){
			meshRender.enabled = false;
			return;
		}
		
		meshRender.enabled = true;
		//Profiler.BeginSample("Update Pos");
		visibleFlares = 0;
		int canCullFlares = 0;
		for(int F = 0; F < Flares.Count; F++){
			
			//Profiler.BeginSample("Lens Pos");

			Flares[F].LensPosition = GameCamera.WorldToViewportPoint(Flares[F].thisTransform.position);

			Vector3 LensPosition = Flares[F].LensPosition;
			
			bool isVisible = (LensPosition.z > 0f && LensPosition.x+Flares[F].OffScreenFadeDist > 0f && LensPosition.x-Flares[F].OffScreenFadeDist < 1f && LensPosition.y+Flares[F].OffScreenFadeDist > 0f && LensPosition.y-Flares[F].OffScreenFadeDist < 1f);
			Flares[F].isVisible = isVisible;
			//Profiler.EndSample();
			//Profiler.BeginSample("offScreen Fading");

			//Off Screen fading
			float offScreenFade = 1;
			if(!(LensPosition.x > 0f && LensPosition.x < 1f && LensPosition.y > 0f && LensPosition.y < 1f)){
				float offScreenNorm = 1f/Flares[F].OffScreenFadeDist;
				float xPos = 0;
				float yPos = 0;
				
				if(!(LensPosition.x > 0f && LensPosition.x < 1f))
					xPos = LensPosition.x > 0.5f ? LensPosition.x-1f : Mathf.Abs(LensPosition.x);
				
				if(!(LensPosition.y > 0f && LensPosition.y < 1f))
					yPos = LensPosition.y > 0.5f ? LensPosition.y-1f : Mathf.Abs(LensPosition.y);
				
				offScreenFade = Mathf.Clamp01( offScreenFade - (Mathf.Max(xPos,yPos))*offScreenNorm);
			}
	        
			
			//Profiler.EndSample();
			//Profiler.BeginSample("Dynamic Triggering");

			//Dynamic Triggering Center
			float centerBoost = 0;
			if(LensPosition.x > 0.5f-Flares[F].DynamicCenterRange && LensPosition.x < 0.5f+Flares[F].DynamicCenterRange && LensPosition.y > 0.5f-Flares[F].DynamicCenterRange && LensPosition.y < 0.5f+Flares[F].DynamicCenterRange){
				if(Flares[F].DynamicCenterRange > 0){
					float centerBoostNorm = 1/(Flares[F].DynamicCenterRange);
					centerBoost = 1-Mathf.Max(Mathf.Abs(LensPosition.x-0.5f),Mathf.Abs(LensPosition.y-0.5f))*centerBoostNorm;
				}
			}
		
			//Dynamic Triggering Edge
			float DynamicEdgeAmount = 0;
			
			bool isInEdgeZone1 = (
	                              LensPosition.x > 0f+Flares[F].DynamicEdgeBias+(Flares[F].DynamicEdgeRange) &&
	                              LensPosition.x < 1f-Flares[F].DynamicEdgeBias-(Flares[F].DynamicEdgeRange) &&
	                              LensPosition.y > 0f+Flares[F].DynamicEdgeBias+(Flares[F].DynamicEdgeRange) &&
	                              LensPosition.y < 1f-Flares[F].DynamicEdgeBias-(Flares[F].DynamicEdgeRange)
	                              );
			
			bool isInEdgeZone2 = (
	                              LensPosition.x+(Flares[F].DynamicEdgeRange) > 0f+Flares[F].DynamicEdgeBias &&
	                              LensPosition.x-(Flares[F].DynamicEdgeRange) < 1f-Flares[F].DynamicEdgeBias &&
	                              LensPosition.y+(Flares[F].DynamicEdgeRange) > 0f+Flares[F].DynamicEdgeBias &&
	                              LensPosition.y-(Flares[F].DynamicEdgeRange) < 1f-Flares[F].DynamicEdgeBias
	                              );
			
			if(!isInEdgeZone1&&isInEdgeZone2){
				
				float DynamicEdgeNormalizeValue = 1/(Flares[F].DynamicEdgeRange);
				float DynamicEdgeX = 0;
				float DynamicEdgeY = 0;
				
				bool isInEdgeZoneX1 = (LensPosition.x > 0f+Flares[F].DynamicEdgeBias+(Flares[F].DynamicEdgeRange) && LensPosition.x < 1f-Flares[F].DynamicEdgeBias-(Flares[F].DynamicEdgeRange));
				bool isInEdgeZoneX2 = (LensPosition.x+(Flares[F].DynamicEdgeRange) > 0f+Flares[F].DynamicEdgeBias && LensPosition.x-(Flares[F].DynamicEdgeRange) < 1f-Flares[F].DynamicEdgeBias);
				bool isInEdgeZoneY1 = (LensPosition.y > 0f+Flares[F].DynamicEdgeBias+(Flares[F].DynamicEdgeRange) && LensPosition.y < 1f-Flares[F].DynamicEdgeBias-(Flares[F].DynamicEdgeRange));
				bool isInEdgeZoneY2 = (LensPosition.y+(Flares[F].DynamicEdgeRange) > 0f+Flares[F].DynamicEdgeBias && LensPosition.y-(Flares[F].DynamicEdgeRange) < 1f-Flares[F].DynamicEdgeBias);
				
				if(!isInEdgeZoneX1&&isInEdgeZoneX2){
					DynamicEdgeX = LensPosition.x > 0.5f ? (LensPosition.x - 1 +Flares[F].DynamicEdgeBias) + (Flares[F].DynamicEdgeRange) : Mathf.Abs(LensPosition.x -Flares[F].DynamicEdgeBias - (Flares[F].DynamicEdgeRange));
	                
					DynamicEdgeX = (DynamicEdgeX*DynamicEdgeNormalizeValue)*0.5f;
				}
				
				if(!isInEdgeZoneY1&&isInEdgeZoneY2){
					DynamicEdgeY = LensPosition.y > 0.5f ? (LensPosition.y - 1 + Flares[F].DynamicEdgeBias) + (Flares[F].DynamicEdgeRange) : Mathf.Abs(LensPosition.y-Flares[F].DynamicEdgeBias - (Flares[F].DynamicEdgeRange));
					
					DynamicEdgeY = (DynamicEdgeY*DynamicEdgeNormalizeValue)*0.5f;
				}
				
				DynamicEdgeAmount = Mathf.Max(DynamicEdgeX,DynamicEdgeY);
			}
			
						
			DynamicEdgeAmount = Flares[F].DynamicEdgeCurve.Evaluate(DynamicEdgeAmount);
			
			//Profiler.EndSample();
			//Profiler.BeginSample("Angle Fall Off");

			
			float AngleFallOff = 1;
			
			if(Flares[F].UseAngleLimit){
				Vector3 playerAngle = Flares[F].thisTransform.forward;
	            
				Vector3 cameraAngle = GameCameraTrans.forward;
				
				float horizDiffAngle = Vector3.Angle(cameraAngle, playerAngle);
				
				horizDiffAngle = Mathf.Abs( Mathf.Clamp(180-horizDiffAngle,-Flares[F].maxAngle,Flares[F].maxAngle));
				
				AngleFallOff = 1f-(horizDiffAngle*(1f/(Flares[F].maxAngle*0.5f)));
				
				if(Flares[F].UseAngleCurve)
					AngleFallOff = Flares[F].AngleCurve.Evaluate(AngleFallOff);
			}
			
			//Profiler.EndSample();
			//Profiler.BeginSample("Distance Check");

			
			float distanceFalloff = 1f;
			
			if(Flares[F].useMaxDistance){
				
				Vector3 heading  = Flares[F].thisTransform.position - GameCameraTrans.position;
				
				float distance = Vector3.Dot(heading, GameCameraTrans.forward);
				
				float distanceNormalised = 1f-(distance/Flares[F].GlobalMaxDistance);
				
				distanceFalloff =  1f*distanceNormalised;
				if(distanceFalloff < 0.001f)
					Flares[F].isVisible = false;
				
			}
			//Profiler.EndSample();
			//Profiler.BeginSample("Check Occlusion Data");

			
			if(!dirty)
				if(FlareOcclusionData[F] != null)
					if(FlareOcclusionData[F].occluded){
						 
			           	FlareOcclusionData[F].occlusionScale = Mathf.Lerp(FlareOcclusionData[F].occlusionScale,0,Time.deltaTime*16);
					}else{
						FlareOcclusionData[F].occlusionScale = Mathf.Lerp(FlareOcclusionData[F].occlusionScale,1,Time.deltaTime*16);
					}
			
			//Profiler.EndSample();
			//Profiler.BeginSample("Final Lens Pos Set");
			
//			Debug.Log("Visible = "+Flares[F].isVisible+" | offScreenFade "+offScreenFade);
//			Debug.Log("distanceFalloff "+distanceFalloff);
			
			if(!Flares[F].isVisible)
				offScreenFade = 0;
				
			float tempScale = 1;
			
		 	if(FlareCamera)
		 		helperTransform.position = FlareCamera.ViewportToWorldPoint(LensPosition);
			
		  	LensPosition = helperTransform.localPosition;
			
			//Profiler.EndSample();
			
			if((!VR_Mode) && (!SingleCamera_Mode))
			  	LensPosition.z = 0f;
	        
			float finalAlpha;
			
			//Profiler.BeginSample("Elements Loo[");
			for(int i = 0; i < Flares[F].Elements.Count; i++){
				ProFlareElement _element = Flares[F].Elements[i];
				Color GlobalColor = Flares[F].GlobalTintColor;
				if(isVisible)
					switch(_element.type){
						case(ProFlareElement.Type.Single):
							//Do the color stuff.
					
							_element.ElementFinalColor.r = (_element.ElementTint.r * GlobalColor.r);
						 	_element.ElementFinalColor.g = (_element.ElementTint.g * GlobalColor.g);
						 	_element.ElementFinalColor.b = (_element.ElementTint.b * GlobalColor.b);
	                        
							finalAlpha = _element.ElementTint.a * GlobalColor.a;
							
							if(Flares[F].useDynamicEdgeBoost){
								if(_element.OverrideDynamicEdgeBrightness)
									finalAlpha = finalAlpha + (_element.DynamicEdgeBrightnessOverride*DynamicEdgeAmount);
								else
									finalAlpha = finalAlpha + (Flares[F].DynamicEdgeBrightness*DynamicEdgeAmount);
							}
							
							if(Flares[F].useDynamicCenterBoost){
								if(_element.OverrideDynamicCenterBrightness)
	                                finalAlpha += (_element.DynamicCenterBrightnessOverride*centerBoost);
								else
	                                finalAlpha += (Flares[F].DynamicCenterBrightness*centerBoost);
	                        }
							
							if(Flares[F].UseAngleBrightness)
								finalAlpha *= AngleFallOff;
	                        
							if(Flares[F].useDistanceFade)
								finalAlpha *= distanceFalloff;
	                        
 							finalAlpha *= FlareOcclusionData[F].occlusionScale;
							
							finalAlpha *= FlareOcclusionData[F].cullFader;
					
							finalAlpha *= offScreenFade;
												
							_element.ElementFinalColor.a = finalAlpha;
	                        
	                        break;
	                        
						case(ProFlareElement.Type.Multi):
					//Profiler.BeginSample("Color Mutli Loop");
							for(int i2 = 0; i2 < _element.subElements.Count; i2++){
	                            //Do the color stuff.
 								SubElement _subElement = _element.subElements[i2];
	                            _subElement.colorFinal.r = (_subElement.color.r * GlobalColor.r);
	                           	_subElement.colorFinal.g = (_subElement.color.g * GlobalColor.g);
	                            _subElement.colorFinal.b = (_subElement.color.b * GlobalColor.b);
	                            
	                            finalAlpha = _subElement.color.a * GlobalColor.a;
	                            
	                            if(Flares[F].useDynamicEdgeBoost){
	                                if(_element.OverrideDynamicEdgeBrightness)
	                                    finalAlpha = finalAlpha + (_element.DynamicEdgeBrightnessOverride*DynamicEdgeAmount);
	                                else
	                                    finalAlpha = finalAlpha + (Flares[F].DynamicEdgeBrightness*DynamicEdgeAmount);
	                            }
	                            
	                            if(Flares[F].useDynamicCenterBoost){
	                                if(_element.OverrideDynamicCenterBrightness)
	                                    finalAlpha += (_element.DynamicCenterBrightnessOverride*centerBoost);
	                                else
	                                    finalAlpha += (Flares[F].DynamicCenterBrightness*centerBoost);
	                            } 
	                            
	                            if(Flares[F].UseAngleBrightness)
	                                finalAlpha *= AngleFallOff;
	                            
	                            if(Flares[F].useDistanceFade)
	                                finalAlpha *= distanceFalloff;
	                            
								finalAlpha *= FlareOcclusionData[F].occlusionScale;
						
								finalAlpha *= FlareOcclusionData[F].cullFader;
						
	                            finalAlpha *= offScreenFade;
	                            
 	                            _subElement.colorFinal.a = finalAlpha;
	                            
							}
					//Profiler.EndSample();
	                        break;
					}
				else{
					switch(_element.type){
						case(ProFlareElement.Type.Single):
						//	_element.ElementFinalColor = Color.black;
							tempScale = 0;
						break;
						case(ProFlareElement.Type.Multi):
						//	for(int i2 = 0; i2 < _element.subElements.Count; i2++){
						//		_element.subElements[i2].colorFinal = Color.black;
						//	}
							tempScale = 0;
						break;
					}
				}
	            
				//Element Scale
				float finalScale = tempScale;
				
				if(Flares[F].useDynamicEdgeBoost){
					if(_element.OverrideDynamicEdgeBoost)
						finalScale = finalScale+((DynamicEdgeAmount)*_element.DynamicEdgeBoostOverride);
					else
						finalScale = finalScale+((DynamicEdgeAmount)*Flares[F].DynamicEdgeBoost);
				}
				
				if(Flares[F].useDynamicCenterBoost){
					if(_element.OverrideDynamicCenterBoost)
						finalScale = finalScale+(_element.DynamicCenterBoostOverride*centerBoost);
					else
						finalScale = finalScale+(Flares[F].DynamicCenterBoost*centerBoost);
				}
				
				if(finalScale < 0) finalScale = 0;
				
				if(Flares[F].UseAngleScale)
					finalScale *= AngleFallOff;
				
				if(Flares[F].useDistanceScale)
					finalScale *= distanceFalloff;
				
				finalScale *= FlareOcclusionData[F].occlusionScale;
				
				if(!_element.Visible)
					finalScale = 0;
				
				if(!isVisible)
					finalScale = 0;
				
				_element.ScaleFinal = finalScale;
	            
				
				//Apply final screen position.
				if(isVisible)
				switch(_element.type){
					case(ProFlareElement.Type.Single):{
						Vector3 pos = LensPosition*-_element.position;
						
						
						float zpos = LensPosition.z;
						
						if(VR_Mode){
							float flarePos = (_element.position*-1)-1; 
							zpos = LensPosition.z *((flarePos*VR_Depth)+1);
						} 
					
						Vector3 newVect = new Vector3(
	                                                  Mathf.Lerp(pos.x,LensPosition.x,_element.Anamorphic.x),
	                                                  Mathf.Lerp(pos.y,LensPosition.y,_element.Anamorphic.y),
													  zpos
	                                                  );
						
						newVect = newVect + _element.OffsetPostion;
						_element.OffsetPosition = newVect;
	                    
						}break;
					case(ProFlareElement.Type.Multi):{
					//Profiler.BeginSample("Scale Mutli Loop");
						for(int i2 = 0; i2 < _element.subElements.Count; i2++){
							SubElement _subElement = _element.subElements[i2];
	                        if(_element.useRangeOffset){
								
	                            Vector3 posM = LensPosition*-_subElement.position;
 							
								
								float zpos = LensPosition.z;
						
								if(VR_Mode){
									float  flarePos = (_subElement.position*-1)-1;
 									 
									zpos = LensPosition.z *((flarePos*VR_Depth)+1);
								} 
							
	                            Vector3 newVectM = new Vector3(
	                                                           Mathf.Lerp(posM.x,LensPosition.x,_element.Anamorphic.x),
	                                                           Mathf.Lerp(posM.y,LensPosition.y,_element.Anamorphic.y),
															   zpos
	                                                           );
	                            
	                            newVectM = newVectM + _element.OffsetPostion;
	                            
	                            _subElement.offset = newVectM;
								
 								
	                        }
	                        else
	                            _subElement.offset = LensPosition*-_element.position;
						}
						//Profiler.EndSample();
						}break;
				}
				
				//Apply final element angle.
				float angles = 0;
				
				if(_element.rotateToFlare){
					angles =  (Div180_PI)*(Mathf.Atan2(LensPosition.y - 0,LensPosition.x - 0));
				}
				angles = angles + (LensPosition.x*_element.rotationSpeed);
				
				angles = angles + (LensPosition.y*_element.rotationSpeed);
				
				angles = angles + (Time.time*_element.rotationOverTime);
				
				_element.FinalAngle = (_element.angle)+angles;
				
				
			}
			//Profiler.EndSample();
			
			
			
			if((!Flares[F].neverCull)&&useCulling){
				FlareOcclusion.CullingState _CullingState = FlareOcclusionData[F]._CullingState;
				 
				if(Flares[F].isVisible){
					visibleFlares++;
					
					
					if(FlareOcclusionData[F].occluded){
						if(_CullingState == FlareOcclusion.CullingState.Visible){
							//Debug.Log("Culled via Occlusion");
							FlareOcclusionData[F].CullTimer = cullFlaresAfterTime;
							_CullingState = FlareOcclusion.CullingState.CullCountDown;
						}
					}else{
 						if(_CullingState == FlareOcclusion.CullingState.Culled){
							//Debug.Log("Re show not occluded");
							culledFlaresNowVisiable = true;
							
						}
						_CullingState = FlareOcclusion.CullingState.Visible;
					}
					
						
				}else{
					if(_CullingState == FlareOcclusion.CullingState.Visible){
						//Debug.Log("Culled via Not Vis");
						FlareOcclusionData[F].CullTimer = cullFlaresAfterTime;
						_CullingState = FlareOcclusion.CullingState.CullCountDown;
					}
				}
				
				switch(_CullingState){
					case(FlareOcclusion.CullingState.Visible):{
					
						
					}break;
					case(FlareOcclusion.CullingState.CullCountDown):{
						
						FlareOcclusionData[F].CullTimer = FlareOcclusionData[F].CullTimer-Time.deltaTime;
						
						if(FlareOcclusionData[F].CullTimer < 0)
							_CullingState = FlareOcclusion.CullingState.CanCull;
						
					}break;
				}
				
				if(_CullingState != FlareOcclusion.CullingState.Culled)
					FlareOcclusionData[F].cullFader = Mathf.Clamp01(FlareOcclusionData[F].cullFader+(Time.deltaTime));
	
				
				if(_CullingState == FlareOcclusion.CullingState.CanCull)
					canCullFlares++;
				
				FlareOcclusionData[F]._CullingState = _CullingState;
				
			}
		 
			reshowCulledFlaresTimer += Time.deltaTime;
			
			if(reshowCulledFlaresTimer > reshowCulledFlaresAfter){
				reshowCulledFlaresTimer = 0;
				
				if(culledFlaresNowVisiable){
					//Debug.Log("A culled flare has now become visiable");
					dirty = true;
					culledFlaresNowVisiable = false;
					
				}
			}
			
			if(!dirty)
				if(canCullFlares >= cullFlaresAfterCount){
					
					Debug.Log("Culling Flares");dirty = true;	
				}
		}
		
		//Profiler.BeginSample("Rendering Bit");
		//Rendering Bit
		int count = 0;
		for(int i = 0; i < FlareElementsArray.Length; i++){
			switch(FlareElementsArray[i].type){
				case(ProFlareElement.Type.Single):
				{
					int extra = (count) * 4;
					
					//Check for DisabledPlayMode, then scale to zero. then skip over the rest of the calculations
					if(FlareElementsArray[i].flare.DisabledPlayMode){

					
						vertices[0+extra] = Vector3.zero;
						vertices[1+extra] = Vector3.zero;
						vertices[2+extra] = Vector3.zero;
						vertices[3+extra] = Vector3.zero;
						
					}
					
					_scale = ((FlareElementsArray[i].size*FlareElementsArray[i].Scale*0.01f)*FlareElementsArray[i].flare.GlobalScale)*FlareElementsArray[i].ScaleFinal;
					
					//Avoid Negative scaling
					if((_scale.x < 0)||(_scale.y < 0))
                        _scale = Vector3.zero;
					
					Vector3 offset = FlareElementsArray[i].OffsetPosition;
					
					float angle = FlareElementsArray[i].FinalAngle;
					
					_color = FlareElementsArray[i].ElementFinalColor;
					
					if(useBrightnessThreshold){
                        
						if(_color.a < BrightnessThreshold){
							_scale = Vector2.zero;
						}else if(_color.r+_color.g+_color.b < BrightnessThreshold){
							_scale = Vector2.zero;
						}
					}
					
					if(overdrawDebug)
						_color = new Color32(20,20,20,100);
					
					if(!FlareElementsArray[i].flare.DisabledPlayMode){
						//Precalculate some of the RotatePoint function to avoid repeat math.
						float r = angle * (PI_Div180);
						float ct = Mathf.Cos(r);
						float st = Mathf.Sin(r);
                      
						vertices[0+extra] = new Vector3((ct * (1*_scale.x) - st * (1*_scale.y)),(st * (1*_scale.x) + ct * (1*_scale.y)),0)+offset;
						vertices[1+extra] = new Vector3((ct * (1*_scale.x) - st * (-1*_scale.y)),(st * (1*_scale.x) + ct * (-1*_scale.y)),0)+offset;
						vertices[2+extra] = new Vector3((ct * (-1*_scale.x) - st * (1*_scale.y)),(st * (-1*_scale.x) + ct * (1*_scale.y)),0)+offset;
						vertices[3+extra] = new Vector3((ct * (-1*_scale.x) - st * (-1*_scale.y)),(st * (-1*_scale.x) + ct * (-1*_scale.y)),0)+offset;
					
					
					}
				

					Color32 _color32 = _color;
					colors[0+extra] = _color32;
					colors[1+extra] = _color32; 
					colors[2+extra] = _color32; 
					colors[3+extra] = _color32;
					
                    count++;
				}
                    break;
                    
				case(ProFlareElement.Type.Multi):
				{
					for(int i2 = 0; i2 < FlareElementsArray[i].subElements.Count; i2++){
						int extra2 = (count+i2) * 4;
                        
						//Check for DisabledPlayMode, then scale to zero. then skip over the rest of the calculations
                        if(FlareElementsArray[i].flare.DisabledPlayMode){
							vertices[0+extra2] = Vector3.zero;
							vertices[1+extra2] = Vector3.zero;
							vertices[2+extra2] = Vector3.zero;
							vertices[3+extra2] = Vector3.zero;
							continue;
						}
                        
                        ////Profiler.BeginSample("Calc Scale");
						_scale = (((FlareElementsArray[i].size*FlareElementsArray[i].Scale*0.01f)*FlareElementsArray[i].flare.GlobalScale)*FlareElementsArray[i].subElements[i2].scale)*FlareElementsArray[i].ScaleFinal;
						//Avoid Negative scaling
						if((_scale.x < 0)||(_scale.y < 0))
							_scale = Vector3.zero;

						////Profiler.EndSample();
					
						////Profiler.BeginSample("Calc Extras");

						Vector3 offset = FlareElementsArray[i].subElements[i2].offset;
                        
						float angle = FlareElementsArray[i].FinalAngle;
						
						angle += FlareElementsArray[i].subElements[i2].angle;
						
						_color = FlareElementsArray[i].subElements[i2].colorFinal;
                        
						if(useBrightnessThreshold){
							if(_color.a < BrightnessThreshold){
								_scale = Vector2.zero;
							}else if(_color.r+_color.g+_color.b < BrightnessThreshold){
								_scale = Vector2.zero;
							}
						}
                        
						if(overdrawDebug)
							_color = new Color32(20,20,20,100);
                       // //Profiler.EndSample();
						////Profiler.BeginSample("Set Pos and Rot");

						if(!FlareElementsArray[i].flare.DisabledPlayMode){
                            
							//Precalculate some of the RotatePoint function to avoid repeat math.
							float r = angle * (PI_Div180);
							float ct = Mathf.Cos(r);
							float st = Mathf.Sin(r);
						
							vertices[0+extra2] = new Vector3((ct * (1*_scale.x) - st * (1*_scale.y)),(st * (1*_scale.x) + ct * (1*_scale.y)),0)+offset;
							vertices[1+extra2] = new Vector3((ct * (1*_scale.x) - st * (-1*_scale.y)),(st * (1*_scale.x) + ct * (-1*_scale.y)),0)+offset;
							vertices[2+extra2] = new Vector3((ct * (-1*_scale.x) - st * (1*_scale.y)),(st * (-1*_scale.x) + ct * (1*_scale.y)),0)+offset;
							vertices[3+extra2] = new Vector3((ct * (-1*_scale.x) - st * (-1*_scale.y)),(st * (-1*_scale.x) + ct * (-1*_scale.y)),0)+offset;
							
						}
						////Profiler.EndSample();
						////Profiler.BeginSample("SetColors");
						 
							Color32 _color32 = _color;
							colors[0+extra2] = _color32;
							colors[1+extra2] = _color32;
							colors[2+extra2] = _color32;
							colors[3+extra2] = _color32;

 						////Profiler.EndSample();
					}
					count = count+FlareElementsArray[i].subElements.Count;
				}
                break;
			}
		}
		
		//Profiler.EndSample();

    }
}