using UI;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Helper class containing generic functions used throughout the UI library.
/// </summary>

static public class UITools
{
	static AudioListener mListener;

	static bool mLoaded = false;
	static float mGlobalVolume = 1f;

	/// <summary>
	/// Globally accessible volume affecting all sounds played via NGUITools.PlaySound().
	/// </summary>

	static public float soundVolume
	{
		get
		{
			if (!mLoaded)
			{
				mLoaded = true;
				mGlobalVolume = PlayerPrefs.GetFloat("Sound", 1f);
			}
			return mGlobalVolume;
		}
		set
		{
			if (mGlobalVolume != value)
			{
				mLoaded = true;
				mGlobalVolume = value;
				PlayerPrefs.SetFloat("Sound", value);
			}
		}
	}

	/// <summary>
	/// Helper function -- whether the disk access is allowed.
	/// </summary>

	static public bool fileAccess
	{
		get
		{
			return Application.platform != RuntimePlatform.WindowsWebPlayer &&
				Application.platform != RuntimePlatform.OSXWebPlayer;
		}
	}

	/// <summary>
	/// Play the specified audio clip.
	/// </summary>

	static public AudioSource PlaySound (AudioClip clip) { return PlaySound(clip, 1f, 1f); }

	/// <summary>
	/// Play the specified audio clip with the specified volume.
	/// </summary>

	static public AudioSource PlaySound (AudioClip clip, float volume) { return PlaySound(clip, volume, 1f); }

	static float mLastTimestamp = 0f;
	static AudioClip mLastClip;

    [System.Diagnostics.DebuggerHidden]
    [System.Diagnostics.DebuggerStepThrough]
    static public bool GetActive(Behaviour mb)
    {
        return mb && mb.enabled && mb.gameObject.activeInHierarchy;
    }

    /// <summary>
    /// Unity4 has changed GameObject.active to GameObject.activeself.
    /// </summary>

    [System.Diagnostics.DebuggerHidden]
    [System.Diagnostics.DebuggerStepThrough]
    static public bool GetActive(GameObject go)
    {
        return go && go.activeInHierarchy;
    }

    /// <summary>
    /// Play the specified audio clip with the specified volume and pitch.
    /// </summary>

    static public AudioSource PlaySound (AudioClip clip, float volume, float pitch)
	{
		float time = Time.time;
		if (mLastClip == clip && mLastTimestamp + 0.1f > time) return null;

		mLastClip = clip;
		mLastTimestamp = time;
		volume *= soundVolume;

		if (clip != null && volume > 0.01f)
		{
			if (mListener == null || !GetActive(mListener))
			{
				AudioListener[] listeners = GameObject.FindObjectsOfType(typeof(AudioListener)) as AudioListener[];

				if (listeners != null)
				{
					for (int i = 0; i < listeners.Length; ++i)
					{
						if (GetActive(listeners[i]))
						{
							mListener = listeners[i];
							break;
						}
					}
				}

				if (mListener == null)
				{
					Camera cam = Camera.main;
					if (cam == null) cam = GameObject.FindObjectOfType(typeof(Camera)) as Camera;
					if (cam != null) mListener = cam.gameObject.AddComponent<AudioListener>();
				}
			}

			if (mListener != null && mListener.enabled && GetActive(mListener.gameObject))
			{
#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
				AudioSource source = mListener.audio;
#else
				AudioSource source = mListener.GetComponent<AudioSource>();
#endif
				if (source == null) source = mListener.gameObject.AddComponent<AudioSource>();
#if !UNITY_FLASH
				source.priority = 50;
				source.pitch = pitch;
#endif
				source.PlayOneShot(clip, volume);
				return source;
			}
		}
		return null;
	}

	/// <summary>
	/// Convenience method that works without warnings in both Unity 3 and 4.
	/// </summary>

	static public void RegisterUndo (UnityEngine.Object obj, string name)
	{
#if UNITY_EDITOR
		UnityEditor.Undo.RecordObject(obj, name);
        SetDirty(obj);
#endif
	}

	/// <summary>
	/// Convenience function that marks the specified object as dirty in the Unity Editor.
	/// </summary>

	static public void SetDirty (UnityEngine.Object obj)
	{
#if UNITY_EDITOR
		if (obj)
		{
			//if (obj is Component) Debug.Log(NGUITools.GetHierarchy((obj as Component).gameObject), obj);
			//else if (obj is GameObject) Debug.Log(NGUITools.GetHierarchy(obj as GameObject), obj);
			//else Debug.Log("Hmm... " + obj.GetType(), obj);
			UnityEditor.EditorUtility.SetDirty(obj);
		}
#endif
	}

	/// <summary>
	/// Add a new child game object.
	/// </summary>

	static public GameObject AddChild (this GameObject parent) { return AddChild(parent, true); }

	/// <summary>
	/// Add a new child game object.
	/// </summary>

	static public GameObject AddChild (this GameObject parent, bool undo)
	{
		GameObject go = new GameObject();
#if UNITY_EDITOR
		if (undo) UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
		if (parent != null)
		{
			Transform t = go.transform;
			t.parent = parent.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			go.layer = parent.layer;
		}
		return go;
	}

	/// <summary>
	/// Instantiate an object and add it to the specified parent.
	/// </summary>

	static public GameObject AddChild (this GameObject parent, GameObject prefab)
	{
		GameObject go = GameObject.Instantiate(prefab) as GameObject;
        //对象池使用原来预制体的name来做key
        go.name = prefab.name;
#if UNITY_EDITOR
		UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Create Object");
#endif
		if (go != null && parent != null)
		{
			Transform t = go.transform;
			t.parent = parent.transform;
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			t.localScale = Vector3.one;
			go.layer = parent.layer;
		}
		return go;
	}

    static public GameObject AddToChild(this GameObject parent, GameObject child)
    {
#if UNITY_EDITOR
        UnityEditor.Undo.RegisterCreatedObjectUndo(parent, "AddToChild");
#endif
        if (child != null && parent != null)
        {
            Transform t = child.transform;
            t.parent = parent.transform;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            child.layer = parent.layer;
        }

        return child;
    }

    /// <summary>
    /// Helper function that recursively sets all children with widgets' game objects layers to the specified value.
    /// </summary>

    static public void SetChildLayer (this Transform t, int layer)
	{
		for (int i = 0; i < t.childCount; ++i)
		{
			Transform child = t.GetChild(i);
			child.gameObject.layer = layer;
			SetChildLayer(child, layer);
		}
	}

    public static bool IsInherit(this Transform parent, Transform child)
    {
        while (true)
        {
            if (child.parent == parent)
                return true;

            child = child.parent;
            if (child == null)
                return false;
        }
    }

    static public string GetTypeName<T>()
    {
        string s = typeof(T).ToString();
        if (s.StartsWith("UI")) s = s.Substring(2);
        else if (s.StartsWith("UnityEngine.")) s = s.Substring(12);
        return s;
    }

    static Dictionary<Type, string> mTypeNames = new Dictionary<Type, string>();

    /// <summary>
    /// Add a child object to the specified parent and attaches the specified script to it.
    /// </summary>

    static public T AddChild<T> (this GameObject parent) where T : Component
	{
		GameObject go = AddChild(parent);
		string name;

		if (!mTypeNames.TryGetValue(typeof(T), out name) || name == null)
		{
			name = GetTypeName<T>();
			mTypeNames[typeof(T)] = name;
		}
		go.name = name;
		return go.AddComponent<T>();
	}

	/// <summary>
	/// Add a child object to the specified parent and attaches the specified script to it.
	/// </summary>

	static public T AddChild<T> (this GameObject parent, bool undo) where T : Component
	{
		GameObject go = AddChild(parent, undo);
		string name;

		if (!mTypeNames.TryGetValue(typeof(T), out name) || name == null)
		{
			name = GetTypeName<T>();
			mTypeNames[typeof(T)] = name;
		}
		go.name = name;
		return go.AddComponent<T>();
	}

	/// <summary>
	/// Destroy the specified object, immediately if in edit mode.
	/// </summary>

	static public void Destroy (UnityEngine.Object obj)
	{
		if (obj)
		{
			if (obj is Transform)
			{
				Transform t = (obj as Transform);
				GameObject go = t.gameObject;

				if (Application.isPlaying)
				{
					t.parent = null;
					UnityEngine.Object.Destroy(go);
				}
				else UnityEngine.Object.DestroyImmediate(go);
			}
			else if (obj is GameObject)
			{
				GameObject go = obj as GameObject;
				Transform t = go.transform;

				if (Application.isPlaying)
				{
					t.parent = null;
					UnityEngine.Object.Destroy(go);
				}
				else UnityEngine.Object.DestroyImmediate(go);
			}
			else if (Application.isPlaying) UnityEngine.Object.Destroy(obj);
			else UnityEngine.Object.DestroyImmediate(obj);
		}
	}

	/// <summary>
	/// Convenience extension that destroys all children of the transform.
	/// </summary>

	static public void XYjDestroyChildren (this Transform t)
	{
		bool isPlaying = Application.isPlaying;

		while (t.childCount != 0)
		{
			Transform child = t.GetChild(0);

			if (isPlaying)
			{
				child.SetParent(null);
				UnityEngine.Object.Destroy(child.gameObject);
			}
			else UnityEngine.Object.DestroyImmediate(child.gameObject);
		}
	}

	/// <summary>
	/// Destroy the specified object immediately, unless not in the editor, in which case the regular Destroy is used instead.
	/// </summary>

	static public void DestroyImmediate (UnityEngine.Object obj)
	{
		if (obj != null)
		{
			if (Application.isEditor) UnityEngine.Object.DestroyImmediate(obj);
			else UnityEngine.Object.Destroy(obj);
		}
	}

	/// <summary>
	/// Extension for the game object that checks to see if the component already exists before adding a new one.
	/// If the component is already present it will be returned instead.
	/// </summary>

	static public T XYJAddMissingComponent<T> (this GameObject go) where T : Component
	{
#if UNITY_FLASH
		object comp = go.GetComponent<T>();
#else
		T comp = go.GetComponent<T>();
#endif
		if (comp == null)
		{
#if UNITY_EDITOR
			if (!Application.isPlaying)
				RegisterUndo(go, "Add " + typeof(T));
#endif
			comp = go.AddComponent<T>();
		}
#if UNITY_FLASH
		return (T)comp;
#else
		return comp;
#endif
	}

    // 点是否在mask对象内
    // true在范围内
    // false不在范围内
    static public bool IsInRect(Camera eventCamera, RectSoftAlphaMask mask, Vector2 pointerPosition)
    {
        if (!mask.enabled)
            return true;

        if (!RectTransformUtility.RectangleContainsScreenPoint(mask.rectTransform, pointerPosition, eventCamera))
            return false;

        if (mask.ParentMask == null || !mask.ParentMask.enabled)
            return true;

        return IsInRect(eventCamera, mask.ParentMask, pointerPosition);
    }

    // true在范围内
    // false不在范围内
    static public bool IsMask(Camera eventCamera, Graphic graphic, Vector2 pointerPosition)
    {
        // Mask
        {
            Mask mask = graphic.GetComponentInParent<Mask>();
            if (mask != null && mask.enabled && !RectTransformUtility.RectangleContainsScreenPoint(mask.rectTransform, pointerPosition, eventCamera))
                return false;
        }

        // RectMask2D
        {
            RectMask2D mask2d = graphic.GetComponentInParent<RectMask2D>();
            if (mask2d != null && mask2d.enabled && !RectTransformUtility.RectangleContainsScreenPoint(mask2d.rectTransform, pointerPosition, eventCamera))
                return false;
        }

        AlphaMaskMaterial amask = graphic.GetComponent<AlphaMaskMaterial>();
        if (amask != null && amask.enabled && amask.rectParent != null && amask.rectParent.enabled)
            return IsInRect(eventCamera, amask.rectParent, pointerPosition);

        return true;            
    }
}
