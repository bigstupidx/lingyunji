#if UNITY_EDITOR
using UnityEngine;
using System.Collections.Generic;

public class ParamList
{
    public delegate object Create();
    public delegate void Release(object p);

//     class ObjType
//     {
//         public Create create;
//         public Release release;
//     }
// 
//     Dictionary<System.Type, ObjType> FactoryList = new Dictionary<System.Type, ObjType>(); // 工厂列表
// 
//     public void RegType<T>(Create create, Release release)
//     {
//         System.Type type = typeof(T);
//         ObjType ot = null;
//         if (FactoryList.TryGetValue(type, out ot))
//         {
//             Debuger.LogError("FactoryList.TryGetValue(type, out ot) exits!");
//             return;
//         }
// 
//         ot = new ObjType();
//         ot.create = create;
//         ot.release = release;
//         FactoryList.Add(type, ot);
//     }

    Dictionary<string, object> Maps = new Dictionary<string, object>();

    public void ReleaseAll()
    {
        List<string> keys = new List<string>();
        foreach (KeyValuePair<string, object> itor in Maps)
            keys.Add(itor.Key);

        foreach (string key in keys)
            Remove(key);
    }

    public object this[string key] 
    { 
        get { return Get<object>(key); }
        set { Set(key, value); } 
    }

    public T GetOnly<T>(string parent, string child) where T : new()
    {
        ParamList pl = Get<ParamList>(parent);
        if (pl.Has(child))
            return pl.Get<T>(child);

        pl.ReleaseAll();
        return pl.Get<T>(child);
    }

    public T Get<T>(string name) where T : new()
    {
        return Get<T>(name, default(T));
    }

    public T Get<T>(string name, T def) where T : new()
    {
        object value = null;
        if (Maps.TryGetValue(name, out value))
            return (T)value;

        if (def == null)
            def = new T();

        Maps.Add(name, def);
        return (T)def;
    }

    public T Get<T>(string name, System.Func<T> fun)
    {
        object value = null;
        if (Maps.TryGetValue(name, out value))
            return (T)value;

        value = fun();
        Maps.Add(name, value);
        return (T)value;
    }

    public string GetString(string name, string def = "")
    {
        object value = null;
        if (Maps.TryGetValue(name, out value))
            return value as string;

        return def;
    }

    public void Set(string name, object p)
    {
        Maps[name] = p;
    }

    public bool Has(string name)
    {
        return Maps.ContainsKey(name);
    }

    public void Remove(string key)
    {
        object value = null;
        if (!Maps.TryGetValue(key, out value))
            return;

        Maps.Remove(key);
    }
}
#endif