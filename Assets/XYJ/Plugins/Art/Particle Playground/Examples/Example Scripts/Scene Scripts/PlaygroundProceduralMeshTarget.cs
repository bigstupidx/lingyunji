using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ParticlePlayground;

namespace UnityEngine {
/// <summary>
/// Particle Playground mesh target of a procedural mesh.
/// Assign this script to an empty GameObject in your scene. A Manipulator of your assigned Particle Playground system (particles) will be created.
/// 
/// References:
/// http://wiki.unity3d.com/index.php/ProceduralPrimitives#C.23_-_IsoSphere
/// http://answers.unity3d.com/questions/259127/does-anyone-have-any-code-to-subdivide-a-mesh-and.html
/// </summary>
	public class PlaygroundProceduralMeshTarget : MonoBehaviour {

		/// <summary>
		/// Assign the particles in Inspector.
		/// </summary>
		public PlaygroundParticlesC particles;
		public float manipulatorSize = 100f;
		public float manipulatorStrength = 20f;
		public float proceduralMeshSize = 3f;
		public int proceduralMeshRecursion = 1;
		public float rotationSpeed = 10f;

		/// <summary>
		/// The cached reference to your Manipulator.
		/// </summary>
		ManipulatorObjectC manipulator;
		int subdivisionFactor;

		void Start () {

			// Create an isosphere on this GameObject (could be any type of mesh configuration)
			IsoSphere.CreateMesh(gameObject, proceduralMeshRecursion, proceduralMeshSize, false);

			// Create a Manipulator on the particle system and set it up to work as a mesh target
			if (particles!=null) {

				// Create the new manipulator
				manipulator = PlaygroundC.ManipulatorObject(gameObject.transform, particles);

				// Set the manipulator to Property: Mesh Target and make it transition its particles
				manipulator.type = MANIPULATORTYPEC.Property;
				manipulator.property.type = MANIPULATORPROPERTYTYPEC.MeshTarget;
				manipulator.property.meshTarget.gameObject = gameObject;
				manipulator.property.transition = MANIPULATORPROPERTYTRANSITIONC.Linear;

			} else Debug.Log ("You must assign a Particle Playground system to this script.", gameObject);
		}
		
		void Update () {

			// Make particle system follow the mouse position
			particles.particleSystemTransform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));

			// Set size & strength of manipulator from this script
			manipulator.size = manipulatorSize;
			manipulator.strength = manipulatorStrength;

			// Rotate mesh transform
			transform.RotateAround(Vector3.zero, Vector3.up, -rotationSpeed * Time.deltaTime);

		}

		void OnGUI () {

			// Manipulator values
			GUI.Box (new Rect(0,0,220,125), "Manipulator");
			GUI.Label (new Rect(10, 30, 200, 30), "Strength: "+manipulatorStrength.ToString("f1"));
			manipulatorStrength = GUI.HorizontalSlider(new Rect(10, 55, 200, 30), manipulatorStrength, 0f, 30f);
			if (GUI.Button (new Rect(10, 85, 200, 30), "Transition: "+manipulator.property.transition.ToString())) {
				if (manipulator.property.transition == MANIPULATORPROPERTYTRANSITIONC.Linear)
					manipulator.property.transition = MANIPULATORPROPERTYTRANSITIONC.Lerp;
				else
					manipulator.property.transition = MANIPULATORPROPERTYTRANSITIONC.Linear;
			}

			// Turbulence values
			GUI.Box (new Rect(240,0,220,125), "Turbulence");
			GUI.Label (new Rect(250, 30, 200, 30), "Strength: "+particles.turbulenceStrength.ToString("f1"));
			particles.turbulenceStrength = GUI.HorizontalSlider(new Rect(250, 55, 200, 30), particles.turbulenceStrength, 0f, 50f);
			if (GUI.Button (new Rect(250, 85, 200, 30), "Type: "+particles.turbulenceType.ToString())) {
				if (particles.turbulenceType == TURBULENCETYPE.Perlin)
					particles.turbulenceType = TURBULENCETYPE.Simplex;
				else
					particles.turbulenceType = TURBULENCETYPE.Perlin;
			}

			// Mesh values
			GUI.Box (new Rect(Screen.width-220,0,220,125), "Mesh");
			GUI.Label (new Rect(Screen.width-210, 30, 200, 30), "Rotation Speed: "+rotationSpeed.ToString("f1"));
			rotationSpeed = GUI.HorizontalSlider(new Rect(Screen.width-210, 55, 200, 30), rotationSpeed, -100f, 100f);
			if (GUI.Button (new Rect(Screen.width-210, 85, 200, 30), "Subdivide x"+subdivisionFactor.ToString())) {
				subdivisionFactor++; subdivisionFactor=subdivisionFactor%4;

				// Subdivide or recreate the original mesh depending on subdivision factor
				if (subdivisionFactor==0)
					IsoSphere.CreateMesh(gameObject, 1, proceduralMeshSize, false);
				else
					SubdivideMesh.Subdivide(gameObject.GetComponent<MeshFilter>().mesh);

				UpdateTargetManipulator();
			}
		}

		/// <summary>
		/// All you need to do when changing your mesh is to call meshTarget.Update() of your manipulator property.
		/// Particles will get new targets and target sorting inside the calculation loop.
		/// </summary>
		void UpdateTargetManipulator () {
			manipulator.property.meshTarget.Update();
		}
	}

	/****************************************************************************************************
		Everything below is for working with the procedural mesh.
	****************************************************************************************************/

	/// <summary>
	/// Class for generating an isosphere.
	/// This is for demonstration purpose only - use any type of mesh configuration to target particles.
	/// Courtesy of Bérenger http://wiki.unity3d.com/index.php/ProceduralPrimitives#C.23_-_IsoSphere
	/// </summary>
	public class IsoSphere {
		private struct TriangleIndices {
			public int v1;
			public int v2;
			public int v3;
			
			public TriangleIndices(int v1, int v2, int v3) {
				this.v1 = v1;
				this.v2 = v2;
				this.v3 = v3;
			}
		}
		
		// return index of point in the middle of p1 and p2
		private static int getMiddlePoint(int p1, int p2, ref List<Vector3> vertices, ref Dictionary<long, int> cache, float radius) {
			// first check if we have it already
			bool firstIsSmaller = p1 < p2;
			long smallerIndex = firstIsSmaller ? p1 : p2;
			long greaterIndex = firstIsSmaller ? p2 : p1;
			long key = (smallerIndex << 32) + greaterIndex;
			
			int ret;
			if (cache.TryGetValue(key, out ret)) {
				return ret;
			}
			
			// not in cache, calculate it
			Vector3 point1 = vertices[p1];
			Vector3 point2 = vertices[p2];
			Vector3 middle = new Vector3
				(
					(point1.x + point2.x) / 2f, 
					(point1.y + point2.y) / 2f, 
					(point1.z + point2.z) / 2f
					);
			
			// add vertex makes sure point is on unit sphere
			int i = vertices.Count;
			vertices.Add( middle.normalized * radius ); 
			
			// store it, return index
			cache.Add(key, i);
			
			return i;
		}

		/// <summary>
		/// Creates the isosphere mesh and all necessary components to render it. The mesh will be attached to the passed in GameObject.
		/// </summary>
		/// <param name="gameObject">GameObject the isosphere should be created on.</param>
		/// <param name="recursionLevel">Recursion level (triangle resolution).</param>
		/// <param name="radius">Radius.</param>
		/// <param name="visible">Should the renderer be visible from start?</param>
		public static void CreateMesh(GameObject gameObject, int recursionLevel, float radius, bool visible) {
			MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
			if (renderer==null)
				renderer = gameObject.AddComponent<MeshRenderer>();
			renderer.enabled = visible;
			MeshFilter filter = gameObject.GetComponent<MeshFilter>();
			if (filter==null)
				filter = gameObject.AddComponent<MeshFilter>();
			Mesh mesh = filter.mesh;
			mesh.Clear();
			
			List<Vector3> vertList = new List<Vector3>();
			Dictionary<long, int> middlePointIndexCache = new Dictionary<long, int>();
			
			// create 12 vertices of a icosahedron
			float t = (1f + Mathf.Sqrt(5f)) / 2f;
			
			vertList.Add(new Vector3(-1f,  t,  0f).normalized * radius);
			vertList.Add(new Vector3( 1f,  t,  0f).normalized * radius);
			vertList.Add(new Vector3(-1f, -t,  0f).normalized * radius);
			vertList.Add(new Vector3( 1f, -t,  0f).normalized * radius);
			
			vertList.Add(new Vector3( 0f, -1f,  t).normalized * radius);
			vertList.Add(new Vector3( 0f,  1f,  t).normalized * radius);
			vertList.Add(new Vector3( 0f, -1f, -t).normalized * radius);
			vertList.Add(new Vector3( 0f,  1f, -t).normalized * radius);
			
			vertList.Add(new Vector3( t,  0f, -1f).normalized * radius);
			vertList.Add(new Vector3( t,  0f,  1f).normalized * radius);
			vertList.Add(new Vector3(-t,  0f, -1f).normalized * radius);
			vertList.Add(new Vector3(-t,  0f,  1f).normalized * radius);
			
			
			// create 20 triangles of the icosahedron
			List<TriangleIndices> faces = new List<TriangleIndices>();
			
			// 5 faces around point 0
			faces.Add(new TriangleIndices(0, 11, 5));
			faces.Add(new TriangleIndices(0, 5, 1));
			faces.Add(new TriangleIndices(0, 1, 7));
			faces.Add(new TriangleIndices(0, 7, 10));
			faces.Add(new TriangleIndices(0, 10, 11));
			
			// 5 adjacent faces 
			faces.Add(new TriangleIndices(1, 5, 9));
			faces.Add(new TriangleIndices(5, 11, 4));
			faces.Add(new TriangleIndices(11, 10, 2));
			faces.Add(new TriangleIndices(10, 7, 6));
			faces.Add(new TriangleIndices(7, 1, 8));
			
			// 5 faces around point 3
			faces.Add(new TriangleIndices(3, 9, 4));
			faces.Add(new TriangleIndices(3, 4, 2));
			faces.Add(new TriangleIndices(3, 2, 6));
			faces.Add(new TriangleIndices(3, 6, 8));
			faces.Add(new TriangleIndices(3, 8, 9));
			
			// 5 adjacent faces 
			faces.Add(new TriangleIndices(4, 9, 5));
			faces.Add(new TriangleIndices(2, 4, 11));
			faces.Add(new TriangleIndices(6, 2, 10));
			faces.Add(new TriangleIndices(8, 6, 7));
			faces.Add(new TriangleIndices(9, 8, 1));
			
			
			// refine triangles
			for (int i = 0; i < recursionLevel; i++) {
				List<TriangleIndices> faces2 = new List<TriangleIndices>();
				foreach (var tri in faces) {
					// replace triangle by 4 triangles
					int a = getMiddlePoint(tri.v1, tri.v2, ref vertList, ref middlePointIndexCache, radius);
					int b = getMiddlePoint(tri.v2, tri.v3, ref vertList, ref middlePointIndexCache, radius);
					int c = getMiddlePoint(tri.v3, tri.v1, ref vertList, ref middlePointIndexCache, radius);
					
					faces2.Add(new TriangleIndices(tri.v1, a, c));
					faces2.Add(new TriangleIndices(tri.v2, b, a));
					faces2.Add(new TriangleIndices(tri.v3, c, b));
					faces2.Add(new TriangleIndices(a, b, c));
				}
				faces = faces2;
			}
			
			mesh.vertices = vertList.ToArray();
			
			List< int > triList = new List<int>();
			for( int i = 0; i < faces.Count; i++ ) {
				triList.Add( faces[i].v1 );
				triList.Add( faces[i].v2 );
				triList.Add( faces[i].v3 );
			}		
			mesh.triangles = triList.ToArray();
			mesh.uv = new Vector2[ mesh.vertices.Length ];
			
			Vector3[] normales = new Vector3[ vertList.Count];
			for( int i = 0; i < normales.Length; i++ )
				normales[i] = vertList[i].normalized;

			mesh.normals = normales;
			
			mesh.RecalculateBounds();
		}
	}

	/// <summary>
	/// Class for subdividing a mesh vertices, normals and triangles. 
	/// This is for demonstration purpose only if you want more points for your particles to target. In this particular example you could as well use a higher recursion level for your isosphere.
	/// Courtesy of Bunny83 http://answers.unity3d.com/questions/259127/does-anyone-have-any-code-to-subdivide-a-mesh-and.html
	/// </summary>
	public class SubdivideMesh {
		static List<Vector3> vertices;
		static List<Vector3> normals;

		static List<int> indices;
		static Dictionary<uint,int> newVectices;
		
		static int GetNewVertex(int i1, int i2) {
			// We have to test both directions since the edge
			// could be reversed in another triangle
			uint t1 = ((uint)i1 << 16) | (uint)i2;
			uint t2 = ((uint)i2 << 16) | (uint)i1;
			if (newVectices.ContainsKey(t2))
				return newVectices[t2];
			if (newVectices.ContainsKey(t1))
				return newVectices[t1];
			// generate vertex:
			int newIndex = vertices.Count;
			newVectices.Add(t1,newIndex);
			
			// calculate new vertex
			vertices.Add((vertices[i1] + vertices[i2]) * 0.5f);
			normals.Add((normals[i1] + normals[i2]).normalized);

			return newIndex;
		}
		
		
		public static void Subdivide(Mesh mesh) {
			newVectices = new Dictionary<uint,int>();
			
			vertices = new List<Vector3>(mesh.vertices);
			normals = new List<Vector3>(mesh.normals);
			indices = new List<int>();
			
			int[] triangles = mesh.triangles;
			for (int i = 0; i < triangles.Length; i += 3) {
				int i1 = triangles[i + 0];
				int i2 = triangles[i + 1];
				int i3 = triangles[i + 2];
				
				int a = GetNewVertex(i1, i2);
				int b = GetNewVertex(i2, i3);
				int c = GetNewVertex(i3, i1);
				indices.Add(i1);   indices.Add(a);   indices.Add(c);
				indices.Add(i2);   indices.Add(b);   indices.Add(a);
				indices.Add(i3);   indices.Add(c);   indices.Add(b);
				indices.Add(a );   indices.Add(b);   indices.Add(c); // center triangle
			}
			mesh.vertices = vertices.ToArray();
			mesh.normals = normals.ToArray();
			mesh.triangles = indices.ToArray();
			
			// since this is a static function and it uses static variables
			// we should erase the arrays to free them:
			newVectices = null;
			vertices = null;
			normals = null;

			indices = null;
		}
	}
}
