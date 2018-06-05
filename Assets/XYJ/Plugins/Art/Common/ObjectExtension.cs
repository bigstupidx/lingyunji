using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

public static class ObjectExtension
{
    static public T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        T comp = go.GetComponent<T>();
        if (comp == null)
        {
            comp = go.AddComponent<T>();
        }
        return comp;
    }

    public static UnityEngine.EventSystems.EventTrigger.Entry GetEntry(this UnityEngine.EventSystems.EventTrigger trigger, EventTriggerType ett)
    {
        List<UnityEngine.EventSystems.EventTrigger.Entry> entrys = trigger.triggers;
        for (int i = 0; i < entrys.Count; ++i)
        {
            if (entrys[i].eventID == ett)
                return entrys[i];
        }

        UnityEngine.EventSystems.EventTrigger.Entry entry = new UnityEngine.EventSystems.EventTrigger.Entry();
        entry.eventID = ett;
        entry.callback = new UnityEngine.EventSystems.EventTrigger.TriggerEvent();
        entrys.Add(entry);

        return entry;
    }

    public static void SetListener(this UnityEngine.EventSystems.EventTrigger trigger, EventTriggerType ett, UnityEngine.Events.UnityAction<BaseEventData> call)
    {
        trigger.GetEntry(ett).callback.SetListener(call);
    }

    public static void SetListener(this UnityEngine.EventSystems.EventTrigger trigger, EventTriggerType ett, UnityEngine.Events.UnityAction<BaseEventData> call, int count)
    {
        UnityEngine.EventSystems.EventTrigger.TriggerEvent te = trigger.GetEntry(ett).callback;
        te.RemoveAllListeners();
        te.AddListener(call, count);
    }

    public static void AddListener(this UnityEngine.EventSystems.EventTrigger trigger, EventTriggerType ett, UnityEngine.Events.UnityAction<BaseEventData> call)
    {
        trigger.GetEntry(ett).callback.AddListener(call);
    }

    public static void AddListener(this UnityEngine.EventSystems.EventTrigger trigger, EventTriggerType ett, UnityEngine.Events.UnityAction<BaseEventData> call, int count)
    {
        UnityEngine.EventSystems.EventTrigger.TriggerEvent te = trigger.GetEntry(ett).callback;
        te.AddListener(call, count);
    }

    public static void SetListener<T>(this UnityEngine.Events.UnityEvent<T> f, UnityEngine.Events.UnityAction<T> call)
    {
        f.RemoveAllListeners();
        f.AddListener(call);
    }

    public static UnityEngine.Events.UnityAction<T> AddListener<T>(this UnityEngine.Events.UnityEvent<T> f, UnityEngine.Events.UnityAction<T> fun, int count)
    {
        if (fun == null)
            return (T t) => { };

        if (count <= 0)
        {
            f.AddListener(fun);
            return fun;
        }

        UnityEngine.Events.UnityAction<T> r = null;
        r = (T t) =>
        {
            fun(t);
            --count;
            if (count <= 0)
            {
                f.RemoveListener(r);
            }
        };

        f.AddListener(r);
        return r;
    }

    public static void SetListener(this UnityEngine.Events.UnityEvent f, UnityEngine.Events.UnityAction call)
    {
        f.RemoveAllListeners();
        int count = f.GetPersistentEventCount();
        for (int i = 0; i < count; ++i)
        {
            f.SetPersistentListenerState(0, UnityEngine.Events.UnityEventCallState.Off);
        }

        f.AddListener(call);
    }

    public static UnityEngine.Events.UnityAction AddListener(this UnityEngine.Events.UnityEvent f, UnityEngine.Events.UnityAction fun, int count)
    {
        if (fun == null)
            return () => { };

        if (count <= 0)
        {
            f.AddListener(fun);
            return fun;
        }

        UnityEngine.Events.UnityAction r = null;
        r = () =>
        {
            fun();
            --count;
            if (count <= 0)
            {
                f.RemoveListener(r);
            }
        };

        f.AddListener(r);
        return r;
    }
}
