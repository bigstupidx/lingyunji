// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class ClanPermissionConfig 
    {
        static List<ClanPermissionConfig> DataList = new List<ClanPermissionConfig>();
        static public List<ClanPermissionConfig> GetAll()
        {
            return DataList;
        }

        static public ClanPermissionConfig Get(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].postId == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("ClanPermissionConfig.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(int key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].postId == key)
                    return i;
            }
            return -1;
        }



        // 职位代号
        public int postId { get; set; }

        // 职位
        public string postName { get; set; }

        // 管理氏族建设
        public int clanBuildManage { get; set; }

        // 邀请入氏族
        public int inviteAdd { get; set; }

        // 通过或拒绝入氏族申请
        public int resureApply { get; set; }

        // 逐出氏族
        public int outClan { get; set; }

        // 设定福利
        public int setDkp { get; set; }

        // 氏族活动开启
        public int openAct { get; set; }

        // 群发消息权限
        public int sendMsgGroup { get; set; }

        // 修改氏族公告
        public int changeNotice { get; set; }

        // 参加氏族pvp活动
        public int joinPvp { get; set; }

        // 参加氏族pve活动
        public int joinPve { get; set; }

        // 参与氏族分红
        public int joinDividend { get; set; }

        // 使用氏族修炼
        public int usePractice { get; set; }

        // 参与氏族抽奖
        public int joinLottry { get; set; }

        // 参与花魁竞选
        public int joinOiran { get; set; }

        // 参加氏族联赛
        public int joinUnion { get; set; }

        // 职位自动变更时间
        public int postChangeTime { get; set; }

        // 离线时间
        public int outLineTime { get; set; }

        // 修改氏族名称权限
        public int changeClanName { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(ClanPermissionConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(ClanPermissionConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<ClanPermissionConfig> allDatas = new List<ClanPermissionConfig>();

            {
                string file = "Clan/ClanPermissionConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int postId_index = reader.GetIndex("postId");
                int postName_index = reader.GetIndex("postName");
                int clanBuildManage_index = reader.GetIndex("clanBuildManage");
                int inviteAdd_index = reader.GetIndex("inviteAdd");
                int resureApply_index = reader.GetIndex("resureApply");
                int outClan_index = reader.GetIndex("outClan");
                int setDkp_index = reader.GetIndex("setDkp");
                int openAct_index = reader.GetIndex("openAct");
                int sendMsgGroup_index = reader.GetIndex("sendMsgGroup");
                int changeNotice_index = reader.GetIndex("changeNotice");
                int joinPvp_index = reader.GetIndex("joinPvp");
                int joinPve_index = reader.GetIndex("joinPve");
                int joinDividend_index = reader.GetIndex("joinDividend");
                int usePractice_index = reader.GetIndex("usePractice");
                int joinLottry_index = reader.GetIndex("joinLottry");
                int joinOiran_index = reader.GetIndex("joinOiran");
                int joinUnion_index = reader.GetIndex("joinUnion");
                int postChangeTime_index = reader.GetIndex("postChangeTime");
                int outLineTime_index = reader.GetIndex("outLineTime");
                int changeClanName_index = reader.GetIndex("changeClanName");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        ClanPermissionConfig data = new ClanPermissionConfig();
						data.postId = reader.getInt(i, postId_index, 0);         
						data.postName = reader.getStr(i, postName_index);         
						data.clanBuildManage = reader.getInt(i, clanBuildManage_index, 0);         
						data.inviteAdd = reader.getInt(i, inviteAdd_index, 0);         
						data.resureApply = reader.getInt(i, resureApply_index, 0);         
						data.outClan = reader.getInt(i, outClan_index, 0);         
						data.setDkp = reader.getInt(i, setDkp_index, 0);         
						data.openAct = reader.getInt(i, openAct_index, 0);         
						data.sendMsgGroup = reader.getInt(i, sendMsgGroup_index, 0);         
						data.changeNotice = reader.getInt(i, changeNotice_index, 0);         
						data.joinPvp = reader.getInt(i, joinPvp_index, 0);         
						data.joinPve = reader.getInt(i, joinPve_index, 0);         
						data.joinDividend = reader.getInt(i, joinDividend_index, 0);         
						data.usePractice = reader.getInt(i, usePractice_index, 0);         
						data.joinLottry = reader.getInt(i, joinLottry_index, 0);         
						data.joinOiran = reader.getInt(i, joinOiran_index, 0);         
						data.joinUnion = reader.getInt(i, joinUnion_index, 0);         
						data.postChangeTime = reader.getInt(i, postChangeTime_index, 0);         
						data.outLineTime = reader.getInt(i, outLineTime_index, 0);         
						data.changeClanName = reader.getInt(i, changeClanName_index, 0);         
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
            
            DataList = allDatas;

            {
                MethodInfo method = null;
                {
                    var curType = typeof(ClanPermissionConfig);
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


