// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class AttributeDefine 
    {
        static Dictionary<int, AttributeDefine> DataList = new Dictionary<int, AttributeDefine>();

        static public Dictionary<int, AttributeDefine> GetAll()
        {
            return DataList;
        }

        static public AttributeDefine Get(int key)
        {
            AttributeDefine value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("AttributeDefine.Get({0}) not find!", key);
            return null;
        }



        // ID
        public int id { get; set; }

        // 名称
        public string name { get; set; }

        // 数据类型
        public DataType dataType { get; set; }

        // 数据显示类型
        public UIShowType uiShowType { get; set; }

        // 属性界面显示
        public string attrName { get; set; }

        // 其他界面显示
        public string attrNameByOtherUI { get; set; }

        // 显示类型
        public int attrColumn { get; set; }

        // 角色详细属性排序
        public int attrOrder { get; set; }

        // 灵兽基础属性排序
        public int petBaseAttrOrder { get; set; }

        // 灵兽详细属性排序
        public int petAttrOrder { get; set; }

        // 灵兽加点属性排序
        public int petAddPointOrder { get; set; }

        // 法宝基础属性排序
        public int trumpBaseAttrOrder { get; set; }

        // 法宝详细属性排序
        public int trumpAttrOrder { get; set; }

        // 属性描述
        public string attrDescribe { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(AttributeDefine);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(AttributeDefine);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<AttributeDefine> allDatas = new List<AttributeDefine>();

            {
                string file = "Attribute/AttributeDefine!.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int dataType_index = reader.GetIndex("dataType");
                int uiShowType_index = reader.GetIndex("uiShowType");
                int attrName_index = reader.GetIndex("attrName");
                int attrNameByOtherUI_index = reader.GetIndex("attrNameByOtherUI");
                int attrColumn_index = reader.GetIndex("attrColumn");
                int attrOrder_index = reader.GetIndex("attrOrder");
                int petBaseAttrOrder_index = reader.GetIndex("petBaseAttrOrder");
                int petAttrOrder_index = reader.GetIndex("petAttrOrder");
                int petAddPointOrder_index = reader.GetIndex("petAddPointOrder");
                int trumpBaseAttrOrder_index = reader.GetIndex("trumpBaseAttrOrder");
                int trumpAttrOrder_index = reader.GetIndex("trumpAttrOrder");
                int attrDescribe_index = reader.GetIndex("attrDescribe");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        AttributeDefine data = new AttributeDefine();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.dataType = ((DataType)reader.getInt(i, dataType_index, 0));         
						data.uiShowType = ((UIShowType)reader.getInt(i, uiShowType_index, 0));         
						data.attrName = reader.getStr(i, attrName_index);         
						data.attrNameByOtherUI = reader.getStr(i, attrNameByOtherUI_index);         
						data.attrColumn = reader.getInt(i, attrColumn_index, 0);         
						data.attrOrder = reader.getInt(i, attrOrder_index, 0);         
						data.petBaseAttrOrder = reader.getInt(i, petBaseAttrOrder_index, 0);         
						data.petAttrOrder = reader.getInt(i, petAttrOrder_index, 0);         
						data.petAddPointOrder = reader.getInt(i, petAddPointOrder_index, 0);         
						data.trumpBaseAttrOrder = reader.getInt(i, trumpBaseAttrOrder_index, 0);         
						data.trumpAttrOrder = reader.getInt(i, trumpAttrOrder_index, 0);         
						data.attrDescribe = reader.getStr(i, attrDescribe_index);         
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
                    CsvCommon.Log.Error("AttributeDefine.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(AttributeDefine);
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


