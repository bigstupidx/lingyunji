using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace Eyesblack.EditorTools
{
	public class LightProbeTools
	{
		struct Index
		{
			public Index(int x_, int z_)
			{
				x = x_; z = z_;
			}

			public Index(float x_, float z_)
			{
				x = (int)Mathf.Round(x_); z = (int)Mathf.Round(z_);
			}

			public bool Equals(Index other)
			{
				return x == other.x && z == other.z;
			}

			public override int GetHashCode()
			{
				return x << 16 | z;
			}

			public int x, z;
		}

		const float VOLUM_LAYER2_HEIGHT = 4.0f;
		const float VOLUM_LAYER3_HEIGHT = 20.0f;
		class HitTester
		{
			public HitTester()
			{
				//do nth
			}
			public HitTester(Bounds bounds, float size, float height, float probeHeight)
			{
				m_bounds = bounds;

				m_min = bounds.min;
				m_max = bounds.max;
				m_max.y = m_max.y + m_probeHeight;
				m_min.y = m_min.y - m_probeHeight;
				m_distance = m_max.y - m_min.y;

				m_min.x = (float)System.Math.Floor(m_min.x - m_gridSize / 2);
				m_max.x = (float)System.Math.Ceiling(m_max.x + m_gridSize / 2);
				m_min.z = (float)System.Math.Floor(m_min.z - m_gridSize / 2);
				m_max.z = (float)System.Math.Ceiling(m_max.z + m_gridSize / 2);

				m_gridSize = size;
				m_collideHeight = height;
				m_probeHeight = probeHeight;
			}

            public bool HitMeshCollider(Vector3 pos)
			{
				Vector3 newPos = pos;
				newPos.y += m_collideHeight; //collider checking height for 40
				Collider[] colliders = Physics.OverlapCapsule(pos, newPos, 0.5f);
				if (colliders.Length < 1)
					return false;
				
				bool hitMesh = false;
				for (int i = 0; i < colliders.Length; i++ )
				{
					GameObject obj = PrefabUtility.FindPrefabRoot(colliders[i].gameObject);
					MeshRenderer[] renderers = obj.GetComponentsInChildren<MeshRenderer>();
					if (renderers.Length > 1)
					{
						hitMesh = true;
						break;
					}
				}

				RaycastHit hit;
				if (hitMesh && Physics.Raycast(pos, Vector3.up, out hit, 40))
				{
					if (hit.normal.y < 0)
						return true;
				}

				return false;
			}
 
			// true for adding probe
			public bool Hit(float x, float z, out Vector3 hitpoint)
			{
				m_ray.origin = new Vector3(x, m_max.y, z);
                int terrainId = LayerMask.NameToLayer("Terrains");
                int terrainMask = 1 << terrainId;
                bool v = Physics.Raycast(m_ray, out m_hit, 4096, terrainMask);

				if (!v)
				{
					hitpoint = m_hit.point;
					return false;
				}
				hitpoint = m_hit.point;
				hitpoint.y = (float)(System.Math.Floor(hitpoint.y / m_probeHeight) + 1) * m_probeHeight;
				//hitpoint.y = (int)hitpoint.y;

				if (HitMeshCollider(hitpoint))
					return false;

				return true;
			}

			private void AddProbeToDict(Vector3 probe)
			{
				if (HitMeshCollider(probe))
					return;

				m_probeHash.Add(new Index(probe.x, probe.z), probe);
			}

			public Vector3[] CalcProbes(Bounds bounds)
			{
				FindOriginProbes();

				FixSingleProbes();

				//  OptimizeProbes();

				return VolumizeProbes(bounds);
			}

			void FindOriginProbes()
			{
				for (float x = m_min.x; x < m_max.x; x += m_gridSize)
				{
					for (float z = m_min.z; z < m_max.z; z += m_gridSize)
					{
						Vector3 hitpoint;
						bool v = Hit(x, z, out hitpoint);
						if (!v) 
							continue;
						
						m_probeHash.Add(new Index(hitpoint.x, hitpoint.z), hitpoint);
					}
				}
			}

			void OptimizeProbes()
			{
			}

			void FixSingleProbes()
			{
				// find z line single probes
				for (float x = m_min.x; x < m_max.x; x += m_gridSize)
				{
					for (float z = m_min.z; z < m_max.z; z += m_gridSize)
					{
						Index index = new Index(x, z);
						bool found = m_probeHash.ContainsKey(index);
						if (!found)
							continue;

						Vector3 curProbe = m_probeHash[index];

						// find z line
						Index prev = new Index(x, z - m_gridSize);
						Index next = new Index(x, z + m_gridSize);
						bool foundPrev = m_probeHash.ContainsKey(prev);
						bool foundNext = m_probeHash.ContainsKey(next);
						if (!foundPrev && !foundNext)
						{
							Vector3 addProbe = curProbe;
							addProbe.z = curProbe.z - m_gridSize;
							AddProbeToDict(addProbe);
							addProbe.z = curProbe.z + m_gridSize;
							AddProbeToDict(addProbe);
						}
					}
				}

				// find x line single probes
				for (float x = m_min.x; x < m_max.x; x += m_gridSize)
				{
					for (float z = m_min.z; z < m_max.z; z += m_gridSize)
					{
						Index index = new Index(x, z);
						bool found = m_probeHash.ContainsKey(index);
						if (!found)
							continue;

						Vector3 curProbe = m_probeHash[index];

						// find z line
						Index prev = new Index(x - m_gridSize, z);
						Index next = new Index(x + m_gridSize, z);
						bool foundPrev = m_probeHash.ContainsKey(prev);
						bool foundNext = m_probeHash.ContainsKey(next);
						if (!foundPrev && !foundNext)
						{
							Vector3 addProbe = curProbe;
							addProbe.x = curProbe.x - m_gridSize;
							AddProbeToDict(addProbe);
							addProbe.x = curProbe.x + m_gridSize;
							AddProbeToDict(addProbe);
						}
					}
				}
			}

			Vector3[] VolumizeProbes(Bounds bounds)
			{
				int count = m_probeHash.Count;
				Vector3[] ret = new Vector3[count*3 + 8];
				m_probeHash.Values.CopyTo(ret, 0);

				// 2nd layer of volume probes
				for (int i = 0; i < count; i++)
				{
					ret[i + count] = ret[i];
					ret[i + count].y += VOLUM_LAYER2_HEIGHT;
					ret[i + count].y = (int)ret[i + count].y;
				}

				int index = count * 2;
				// 3rd layer of volume probes
				for (int i = 0; i < count; i++)
				{
					if (Random.Range(0, 8) == 0)
					{
						ret[index] = ret[i];
						ret[index].y += VOLUM_LAYER3_HEIGHT;
						ret[index].y = (int)ret[index].y;
						
						index++;
					}
				}

				//light probe aabb initialized
				Vector3 min = bounds.min;
				Vector3 max = bounds.max;
				min.y -= 500;
				max.y += 500;

				ret[index + 0] = new Vector3(min.x, min.y, min.z);
				ret[index + 1] = new Vector3(min.x, max.y, min.z);
				ret[index + 2] = new Vector3(max.x, min.y, min.z);
				ret[index + 3] = new Vector3(max.x, max.y, min.z);

				ret[index + 4] = new Vector3(min.x, min.y, max.z);
				ret[index + 5] = new Vector3(min.x, max.y, max.z);
				ret[index + 6] = new Vector3(max.x, min.y, max.z);
				ret[index + 7] = new Vector3(max.x, max.y, max.z);

				Vector3[] result = new Vector3[index+8];
				for (int i = 0; i < index + 8; i++ )
				{
					result[i] = ret[i];
				}

				//Debug.Log("count: " + count + "total count: " + index);
				return result;
			}

			// init
			Bounds m_bounds;
			float m_gridSize = 2f;
			float m_collideHeight = 10f;
			float m_probeHeight = 0.5f;
			const int OPTIMIZE_TIMES = 2;

			// runtime
			Dictionary<Index, Vector3> m_probeHash = new Dictionary<Index, Vector3>();
			Ray m_ray = new Ray(Vector3.zero, Vector3.down);
			RaycastHit m_hit;
			Vector3 m_min;
			Vector3 m_max;
			float m_distance;
		}

		public static void GenerateLightProbes(float gridSize, float height, float probeHeight)
		{
			if (gridSize < Mathf.Epsilon)
			{
				Debug.Log("间矩是个无效数值");
				return;
			}

			if (height < Mathf.Epsilon)
			{
				Debug.Log("碰撞高度是无效数值");
				return;
			}

			Bounds bounds = GetNavMeshBounds();

			if (bounds.extents.magnitude < Mathf.Epsilon)
			{
                Debug.Log("场景没有物件的layer属性设置为Terrain,请先完成相应设置.");
				return;
			}
            
			HitTester hitTester = new HitTester(bounds, gridSize, height, probeHeight);
			Vector3[] positions = hitTester.CalcProbes(bounds);

			// then set positions to component
			GameObject lightProbeHolder = GameObject.Find("LightProbeHolder");
			if (!lightProbeHolder)
			{
				lightProbeHolder = new GameObject("LightProbeHolder", typeof(LightProbeGroup));
			}

			if (!lightProbeHolder) return;
			LightProbeGroup lpg = lightProbeHolder.GetComponent<LightProbeGroup>();
			if (!lpg)
				return;

			lpg.probePositions = positions;
            Debug.Log("Generated " + lpg.probePositions.Length.ToString() + " probe points\n");

			HierarchyUtils.DestroyObjByName("LightProbeBox");

			//Lightmapping.Bake();
		}

		const float DIFF_THRESHOLD = 0.1f;
		const float BRIGHTNESS_THRESHOLD = 0.2f;

		class ProbeInfo
		{
			public Vector3 pos;
			public SphericalHarmonicsL2 sh;
			public bool active;

			public ProbeInfo(Vector3 _pos, SphericalHarmonicsL2 _sh)
			{
				pos = new Vector3(_pos.x, _pos.y, _pos.z);
				
				sh = _sh;
				active = true;
			}

			public void Active(bool _active)
			{
				active = _active;
			}

			public bool ColorValid()
			{
				return true;
			}

			public bool ColorDiff(SphericalHarmonicsL2 _sh)
			{
				// cut out the brightness less than threshold
				if (sh[0, 0] + sh[1, 0] + sh[2, 0] < BRIGHTNESS_THRESHOLD)
					return false; 

				for (int i = 0; i < 3; i++)//color
				{
					//for (int j = 0; j < 4; j++)//coefficent
					//{
					//	if (Mathf.Abs(sh[i, j] - _sh[i, j]) > DIFF_THRESHOLD)
					//		return true;
					//}

					// just check the first coefficent element
					if (Mathf.Abs(sh[i, 0] - _sh[i, 0]) > DIFF_THRESHOLD)
						return true;
				}
				return false;
			}
		}

		class ProbeOptimize
		{
			float m_gridSize = 2f;

			public Dictionary<Index, ProbeInfo> m_probeDict = null;
			public Vector3 m_min, m_max;

			public ProbeOptimize(Vector3 min, Vector3 max, float size)
			{
				m_min = min;
				m_max = max;

				m_min.x = (float)System.Math.Floor(min.x - m_gridSize / 2);
				m_max.x = (float)System.Math.Ceiling(max.x + m_gridSize / 2);
				m_min.z = (float)System.Math.Floor(min.z - m_gridSize / 2);
				m_max.z = (float)System.Math.Ceiling(max.z + m_gridSize / 2);

				m_probeDict = new Dictionary<Index, ProbeInfo>();
				m_probeDict.Clear();

				m_gridSize = size;
			}

			public int Size()
			{
				return m_probeDict.Count;
			}

			public int Extend()
			{
				return (int)Mathf.Min(m_max.x - m_min.x, m_max.z - m_min.z);
			}

			public void AddProbe(Index _index, ProbeInfo _info)
			{
				if (m_probeDict.ContainsKey(_index))
				{
					// TODO:
					//m_probeDict[_index] = _info;
				}
				else
				{
					m_probeDict.Add(_index, _info);
				}
			}

			public float FindFirstCol()
			{
				for (float x = m_min.x; x < m_max.x; x += m_gridSize)
				{
					for (float z = m_min.z; z < m_max.z; z += m_gridSize)
					{
						Index index = new Index(x, z);
						if (m_probeDict.ContainsKey(index))
							return x;
					}
				}
				//impossible
				return 0f;
			}

			public float FindFirstRow()
			{
				for (float z = m_min.z; z < m_max.z; z += m_gridSize)
				{
					for (float x = m_min.x; x < m_max.x; x += m_gridSize)
					{
						Index index = new Index(x, z);
						if (m_probeDict.ContainsKey(index))
							return z;
					}
				}
				//impossible
				return 0f;
			}
			
			public float FindNextInCol(float x, float beg)
			{
				for (float z = beg; z < m_max.z; z += m_gridSize)
				{
					Index index = new Index(x, z);
					if (m_probeDict.ContainsKey(index))
						return z;
				}
				return -1;
			}

			public void RemoveProbeInCol()
			{
				for (float x = m_min.x; x < m_max.x; x += m_gridSize)
				{
					float beg = m_min.z;
					float mid, end;
					while (beg < m_max.z)
					{
						beg = FindNextInCol(x, beg);
						mid = FindNextInCol(x, beg + m_gridSize);
						end = FindNextInCol(x, mid + m_gridSize);

						if (beg < 0 || mid < 0 || end < 0)
							break;

						Index iBeg = new Index(x, beg);
						Index iMid = new Index(x, mid);
						Index iEnd = new Index(x, end);

						SphericalHarmonicsL2 sh = new SphericalHarmonicsL2();
						//for (int ci = 0; ci < 3; ci++)
						{
							float temp1 = (float)(mid - beg) / (end - beg);
							float temp2 = (float)(end - mid) / (end - beg);
							sh = m_probeDict[iBeg].sh * temp1 + m_probeDict[iEnd].sh * temp2;
						}
						if (!m_probeDict[iMid].ColorDiff(sh))
						{
							m_probeDict.Remove(iMid);
						}
						beg = end;
					}
				}
			}
			public float FindNextInRow(float beg, float z)
			{
				for (float x = beg; x < m_max.x; x += m_gridSize)
				{
					Index index = new Index(x, z);
					if (m_probeDict.ContainsKey(index))
						return x;
				}
				return -1;
			}

			public void RemoveProbeInRow()
			{
				for (float z = m_min.z; z < m_max.z; z += m_gridSize)
				{
					float beg = m_min.x;
					float mid, end;
					while (beg < m_max.x)
					{
						beg = FindNextInRow(beg, z);
						mid = FindNextInRow(beg + m_gridSize, z);
						end = FindNextInRow(mid + m_gridSize, z);

						if (beg < 0 || mid < 0 || end < 0)
							break;

						Index iBeg = new Index(beg, z);
						Index iMid = new Index(mid, z);
						Index iEnd = new Index(end, z);

						SphericalHarmonicsL2 sh = new SphericalHarmonicsL2();
						//for (int ci = 0; ci < 3; ci++)
						{
							float temp1 = (float)(mid - beg) / (end - beg);
							float temp2 = (float)(end - mid) / (end - beg);
							sh = m_probeDict[iBeg].sh * temp1 + m_probeDict[iEnd].sh * temp2;
						}
						if (!m_probeDict[iMid].ColorDiff(sh))
						{
							m_probeDict.Remove(iMid);
						}
						beg = end;
					}
				}
			}
			
			public Vector3[] VolumizeProbes(Bounds bounds)
			{
				int count = m_probeDict.Count;
				Vector3[] ret = new Vector3[count*3 + 8];
				int index = 0;
				foreach (KeyValuePair<Index, ProbeInfo> kvp in m_probeDict)
				{
					ret[index] = kvp.Value.pos;
					index++;
				}

				// 2nd layer of volume probes
				for (int i = 0; i < count; i++)
				{
					ret[i + count] = ret[i];
					ret[i + count].y += VOLUM_LAYER2_HEIGHT;
					ret[i + count].y = (int)ret[i + count].y;
				}

				index = count * 2;
				// 3rd layer of volume probes
				for (int i = 0; i < count; i++)
				{
					if (Random.Range(0, 8) == 0)
					{
						ret[index] = ret[i];
						ret[index].y += VOLUM_LAYER3_HEIGHT;
						ret[index].y = (int)ret[index].y;

						index++;
					}
				}

				//light probe aabb initialized
				Vector3 min = bounds.min;
				Vector3 max = bounds.max;
				min.y -= 500;
				max.y += 500;

				ret[index + 0] = new Vector3(min.x, min.y, min.z);
				ret[index + 1] = new Vector3(min.x, max.y, min.z);
				ret[index + 2] = new Vector3(max.x, min.y, min.z);
				ret[index + 3] = new Vector3(max.x, max.y, min.z);

				ret[index + 4] = new Vector3(min.x, min.y, max.z);
				ret[index + 5] = new Vector3(min.x, max.y, max.z);
				ret[index + 6] = new Vector3(max.x, min.y, max.z);
				ret[index + 7] = new Vector3(max.x, max.y, max.z);

				Vector3[] result = new Vector3[index + 8];
				for (int i = 0; i < index + 8; i++)
				{
					result[i] = ret[i];
				}

				return result;
			}
		}

		static Bounds GetNavMeshBounds()
		{
			// clear collider in no-walkable game objects with NavMesh set
            int terrainId = LayerMask.NameToLayer("Terrains");
            GameObject[] objs = ObjectUtils.GetGameObjectsByLayer(terrainId);

			int areaId;
			for (int i = 0; i < objs.Length; i++)
			{
				areaId = GameObjectUtility.GetNavMeshArea(objs[i]);
				// whether non-walkable
				if (areaId == 1)
				{
					Collider[] colliders = objs[i].GetComponents<Collider>();
					for (int j = 0; j < colliders.Length; j++)
					{
						GameObject.DestroyImmediate(colliders[j]);
					}
					continue;
				}

				objs[i].AddComponent<BoxCollider>();
			}

			Vector3 center, size;
			List<Vector3> verts = new List<Vector3>();

			for (int i = 0; i < objs.Length; i++ )
			{
				areaId = GameObjectUtility.GetNavMeshArea(objs[i]);
				if (areaId != 1)
				{
					BoxCollider collider = objs[i].GetComponent<BoxCollider>();
					if (collider != null)
					{
						center = objs[i].transform.TransformPoint(collider.center);
						size = objs[i].transform.TransformPoint(collider.size);

						verts.Add(center + size);
						verts.Add(center - size);

						GameObject.DestroyImmediate(collider);
					}

					Terrain terrain = objs[i].GetComponent<Terrain>();
					if (terrain != null)
					{
						TerrainData data = terrain.terrainData;
						verts.Add(data.bounds.min);
						verts.Add(data.bounds.max);
					}
				}				
			}
			
			// calculate bound
			//UnityEngine.AI.NavMeshTriangulation tri = UnityEngine.AI.NavMesh.CalculateTriangulation();
			//Vector3[] verts = tri.vertices;
			//int[] ids = tri.indices;
			//
			Bounds bound;

			if (verts.Count > 0)
				bound = new Bounds(verts[0], Vector3.zero);
			else
				bound = new Bounds();

			foreach (Vector3 v in verts)
			{
				bound.Encapsulate(v);
			}

			return bound;
		}

		public static void OptimizeProbes(float gridSize)
		{
			if (gridSize < Mathf.Epsilon)
			{
				Debug.Log("间矩是个无效数值");
				return;
			}

			Bounds bounds = GetNavMeshBounds();

			if (bounds.extents.magnitude < Mathf.Epsilon)
			{
				Debug.Log("场景没有物件的layer属性设置为Terrain,请先完成相应设置.");
				return;
			}
			if (LightmapSettings.lightProbes == null || LightmapSettings.lightProbes.positions.Length < 1)
            {
                Debug.Log("场景还没有烘培，请先烘培场景");
                return;
            }

			GameObject lightProbeHolder = GameObject.Find("LightProbeHolder");
			LightProbeGroup lpg = lightProbeHolder.GetComponent<LightProbeGroup>();
			
            if (!lpg)
			{
                Debug.Log("场景没有light probe，请先通过[Tools/场景工具/LightProbe/LightProbes生成]菜单生成lightProbe");
				return;
			}

			int count1 = LightmapSettings.lightProbes.positions.Length;

			ProbeOptimize probeOpt = new ProbeOptimize(bounds.min, bounds.max, gridSize);

			// the last 8 lightProbes are aabb-box, and 3-layers(3rd layer doesn't cover all the 1st layer)
			for (int i = 0, len = (count1 - 8)/2; i < len; i++) 
			{
				Index index = new Index(LightmapSettings.lightProbes.positions[i].x,
										LightmapSettings.lightProbes.positions[i].z);

				ProbeInfo info = new ProbeInfo(LightmapSettings.lightProbes.positions[i],
											   LightmapSettings.lightProbes.bakedProbes[i]);
				probeOpt.AddProbe(index, info);
			}

			//clear probes
			int count2;
			int stepMax = (int)Mathf.Min(probeOpt.Extend(), 4);
			for (int i = stepMax; i > 1; i--)
			{
				probeOpt.RemoveProbeInRow();
				probeOpt.RemoveProbeInCol();
			}

			count2 = probeOpt.Size();
			Debug.Log("count1: " + count1 + "  ,count2: " + count2);

			Vector3[] positions = probeOpt.VolumizeProbes(bounds);
			lpg.probePositions = positions;
#if false
			//light probe aabbb-box
			GameObject lightProbeBox = GameObject.Find("LightProbeBox");
			if (!lightProbeBox)
			{
				lightProbeBox = new GameObject("LightProbeBox", typeof(LightProbeGroup));
			}

			if (!lightProbeBox) return;
			LightProbeGroup lpgBox = lightProbeBox.GetComponent<LightProbeGroup>();
			if (!lpgBox)
				return;

			//light probe aabb initialized
			Vector3 min = bounds.min;
			Vector3 max = bounds.max;
			min.y -= 500;
			max.y += 500;

			Vector3[] vecBound = new Vector3[8];
			vecBound[0] = new Vector3(min.x, min.y, min.z);
			vecBound[1] = new Vector3(min.x, max.y, min.z);
			vecBound[2] = new Vector3(max.x, min.y, min.z);
			vecBound[3] = new Vector3(max.x, max.y, min.z);

			vecBound[4] = new Vector3(min.x, min.y, max.z);
			vecBound[5] = new Vector3(min.x, max.y, max.z);
			vecBound[6] = new Vector3(max.x, min.y, max.z);
			vecBound[7] = new Vector3(max.x, max.y, max.z);

			lpgBox.probePositions = vecBound;
			Debug.Log("gernerate extra aabb lightProbes");
#endif
			//Lightmapping.Bake();
		}
	}
}