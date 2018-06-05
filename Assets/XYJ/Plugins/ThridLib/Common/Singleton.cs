using UnityEngine;

public class Singleton<T> 
#if MEMORY_CHECK
        : MemoryObject
#endif
    where T : new()
{
    static protected T sInstance;
    static protected bool IsCreate = false;

    public static T Instance
    {
        get
        {
//             if (IsCreate == false)
//             {
//                 CreateInstance();
//             }
            return sInstance;
        }
    }

    public static bool IsHas { get { return IsCreate; } }

    public static void CreateInstance()
    {
        if (IsCreate == true)
            return;

        IsCreate = true;
        sInstance = new T();
    }

    public static void ReleaseInstance()
    {
        sInstance = default(T);
        IsCreate = false;
    }

    public static T GetInstance(bool newIfNotExist= true)
    {
        if (IsCreate == false&& newIfNotExist)
            CreateInstance();
        return sInstance;
    }
}

public abstract class SingletonMonoBehaviour<T> : MonoBehaviour where T : SingletonMonoBehaviour<T>
{
    protected static T sInstance = null;
    protected static bool IsCreate = false;

    public static bool isCreate { get { return IsCreate; } }

    public static T Instance
    {
        get
        {
#if UNITY_EDITOR
            CreateInstance();
#endif
            return sInstance;
        }
    }

    protected virtual void Awake()
    {
        if (sInstance == null)
        {
            sInstance = this as T;
            IsCreate = true;
            DontDestroyOnLoad(sInstance.gameObject);

            Init();
        }
    }

    protected virtual void Init()
    {

    }

    protected virtual void OnDestroy()
    {
        sInstance = null;
        IsCreate = false;
    }

    protected virtual void OnApplicationQuit()
    {
        sInstance = null;
        IsCreate = false;
    }

    public static void CreateInstance()
    {
        if (IsCreate == true)
            return;

        IsCreate = true;
        T[] managers = FindObjectsOfType(typeof(T)) as T[];
        if (managers.Length != 0)
        {
            if (managers.Length == 1)
            {
                sInstance = managers[0];
                sInstance.gameObject.name = typeof(T).Name;
                DontDestroyOnLoad(sInstance.gameObject);
                return;
            }
            else
            {
                foreach (T manager in managers)
                {
                    Destroy(manager.gameObject);
                }
            }
        }

        GameObject gO = new GameObject(typeof(T).Name, typeof(T));
        sInstance = gO.GetComponent<T>();
        Object.DontDestroyOnLoad(gO);
    }

    public static void ReleaseInstance()
    {
        if (sInstance != null)
        {
            Destroy(sInstance.gameObject);
            sInstance = null;
            IsCreate = false;
        }
    }
}