namespace xys
{
    using System.Collections.Generic;

    public class LogStream
    {
        public int depth;
        public System.Text.StringBuilder sb = new System.Text.StringBuilder();

        void AddDepth()
        {
            for (int i = 0; i < depth; ++i)
                sb.Append("    ");
        }

        public void AppendLineFormat(string format, params object[] objs)
        {
            AddDepth();
            sb.AppendLineFormat(format, objs);
        }
    }

    public static class GameObjectLog
    {
        interface IObj
        {
            void Log(object obj, LogStream log);
        }

        static Dictionary<System.Type, IObj> Logs = new Dictionary<System.Type, IObj>();

        class SimpleLog<T> : IObj
        {
            public static SimpleLog<T> Create(System.Action<T, LogStream> fun)
            {
                var v = new SimpleLog<T>();
                v.fun = fun;

                return v;
            }

            System.Action<T, LogStream> fun;
            void IObj.Log(object obj, LogStream log)
            {
                fun((T)obj, log);
            }
        }

        static void Reg<T>(System.Action<T, LogStream> fun)
        {
            Logs.Add(typeof(T), SimpleLog<T>.Create(fun));
        }

        static GameObjectLog()
        {
            Reg<UnityEngine.RectTransform>((compent, log) => { log.AppendLineFormat("pos:{0} rot:{1} scale:{2}", compent.localPosition, compent.localEulerAngles, compent.localScale); });
            Reg<UnityEngine.UI.Image>((compent, log)=> { log.AppendLineFormat("sprite:{0} color:{1}", compent.sprite == null ? "null" : compent.sprite.name, compent.color); });
            Reg<UnityEngine.CanvasGroup>((compent, log) => { log.AppendLineFormat("alpha:{0}", compent.alpha); });
        }

        static IObj Get(System.Type t)
        {
            IObj obj;
            if (Logs.TryGetValue(t, out obj))
                return obj;

            return null;
        }

        public static void AppendLineFormat(this System.Text.StringBuilder sb, string format, params object[] objs)
        {
            if (objs == null || objs.Length == 0)
                sb.AppendLine(format);
            else
            {
                sb.AppendFormat(format, objs);
                sb.AppendLine();
            }
        }

        public static string Log(UnityEngine.GameObject go)
        {
            LogStream log = new LogStream();
            Log(go, log);
            return log.sb.ToString();
        }

        public static void Log(UnityEngine.GameObject go, LogStream log)
        {
            var components = go.GetComponents(typeof(UnityEngine.Component));
            var transform = go.transform;
            int childCount = transform.childCount;
            log.AppendLineFormat("go:{0} enable:{1} layer:{2} components:{3} child:{4}", 
                go.name, 
                go.activeSelf, 
                UnityEngine.LayerMask.LayerToName(go.layer), 
                components.Length, 
                childCount);

            log.depth++;

            foreach (var c in components)
            {
                IObj g = Get(c.GetType());
                log.AppendLineFormat("type:{0} v:{1} enable:{2}", c.GetType().Name, c.ToString(), (c is UnityEngine.Behaviour) ? ((UnityEngine.Behaviour)c).enabled : true);
                log.depth++;
                if (g != null)
                {
                    g.Log(c, log);
                }
                else
                {

                }
                log.depth--;
            }

            for (int i = 0; i < childCount; ++i)
            {
                var child = transform.GetChild(i).gameObject;
                log.depth++;
                Log(child, log);
                log.depth--;
            }
        }
    }
}