using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
#if USE_RESOURCESEXPORT
    public partial class ComponentSave
    {
        public ComponentSave()
        {
#if UNITY_EDITOR
            Stream = new MemoryStream(1024 * 4);
#endif
        }

#if UNITY_EDITOR
        class SaveData
        {
            public Component component;
            public ComData comData;

            public SaveData(Component c, ComData cd)
            {
                component = c;
                comData = cd;
            }
        }

        static List<Component> results = new List<Component>(2048);

        void Save(GameObject go, List<SaveData> dataList)
        {
            results.Clear();
            go.GetComponentsInChildren<Component>(true, results);
            ComData comData = null;
            for (int i = 0; i < results.Count; ++i)
            {
                if (results[i] != null)
                {
                    if (ComDataList.TryGetValue(results[i].GetType(), out comData))
                    {
                        dataList.Add(new SaveData(results[i], comData));
                    }
                }
            }
            results.Clear();
        }

        public List<int> PositionList = new List<int>();
        public List<Component> CompList = new List<Component>();
        public MemoryStream Stream;

        public byte[] GetSaveData()
        {
            byte[] bytes = new byte[Stream.Length];
            Stream.Position = 0;
            Stream.Read(bytes, 0, (int)Stream.Length);
            return bytes;
        }

        static long component_total = 0; // 总个数
        static long component_vaildTotal = 0; // 有效个数

        public void Save(GameObject go, IAssetsExport mgr)
        {
            List<SaveData> dataList = new List<SaveData>(1024);
            Save(go, dataList);
            component_total += dataList.Count;
            BinaryWriter Writer = new BinaryWriter(Stream);
            for (int i = 0; i < dataList.Count; ++i)
            {
                long pos = Writer.BaseStream.Position;
                mgr.current = dataList[i].component;
                if (dataList[i].comData.comSave.Collect(dataList[i].component, Writer, mgr))
                {
                    if (Writer.BaseStream.Position != pos)
                    {
                        CompList.Add(dataList[i].component);
                        PositionList.Add((int)pos);
                        ++component_vaildTotal;
                    }
                }
                else
                {
                    Writer.BaseStream.Position = pos;
                }
            }
        }

        public static string ToInfo()
        {
            return string.Format("Total:{0} vaildTotal:{1}", component_total, component_vaildTotal);
        }
#endif
        public delegate void OnResourcesEnd(ComponentSave cos); // 加载完成
        private OnResourcesEnd End;
        private int total; // 总个数
        private int current; // 当前个数

        public void Release()
        {
            End = null;
            total = 0;
            current = 0;
            OnAddDep = null;

#if UNITY_EDITOR && COM_DEBUG
            DependenceList.Clear();
#endif
            depList.Clear();

#if UNITY_EDITOR
            PositionList.Clear();
            CompList.Clear();
            Stream.Position = 0;
            Stream.SetLength(0);
#endif
        }

#if UNITY_EDITOR && COM_DEBUG
        // 这个对象的依赖资源
        public Dictionary<int, Object> DependenceList = new Dictionary<int, Object>();

        public void AddDep(Object obj)
        {
            if (obj == null)
                return;

            if (DependenceList.ContainsKey(obj.GetInstanceID()))
                return;

            DependenceList.Add(obj.GetInstanceID(), obj);
        }

        public Object[] GetDepList()
        {
            Object[] objs = new Object[DependenceList.Count];
            int index = 0;
            foreach (KeyValuePair<int, Object> itor in DependenceList)
                objs[index++] = itor.Value;

            return objs;
        }

#endif
        HashSet<string> depList = new HashSet<string>();

        System.Action<string> OnAddDep = null;

        public void AddDepKey(string key)
        {
            if (depList == null)
                depList = new HashSet<string>();

            if (key.StartsWith("Default-"))
                return;

            if (depList.Add(key))
            {
                OnAddDep(key);
            }
        }

//         public void GetDepList(List<string> keys)
//         {
//             keys.Clear();
//             keys.Capacity = depList.Count;
//             foreach (string key in depList)
//             {
//                 if (key.LastIndexOf('.') != -1)
//                     keys.Add(key);
//             }
//         }

        public bool isDone { get { return total == current ? true : false; } }

        public float progress { get { return 1.0f * current / total; } }

#if UNITY_EDITOR
        HashSet<Component> Loadings = new HashSet<Component>();

        public Component[] GetLoadings()
        {
            Component[] cs = new Component[Loadings.Count];
            int i = 0;
            foreach (Component c in Loadings)
                cs[i++] = c;

            return cs;
        }
#endif

        public int LoadResources(Stream data_stream, Component[] compList, Stream pos_stream, OnResourcesEnd end, System.Action<string> ondep)
        {
            current = 0;
            total = 0;
            End = end;
            OnAddDep = ondep;

            ComData comData = null;
            ComponentData.ParamData pd = null;
            BinaryReader Reader = new BinaryReader(data_stream);
            ComponentData.Data data = null;
            if (pos_stream == null)
            {
                for (int i = 0; i < compList.Length; ++i)
                {
                    Component com = compList[i];
                    if (ComDataList.TryGetValue(com.GetType(), out comData))
                    {
                        pd = ComponentData.CreateParamData(com, OnComponentEnd, com);
                        pd.parent = this;

                        data = comData.comSave.LoadResources(pd, Reader);
                        if (data.mTotal > 0)
                        {
                            ++total;

#if UNITY_EDITOR
                            Loadings.Add(com);
#endif
                        }
                        else
                        {
                            ComponentData.FreeParamData(pd);
                            data.Release();
                        }
                    }
                    else
                    {
                        Debuger.LogError(string.Format("ComponentSave type:{0} not find!", com.GetType().Name));
                    }
                }
            }
            else
            {
                BinaryReader pos_reader = new BinaryReader(pos_stream);
                int newPos = -1;
                for (int i = 0; i < compList.Length; ++i)
                {
                    Component com = compList[i];
                    if (com == null)
                    {
                        if (i != compList.Length - 1)
                        {
                            pos_reader.BaseStream.Position = (i + 1) * 4;
                            newPos = pos_reader.ReadInt32();
                            Reader.BaseStream.Position = newPos;
                            continue;
                        }
                    }
                    else
                    {
                        if (ComDataList.TryGetValue(com.GetType(), out comData))
                        {
                            pd = ComponentData.CreateParamData(com, OnComponentEnd, com);
                            pd.parent = this;
                            data = comData.comSave.LoadResources(pd, Reader);
                            if (data.mTotal > 0)
                            {
                                ++total;
#if UNITY_EDITOR
                                Loadings.Add(com);
#endif
                            }
                            else
                            {
                                ComponentData.FreeParamData(pd);
                                data.Release();
                            }
                        }
                        else
                        {
                            Debuger.LogError(string.Format("ComponentSave type:{0} not find!", com.GetType().Name));
                        }
                    }
                }

                pos_stream.Close();
                pos_reader.Close();
            }

            data_stream.Close();
            Reader.Close();
            return total;
        }

        void OnComponentEnd(object p)
        {
#if UNITY_EDITOR
            Loadings.Remove((Component)p);
#endif
            current++;
            if (current == total)
            {
                if (End != null)
                {
                    End(this);
                }
            }
            else if (current > total)
            {
                Debuger.ErrorLog("current > total");
            }
        }
    }
#endif
}
