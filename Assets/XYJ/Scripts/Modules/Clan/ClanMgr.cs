
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys
{
    using Network;
    using NetProto;
    using UI;
    using System;
    using Config;

    public class ClanMgr : W2CClanRespone
    {
        public ClanMgr(LocalPlayer l) : base(App.my.world.remote)
        {
            request = new C2WClanRequest(App.my.world.local);

            localPlayer = App.my.localPlayer;

            App.my.eventSet.Subscribe<CreateData>(EventID.Clan_Create, this.OnClickCreateClan);
            App.my.eventSet.Subscribe(EventID.Clan_GetAllClan, this.OnClickGetAllClanData);
            App.my.eventSet.Subscribe(EventID.Clan_GetSelfClan, this.OnClickGetCurClanData);

            App.my.eventSet.Subscribe<long>(EventID.Clan_ApplyClan, this.OnApplyClan);
            App.my.eventSet.Subscribe(EventID.Clan_OneKeyApply, this.OnOneKeyApplyClan);
            App.my.eventSet.Subscribe<long>(EventID.Clan_ContactLeader, this.OnContactLeader);

            App.my.eventSet.Subscribe<ResponeClanMsg>(EventID.Clan_Respone, this.OnResponeClan);

            App.my.eventSet.Subscribe<ClanDbData>(EventID.Clan_UpdataInfo, this.OnUpdataClan);

            App.my.eventSet.Subscribe<int>(EventID.Clan_Build_LevelUp, this.OnBuildLevelUp);

            App.my.handler.Reg<ClanDbData>(Protoid.W2C_ClanUpdata, OnRecvClanUpdata);

            

        }

        public LocalPlayer localPlayer { get; private set; }
        public C2WClanRequest request { get; private set; }

        public ClanAllDbData m_allClanData { get; private set; }

        private ClanDbData m_curClanData { get; set; }
        protected override None OnW2CRpcTest(NetProto.Int64 input)
        {
            return new None();
        }


        const int ONE_KEY_NUM = 10;

        protected void OnRecvClanUpdata(ClanDbData data)
        {
            if (data != null)
            {
                ClanDbData tempData = null;
                if (m_allClanData != null)
                {
                    if (m_allClanData.allClanData.TryGetValue(data.clanid, out tempData))
                    {
                        tempData = null;
                        tempData = data;
                    }
                    else
                    {
                        m_allClanData.allClanData.Add(data.clanid, data);
                    }
                    App.my.eventSet.FireEvent<ClanAllDbData>(EventID.Clan_RecvAllClan, m_allClanData);
                }
                if (m_curClanData != null && m_curClanData.clanid == data.clanid)
                {
                    m_curClanData = null;
                    m_curClanData = data;
                    App.my.eventSet.FireEvent<ClanDbData>(EventID.Clan_RecvSelfClan, m_curClanData);
                }              
            }
        }
        public string GetLeaderName(long clanId)
        {
            if (m_allClanData != null)
            {
                ClanDbData data = new ClanDbData();
                if (m_allClanData.allClanData.TryGetValue(clanId, out data))
                {
                    foreach (var item in data.member.membermap)
                    {
                        if (item.Value.post == ClanPost.Clan_Leader)
                        {
                            return item.Value.name;
                        }
                    }
                }
            }
            return "";
        }

        public Dictionary<long, ClanDbData> GetClanData(int isrespone = 0)
        {
            Dictionary<long, ClanDbData> tempMap = new Dictionary<long, ClanDbData>();

            if (m_allClanData != null)
            {
                foreach (var item in m_allClanData.allClanData)
                {
                    if (item.Value.isrespone == isrespone)
                    {
                        tempMap.Add(item.Key, item.Value);
                    }
                }
            }
            return tempMap;
        }

        public void UpdataClanInfo(ClanDbData data)
        {
            if (m_allClanData != null && data != null)
            {
                if (m_allClanData.allClanData.ContainsKey(data.clanid))
                {
                    m_allClanData.allClanData[data.clanid] = null;
                    m_allClanData.allClanData[data.clanid] = data;
                }
            }
        }

        public bool IsCurDataInCreate()
        {
            return IsInCreate(m_curClanData);
        }

        //是否创建了氏族 并且在响应中
        public bool IsInCreate(ClanDbData data)
        {

            if (IsInClan())
            {
                return false;
            }
            if (localPlayer != null && localPlayer.clanResponeIdValue > 0 && data != null && data.isrespone == 0 && data.leaderid == App.my.localPlayer.charid)
            {
                return true;
            }

            return false;
        }

        //是否响应了氏族
        public bool IsInRespone(ClanDbData data)
        {

            if (IsInClan())
            {
                return false;
            }
            if (localPlayer != null && localPlayer.clanResponeIdValue > 0 && data != null && data.isrespone == 0 && data.leaderid != App.my.localPlayer.charid)
            {
                return true;
            }

            return false;
        }

        //是否已经加入了氏族
        public bool IsInClan()
        {
            if (localPlayer != null && localPlayer.clanIdValue > 0)
            {
                return true;
            }
            return false;
        }


        //创建氏族
        public void OnClickCreateClan(CreateData data)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteCreateClan(data));
        }

        //获取自己的氏族
        public ClanDbData GetSelfCurClan()
        {
            return m_curClanData;
        }

        protected IEnumerator ExecuteCreateClan(CreateData data)
        {
            var yyd = request.CreateClanYield(data);
            yield return yyd;

            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc CreateClanYield fail");
            }
            else
            {
                ClanInfo info = yyd.result;
                switch (info.code)
                {
                    case ReturnCode.Clan_Succeed:
                        SystemHintMgr.ShowHint("氏族创建成功！");
                        //OnClickGetAllClanData();
                        /*
                        if (!m_allClanData.allClanData.ContainsKey(info.data.clanid))
                        {
                            m_allClanData.allClanData.Add(info.data.clanid, info.data);
                            App.my.eventSet.FireEvent<ClanAllDbData>(EventID.Clan_RecvAllClan, m_allClanData);
                        }
                        */
                        OnClickGetCurClanData();

                        break;
                    case ReturnCode.Clan_IsExist:
                        SystemHintMgr.ShowHint("你已经拥有氏族！");
                        break;
                    case ReturnCode.Clan_IsNotExist:
                        SystemHintMgr.ShowHint("氏族名称已被使用！");
                        break;
                    case ReturnCode.Clan_IsEmpty:
                        SystemHintMgr.ShowHint("氏族名称或公告不能为空");
                        break;
                    case ReturnCode.Clan_IsOutRange:
                        SystemHintMgr.ShowHint("氏族名称或公告长度超出上限");
                        break;
                    case ReturnCode.Clan_IsInRespone:

                        break;
                    case ReturnCode.Clan_MeneyLimit:
                        break;
                    case ReturnCode.Clan_NameCantUse:
                        SystemHintMgr.ShowHint("非法氏族名称不可用");
                        break;
                    case ReturnCode.Clan_DecCanUse:
                        SystemHintMgr.ShowHint("公告中有非法词语请修改");
                        break;
                    case ReturnCode.Clan_InRespone:
                        SystemHintMgr.ShowHint("你在创建氏族中无法创建其他氏族");
                        break;
                    case ReturnCode.Clan_InCreate:
                        SystemHintMgr.ShowHint("你在响应其他氏族中不能再创建其他氏族");
                        break;
                    case ReturnCode.Clan_NameBeUsed:
                        SystemHintMgr.ShowHint("氏族名称已被使用");
                        break;
                    default:
                        break;
                }


            }

        }

        //获取所有氏族列表
        public void OnClickGetAllClanData()
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteGetAllClanData());
        }

        protected IEnumerator ExecuteGetAllClanData()
        {
            var yyd = request.GetAllClanYield();
            yield return yyd;

            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc GetAllClanYield fail");
            }
            else
            {
                ClanAllDbData alldata = yyd.result;

                if (alldata != null)
                {
                    m_allClanData = null;
                    m_allClanData = alldata;

                    App.my.eventSet.FireEvent<ClanAllDbData>(EventID.Clan_RecvAllClan, m_allClanData);
                }
            }
        }

        //获取自己的氏族
        public void OnClickGetCurClanData()
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteGetCurClanData());
        }

        protected IEnumerator ExecuteGetCurClanData()
        {
            var yyd = request.GetCurClanYield();
            yield return yyd;

            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc GetAllClanYield fail");
            }
            else
            {
                ClanInfo selfdata = yyd.result;

                if (selfdata.code == ReturnCode.Clan_Succeed)
                {
                    m_curClanData = null;
                    m_curClanData = selfdata.data;
                    App.my.eventSet.FireEvent<ClanDbData>(EventID.Clan_RecvSelfClan, m_curClanData);
                }
                else if (selfdata.code == ReturnCode.Clan_IsEmpty)
                {
                    m_curClanData = null;
                }

            }
        }

        //申请加入
        public void OnApplyClan(long clanId)
        {

            App.my.mainCoroutine.StartCoroutine(ExecuteApplyClan(clanId));
        }

        protected IEnumerator ExecuteApplyClan(long clanId)
        {
            var yyd = request.ApplyClanYield(new NetProto.Int64 { value = clanId });
            yield return yyd;

            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc ApplyClanYield fail");
            }
            else
            {
                ClanInfo tempdata = yyd.result;

                if (tempdata.code == ReturnCode.Clan_Succeed)
                {
                    SystemHintMgr.ShowHint("申请成功");

                }
                else if (tempdata.code == ReturnCode.Clan_IsExist)
                {
                    //SystemHintMgr.ShowHint("你已经发送过申请啦~");
                }
                else if (tempdata.code == ReturnCode.Clan_IsEmpty)
                {
                    //SystemHintMgr.ShowHint("该氏族不存在或者已经解散啦~");
                }
            }
        }

        //一键申请
        public void OnOneKeyApplyClan()
        {

            Dictionary<long, ClanDbData> tempMap = GetClanData(1);
            if (ONE_KEY_NUM >= tempMap.Count)
            {
                foreach (var item in tempMap)
                {
                    OnApplyClan(item.Key);
                }
            }
            else
            {
                Dictionary<long, ClanDbData> temp = GetRandomList(ONE_KEY_NUM);
                foreach (var item in temp)
                {
                    OnApplyClan(item.Key);
                }
            }

        }

        //随机抽取的数量的氏族
        public Dictionary<long, ClanDbData> GetRandomList(int number)
        {
            Dictionary<long, ClanDbData> resultMap = new Dictionary<long, ClanDbData>();
            resultMap.Clear();
            GetRandomData(this.GetClanData(1), resultMap, number);

            return resultMap;
        }

        public void GetRandomData(Dictionary<long, ClanDbData> clanMap, Dictionary<long, ClanDbData> ResultMap, int number)
        {
            if (clanMap == null || clanMap.Count <= 0 || number == 0 || number > clanMap.Count)
                return;

            List<long> idList = new List<long>();
            idList.Clear();
            foreach (var item in clanMap)
            {
                idList.Add(item.Key);
            }

            Random ra = new Random(number);
            int idx = ra.Next(number);
            ResultMap.Add(idList[idx], clanMap[idList[idx]]);
            clanMap.Remove(idList[idx]);
            GetRandomData(clanMap, ResultMap, number - 1);
        }

        //联系族长
        public void OnContactLeader(long clanId)
        {
            if (m_allClanData != null && m_allClanData.allClanData.ContainsKey(clanId))
            {
                long leaderId = m_allClanData.allClanData[clanId].leaderid;
                App.my.mainCoroutine.StartCoroutine(ExecuteContactLeader(leaderId));
            }
        }

        protected IEnumerator ExecuteContactLeader(long leaderId)
        {
            var yyd = request.ContactClanLeaderYield(new NetProto.Int64 { value = leaderId });
            yield return yyd;

            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc ContactClanLeaderYield fail");
            }
            else
            {
                ClanInfo tempdata = yyd.result;

                if (tempdata.code == ReturnCode.Clan_Succeed)
                {
                    //SystemHintMgr.ShowHint("打开好友-最近聊天中查看");
                    xys.App.my.uiSystem.ShowPanel("UIFriendPanel", new object() { }, true);
                    xys.App.my.uiSystem.HidePanel("UIClanCreatePanel");
                    App.my.eventSet.FireEvent<long>(EventID.Clan_ToFriend_Select, leaderId);
                }
            }
        }

        //响应 取消响应
        public void OnResponeClan(ResponeClanMsg msg)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteResponeClan(msg));
        }


        protected IEnumerator ExecuteResponeClan(ResponeClanMsg msg)
        {
            var yyd = request.ResponeClanYield(msg);
            yield return yyd;

            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc ResponeClanYield fail");
            }
            else
            {
                ClanInfo tempdata = yyd.result;

                if (tempdata.code == ReturnCode.Clan_Succeed)
                {
                    if (!msg.iscancel)
                    {
                        SystemHintMgr.ShowHint("成功响应了{0}氏族", tempdata.data.name);
                    }
                    else
                    {
                        SystemHintMgr.ShowHint("取消响应了{0}氏族", tempdata.data.name);
                    }

                    this.UpdataClanInfo(tempdata.data);
                    OnClickGetCurClanData();
                    App.my.eventSet.FireEvent<ClanAllDbData>(EventID.Clan_RecvAllClan, m_allClanData);
                }
            }
        }

        public void OnUpdataClan(ClanDbData data)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteUpdataClan(data));
        }

        protected IEnumerator ExecuteUpdataClan(ClanDbData data)
        {
            var yyd = request.UpDataClanYield(data);
            yield return yyd;

            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc UpDataClanYield fail");
            }
            else
            {
                ClanInfo tempdata = yyd.result;
                if (tempdata.code == ReturnCode.Clan_Succeed)
                {
                    SystemHintMgr.ShowHint("修改成功");
                }
            }
        }

        //升级建筑
        public void OnBuildLevelUp(int buildId)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteBuildLevelUp(buildId));

        }

        protected IEnumerator ExecuteBuildLevelUp(int buildId)
        {
            var yyd = request.ClanBuildLevelUpYield(new NetProto.Int32 { value = buildId } );
            yield return yyd;

            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc ClanBuildLevelUpYield fail");
            }
            else
            {
                ClanInfo tempdata = yyd.result;
                if (tempdata.code == ReturnCode.Clan_Succeed)
                {
                    SystemHintMgr.ShowHint("升级成功");
                }
            }
        }
    }
}
