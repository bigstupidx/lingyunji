// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class RoleJob 
    {
        static Dictionary<int, RoleJob> DataList = new Dictionary<int, RoleJob>();

        static public Dictionary<int, RoleJob> GetAll()
        {
            return DataList;
        }

        static public RoleJob Get(int key)
        {
            RoleJob value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("RoleJob.Get({0}) not find!", key);
            return null;
        }



        // 职业ID
        public int id { get; set; }

        // 职业名称
        public string name { get; set; }

        // 男角色ID
        public int maleId { get; set; }

        // 女角色ID
        public int felmaleId { get; set; }

        // 是否开放
        public bool isOpen { get; set; }

        // 五行属性
        public int attributeType { get; set; }

        // 姿态1ID
        public int postureID1 { get; set; }

        // 姿态1名称
        public string postureName1 { get; set; }

        // 姿态2ID
        public int postureID2 { get; set; }

        // 姿态2名称
        public string postureName2 { get; set; }

        // 姿态1技能
        public string posture1Skills { get; set; }

        // 姿态2技能
        public string posture2Skills { get; set; }

        // 受身技能
        public string postureAllSkills { get; set; }

        // 绝技技能
        public string ultSkills { get; set; }

        // 轻功
        public string jump { get; set; }

        // 职业心法
        public string xinfaList { get; set; }

        // 姿态1心法
        public string posture1Xinfa { get; set; }

        // 姿态2心法
        public string posture2Xinfa { get; set; }

        // 心法提升参数
        public int xinfaParam { get; set; }

        // 男角色介绍
        public string maleIntroduce { get; set; }

        // 女角色介绍
        public string femaleIntroduce { get; set; }

        // 图标
        public string icon { get; set; }

        // 职业图标背景色
        public string colorIcon { get; set; }

        // 职业组队背景
        public string teamColorIcon { get; set; }

        // 属性面板图标
        public string attrPanelIcon { get; set; }

        // 男角色头像
        public string maleIcon { get; set; }

        // 女角色头像
        public string femalIcon { get; set; }

        // 男角色创建动画
        public string maleCreateAnim { get; set; }

        // 女角色创建动画
        public string femaleCreateAnim { get; set; }

        // 创角界面职业美术字
        public string createFont { get; set; }

        // 男头像底板
        public string maleHeadBack { get; set; }

        // 女头像底板
        public string femalHeadBank { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(RoleJob);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(RoleJob);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<RoleJob> allDatas = new List<RoleJob>();

            {
                string file = "Role/RoleJob.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int maleId_index = reader.GetIndex("maleId");
                int felmaleId_index = reader.GetIndex("felmaleId");
                int isOpen_index = reader.GetIndex("isOpen");
                int attributeType_index = reader.GetIndex("attributeType");
                int postureID1_index = reader.GetIndex("postureID1");
                int postureName1_index = reader.GetIndex("postureName1");
                int postureID2_index = reader.GetIndex("postureID2");
                int postureName2_index = reader.GetIndex("postureName2");
                int posture1Skills_index = reader.GetIndex("posture1Skills");
                int posture2Skills_index = reader.GetIndex("posture2Skills");
                int postureAllSkills_index = reader.GetIndex("postureAllSkills");
                int ultSkills_index = reader.GetIndex("ultSkills");
                int jump_index = reader.GetIndex("jump");
                int xinfaList_index = reader.GetIndex("xinfaList");
                int posture1Xinfa_index = reader.GetIndex("posture1Xinfa");
                int posture2Xinfa_index = reader.GetIndex("posture2Xinfa");
                int xinfaParam_index = reader.GetIndex("xinfaParam");
                int maleIntroduce_index = reader.GetIndex("maleIntroduce");
                int femaleIntroduce_index = reader.GetIndex("femaleIntroduce");
                int icon_index = reader.GetIndex("icon");
                int colorIcon_index = reader.GetIndex("colorIcon");
                int teamColorIcon_index = reader.GetIndex("teamColorIcon");
                int attrPanelIcon_index = reader.GetIndex("attrPanelIcon");
                int maleIcon_index = reader.GetIndex("maleIcon");
                int femalIcon_index = reader.GetIndex("femalIcon");
                int maleCreateAnim_index = reader.GetIndex("maleCreateAnim");
                int femaleCreateAnim_index = reader.GetIndex("femaleCreateAnim");
                int createFont_index = reader.GetIndex("createFont");
                int maleHeadBack_index = reader.GetIndex("maleHeadBack");
                int femalHeadBank_index = reader.GetIndex("femalHeadBank");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        RoleJob data = new RoleJob();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.maleId = reader.getInt(i, maleId_index, 0);         
						data.felmaleId = reader.getInt(i, felmaleId_index, 0);         
						data.isOpen = reader.getBool(i, isOpen_index, false);         
						data.attributeType = reader.getInt(i, attributeType_index, 0);         
						data.postureID1 = reader.getInt(i, postureID1_index, 0);         
						data.postureName1 = reader.getStr(i, postureName1_index);         
						data.postureID2 = reader.getInt(i, postureID2_index, 0);         
						data.postureName2 = reader.getStr(i, postureName2_index);         
						data.posture1Skills = reader.getStr(i, posture1Skills_index);         
						data.posture2Skills = reader.getStr(i, posture2Skills_index);         
						data.postureAllSkills = reader.getStr(i, postureAllSkills_index);         
						data.ultSkills = reader.getStr(i, ultSkills_index);         
						data.jump = reader.getStr(i, jump_index);         
						data.xinfaList = reader.getStr(i, xinfaList_index);         
						data.posture1Xinfa = reader.getStr(i, posture1Xinfa_index);         
						data.posture2Xinfa = reader.getStr(i, posture2Xinfa_index);         
						data.xinfaParam = reader.getInt(i, xinfaParam_index, 0);         
						data.maleIntroduce = reader.getStr(i, maleIntroduce_index);         
						data.femaleIntroduce = reader.getStr(i, femaleIntroduce_index);         
						data.icon = reader.getStr(i, icon_index);         
						data.colorIcon = reader.getStr(i, colorIcon_index);         
						data.teamColorIcon = reader.getStr(i, teamColorIcon_index);         
						data.attrPanelIcon = reader.getStr(i, attrPanelIcon_index);         
						data.maleIcon = reader.getStr(i, maleIcon_index);         
						data.femalIcon = reader.getStr(i, femalIcon_index);         
						data.maleCreateAnim = reader.getStr(i, maleCreateAnim_index);         
						data.femaleCreateAnim = reader.getStr(i, femaleCreateAnim_index);         
						data.createFont = reader.getStr(i, createFont_index);         
						data.maleHeadBack = reader.getStr(i, maleHeadBack_index);         
						data.femalHeadBank = reader.getStr(i, femalHeadBank_index);         
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
                    CsvCommon.Log.Error("RoleJob.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(RoleJob);
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


