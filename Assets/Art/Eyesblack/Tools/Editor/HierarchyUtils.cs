using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Eyesblack.EditorTools
{
	public class HierarchyUtils
	{
		// undocumented method, can iterator inactive object
		// ref: http://answers.unity3d.com/questions/27729/finding-the-root-gameobjects-in-the-scene-.html
		// added by hdh
		public static IEnumerable<GameObject> SceneRoots()
		{
			var prop = new HierarchyProperty(HierarchyType.GameObjects);
			var expanded = new int[0];
			while (prop.Next(expanded))
			{
				yield return prop.pptrValue as GameObject;
			}
		}

		public static IEnumerable<Transform> AllSceneObjects()
		{
			var queue = new Queue<Transform>();

			foreach (var root in SceneRoots())
			{
				var tf = root.transform;
				yield return tf;
				queue.Enqueue(tf);
			}

			while (queue.Count > 0)
			{
				foreach (Transform child in queue.Dequeue())
				{
					yield return child;
					queue.Enqueue(child);
				}
			}
		}

		// find game object, even object is inactive
		// the function should be slow
		public static GameObject FindGameObject(string name)
		{
			foreach (var obj in AllSceneObjects())
			{
				if (obj.gameObject != null && obj.gameObject.name == name)
					return obj.gameObject;
			}
			return null;
		}

		// find root game object, even object is inactive
		public static GameObject FindRootGameObject(string name)
		{
			foreach (var obj in SceneRoots())
			{
				if (obj != null && obj.name == name)
					return obj;
			}
			return null;
		}

		public static void DestroyObjByName(string name)
		{
#if false
		GameObject[] allObjects = (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject));
		for (int i = 0, len = allObjects.Length; i < len; i++)
		{
			if (allObjects[i].name == name)
			{
				GameObject.DestroyImmediate(allObjects[i]);
				return;
			}
		}
#else
			foreach (var obj in SceneRoots())
			{
				if (obj != null && obj.name == name)
				{
					GameObject.DestroyImmediate(obj);
					break;
				}
			}
#endif
		}
	}
}
