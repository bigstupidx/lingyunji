using UnityEngine;

public class AssetResoruce : ScriptableObject
{
    public Object asset;

    public T Get<T>() where T : Object
    {
        return asset as T;
    }

    public static T Load<T>(string name) where T : Object
    {
        AssetResoruce ar = Resources.Load<AssetResoruce>(name);
        return ar == null ? null : ar.Get<T>();
    }
}