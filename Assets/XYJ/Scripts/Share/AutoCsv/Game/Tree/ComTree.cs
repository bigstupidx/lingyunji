// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ComTree 
    {
        static Dictionary<int, ComTree> DataList = new Dictionary<int, ComTree>();

        static public Dictionary<int, ComTree> GetAll()
        {
            return DataList;
        }

        static public ComTree Get(int key)
        {
            ComTree value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("ComTree.Get({0}) not find!", key);
            return null;
        }



        // id
        public int id { get; set; }

        // 种树场景id
        public int sceneId { get; set; }

        // 点集id
        public string pointId { get; set; }

        // 种树阶段时间
        public int[] plantStageTime { get; set; }

        // 种树阶段资源id
        public int[] resId { get; set; }

        // 种树可以加速次数
        public int speedUpTimes { get; set; }

        // 种树成长值上限
        public int growthMax { get; set; }

        // 种树加速道具id
        public int speedUpItemId { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ComTree);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ComTree);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ComTree> allDatas = new List<ComTree>();

            {
                string file = "Tree/ComTree.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int sceneId_index = reader.GetIndex("sceneId");
                int pointId_index = reader.GetIndex("pointId");
                int plantStageTime_index = reader.GetIndex("plantStageTime");
                int resId_index = reader.GetIndex("resId");
                int speedUpTimes_index = reader.GetIndex("speedUpTimes");
                int growthMax_index = reader.GetIndex("growthMax");
                int speedUpItemId_index = reader.GetIndex("speedUpItemId");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ComTree data = new ComTree();
						data.id = reader.getInt(i, id_index, 0);         
						data.sceneId = reader.getInt(i, sceneId_index, 0);         
						data.pointId = reader.getStr(i, pointId_index);         
						data.plantStageTime = reader.getInts(i, plantStageTime_index, ';');         
						data.resId = reader.getInts(i, resId_index, ';');         
						data.speedUpTimes = reader.getInt(i, speedUpTimes_index, 0);         
						data.growthMax = reader.getInt(i, growthMax_index, 0);         
						data.speedUpItemId = reader.getInt(i, speedUpItemId_index, 0);         
                        if (lineParseMethod != null)
                            lineParseMethod.Invoke(null, new object[3] {data, reader, i });
                        allDatas.Add(data);
                    }
                    catch (System.Exception ex)
                    {
                        CsvCommon.Log.Error("file:{0} line:{1} error!", file, i);
                        CsvCommon.Log.Exception(ex);
                    }
                }
            }
            
            foreach(var data in allDatas)
            {
                if (DataList.ContainsKey(data.id))
                {
                    CsvCommon.Log.Error("ComTree.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(ComTree);
                    while (null != curType)
                    {
                        method = curType.GetMethod("OnLoadEnd", BindingFlags.Static | BindingFlags.NonPublic);
                        if (null != method)
                            break;
                        curType = curType.BaseType;
                    }
                }
                if (method != null)
                    method.Invoke(null, new object[0]);
            }
        }
    }
}


