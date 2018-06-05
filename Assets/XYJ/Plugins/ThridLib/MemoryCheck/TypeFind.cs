#if MEMORY_CHECK
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TypeFind
{
    static Dictionary<string, System.Type> Types = new Dictionary<string, System.Type>();

    static bool isInit = false;

    static void Init()
    {
        if (!isInit)
            return;

        isInit = true;

        List<System.Reflection.Assembly> Assemblys = new List<System.Reflection.Assembly>();
        var assembly = System.Reflection.Assembly.GetCallingAssembly();
        if (assembly != null)
            Assemblys.Add(assembly);

        assembly = System.Reflection.Assembly.GetEntryAssembly();
        if (assembly != null)
            Assemblys.Add(assembly);

        assembly = System.Reflection.Assembly.GetExecutingAssembly();
        if (assembly != null)
            Assemblys.Add(assembly);

        for (int i = 0; i < Assemblys.Count; ++i)
        {
            System.Type[] types = Assemblys[i].GetTypes();
            if (types == null || types.Length == 0)
                continue;

            foreach (var type in types)
            {
                Types.Add(type.FullName, type);
            }
        }
    }

    static public System.Type Get(string fullname)
    {
        Init();
        System.Type t = null;
        if (Types.TryGetValue(fullname, out t))
            return t;

        t = System.Type.GetType(fullname);
        Types.Add(fullname, t);
        return t;
    }
}

#endif