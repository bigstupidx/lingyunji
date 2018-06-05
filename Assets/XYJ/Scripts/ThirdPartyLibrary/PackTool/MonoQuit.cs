#if UNITY_EDITOR
using System.Collections.Generic;

public class MonoQuit : SingletonMonoBehaviour<MonoQuit>
{
    List<System.Action> actions = new List<System.Action>();

    public static void AddQuit(System.Action action)
    {
        Instance.actions.Add(action);
    }

    protected override void OnApplicationQuit()
    {
        for (int i = 0; i < actions.Count; ++i)
        {
            if (actions[i] == null)
                continue;

            try
            {
                actions[i]();
            }
            catch (System.Exception ex)
            {
                XYJLogger.LogException(ex);
            }
        }

        actions.Clear();
        base.OnApplicationQuit();
    }
}
#endif