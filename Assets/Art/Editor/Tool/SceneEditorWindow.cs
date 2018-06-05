using UnityEngine;  
using UnityEditor;  
using System.Collections;  
using System.Text;  
  
public class SceneEditorWindow : EditorWindow {  
  
    RaycastHit _hitInfo;  
    SceneView.OnSceneFunc _delegate;  
    static SceneEditorWindow _windowInstance;  
  
    [MenuItem("Window/Freezing Terrain #`")]  
    static void Init()  
    {  
        if(_windowInstance == null)  
        {  
            _windowInstance = EditorWindow.GetWindow(typeof(SceneEditorWindow)) as SceneEditorWindow;  
            _windowInstance._delegate = new SceneView.OnSceneFunc(OnSceneFunc);  
            SceneView.onSceneGUIDelegate += _windowInstance._delegate;  
        }  
    }  
  
    void OnEnable()  
    {  
    }  
  
    void OnDisable()  
    {  
    }  
  
    void OnDestroy()  
    {  
        if (_delegate != null)  
        {  
            SceneView.onSceneGUIDelegate -= _delegate;  
        }  
    }  
  
    void OnGUI()  
    {  
    }  
  
    void OnInspectorGUI()  
    {  
        Debug.Log("OnInspectorGUI");  
    }  
  
    static public void OnSceneFunc(SceneView sceneView)  
    {  
        _windowInstance.CustomSceneGUI(sceneView);  
    }  
  
    void CustomSceneGUI(SceneView sceneView)  
    {
        // Camera cameara = sceneView.camera;  
        // Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);  
        // if (Physics.Raycast(ray, out _hitInfo, 10000, -1))  
        // {  
        //     //Debug.DrawRay(ray.origin, ray.direction, Color.yellow);  
        //     Vector3 origin = _hitInfo.point;  
        //     origin.y += 100;  
        //     if (Physics.Raycast(origin, Vector3.down, out _hitInfo))  
        //     {  
        //         Handles.color = Color.yellow;  
        //         Handles.DrawLine(_hitInfo.point, origin);  
        //         float arrowSize = 1;  
        //         Vector3 pos = _hitInfo.point;  
        //         Quaternion quat;  
        //         Handles.color = Color.green;  
        //         quat = Quaternion.LookRotation(Vector3.up, Vector3.up);  
        //         Handles.ArrowCap(0, pos, quat, arrowSize);  
        //         Handles.color = Color.red;  
        //         quat = Quaternion.LookRotation(Vector3.right, Vector3.up);  
        //         Handles.ArrowCap(0, pos, quat, arrowSize);  
        //         Handles.color = Color.blue;  
        //         quat = Quaternion.LookRotation(Vector3.forward, Vector3.up);  
        //         Handles.ArrowCap(0, pos, quat, arrowSize);  
        //         //Handles.DrawLine(pos + new Vector3(0, 3, 0), pos);  
        //     }  
        // }  
        if (Selection.activeGameObject != null)
        {
            Terrain tr = Selection.activeGameObject.GetComponent<Terrain>() as Terrain;
            if (tr != null)
                Selection.activeGameObject = null;
            tr = null;
        }
        SceneView.RepaintAll();  
    }  
}  