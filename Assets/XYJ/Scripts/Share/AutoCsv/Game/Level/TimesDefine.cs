// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class TimesDefine 
    {
        static Dictionary<int, TimesDefine> DataList = new Dictionary<int, TimesDefine>();

        static public Dictionary<int, TimesDefine> GetAll()
        {
            return DataList;
        }

        static public TimesDefine Get(int key)
        {
            TimesDefine value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("TimesDefine.Get({0}) not find!", key);
            return null;
        }



        // AutoId
        public int id { get; set; }

        // 时间组ID
        public int timeId { get; set; }

        // 周
        public int[] weekTime { get; set; }

        // 开始时间-年
        public int beginYear { get; set; }

        // 开始时间-月
        public int beginMonth { get; set; }

        // 开始时间-日
        public int beginDay { get; set; }

        // 开始时间-时
        public int beginHour { get; set; }

        // 开始时间-分
        public int beginMinute { get; set; }

        // 结束时间-年
        public int endYear { get; set; }

        // 结束时间-月
        public int endMonth { get; set; }

        // 结束时间-日
        public int endDay { get; set; }

        // 结束时间-时
        public int endHour { get; set; }

        // 结束时间-分
        public int endMinute { get; set; }

        // 备注
        public string desc { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(TimesDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(TimesDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<TimesDefine> allDatas = new List<TimesDefine>();

            {
                string file = "Level/TimesDefine.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int timeId_index = reader.GetIndex("timeId");
                int weekTime_index = reader.GetIndex("weekTime");
                int beginYear_index = reader.GetIndex("beginYear");
                int beginMonth_index = reader.GetIndex("beginMonth");
                int beginDay_index = reader.GetIndex("beginDay");
                int beginHour_index = reader.GetIndex("beginHour");
                int beginMinute_index = reader.GetIndex("beginMinute");
                int endYear_index = reader.GetIndex("endYear");
                int endMonth_index = reader.GetIndex("endMonth");
                int endDay_index = reader.GetIndex("endDay");
                int endHour_index = reader.GetIndex("endHour");
                int endMinute_index = reader.GetIndex("endMinute");
                int desc_index = reader.GetIndex("desc");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        TimesDefine data = new TimesDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.timeId = reader.getInt(i, timeId_index, 0);         
						data.weekTime = reader.getInts(i, weekTime_index, ';');         
						data.beginYear = reader.getInt(i, beginYear_index, 0);         
						data.beginMonth = reader.getInt(i, beginMonth_index, 0);         
						data.beginDay = reader.getInt(i, beginDay_index, 0);         
						data.beginHour = reader.getInt(i, beginHour_index, 0);         
						data.beginMinute = reader.getInt(i, beginMinute_index, 0);         
						data.endYear = reader.getInt(i, endYear_index, 0);         
						data.endMonth = reader.getInt(i, endMonth_index, 0);         
						data.endDay = reader.getInt(i, endDay_index, 0);         
						data.endHour = reader.getInt(i, endHour_index, 0);         
						data.endMinute = reader.getInt(i, endMinute_index, 0);         
						data.desc = reader.getStr(i, desc_index);         
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
                    CsvCommon.Log.Error("TimesDefine.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(TimesDefine);
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


