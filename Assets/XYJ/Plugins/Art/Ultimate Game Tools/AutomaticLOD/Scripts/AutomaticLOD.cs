using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UltimateGameTools.MeshSimplifier;

public class AutomaticLOD : MonoBehaviour
{
  [Serializable]
  public enum EvalMode
  {
    CameraDistance,
    ScreenCoverage
  }

  [Serializable]
  public enum LevelsToGenerate
  {
    _1 = 1,
    _2,
    _3,
    _4,
    _5,
    _6
  }

  [Serializable]
  public class LODLevelData
  {
    public float m_fScreenCoverage;
    public float m_fMaxCameraDistance;
    public float m_fMeshVerticesAmount; // 0.0 - 1.0
    public int   m_nColorEditorBarIndex;
    public Mesh  m_mesh;

    public LODLevelData GetCopy()
    {
      LODLevelData data = new LODLevelData();

      data.m_fScreenCoverage      = m_fScreenCoverage;
      data.m_fMaxCameraDistance   = m_fMaxCameraDistance;
      data.m_fMeshVerticesAmount  = m_fMeshVerticesAmount;
      data.m_nColorEditorBarIndex = m_nColorEditorBarIndex;
      data.m_mesh                 = m_mesh;

      return data;
    }
  }

  [HideInInspector]
  public Mesh m_originalMesh = null;

  [HideInInspector]
  public EvalMode m_evalMode = EvalMode.ScreenCoverage;

  [HideInInspector]
  public bool m_bEnablePrefabUsage = false;

  [HideInInspector]
  public string m_strAssetPath = null;

  [HideInInspector]
  public float m_fMaxCameraDistance = 1000.0f;

  [HideInInspector]
  public int m_nColorEditorBarNewIndex = 0;

  [HideInInspector]
  public List<LODLevelData> m_listLODLevels;

  [HideInInspector]
  public AutomaticLOD m_LODObjectRoot;

  [HideInInspector]
  public List<AutomaticLOD> m_listDependentChildren = new List<AutomaticLOD>();

  public bool m_bExpandRelevanceSpheres = true;

  public RelevanceSphere[] m_aRelevanceSpheres = null;

  [SerializeField]
  private Simplifier m_meshSimplifier = null;

  [SerializeField]
  private bool m_bGenerateIncludeChildren = true;

  [SerializeField]
  private LevelsToGenerate m_levelsToGenerate = LevelsToGenerate._3;

  [SerializeField]
  private bool m_bOverrideRootSettings = false;

  [SerializeField, HideInInspector]
  private bool m_bLODDataDirty = true;

  [SerializeField, HideInInspector]
  private AutomaticLOD m_LODObjectRootPersist = null; // Will persist even if you exclude it from the LOD tree

  private bool m_bUseAutomaticCameraLODSwitch = true;

  void Awake()
  {
    if (m_originalMesh)
    {
      MeshFilter meshFilter = GetComponent<MeshFilter>();

      if(meshFilter != null)
      {
        meshFilter.sharedMesh = m_originalMesh;
      }
      else
      {
        SkinnedMeshRenderer skin = GetComponent<SkinnedMeshRenderer>();

        if (skin != null)
        {
          skin.sharedMesh = m_originalMesh;
        }
      }
    }

    m_localCenter = transform.InverseTransformPoint(ComputeWorldCenter());

    m_cachedFrameLODLevel = new Dictionary<Camera, int>();
  }

  void Update()
  {
    m_cachedFrameLODLevel.Clear();
  }

    int lastLODLevels = -1;

  void OnWillRenderObject()
  {
        Camera mainCamera = Camera.current;

#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            if (mainCamera.name == "Preview Camera" || mainCamera.name == "SceneCamera")
                return;
        }
#endif

    if(m_bUseAutomaticCameraLODSwitch == false)
    {
      return;
    }

    if (m_LODObjectRoot != null || m_LODObjectRootPersist != null)
    {
      // Dependent object
            int nLODLevel = m_LODObjectRootPersist != null ? m_LODObjectRootPersist.GetLODLevelUsingCamera(mainCamera) : m_LODObjectRoot.GetLODLevelUsingCamera(mainCamera);

            if (nLODLevel != -1 && lastLODLevels != nLODLevel)
            {
                lastLODLevels = nLODLevel;
        SwitchToLOD(nLODLevel, false);
      }
    }
    else
    {
      if (m_originalMesh)
      {
        // Root object with mesh data

                int nLODLevel = GetLODLevelUsingCamera(mainCamera);

                if (nLODLevel != -1 && lastLODLevels != nLODLevel)
        {
                    lastLODLevels = nLODLevel;
          SwitchToLOD(nLODLevel, false);
        }
      }
    }
  }

#if UNITY_EDITOR

  void OnDrawGizmos()
  {
    if (m_LODObjectRoot != null)
    {
      if (m_LODObjectRoot.m_bExpandRelevanceSpheres == false)
      {
        return;
      }
    }
    else
    {
      if (m_bExpandRelevanceSpheres == false)
      {
        return;
      }
    }

    Gizmos.color = Color.red;

    RelevanceSphere[] aRelevanceSpheres = m_LODObjectRoot != null ? m_LODObjectRoot.m_aRelevanceSpheres : m_aRelevanceSpheres;

    if (aRelevanceSpheres == null)
    {
      return;
    }

    bool bDrawVertices = false;

    for (int i = 0; i < UnityEditor.Selection.gameObjects.Length; i++)
    {
      if (((UnityEditor.Selection.gameObjects[i] == this.gameObject) && m_LODObjectRoot == null) || ((m_LODObjectRoot != null) && (UnityEditor.Selection.gameObjects[i] == m_LODObjectRoot.gameObject)))
      {
        bDrawVertices = true;
      }
    }

    if (bDrawVertices == false)
    {
      return;
    }

    Vector3[] aVerticesWorld = Simplifier.GetWorldVertices(this.gameObject);

    if(aVerticesWorld == null)
    {
      return;
    }

    Matrix4x4[] aSphereMatrices = new Matrix4x4[aRelevanceSpheres.Length];

    for (int nSphere = 0; nSphere < aRelevanceSpheres.Length; nSphere++)
    {
      aSphereMatrices[nSphere] = Matrix4x4.TRS(aRelevanceSpheres[nSphere].m_v3Position, Quaternion.Euler(aRelevanceSpheres[nSphere].m_v3Rotation), aRelevanceSpheres[nSphere].m_v3Scale).inverse;
    }

    for (int nVertex = 0; nVertex < aVerticesWorld.Length; nVertex++)
    {
      for (int nSphere = 0; nSphere < aRelevanceSpheres.Length; nSphere++)
      {
        if (aRelevanceSpheres[nSphere].m_bExpanded)
        {
          Vector3 v3VertexSphereLocal = aSphereMatrices[nSphere].MultiplyPoint(aVerticesWorld[nVertex]);

          if (v3VertexSphereLocal.magnitude <= 0.5)
          {
            Gizmos.DrawCube(aVerticesWorld[nVertex], Vector3.one * UnityEditor.HandleUtility.GetHandleSize(aVerticesWorld[nVertex]) * 0.05f);
            break;
          }
        }
      }
    }
  }

#endif

  public static bool HasValidMeshData(GameObject go)
  {
    MeshFilter meshFilter = go.GetComponent<MeshFilter>();

    if (meshFilter != null)
    {
      return true;
    }
    else
    {
      SkinnedMeshRenderer skin = go.GetComponent<SkinnedMeshRenderer>();

      if (skin != null)
      {
        return true;
      }
    }

    return false;
  }

  public static bool IsRootOrBelongsToLODTree(AutomaticLOD automaticLOD, AutomaticLOD root)
  {
    if(automaticLOD == null)
    {
      return false;
    }

    return (automaticLOD.m_LODObjectRoot == null) || (automaticLOD.m_LODObjectRoot == root) || (automaticLOD == root) || (automaticLOD.m_LODObjectRoot == root.m_LODObjectRoot);
  }

  public int GetNumberOfLevelsToGenerate()
  {
    return (int)m_levelsToGenerate;
  }

  public bool IsGenerateIncludeChildrenActive()
  {
    return m_bGenerateIncludeChildren;
  }

  public bool IsRootAutomaticLOD()
  {
    return m_LODObjectRoot == null;
  }

  public bool HasDependentChildren()
  {
    return m_listDependentChildren != null && m_listDependentChildren.Count > 0;
  }

  public bool HasLODDataDirty()
  {
    return m_bLODDataDirty;
  }

  public bool SetLODDataDirty(bool bDirty)
  {
    return m_bLODDataDirty = bDirty;
  }

  public int GetLODLevelCount()
  {
    return m_listLODLevels != null ? m_listLODLevels.Count : 0;
  }

  public float ComputeScreenCoverage(Camera camera)
  {
    float fMinX = float.MaxValue;
    float fMinY = float.MaxValue;
    float fMaxX = float.MinValue;
    float fMaxY = float.MinValue;

    if (m_originalMesh)
    {
      for (int i = 0; i < 2; i++)
      {
        Vector3 v3Point  = i == 0 ? GetComponent<Renderer>().bounds.min : GetComponent<Renderer>().bounds.max;
        Vector3 v3Screen = camera.WorldToViewportPoint(v3Point);

        if (v3Screen.x < fMinX) fMinX = v3Screen.x;
        if (v3Screen.y < fMinY) fMinY = v3Screen.y;
        if (v3Screen.x > fMaxX) fMaxX = v3Screen.x;
        if (v3Screen.y > fMaxY) fMaxY = v3Screen.y;
      }
    }

    for (int nObject = 0; nObject < m_listDependentChildren.Count; nObject++)
    {
      if (m_listDependentChildren[nObject] != null && m_listDependentChildren[nObject].m_originalMesh)
      {
        for (int i = 0; i < 2; i++)
        {
          Vector3 v3Point  = i == 0 ? m_listDependentChildren[nObject].GetComponent<Renderer>().bounds.min : m_listDependentChildren[nObject].GetComponent<Renderer>().bounds.max;
          Vector3 v3Screen = camera.WorldToViewportPoint(v3Point);

          if (v3Screen.x < fMinX) fMinX = v3Screen.x;
          if (v3Screen.y < fMinY) fMinY = v3Screen.y;
          if (v3Screen.x > fMaxX) fMaxX = v3Screen.x;
          if (v3Screen.y > fMaxY) fMaxY = v3Screen.y;
        }
      }
    }

    float fScreenCoveragePixels = (fMaxX - fMinX) * (fMaxY - fMinY);

    return fScreenCoveragePixels;
  }

  public Vector3 ComputeWorldCenter()
  {
    float fMinX = float.MaxValue;
    float fMinY = float.MaxValue;
    float fMinZ = float.MaxValue;
    float fMaxX = float.MinValue;
    float fMaxY = float.MinValue;
    float fMaxZ = float.MinValue;

    if (m_originalMesh)
    {
      for (int i = 0; i < 2; i++)
      {
        Vector3 v3World = i == 0 ? GetComponent<Renderer>().bounds.min : GetComponent<Renderer>().bounds.max;

        if (v3World.x < fMinX) fMinX = v3World.x;
        if (v3World.y < fMinY) fMinY = v3World.y;
        if (v3World.z < fMinZ) fMinZ = v3World.z;
        if (v3World.x > fMaxX) fMaxX = v3World.x;
        if (v3World.y > fMaxY) fMaxY = v3World.y;
        if (v3World.z > fMaxZ) fMaxZ = v3World.z;
      }
    }

    for (int nObject = 0; nObject < m_listDependentChildren.Count; nObject++)
    {
      if (m_listDependentChildren[nObject] != null && m_listDependentChildren[nObject].m_originalMesh)
      {
        for (int i = 0; i < 2; i++)
        {
          Vector3 v3World = i == 0 ? m_listDependentChildren[nObject].GetComponent<Renderer>().bounds.min : m_listDependentChildren[nObject].GetComponent<Renderer>().bounds.max;

          if (v3World.x < fMinX) fMinX = v3World.x;
          if (v3World.y < fMinY) fMinY = v3World.y;
          if (v3World.z < fMinZ) fMinZ = v3World.z;
          if (v3World.x > fMaxX) fMaxX = v3World.x;
          if (v3World.y > fMaxY) fMaxY = v3World.y;
          if (v3World.z > fMaxZ) fMaxZ = v3World.z;
        }
      }
    }

    Vector3 v3Min = new Vector3(fMinX, fMinY, fMinZ);
    Vector3 v3Max = new Vector3(fMaxX, fMaxY, fMaxZ);

    return (v3Max + v3Min) * 0.5f;
  }

  public float ComputeViewSpaceBounds(Vector3 v3CameraPos, Vector3 v3CameraDir, Vector3 v3CameraUp, out Vector3 v3Min, out Vector3 v3Max, out Vector3 v3Center)
  {
    Matrix4x4 mtxView = Matrix4x4.TRS(v3CameraPos, Quaternion.LookRotation(v3CameraDir, v3CameraUp), Vector3.one);

    float fMinX = float.MaxValue;
    float fMinY = float.MaxValue;
    float fMinZ = float.MaxValue;
    float fMaxX = float.MinValue;
    float fMaxY = float.MinValue;
    float fMaxZ = float.MinValue;

    v3Center = mtxView.inverse.MultiplyPoint(transform.TransformPoint(Vector3.zero));

    if (m_originalMesh)
    {
      for (int i = 0; i < 2; i++)
      {
        Vector3 v3World = i == 0 ? GetComponent<Renderer>().bounds.min : GetComponent<Renderer>().bounds.max;
        Vector3 v3View  = mtxView.inverse.MultiplyPoint(v3World);

        if (v3View.x < fMinX) fMinX = v3View.x;
        if (v3View.y < fMinY) fMinY = v3View.y;
        if (v3View.z < fMinZ) fMinZ = v3View.z;
        if (v3View.x > fMaxX) fMaxX = v3View.x;
        if (v3View.y > fMaxY) fMaxY = v3View.y;
        if (v3View.z > fMaxZ) fMaxZ = v3View.z;
      }
    }

    for (int nObject = 0; nObject < m_listDependentChildren.Count; nObject++)
    {
      if (m_listDependentChildren[nObject] != null && m_listDependentChildren[nObject].m_originalMesh)
      {
        for (int i = 0; i < 2; i++)
        {
          Vector3 v3World = i == 0 ? m_listDependentChildren[nObject].GetComponent<Renderer>().bounds.min : m_listDependentChildren[nObject].GetComponent<Renderer>().bounds.max;
          Vector3 v3View  = mtxView.inverse.MultiplyPoint(v3World);

          if (v3View.x < fMinX) fMinX = v3View.x;
          if (v3View.y < fMinY) fMinY = v3View.y;
          if (v3View.z < fMinZ) fMinZ = v3View.z;
          if (v3View.x > fMaxX) fMaxX = v3View.x;
          if (v3View.y > fMaxY) fMaxY = v3View.y;
          if (v3View.z > fMaxZ) fMaxZ = v3View.z;
        }
      }
    }

    v3Min = new Vector3(fMinX, fMinY, fMinZ);
    v3Max = new Vector3(fMaxX, fMaxY, fMaxZ);

    float fViewSurfaceArea = (fMaxX - fMinX) * (fMaxY - fMinY);

    return fViewSurfaceArea;
  }

  public void SetAutomaticCameraLODSwitch(bool bEnabled)
  {
    SetAutomaticCameraLODSwitchRecursive(this, this.gameObject, bEnabled);
  }

  private static void SetAutomaticCameraLODSwitchRecursive(AutomaticLOD root, GameObject gameObject, bool bEnabled)
  {
    AutomaticLOD automaticLOD = gameObject.GetComponent<AutomaticLOD>();

    if(automaticLOD != null && IsRootOrBelongsToLODTree(automaticLOD, root))
    {
      automaticLOD.m_bUseAutomaticCameraLODSwitch = bEnabled;
    }

    for (int nChild = 0; nChild < gameObject.transform.childCount; nChild++)
    {
      SetAutomaticCameraLODSwitchRecursive(root, gameObject.transform.GetChild(nChild).gameObject, bEnabled);
    }
  }

  public void SetLODLevels(List<LODLevelData> listLODLevelData, EvalMode evalMode, float fMaxCameraDistance, bool bRecurseIntoChildren)
  {
    m_listLODLevels = listLODLevelData;

    m_fMaxCameraDistance      = fMaxCameraDistance;
    m_nColorEditorBarNewIndex = listLODLevelData.Count;
    m_evalMode                = evalMode;
    m_LODObjectRoot           = null;
    m_LODObjectRootPersist    = null;

    m_listDependentChildren = new List<AutomaticLOD>();

    if (bRecurseIntoChildren)
    {
      for (int nChild = 0; nChild < transform.childCount; nChild++)
      {
        SetLODLevelsRecursive(this, transform.GetChild(nChild).gameObject);
      }
    }
  }

  private static void SetLODLevelsRecursive(AutomaticLOD root, GameObject gameObject)
  {
    AutomaticLOD automaticLOD = gameObject.GetComponent<AutomaticLOD>();

    bool bProcess = false;

    if(automaticLOD != null)
    {
      if (IsRootOrBelongsToLODTree(automaticLOD, root))
      {
        bProcess = true;
      }
    }
    else
    {
      if(HasValidMeshData(gameObject))
      {
        automaticLOD = gameObject.AddComponent<AutomaticLOD>();
        bProcess = true;
      }
    }

    if(bProcess && automaticLOD)
    {
      automaticLOD.m_fMaxCameraDistance      = root.m_fMaxCameraDistance;
      automaticLOD.m_nColorEditorBarNewIndex = root.m_nColorEditorBarNewIndex;
      automaticLOD.m_evalMode                = root.m_evalMode;
      automaticLOD.m_listLODLevels           = new List<LODLevelData>();
      automaticLOD.m_LODObjectRoot           = root;
      automaticLOD.m_LODObjectRootPersist    = root;

      root.m_listDependentChildren.Add(automaticLOD);

      for (int i = 0; i < root.m_listLODLevels.Count; i++)
      {
        automaticLOD.m_listLODLevels.Add(root.m_listLODLevels[i].GetCopy());
        automaticLOD.m_listLODLevels[i].m_mesh = CreateNewEmptyMesh(automaticLOD);
      }
    }

    for (int nChild = 0; nChild < gameObject.transform.childCount; nChild++)
    {
      SetLODLevelsRecursive(root, gameObject.transform.GetChild(nChild).gameObject);
    }
  }

  public void AddLODLevel(int nLevel, bool bBefore, bool bCreateMesh, bool bRecurseIntoChildren)
  {
    AddLODLevelRecursive(this, this.gameObject, nLevel, bBefore, bCreateMesh, bRecurseIntoChildren);
  }

  public static void AddLODLevelRecursive(AutomaticLOD root, GameObject gameObject, int nLevel, bool bBefore, bool bCreateMesh, bool bRecurseIntoChildren)
  {
    if (Simplifier.Cancelled)
    {
      return;
    }

    AutomaticLOD automaticLOD = gameObject.GetComponent<AutomaticLOD>();

    if (automaticLOD != null)
    {
      if (IsRootOrBelongsToLODTree(automaticLOD, root))
      {
        bool bProcess = true;

        if (automaticLOD.m_listLODLevels == null)
        {
          bProcess = false;
        }
        else
        {
          if (nLevel < 0 || nLevel >= automaticLOD.m_listLODLevels.Count)
          {
            bProcess = false;
          }
        }

        if(bProcess)
        {
          LODLevelData data = new LODLevelData();

          if (bBefore)
          {
            if (nLevel == 0)
            {
              data.m_fScreenCoverage      = automaticLOD.m_listLODLevels[0].m_fScreenCoverage;
              data.m_fMaxCameraDistance   = automaticLOD.m_listLODLevels[0].m_fMaxCameraDistance;
              data.m_fMeshVerticesAmount  = 1.0f;
              data.m_nColorEditorBarIndex = automaticLOD.m_nColorEditorBarNewIndex++;

              if (automaticLOD.m_listLODLevels.Count > 1)
              {
                automaticLOD.m_listLODLevels[0].m_fScreenCoverage    = (automaticLOD.m_listLODLevels[0].m_fScreenCoverage    + automaticLOD.m_listLODLevels[1].m_fScreenCoverage)    / 2.0f;
                automaticLOD.m_listLODLevels[0].m_fMaxCameraDistance = (automaticLOD.m_listLODLevels[0].m_fMaxCameraDistance + automaticLOD.m_listLODLevels[1].m_fMaxCameraDistance) / 2.0f;
              }
              else
              {
                automaticLOD.m_listLODLevels[0].m_fScreenCoverage    *= 0.5f;
                automaticLOD.m_listLODLevels[0].m_fMaxCameraDistance *= 2.0f;

                if(Mathf.Approximately(automaticLOD.m_listLODLevels[0].m_fMaxCameraDistance, 0.0f))
                {
                  automaticLOD.m_listLODLevels[0].m_fMaxCameraDistance = automaticLOD.m_fMaxCameraDistance * 0.5f;
                }
              }
            }
            else
            {
              data.m_fScreenCoverage      = (automaticLOD.m_listLODLevels[nLevel - 1].m_fScreenCoverage     + automaticLOD.m_listLODLevels[nLevel].m_fScreenCoverage)     / 2.0f;
              data.m_fMaxCameraDistance   = (automaticLOD.m_listLODLevels[nLevel - 1].m_fMaxCameraDistance  + automaticLOD.m_listLODLevels[nLevel].m_fMaxCameraDistance)  / 2.0f;
              data.m_fMeshVerticesAmount  = (automaticLOD.m_listLODLevels[nLevel - 1].m_fMeshVerticesAmount + automaticLOD.m_listLODLevels[nLevel].m_fMeshVerticesAmount) / 2.0f;
              data.m_nColorEditorBarIndex = automaticLOD.m_nColorEditorBarNewIndex++;
            }

            if(bCreateMesh)
            {
              if (data.m_mesh == null)
              {
                data.m_mesh = CreateNewEmptyMesh(automaticLOD);
              }
            }

            automaticLOD.m_listLODLevels.Insert(nLevel, data);
          }
          else
          {
            int nLastLevel = automaticLOD.m_listLODLevels.Count - 1;

            if (nLevel == nLastLevel)
            {
              data.m_fScreenCoverage      = automaticLOD.m_listLODLevels[nLastLevel].m_fScreenCoverage * 0.5f;
              data.m_fMaxCameraDistance = (automaticLOD.m_listLODLevels[nLastLevel].m_fMaxCameraDistance + automaticLOD.m_fMaxCameraDistance) * 0.5f;
              data.m_fMeshVerticesAmount  = automaticLOD.m_listLODLevels[nLastLevel].m_fMeshVerticesAmount * 0.5f;
              data.m_nColorEditorBarIndex = automaticLOD.m_nColorEditorBarNewIndex++;
            }
            else
            {
              data.m_fScreenCoverage      = (automaticLOD.m_listLODLevels[nLevel + 1].m_fScreenCoverage     + automaticLOD.m_listLODLevels[nLevel].m_fScreenCoverage)     / 2.0f;
              data.m_fMaxCameraDistance   = (automaticLOD.m_listLODLevels[nLevel + 1].m_fMaxCameraDistance  + automaticLOD.m_listLODLevels[nLevel].m_fMaxCameraDistance)  / 2.0f;
              data.m_fMeshVerticesAmount  = (automaticLOD.m_listLODLevels[nLevel + 1].m_fMeshVerticesAmount + automaticLOD.m_listLODLevels[nLevel].m_fMeshVerticesAmount) / 2.0f;
              data.m_nColorEditorBarIndex = automaticLOD.m_nColorEditorBarNewIndex++;
            }

            if (bCreateMesh)
            {
              if (data.m_mesh == null)
              {
                data.m_mesh = CreateNewEmptyMesh(automaticLOD);
              }
            }

            if (nLevel == nLastLevel)
            {
              automaticLOD.m_listLODLevels.Add(data);
            }
            else
            {
              automaticLOD.m_listLODLevels.Insert(nLevel + 1, data);
            }
          }
        }
      }
    }

    if (bRecurseIntoChildren)
    {
      for (int nChild = 0; nChild < gameObject.transform.childCount; nChild++)
      {
        AddLODLevelRecursive(root, gameObject.transform.GetChild(nChild).gameObject, nLevel, bBefore, bCreateMesh, bRecurseIntoChildren);

        if (Simplifier.Cancelled)
        {
          return;
        }
      }
    }
  }

  public void RemoveLODLevel(int nLevel, bool bDeleteMesh, bool bRecurseIntoChildren)
  {
    RemoveLODLevelRecursive(this, this.gameObject, nLevel, bDeleteMesh, bRecurseIntoChildren);
  }

  public static void RemoveLODLevelRecursive(AutomaticLOD root, GameObject gameObject, int nLevel, bool bDeleteMesh, bool bRecurseIntoChildren)
  {
    AutomaticLOD automaticLOD = gameObject.GetComponent<AutomaticLOD>();

    if (automaticLOD != null)
    {
      if (IsRootOrBelongsToLODTree(automaticLOD, root))
      {
        bool bProcess = true;

        if (automaticLOD.m_listLODLevels == null)
        {
          bProcess = false;
        }
        else
        {
          if (nLevel < 0 || nLevel >= automaticLOD.m_listLODLevels.Count || automaticLOD.m_listLODLevels.Count == 1)
          {
            bProcess = false;
          }
        }

        if (bProcess)
        {
          if (bDeleteMesh)
          {
            if (automaticLOD.m_listLODLevels[nLevel].m_mesh != null)
            {
              automaticLOD.m_listLODLevels[nLevel].m_mesh.Clear();
            }
          }

          if (nLevel == 0)
          {
            if(automaticLOD.m_listLODLevels.Count > 1)
            {
              automaticLOD.m_listLODLevels[1].m_fMaxCameraDistance = 0.0f;
              automaticLOD.m_listLODLevels[1].m_fScreenCoverage    = 1.0f;
            }
          }

          automaticLOD.m_listLODLevels.RemoveAt(nLevel);
        }
      }
    }

    if (bRecurseIntoChildren)
    {
      for (int nChild = 0; nChild < gameObject.transform.childCount; nChild++)
      {
        RemoveLODLevelRecursive(root, gameObject.transform.GetChild(nChild).gameObject, nLevel, bDeleteMesh, bRecurseIntoChildren);
      }
    }
  }

  public Simplifier GetMeshSimplifier()
  {
    return m_meshSimplifier;
  }

  public void ComputeLODData(bool bRecurseIntoChildren, Simplifier.ProgressDelegate progress = null)
  {
    ComputeLODDataRecursive(this, this.gameObject, bRecurseIntoChildren, progress);
  }

  private void ComputeLODDataRecursive(AutomaticLOD root, GameObject gameObject, bool bRecurseIntoChildren, Simplifier.ProgressDelegate progress = null)
  {
    if (Simplifier.Cancelled)
    {
      return;
    }

    AutomaticLOD automaticLOD = gameObject.GetComponent<AutomaticLOD>();

    if(automaticLOD != null)
    {
      if (IsRootOrBelongsToLODTree(automaticLOD, root))
      {
        automaticLOD.FreeLODData(false);

        MeshFilter meshFilter = automaticLOD.GetComponent<MeshFilter>();

        if (meshFilter != null)
        {
          if (automaticLOD.m_originalMesh == null)
          {
            automaticLOD.m_originalMesh = meshFilter.sharedMesh;
          }

          Simplifier[] simplifiers = automaticLOD.GetComponents<Simplifier>();

          for (int c = 0; c < simplifiers.Length; c++)
          {
            if (Application.isEditor && Application.isPlaying == false)
            {
              DestroyImmediate(simplifiers[c]);
            }
            else
            {
              Destroy(simplifiers[c]);
            }
          }

          automaticLOD.m_meshSimplifier = automaticLOD.gameObject.AddComponent<Simplifier>();
          automaticLOD.m_meshSimplifier.hideFlags = HideFlags.HideInInspector;
          IEnumerator enumerator = automaticLOD.m_meshSimplifier.ProgressiveMesh(gameObject, automaticLOD.m_originalMesh, root.m_aRelevanceSpheres, automaticLOD.name, progress);

          while (enumerator.MoveNext())
          {
            if (Simplifier.Cancelled)
            {
              return;
            }
          }

          if (Simplifier.Cancelled)
          {
            return;
          }
        }
        else
        {
          SkinnedMeshRenderer skin = automaticLOD.GetComponent<SkinnedMeshRenderer>();

          if (skin != null)
          {
            if (automaticLOD.m_originalMesh == null)
            {
              automaticLOD.m_originalMesh = skin.sharedMesh;
            }

            Simplifier[] simplifiers = automaticLOD.GetComponents<Simplifier>();

            for (int c = 0; c < simplifiers.Length; c++)
            {
              if (Application.isEditor && Application.isPlaying == false)
              {
                DestroyImmediate(simplifiers[c]);
              }
              else
              {
                Destroy(simplifiers[c]);
              }
            }

            automaticLOD.m_meshSimplifier = automaticLOD.gameObject.AddComponent<Simplifier>();
            automaticLOD.m_meshSimplifier.hideFlags = HideFlags.HideInInspector;
            IEnumerator enumerator = automaticLOD.m_meshSimplifier.ProgressiveMesh(gameObject, automaticLOD.m_originalMesh, root.m_aRelevanceSpheres, automaticLOD.name, progress);

            while(enumerator.MoveNext())
            {
              if(Simplifier.Cancelled)
              {
                return;
              }
            }

            if (Simplifier.Cancelled)
            {
              return;
            }
          }
        }
      }

      for (int i = 0; i < automaticLOD.m_listLODLevels.Count; i++)
      {
        automaticLOD.m_listLODLevels[i].m_mesh = null;
      }

      automaticLOD.m_bLODDataDirty = false;
    }

    if(bRecurseIntoChildren)
    {
      for (int nChild = 0; nChild < gameObject.transform.childCount && Simplifier.Cancelled == false; nChild++)
      {
        ComputeLODDataRecursive(root, gameObject.transform.GetChild(nChild).gameObject, bRecurseIntoChildren, progress);

        if (Simplifier.Cancelled)
        {
          return;
        }
      }
    }
  }

  public bool HasLODData()
  {
    return m_meshSimplifier != null && m_listLODLevels != null && m_listLODLevels.Count > 0;
  }

  public int GetLODLevelUsingCamera(Camera currentCamera)
  {
    // Only compute it once per camera and frame

    if (m_cachedFrameLODLevel.ContainsKey(currentCamera))
    {
      return m_cachedFrameLODLevel[currentCamera];
    }

    if (m_listLODLevels == null || m_listLODLevels.Count == 0)
    {
      return -1;
    }

    float fDistanceToCamera = 0.0f;
    float fScreenCoverage   = 0.0f;

    if (m_evalMode == EvalMode.CameraDistance)
    {
      Vector3 v3WorldCenter = transform.TransformPoint(m_localCenter.x, m_localCenter.y, m_localCenter.z);
      fDistanceToCamera = Vector3.Distance(v3WorldCenter, currentCamera.transform.position);
    }
    else if(m_evalMode == EvalMode.ScreenCoverage)
    {
      fScreenCoverage = ComputeScreenCoverage(currentCamera);
    }

    int nLODLevel = 0;

    for (nLODLevel = 0; nLODLevel < m_listLODLevels.Count; nLODLevel++)
    {
      if (nLODLevel == m_listLODLevels.Count - 1)
      {
        break;
      }

      if (m_evalMode == EvalMode.CameraDistance)
      {
        if (fDistanceToCamera < m_listLODLevels[nLODLevel + 1].m_fMaxCameraDistance)
        {
          break;
        }
      }
      else if (m_evalMode == EvalMode.ScreenCoverage)
      {
        if (fScreenCoverage > m_listLODLevels[nLODLevel + 1].m_fScreenCoverage)
        {
          break;
        }
      }
    }

    // Set the cached value for this frame

    m_cachedFrameLODLevel.Add(currentCamera, nLODLevel);

    return nLODLevel;
  }

  public int GetCurrentLODLevel()
  {
    return m_nCurrentLOD;
  }

  public void SwitchToLOD(int nLevel, bool bRecurseIntoChildren)
  {
    SwitchToLODRecursive(this, this.gameObject, nLevel, bRecurseIntoChildren);
  }

  private static void SwitchToLODRecursive(AutomaticLOD root, GameObject gameObject, int nLODLevel, bool bRecurseIntoChildren)
  {
    AutomaticLOD automaticLOD = gameObject.GetComponent<AutomaticLOD>();

    if (automaticLOD != null)
    {
      if (IsRootOrBelongsToLODTree(automaticLOD, root))
      {
        if (nLODLevel >= 0 && nLODLevel < automaticLOD.m_listLODLevels.Count)
        {
          MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();

          if (meshFilter != null)
          {
            if (meshFilter.sharedMesh != automaticLOD.m_listLODLevels[nLODLevel].m_mesh)
            {
              meshFilter.sharedMesh = automaticLOD.m_listLODLevels[nLODLevel].m_mesh;
            }
          }
          else
          {
            SkinnedMeshRenderer skin = gameObject.GetComponent<SkinnedMeshRenderer>();

            if (skin != null)
            {
              if (skin.sharedMesh != automaticLOD.m_listLODLevels[nLODLevel].m_mesh)
              {
                if (automaticLOD.m_listLODLevels[nLODLevel].m_mesh != null && automaticLOD.m_listLODLevels[nLODLevel].m_mesh.vertexCount == 0)
                {
                  // Avoid editor warning of mesh not having skinning data
                  if (skin.sharedMesh != null)
                  {
                    skin.sharedMesh = null;
                  }
                }
                else
                {
                  skin.sharedMesh = automaticLOD.m_listLODLevels[nLODLevel].m_mesh;
                }
              }
            }
          }

          automaticLOD.m_nCurrentLOD = nLODLevel;
        }
      }
    }

    if (bRecurseIntoChildren)
    {
      for (int nChild = 0; nChild < gameObject.transform.childCount; nChild++)
      {
        SwitchToLODRecursive(root, gameObject.transform.GetChild(nChild).gameObject, nLODLevel, true);
      }
    }
  }

  public void ComputeAllLODMeshes(bool bRecurseIntoChildren, Simplifier.ProgressDelegate progress = null)
  {
    if (m_listLODLevels != null)
    {
      for(int i = 0; i < m_listLODLevels.Count; i++)
      {
        ComputeLODMeshRecursive(this, this.gameObject, i, bRecurseIntoChildren, progress);

        if (Simplifier.Cancelled)
        {
          return;
        }
      }
    }
  }

  public void ComputeLODMesh(int nLevel, bool bRecurseIntoChildren, Simplifier.ProgressDelegate progress = null)
  {
    ComputeLODMeshRecursive(this, this.gameObject, nLevel, bRecurseIntoChildren, progress);
  }

  private static void ComputeLODMeshRecursive(AutomaticLOD root, GameObject gameObject, int nLevel, bool bRecurseIntoChildren, Simplifier.ProgressDelegate progress = null)
  {
    if (Simplifier.Cancelled)
    {
      return;
    }

    AutomaticLOD automaticLOD = gameObject.GetComponent<AutomaticLOD>();

    if (automaticLOD != null)
    {
      if (IsRootOrBelongsToLODTree(automaticLOD, root))
      {
        if (automaticLOD.m_meshSimplifier != null)
        {
          if (automaticLOD.m_listLODLevels[nLevel].m_mesh)
          {
            automaticLOD.m_listLODLevels[nLevel].m_mesh.Clear();
          }

          float fAmount = automaticLOD.m_listLODLevels[nLevel].m_fMeshVerticesAmount;

          if (automaticLOD.m_bOverrideRootSettings == false && automaticLOD.m_LODObjectRoot != null)
          {
            fAmount = automaticLOD.m_LODObjectRoot.m_listLODLevels[nLevel].m_fMeshVerticesAmount;
          }

          if (automaticLOD.m_listLODLevels[nLevel].m_mesh == null)
          {
            automaticLOD.m_listLODLevels[nLevel].m_mesh = CreateNewEmptyMesh(automaticLOD);
          }

          IEnumerator enumerator = automaticLOD.m_meshSimplifier.ComputeMeshWithVertexCount(gameObject, automaticLOD.m_listLODLevels[nLevel].m_mesh, Mathf.RoundToInt(fAmount * automaticLOD.m_meshSimplifier.GetOriginalMeshUniqueVertexCount()), automaticLOD.name + " LOD " + nLevel, progress);

          while (enumerator.MoveNext())
          {
            if (Simplifier.Cancelled)
            {
              return;
            }
          }

          if (Simplifier.Cancelled)
          {
            return;
          }
        }
      }
    }

    if (bRecurseIntoChildren)
    {
      for (int nChild = 0; nChild < gameObject.transform.childCount; nChild++)
      {
        ComputeLODMeshRecursive(root, gameObject.transform.GetChild(nChild).gameObject, nLevel, bRecurseIntoChildren, progress);

        if (Simplifier.Cancelled)
        {
          return;
        }
      }
    }
  }

  public void RestoreOriginalMesh(bool bDeleteLODData, bool bRecurseIntoChildren)
  {
    RestoreOriginalMeshRecursive(this, this.gameObject, bDeleteLODData, bRecurseIntoChildren);
  }

  private static void RestoreOriginalMeshRecursive(AutomaticLOD root, GameObject gameObject, bool bDeleteLODData, bool bRecurseIntoChildren)
  {
    AutomaticLOD automaticLOD = gameObject.GetComponent<AutomaticLOD>();

    if (automaticLOD != null)
    {
      if (IsRootOrBelongsToLODTree(automaticLOD, root))
      {
        if (automaticLOD.m_originalMesh != null)
        {
          MeshFilter meshFilter = automaticLOD.GetComponent<MeshFilter>();

          if (meshFilter != null)
          {
            meshFilter.sharedMesh = automaticLOD.m_originalMesh;
          }
          else
          {
            SkinnedMeshRenderer skin = automaticLOD.GetComponent<SkinnedMeshRenderer>();

            if (skin != null)
            {
              skin.sharedMesh = automaticLOD.m_originalMesh;
            }
          }
        }

        automaticLOD.m_nCurrentLOD = -1;

        if (bDeleteLODData)
        {
          automaticLOD.FreeLODData(false);
          automaticLOD.m_listLODLevels.Clear();
          automaticLOD.m_listLODLevels = null;
          automaticLOD.m_listDependentChildren.Clear();
        }
      }
    }

    if (bRecurseIntoChildren)
    {
      for (int nChild = 0; nChild < gameObject.transform.childCount; nChild++)
      {
        RestoreOriginalMeshRecursive(root, gameObject.transform.GetChild(nChild).gameObject, bDeleteLODData, bRecurseIntoChildren);
      }
    }
  }

  public bool HasOriginalMeshActive(bool bRecurseIntoChildren)
  {
    return HasOriginalMeshActiveRecursive(this, this.gameObject, bRecurseIntoChildren);
  }

  private static bool HasOriginalMeshActiveRecursive(AutomaticLOD root, GameObject gameObject, bool bRecurseIntoChildren)
  {
    AutomaticLOD automaticLOD = gameObject.GetComponent<AutomaticLOD>();

    bool bHasOriginalMeshActive = false;

    if (automaticLOD != null)
    {
      if (IsRootOrBelongsToLODTree(automaticLOD, root))
      {
        if (automaticLOD.m_originalMesh != null)
        {
          MeshFilter meshFilter = automaticLOD.GetComponent<MeshFilter>();

          if (meshFilter != null)
          {
            if(meshFilter.sharedMesh == automaticLOD.m_originalMesh)
            {
              bHasOriginalMeshActive = true;
            }
          }
          else
          {
            SkinnedMeshRenderer skin = automaticLOD.GetComponent<SkinnedMeshRenderer>();

            if (skin != null)
            {
              if(skin.sharedMesh == automaticLOD.m_originalMesh)
              {
                bHasOriginalMeshActive = true;
              }
            }
          }
        }
      }
    }

    if (bRecurseIntoChildren)
    {
      for (int nChild = 0; nChild < gameObject.transform.childCount; nChild++)
      {
        bHasOriginalMeshActive = bHasOriginalMeshActive || HasOriginalMeshActiveRecursive(root, gameObject.transform.GetChild(nChild).gameObject, bRecurseIntoChildren);
      }
    }

    return bHasOriginalMeshActive;
  }

  public bool HasVertexData(int nLevel, bool bRecurseIntoChildren)
  {
    return HasVertexDataRecursive(this, this.gameObject, nLevel, bRecurseIntoChildren);
  }

  private static bool HasVertexDataRecursive(AutomaticLOD root, GameObject gameObject, int nLevel, bool bRecurseIntoChildren)
  {
    AutomaticLOD automaticLOD = gameObject.GetComponent<AutomaticLOD>();

    if (automaticLOD != null)
    {
      if (IsRootOrBelongsToLODTree(automaticLOD, root))
      {
        if (automaticLOD.m_listLODLevels[nLevel].m_mesh && automaticLOD.m_listLODLevels[nLevel].m_mesh.vertexCount > 0)
        {
          return true;
        }
      }
    }

    if (bRecurseIntoChildren)
    {
      for (int nChild = 0; nChild < gameObject.transform.childCount; nChild++)
      {
        if (HasVertexDataRecursive(root, gameObject.transform.GetChild(nChild).gameObject, nLevel, bRecurseIntoChildren))
        {
          return true;
        }
      }
    }

    return false;
  }

  public int GetOriginalVertexCount(bool bRecurseIntoChildren)
  {
    int nVertexCount = 0;
    GetOriginalVertexCountRecursive(this, this.gameObject, ref nVertexCount, bRecurseIntoChildren);
    return nVertexCount;
  }

  private static void GetOriginalVertexCountRecursive(AutomaticLOD root, GameObject gameObject, ref int nVertexCount, bool bRecurseIntoChildren)
  {
    AutomaticLOD automaticLOD = gameObject.GetComponent<AutomaticLOD>();

    if (automaticLOD != null)
    {
      if (IsRootOrBelongsToLODTree(automaticLOD, root))
      {
        if (automaticLOD.m_originalMesh != null)
        {
          nVertexCount += automaticLOD.m_originalMesh.vertexCount;
        }
      }
    }

    if (bRecurseIntoChildren)
    {
      for (int nChild = 0; nChild < gameObject.transform.childCount; nChild++)
      {
        GetOriginalVertexCountRecursive(root, gameObject.transform.GetChild(nChild).gameObject, ref nVertexCount, bRecurseIntoChildren);
      }
    }
  }

  public int GetCurrentVertexCount(bool bRecurseIntoChildren)
  {
    int nVertexCount = 0;
    GetCurrentVertexCountRecursive(this, this.gameObject, ref nVertexCount, bRecurseIntoChildren);
    return nVertexCount;
  }

  private static void GetCurrentVertexCountRecursive(AutomaticLOD root, GameObject gameObject, ref int nVertexCount, bool bRecurseIntoChildren)
  {
    AutomaticLOD automaticLOD = gameObject.GetComponent<AutomaticLOD>();

    if (automaticLOD != null)
    {
      if (IsRootOrBelongsToLODTree(automaticLOD, root))
      {
        MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();

        if(meshFilter != null && meshFilter.sharedMesh != null)
        {
          nVertexCount += meshFilter.sharedMesh.vertexCount;
        }
        else
        {
          SkinnedMeshRenderer skin = gameObject.GetComponent<SkinnedMeshRenderer>();

          if (skin != null && skin.sharedMesh != null)
          {
            nVertexCount += skin.sharedMesh.vertexCount;
          }
        }
      }
    }

    if (bRecurseIntoChildren)
    {
      for (int nChild = 0; nChild < gameObject.transform.childCount; nChild++)
      {
        GetCurrentVertexCountRecursive(root, gameObject.transform.GetChild(nChild).gameObject, ref nVertexCount, bRecurseIntoChildren);
      }
    }
  }

  public int GetLODVertexCount(int nLevel, bool bRecurseIntoChildren)
  {
    int nVertexCount = 0;
    GetLODVertexCountRecursive(this, this.gameObject, nLevel, ref nVertexCount, bRecurseIntoChildren);
    return nVertexCount;
  }

  private static void GetLODVertexCountRecursive(AutomaticLOD root, GameObject gameObject, int nLevel, ref int nVertexCount, bool bRecurseIntoChildren)
  {
    AutomaticLOD automaticLOD = gameObject.GetComponent<AutomaticLOD>();

    if (automaticLOD != null)
    {
      if (IsRootOrBelongsToLODTree(automaticLOD, root))
      {
        if (automaticLOD.m_listLODLevels[nLevel].m_mesh != null)
        {
          nVertexCount += automaticLOD.m_listLODLevels[nLevel].m_mesh.vertexCount;
        }
      }
    }

    if (bRecurseIntoChildren)
    {
      for (int nChild = 0; nChild < gameObject.transform.childCount; nChild++)
      {
        GetLODVertexCountRecursive(root, gameObject.transform.GetChild(nChild).gameObject, nLevel, ref nVertexCount, bRecurseIntoChildren);
      }
    }
  }

  public void RemoveFromLODTree()
  {
    if (m_LODObjectRoot != null)
    {
      m_LODObjectRoot.m_listDependentChildren.Remove(this);
    }

    RestoreOriginalMesh(true, false);
  }

  public void FreeLODData(bool bRecurseIntoChildren)
  {
    FreeLODDataRecursive(this, this.gameObject, bRecurseIntoChildren);
  }

  private static void FreeLODDataRecursive(AutomaticLOD root, GameObject gameObject, bool bRecurseIntoChildren)
  {
    AutomaticLOD automaticLOD = gameObject.GetComponent<AutomaticLOD>();

    if (automaticLOD != null)
    {
      if (IsRootOrBelongsToLODTree(automaticLOD, root))
      {
        if (automaticLOD.m_listLODLevels != null)
        {
          foreach (LODLevelData data in automaticLOD.m_listLODLevels)
          {
            if (data.m_mesh)
            {
              data.m_mesh.Clear();
            }
          }
        }

        Simplifier[] simplifiers = automaticLOD.GetComponents<Simplifier>();

        for (int c = 0; c < simplifiers.Length; c++)
        {
          if (Application.isEditor && Application.isPlaying == false)
          {
            DestroyImmediate(simplifiers[c]);
          }
          else
          {
            Destroy(simplifiers[c]);
          }
        }

        if (automaticLOD.m_meshSimplifier != null)
        {
          automaticLOD.m_meshSimplifier = null;
        }

        automaticLOD.m_bLODDataDirty = true;
      }
    }

    if (bRecurseIntoChildren)
    {
      for (int nChild = 0; nChild < gameObject.transform.childCount; nChild++)
      {
        FreeLODDataRecursive(root, gameObject.transform.GetChild(nChild).gameObject, bRecurseIntoChildren);
      }
    }
  }

  private static Mesh CreateNewEmptyMesh(AutomaticLOD automaticLOD)
  {
    if(automaticLOD.m_originalMesh == null)
    {
      return new Mesh();
    }

    Mesh meshOut = Mesh.Instantiate(automaticLOD.m_originalMesh);
    meshOut.Clear();
    return meshOut;
  }

#if UNITY_EDITOR

  public void DisablePrefabUsage(bool bRecurseIntoChildren)
  {
    DisablePrefabUsageRecursive(this, this.gameObject, bRecurseIntoChildren);
  }

  private static void DisablePrefabUsageRecursive(AutomaticLOD root, GameObject gameObject, bool bRecurseIntoChildren)
  {
    AutomaticLOD automaticLOD = gameObject.GetComponent<AutomaticLOD>();

    if (automaticLOD != null)
    {
      if (IsRootOrBelongsToLODTree(automaticLOD, root))
      {
        if (automaticLOD.m_listLODLevels != null)
        {
          foreach (LODLevelData data in automaticLOD.m_listLODLevels)
          {
            if (data.m_mesh)
            {
              if(UnityEditor.AssetDatabase.IsMainAsset(data.m_mesh) || UnityEditor.AssetDatabase.IsSubAsset(data.m_mesh))
              {
                Mesh newMesh = Instantiate(data.m_mesh) as Mesh;
                data.m_mesh = newMesh;
              }
            }
          }
        }

        automaticLOD.m_strAssetPath = null;
      }
    }

    if (bRecurseIntoChildren)
    {
      for (int nChild = 0; nChild < gameObject.transform.childCount; nChild++)
      {
        DisablePrefabUsageRecursive(root, gameObject.transform.GetChild(nChild).gameObject, bRecurseIntoChildren);
      }
    }
  }

#endif

  void BuildCornerData(Vector3[] av3Corners, Bounds bounds)
  {
    av3Corners[0] = new Vector3(bounds.min.x, bounds.min.y, bounds.min.z);
    av3Corners[1] = new Vector3(bounds.min.x, bounds.min.y, bounds.max.z);
    av3Corners[2] = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
    av3Corners[3] = new Vector3(bounds.min.x, bounds.max.y, bounds.max.z);
    av3Corners[4] = new Vector3(bounds.max.x, bounds.min.y, bounds.min.z);
    av3Corners[5] = new Vector3(bounds.max.x, bounds.min.y, bounds.max.z);
    av3Corners[6] = new Vector3(bounds.max.x, bounds.max.y, bounds.min.z);
    av3Corners[7] = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z);
  }

  private int m_nCurrentLOD = -1;
  private Dictionary<Camera, int> m_cachedFrameLODLevel;
  private Vector3 m_localCenter;
}
