using System;
using System.Collections.Generic;

public class AnyArgs
#if MEMORY_CHECK
        : MemoryObject
#endif
{
    Dictionary<string, object> dic = new Dictionary<string, object>();
    int btnCount = 0;
    public Dictionary<string, object> Values
    {
        get
        {
            return dic;
        }
    }

    public int size { get { return dic.Count; } }

    public AnyArgs()
    {

    }

    public AnyArgs(string key, object value, params object[] objs)
    {
        dic.Add(key, value);
        if (objs != null && objs.Length != 0)
        {
            for (int i = 1; i < objs.Length; i += 2)
            {
                dic[(string)objs[i-1]] = objs[i];
            }
        }
    }

    public bool Has(string key)
    {
        return dic.ContainsKey(key);
    }

    public void Add(string key, object value)
    {
        if (key == null)
        {
            Debuger.ErrorLog("Add Error!key is null!");
            return;
        }

        dic[key] = value;
    }

    public bool TryGetValue<T>(string key, out T value)
    {
        value = default(T);
        object t = null;
        if (!dic.TryGetValue(key, out t))
            return false;

        if (t is T)
        {
            value = (T)t;
            return true;
        }
        else
        {
            try
            {
                value = (T)Convert.ChangeType(t, typeof(T));
                return true;
            }
            catch(Exception e)
            {
                Debuger.ErrorLog( "Key:{0} not type:{1}! {2}", key, typeof(T).Name,e);
                return false;
            }
            
        }
        
        Debuger.ErrorLog("Key:{0} not type:{1}!", key, typeof(T).Name);
        return false;
    }

    public object Get(string key)
    {
        object t = null;
        if (!dic.TryGetValue(key, out t))
            return null;
        return t;
    }

    public T Get<T>(string key, T def)
    {
        T r;
        if (TryGetValue<T>(key, out r))
            return r;

        return def;
    }

    


    //用于按钮回调的小功能
    public void PushBtn(string btnName,Action action)
    {
        ++btnCount;
        Add("btnName" + btnCount, btnName);
        Add("onBtnClick" + btnCount, action);
    }
}