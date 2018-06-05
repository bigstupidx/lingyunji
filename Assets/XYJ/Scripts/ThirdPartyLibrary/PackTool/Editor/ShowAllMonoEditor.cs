using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(ShowAllMono), true)]
public class ShowAllMonoEditor : Editor
{
    List<GameObject> gameObjects;

    void OnEnable()
    {
        gameObjects = new List<GameObject>();
        ShowAllMono sam = ((ShowAllMono)target);
        if (sam.roots != null && sam.roots.Length != 0)
            gameObjects.AddRange(sam.roots);
        else
            gameObjects.Add(sam.gameObject);
    }

    List<Component> monoBehaviours = new List<Component>();

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("更新数据"))
        {
            monoBehaviours.Clear();
            for (int i = 0; i < gameObjects.Count; ++i)
                gameObjects[i].GetComponentsInChildren(true, monoBehaviours);

            Dictionary<System.Type, List<Component>> monos = new Dictionary<System.Type, List<Component>>();
            foreach (Component mono in monoBehaviours)
            {
                if (mono == null)
                    continue;
                
                System.Type type = mono.GetType();
                if (type.Name == "ShowAllMono" || type.Name == "ResourcesRecords" || (type == typeof(Transform)))
                    continue;

                List<Component> ms = null;
                if (!monos.TryGetValue(type, out ms))
                {
                    ms = new List<Component>();
                    monos.Add(type, ms);
                }

                ms.Add(mono);
            }

            List<ShowAllMono.Data> datas = ((ShowAllMono)target).Datas;
            datas.Clear();
            foreach (var itor in monos)
            {
                ShowAllMono.Data d = new ShowAllMono.Data();
                d.Name = itor.Key.Name;

                if (itor.Value[0] is MonoBehaviour)
                {
                    d.script = MonoScript.FromMonoBehaviour((MonoBehaviour)(itor.Value[0]));
                }

                d.type = itor.Key;
                d.monos = itor.Value;
                datas.Add(d);
            }

            List<GameObject> nullMonos = ((ShowAllMono)target).nullMonos;
            nullMonos.Clear();

            foreach (GameObject go in gameObjects)
            {
                Transform[] trans = go.GetComponentsInChildren<Transform>(true);
                List<MonoBehaviour> mbs = new List<MonoBehaviour>();
                for (int i = 0; i < trans.Length; ++i)
                {
                    mbs.Clear();
                    trans[i].GetComponents(mbs);
                    if (mbs.Contains(null))
                    {
                        nullMonos.Add(trans[i].gameObject);
                    }
                }
            }
        }

        if (GUILayout.Button("选中含有移除空组件对象"))
        {
            List<GameObject> nullSelects = new List<GameObject>();
            foreach (var go in gameObjects)
            {
                foreach (var tran in go.GetComponentsInChildren<Transform>(true))
                {
                    List<MonoBehaviour> mbs = new List<MonoBehaviour>();
                    tran.GetComponents(mbs);
                    if (mbs.Contains(null))
                    {
                        nullSelects.Add(tran.gameObject);
                    }
                }
            }

            Selection.objects = nullSelects.ToArray();
        }

        if (GUILayout.Button("称除所有的Mono对象"))
        {
            foreach (var component in monoBehaviours)
            {
                if (component != null && component is MonoBehaviour)
                {
                    Object.DestroyImmediate(component);
                }
            }
        }
    }
}