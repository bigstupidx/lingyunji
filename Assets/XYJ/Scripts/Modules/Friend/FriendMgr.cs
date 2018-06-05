
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

    public class FriendMgr : W2CFriendRespone
    {
        public FriendMgr(LocalPlayer l) : base(App.my.world.remote)
        {

            request = new C2WFriendRequest(App.my.world.local);
            localPlayer = l;

            //App.my.handler.Reg<FriendSearchInfo>(Protoid.W2C_FriendSearch, OnSearchListInfo);

            //App.my.handler.Reg<FriendContactInfo>(Protoid.W2C_Friend_FriendInfo, OnRecvFriendInfo);


            App.my.handler.Reg<FriendItemInfo>(Protoid.W2C_Friend_Updata, OnFriendUpdata);

           // App.my.handler.Reg<FriendsApplyInfos>(Protoid.W2C_FriendApplyLy, OnRecvFriendsApplyInfos);
            //App.my.handler.Reg<FriendItemInfo>(Protoid.W2C_Friend_Detail, OnPushFriendItemInfo);

            App.my.handler.Reg<FriendTips>(Protoid.W2C_Friend_AddFriend_To_Other, OnRecvAddFriendTipsToOther);

            App.my.handler.Reg<FriendTips>(Protoid.W2C_Friend_AddFriend_To_Self, OnRecvAddFriendTipsToSelf);

            App.my.handler.Reg<FriendTips>(Protoid.W2C_Friend_Online, OnRecvFriendOnline);

            App.my.handler.Reg<FriendTips>(Protoid.W2C_Friend_Outline, OnRecvFriendOutline);

            App.my.eventSet.Subscribe<string>(EventID.Friend_Search, this.SearchFriend);
            App.my.eventSet.Subscribe(EventID.Friend_GetApplyData, this.GetApplyData);
            App.my.eventSet.Subscribe<long>(EventID.Friend_Apply, this.ApplyFriend);
            App.my.eventSet.Subscribe<long>(EventID.Friend_AddFriend, this.AddFriend);
            App.my.eventSet.Subscribe(EventID.Friend_GetFriendData, this.GetFriendData);
            App.my.eventSet.Subscribe<long>(EventID.Friend_BlakSomeOne, this.BlakSomeOne);

            App.my.eventSet.Subscribe(EventID.Friend_GetRecentlyInfo, this.GetRecentlyInfo);
            //App.my.eventSet.Subscribe<long>(EventID.Friend_GetRecentlyChat, this.GetRecentlyChat);
            //App.my.eventSet.Subscribe<long>(EventID.Friend_GetRecentlyTeam, this.GetRecentlyTeam);

            App.my.eventSet.Subscribe<long>(EventID.Friend_ClearAllApply, this.ClearApplyRecond);

            m_searchList = new FriendSearchInfo();

            m_ApplyDataList = new Dictionary<long, FriendItemInfo>();

            m_RecentlyChat = new Dictionary<long, FriendItemInfo>();

            m_RecentlyTeam = new Dictionary<long, FriendItemInfo>();

            m_friendsMap = new Dictionary<long, FriendItemInfo>();

            m_enemysMap = new Dictionary<long, FriendItemInfo>();

            m_blacksMap = new Dictionary<long, FriendItemInfo>();

            m_RecentlyType = 0;

        }

        public LocalPlayer localPlayer { get; private set; }
        public C2WFriendRequest request { get; private set; }

        public Dictionary<long,FriendItemInfo> m_friendsMap { get; private set; }

        public Dictionary<long, FriendItemInfo> m_enemysMap { get; private set; }

        public Dictionary<long, FriendItemInfo> m_blacksMap { get; private set; }
        public FriendSearchInfo m_searchList { get; private set; }

        public Dictionary<long, FriendItemInfo> m_ApplyDataList { get; private set; }

        public Dictionary<long, FriendItemInfo> m_RecentlyChat { get; private set; }

        public Dictionary<long, FriendItemInfo> m_RecentlyTeam { get; private set; }

        public FriendDbData m_friendDbData;

        public bool IsAnyType(FriendItemInfo info, FriendListType type)
        {
            if (info != null)
            {
                if ((info.itemType & type) == type)
                {
                    return true;
                }
            }
            return false;
        }
        public Dictionary<long, FriendItemInfo> GetTypeMap(FriendDbData data,FriendListType type)
        {
            Dictionary<long, FriendItemInfo> newMap = new Dictionary<long, FriendItemInfo>();
            newMap.Clear();
            if (data != null)
            {
                foreach (var item in data.friends)
                {
                    if (IsAnyType(item.Value, type))
                    {
                        newMap.Add(item.Key, item.Value);
                    }
                }
            }
            return newMap;
        }

        public void InitFriendData()
        {
            if (m_friendDbData != null)
            {
                m_friendsMap = GetTypeMap(m_friendDbData, FriendListType.FD_Friend);
                m_enemysMap = GetTypeMap(m_friendDbData, FriendListType.FD_Enemy);
                m_blacksMap = GetTypeMap(m_friendDbData, FriendListType.FD_Black);
                m_ApplyDataList = GetTypeMap(m_friendDbData, FriendListType.FD_Apply);
                m_RecentlyChat = GetTypeMap(m_friendDbData, FriendListType.FD_Chat);
                m_RecentlyTeam = GetTypeMap(m_friendDbData, FriendListType.FD_Team);

                App.my.eventSet.fireEvent(EventID.Friend_PushFriendData);
                App.my.eventSet.FireEvent<Dictionary<long,FriendItemInfo>>(EventID.Friend_ApplyDataChange, m_ApplyDataList);
                App.my.eventSet.fireEvent(EventID.Friend_RecentlyChatUpdata);
            }
        }
        public void SetFriendDbData(FriendDbData dbData)
        {
            m_friendDbData = null;
            m_friendDbData = dbData;
            InitFriendData();
        }
        
        public string getNameById(long id)
        {
            FriendItemInfo info = new FriendItemInfo();
            if (m_friendDbData!= null && m_friendDbData.friends.TryGetValue(id, out info))
            {
                return info.name;
            }
            
            return "";
        }

        //查找
        public void SearchFriend(string searchStr)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteSearchFriend(searchStr));
        }
        //申请
        public void ApplyFriend(long goalId)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteApplyFriend(goalId));
        }
        //获取申请列表
        public void GetApplyData()
        {
            this.InitFriendData();
            //App.my.mainCoroutine.StartCoroutine(ExecuteGetApplyData());
        }
        //添加好友
        public void AddFriend(long goalId)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteAddFriend(goalId));
        }
        //获取好友联系人列表数据
        public void GetFriendData()
        {
            this.InitFriendData();
            //App.my.mainCoroutine.StartCoroutine(ExecuteGetFriendData());
        }
        //删除 好友 仇敌 黑明单 申请 最近会话 最近组队
        public void DeleteRecondFromData(long goalId , FriendListType e_type)
        {
            FriendDeleteMsg msg = new FriendDeleteMsg();
            msg.charid = goalId;
            msg.msgtype = e_type;
            
            if (e_type != FriendListType.FD_Black && e_type != FriendListType.FD_Friend && e_type != FriendListType.FD_Enemy)
            {
                App.my.mainCoroutine.StartCoroutine(ExecuteDeleteRecondFromData(goalId, e_type));
            }
            else
            {
            
                string tipsStr = null;
                string name = this.getNameById(goalId);
               
                TipsContent tipsConfig;
                switch (e_type)
                {
                    case FriendListType.FD_Friend:
                        tipsConfig = TipsContent.Get(2251);
                        if (tipsConfig != null)
                        {
                            tipsStr = string.Format(tipsConfig.des, name);
                        }
                        break;
                    case FriendListType.FD_Enemy:
                        tipsConfig = TipsContent.Get(2255);
                        if (tipsConfig != null)
                        {
                            tipsStr = string.Format(tipsConfig.des, name);
                        }
                        break;
                    case FriendListType.FD_Black:
                        tipsConfig = TipsContent.Get(2253);
                        if (tipsConfig != null)
                        {
                            tipsStr = string.Format(tipsConfig.des, name);
                        }
                        break;
                    case FriendListType.FD_Apply:
                        tipsConfig = TipsContent.Get(2258);
                        if (tipsConfig != null)
                        {
                            tipsStr = string.Format(tipsConfig.des, name);
                        }
                        break;
                    case FriendListType.FD_Chat:
                        tipsConfig = TipsContent.Get(2259);
                        if (tipsConfig != null)
                        {
                            tipsStr = string.Format(tipsConfig.des, name);
                        }
                        break;
                    case FriendListType.FD_Team:
                        tipsConfig = TipsContent.Get(2257);
                        if (tipsConfig != null)
                        {
                            tipsStr = string.Format(tipsConfig.des, name);
                        }
                        break;
                    default:
                        tipsStr = "是否删除该记录？";
                        break;
                }

                xys.UI.Dialog.TwoBtn.Show(
                    "",
                    tipsStr,
                    "取消", () => false,
                    "确定", () =>
                    {
                        App.my.mainCoroutine.StartCoroutine(ExecuteDeleteRecondFromData(goalId, e_type));               
                        return false;
                    },true,true);
            }
        }
        //拉入黑名单  
        public int GetRedPoint(Dictionary<long,FriendItemInfo> FriendMap)
        {
            int Count = 0;
            if (FriendMap == null || FriendMap.Count == 0)
            {
                return 0;
            }

            foreach (var item in FriendMap)
            {
                if (item.Value.isRead == 0)
                {
                    Count++;
                }
            }
            return Count;
        }

        public int GetAllReadPoint()
        {
            int cnt = 0;

            if (m_friendsMap != null && m_friendsMap.Count >0)
            {
               cnt = cnt + GetRedPoint(m_friendsMap);
            }
            /*
            if (m_enemysMap != null && m_enemysMap.Count > 0)
            {
               cnt = cnt + GetRedPoint(m_enemysMap);
            }

            if (m_blacksMap != null && m_blacksMap.Count > 0)
            {
               cnt = cnt + GetRedPoint(m_blacksMap);
            }
            */
            if (m_ApplyDataList != null && m_ApplyDataList.Count > 0)
            {
               cnt = cnt + GetRedPoint(m_ApplyDataList);
            }

            if (m_RecentlyChat != null && m_RecentlyChat.Count > 0)
            {
                cnt = cnt + GetRedPoint(m_RecentlyChat);
            }

            /*
            if (m_RecentlyTeam != null && m_RecentlyTeam.Count > 0)
            {
                cnt = cnt + GetRedPoint(m_RecentlyTeam);
            }
            */
            App.my.eventSet.FireEvent(EventID.Friend_ShowRed_Point, cnt);
            return cnt;
        }
        public void BlakSomeOne(long goalId)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteBlakSomeOne(goalId));
        }
        //获取 最近 聊天 组队 信息
        public void GetRecentlyInfo()
        {
            InitFriendData();
            //App.my.mainCoroutine.StartCoroutine(ExecuteGetRecentlyInfo());
        }

        //清空申请列表
        public void ClearApplyRecond(long goalId)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteClearApplyList(goalId));
        }

        //获取 单个好友详情 信息
        public FriendItemInfo GetFriendDetail(long goalId,FriendListType type)
        {
            App.my.mainCoroutine.StartCoroutine(ExecuteGetFriendDetail(goalId, type));
            return null;
            /*
            //如果缓存池里面不存在该详情
            if (!m_AllFriendItemInfoPool.ContainsKey(goalId))
            {
                App.my.mainCoroutine.StartCoroutine(ExecuteGetFriendDetail(goalId));
                return null;
            }
            else
            {
                return m_AllFriendItemInfoPool[goalId];
            }
            */
        }

        //获取关系
        
        public bool IsFriend(long otherCharid)
        {
            if (m_friendsMap.ContainsKey(otherCharid))
            {
                return true;
            }
            return false;
        }

        public bool IsEnemy(long otherCharid)
        {
            if (m_enemysMap.ContainsKey(otherCharid))
            {
                return true;
            }
            return false;
        }

        public bool IsBlack(long otherCharid)
        {
            if (m_blacksMap.ContainsKey(otherCharid))
            {
                return true;
            }
            return false;
        }

        //获取在线人数
        public int GetFriendOnlineCount(Dictionary<long, FriendItemInfo> friendMap)
        {
            int count = 0;

            foreach (var item in friendMap)
            {
                if (item.Value.isOnline == true)
                {
                    count++;
                }
            }

            return count;

        }


        public int m_RecentlyType { get; set; }

        protected IEnumerator ExecuteGetApplyData()
        {
            var yyd = request.QueryApplyListYield();
            yield return yyd;
            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc GetApplyListYield fail");
            }
            else
            {
                //FriendRetunrError info = yyd.result;

                //if (info.code == ReturnCode.Friend_None)
                //{
                    //OnRecvApplyInfo(info);
                //}
                //ReturnCode ret = yyd.result.value;              
            }

            yield break;
        }

        protected IEnumerator ExecuteSearchFriend(string searchStr)
        {

            var yyd = request.SearchFriendYield(new NetProto.Str() { value = searchStr });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc SearchFriendYield fail");
            }
            else
            {
     
                FriendSearchInfo info = yyd.result;
                if (info.code == ReturnCode.Friend_None)
                {
                    OnSyncSearchInfo(info);
                }
            }

            yield break;
        }

        protected IEnumerator ExecuteApplyFriend(long goalId)
        {
            

            var yyd = request.ApplyFriendYield(new NetProto.Int64() { value = goalId });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc ApplyFriendYield fail");
            }
            else
            {

                FriendDetailUpdata data = yyd.result;
               
                if (data.code == ReturnCode.Friend_IsSelf)
                {
                    TipsContent tipsConfig = TipsContent.Get(2233);
                    if (tipsConfig != null)
                    {
                        SystemHintMgr.ShowHint(tipsConfig.des);
                    }
                }

                if (data.code == ReturnCode.Friend_IsExist)
                {
                    TipsContent tipsConfig = TipsContent.Get(2231);
                    if (tipsConfig != null)
                    {
                       
                        SystemHintMgr.ShowHint(string.Format(tipsConfig.des,data.info.name));
                    }
                }

                if (data.code == ReturnCode.Friend_None)
                {
                    /*
                    TipsContent tipsConfig = TipsContent.Get(2221);
                    if (tipsConfig != null)
                    {
                        SystemHintMgr.ShowHint(tipsConfig.des);
                    }
                    */
                    /*
                    TipsContent tipsConfig1 = TipsContent.Get(2234);
                    if (tipsConfig1 != null)
                    {
                        string name = getNameById(goalId);
                        SystemHintMgr.ShowHint(string.Format(tipsConfig1.des, name));
                    }
                    */
                    SyncFriendUpdata(data.info);
                }
                
            }

            yield break;
        }



        protected IEnumerator ExecuteAddFriend(long goalId)
        {           
            var yyd = request.AgreeApplyYield(new NetProto.Int64() { value = goalId });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc AddFriendYield fail");
            }
            else
            {

                FriendDetailUpdata data = yyd.result;

                if (data.code == ReturnCode.Friend_IsSelf)
                {
                    TipsContent tipsConfig = TipsContent.Get(2233);
                    if (tipsConfig != null)
                    {
                        SystemHintMgr.ShowHint(tipsConfig.des);
                    }
                    
                }

                if (data.code == ReturnCode.Friend_FriendMax)
                {
                    TipsContent tipsConfig = TipsContent.Get(2211);
                    if (tipsConfig != null)
                    {
                        SystemHintMgr.ShowHint(tipsConfig.des);
                    }
                }
                /*
                if (data.code == ReturnCode.Friend_IsExist)
                {
                    SystemHintMgr.ShowHint("对方已经是你的好友！");
                    
                    TipsContent tipsConfig = TipsContent.Get(2233);
                    if (tipsConfig != null)
                    {
                        
                    }
                    
                }
                */
                if (data.code == ReturnCode.Friend_None)
                {
                    this.SyncFriendUpdata(data.info);

                    
                    TipsContent tipsConfig = TipsContent.Get(2234);
                    if (tipsConfig != null)
                    {
                        string name = getNameById(goalId);
                        SystemHintMgr.ShowHint(string.Format(tipsConfig.des, name));
                    }
                    
                }
                
            }

            yield break;
        }

        protected IEnumerator ExecuteGetFriendData()
        {
            var yyd = request.QueryFriendDataYield();
            yield return yyd;
            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc GetFriendDataYield fail");
            }
            else
            {
                /*
                FriendContactInfo info = yyd.result;

                if (info.code == ReturnCode.Friend_None)
                {
                    OnSyncFriendContactInfo(info);
                }
                */
            }

            yield break;
        }

        protected IEnumerator ExecuteDeleteRecondFromData(long goalId , FriendListType e_type)
        {
           
            var yyd = request.DeleteRecondFromDataYield(new FriendDeleteMsg() { charid = goalId, msgtype = e_type });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc DeleteFriendYield fail");
            }
            else
            {
                string name = getNameById(goalId);
                FriendDetailUpdata ret = yyd.result;
                if (ret.code == ReturnCode.Friend_IsNotExist)
                {
                    SystemHintMgr.ShowHint("删除失败,不存在该记录！");
                }

                if (ret.code == ReturnCode.Friend_None)
                {
                    this.SyncFriendUpdata(ret.info);
                  
                    switch (e_type)
                    {
                        case FriendListType.FD_Friend:

                            TipsContent tipsConfig1 = TipsContent.Get(2252);
                            if (tipsConfig1 != null)
                            {
                                SystemHintMgr.ShowHint(string.Format(tipsConfig1.des, name));
                            }
                            break;
                        case FriendListType.FD_Enemy:
                            TipsContent tipsConfig2 = TipsContent.Get(2256);
                            if (tipsConfig2 != null)
                            {
                                SystemHintMgr.ShowHint(string.Format(tipsConfig2.des, name));
                            }
                            break;
                        case FriendListType.FD_Black:
                            TipsContent tipsConfig3 = TipsContent.Get(2254);
                            if (tipsConfig3 != null)
                            {
                                SystemHintMgr.ShowHint(string.Format(tipsConfig3.des, name));
                            }
                            break;
                        case FriendListType.FD_Apply:
                            TipsContent tipsConfig4 = TipsContent.Get(2261);
                            if (tipsConfig4 != null)
                            {
                                SystemHintMgr.ShowHint(string.Format(tipsConfig4.des, name));
                            }
                            break;
                        case FriendListType.FD_Chat:
                            TipsContent tipsConfig5 = TipsContent.Get(2262);
                            if (tipsConfig5 != null)
                            {
                                SystemHintMgr.ShowHint(string.Format(tipsConfig5.des, name));
                            }
                            break;
                        case FriendListType.FD_Team:
                            TipsContent tipsConfig6 = TipsContent.Get(2260);
                            if (tipsConfig6 != null)
                            {
                                SystemHintMgr.ShowHint(string.Format(tipsConfig6.des, name));
                            }
                            break;
                        default:
                            break;
                    }
                }

                yield break;
            }
        }
        protected IEnumerator ExecuteBlakSomeOne(long goalId)
        {
            var yyd = request.BlakSomeOneYield(new NetProto.Int64() { value = goalId });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc BlakSomeOneYield fail");
            }
            else
            {
                /*
                FriendContactInfo info = yyd.result;

                if (info.code == ReturnCode.Friend_None)
                {
                    OnSyncFriendContactInfo(info);
                }
                */
            }

            yield break;
        }

        
        protected IEnumerator ExecuteGetRecentlyInfo()
        {
            var yyd = request.QueryRecentlyListYield();
            yield return yyd;
            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc GetRecentlyChatYield fail");
            }
            else
            {
                /*
                FriendRecentlyInfos info = yyd.result;

                if (info.code == ReturnCode.Friend_None)
                {
                    OnPushRecentlyInfo(info);
                }
                */
            }

            yield break;
        }

        protected IEnumerator ExecuteClearApplyList(long goalId)
        {
            var yyd = request.ClearApplyListYield();
            yield return yyd;
            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc ClearApplyListYield fail");
            }
            else
            {
                /*
                FriendsApplyInfos applyInfo = yyd.result;
                if (applyInfo.code == ReturnCode.Friend_None)
                {
                    this.OnRecvApplyInfo(applyInfo);
                }
                */
            }

            yield break;
        }
        

        protected IEnumerator ExecuteGetFriendDetail(long goalId,FriendListType listType)
        {
            var yyd = request.QueryFriendDetalYield(new FriendUpdataMsg() { charid = goalId, type = listType });
            yield return yyd;
            if (wProtobuf.RPC.Error.Success != yyd.code)
            {
                XYJLogger.LogError("Rpc QueryFriendDetalYield fail");
            }
            else
            {

                FriendDetailUpdata info = yyd.result;
                if (info.code  == ReturnCode.Friend_None)
                {
                    this.SyncFriendUpdata(info.info);
                }
                
            }

            yield break;
        }

        

        public FriendSearchInfo GetSearchList()
        {
            return m_searchList;
        }

        protected override None OnW2CRpcTest(NetProto.Int64 input)
        {
            XYJLogger.LogDebug("OnW2CRpcTest");
            return new None();
        }

        void OnSearchListInfo(IPacket packet, FriendSearchInfo msg)
        {
            this.OnSyncSearchInfo(msg);
        }

        public void UpdataSearchState()
        {
            App.my.eventSet.FireEvent<FriendSearchInfo>(EventID.Friend_SearchDataChange, m_searchList);
        }

        public void OnRecvAddFriendTipsToOther(IPacket packet, FriendTips tips)
        {

            TipsContent tipsConfig = TipsContent.Get(2235);
            if (tipsConfig != null)
            {
               if (tips.name != "")
               {
                    SystemHintMgr.ShowHint(string.Format(tipsConfig.des, tips.name));
               }
               else
               {
                    SystemHintMgr.ShowHint(tipsConfig.des);
               }
            }     
        }
        public void OnRecvAddFriendTipsToSelf(IPacket packet, FriendTips tips)
        {

            TipsContent tipsConfig = TipsContent.Get(2234);
            if (tipsConfig != null)
            {
                if (tips.name != "")
                {
                    SystemHintMgr.ShowHint(string.Format(tipsConfig.des, tips.name));
                }
                else
                {
                    SystemHintMgr.ShowHint(tipsConfig.des);
                }
            }
        }
        public void OnRecvFriendOnline(IPacket packet, FriendTips tips)
        {

            TipsContent tipsConfig = TipsContent.Get(2235);
            if (tipsConfig != null)
            {
                if (tips.name != "")
                {
                    SystemHintMgr.ShowHint(string.Format(tipsConfig.des, tips.name));
                }
                else
                {
                    SystemHintMgr.ShowHint(tipsConfig.des);
                }
            }
        }

        public void OnRecvFriendOutline(IPacket packet, FriendTips tips)
        {

            TipsContent tipsConfig = TipsContent.Get(2235);
            if (tipsConfig != null)
            {
                if (tips.name != "")
                {
                    SystemHintMgr.ShowHint(string.Format(tipsConfig.des, tips.name));
                }
                else
                {
                    SystemHintMgr.ShowHint(tipsConfig.des);
                }
            }
        }

        void OnSyncSearchInfo(FriendSearchInfo msg)
        {
            XYJLogger.LogDebug("OnSyncSearchInfo");

            m_searchList = null;
            m_searchList = msg;
            /*
            if (m_searchList != null)
            {
                AddToFriendItemPool(m_searchList.searchMap);
            }
            */
            App.my.eventSet.FireEvent<FriendSearchInfo>(EventID.Friend_SearchDataChange, m_searchList);
        }

        void OnFriendUpdata(IPacket packet, FriendItemInfo data)
        {
            SyncFriendUpdata(data);
        }

        void SyncFriendUpdata(FriendItemInfo data)
        {
            if (m_friendDbData != null)
            {
                if (m_friendDbData.friends.ContainsKey(data.charid))
                {
                    //判断是否是添加好友

                    m_friendDbData.friends[data.charid] = data;

                    if (m_friendDbData.friends[data.charid].itemType == FriendListType.FD_None)
                    {
                        m_friendDbData.friends.Remove(data.charid);
                    }
                }
                else
                {
                    if (data.itemType != FriendListType.FD_None)
                    {
                        m_friendDbData.friends.Add(data.charid, data);
                    }
                }
            }
            InitFriendData();
        }
        /*
        public void OnSyncFriendContactInfo(FriendContactInfo msg)
        {
            m_friendContactInfo = null;
            m_friendContactInfo = msg;
            GetAllReadPoint();
            
            if (m_friendContactInfo != null)
            {
                AddToFriendItemPool(m_friendContactInfo.friends);
                AddToFriendItemPool(m_friendContactInfo.enemys);
                AddToFriendItemPool(m_friendContactInfo.blacks);
            }
            

            App.my.eventSet.FireEvent<FriendContactInfo>(EventID.Friend_PushFriendData, m_friendContactInfo);

        }
        */
        /*
        void OnRecvFriendInfo(IPacket packet,FriendContactInfo msg)
        {
            this.OnSyncFriendContactInfo(msg);
        }
        void OnRecvApplyInfo(FriendsApplyInfos msg)
        {
            this.OnSyncRecvApplyInfo(msg);
        }
        
        void OnSyncRecvApplyInfo(FriendsApplyInfos msg)
        {
            Logger.LogDebug("OnSyncRecvApplyInfo");
            //是否有新纪录
            m_ApplyDataList = null;
            m_ApplyDataList = msg;
            GetAllReadPoint();
            
            if (m_ApplyDataList != null)
            {
                AddToFriendItemPool(m_ApplyDataList.applys);
            }
            
            App.my.eventSet.FireEvent<FriendsApplyInfos>(EventID.Friend_ApplyDataChange, m_ApplyDataList);
        }
    
        void OnRecvFriendsAgreeFriendInfo(FriendsAgreeFriendInfo info)
        {
            m_friendContactInfo.friends = null;
            m_friendContactInfo.friends = info.friends;
            m_ApplyDataList.applys = info.applys;

            App.my.eventSet.FireEvent<FriendsApplyInfos>(EventID.Friend_ApplyDataChange, m_ApplyDataList);
            App.my.eventSet.FireEvent<FriendContactInfo>(EventID.Friend_PushFriendData, m_friendContactInfo);
        }
        
        void OnRecvRecentlyInfo(IPacket packet, FriendRecentlyInfos msg)
        {
            OnPushRecentlyInfo(msg);
        }

        void OnRecvFriendsApplyInfos(IPacket packet, FriendsApplyInfos msg)
        {
            OnSyncRecvApplyInfo(msg);
        }
        void OnPushRecentlyInfo(FriendRecentlyInfos msg)
        {
            m_RecentlyInfo = null;
            m_RecentlyInfo = msg;
            GetAllReadPoint();
            
            if (m_RecentlyInfo != null)
            {
                AddToFriendItemPool(m_RecentlyInfo.chats);
                AddToFriendItemPool(m_RecentlyInfo.teams);
            }
            
            App.my.eventSet.FireEvent<FriendRecentlyInfos>(EventID.Friend_RecentlyChatUpdata, m_RecentlyInfo);
        }

        //添加到对象池
        public void AddToFriendItemPool(Dictionary<long, FriendItemInfo> friendMap)
        {
            if (friendMap != null)
            {
                foreach (var item in friendMap)
                {
                    if (!m_AllFriendItemInfoPool.ContainsKey(item.Key))
                    {
                        m_AllFriendItemInfoPool.Add(item.Key, item.Value);
                    }
                    else
                    {
                        m_AllFriendItemInfoPool[item.Key] = null;
                        m_AllFriendItemInfoPool[item.Key] = item.Value;
                    }
                }
            }

        }

        public void UpdataFriendInfo(Dictionary<long, FriendItemInfo> friendMap ,FriendItemInfo info)
        {
            if (friendMap != null)
            {
                if (friendMap.ContainsKey(info.charid))
                {
                    friendMap[info.charid] = null;
                    friendMap[info.charid] = info;
                }
            }
        }

        void OnPushFriendItemInfo(FriendItemInfo msg)
        {
            if (msg != null)
            {
                
                if (!m_AllFriendItemInfoPool.ContainsKey(msg.charid))
                {
                    m_AllFriendItemInfoPool.Add(msg.charid, msg);                    
                }
                else
                {
                    //更新池子对象信息
                    m_AllFriendItemInfoPool[msg.charid] = null;
                    m_AllFriendItemInfoPool[msg.charid] = msg;
                }
                //更新单一的 在列表中
                
                if (m_friendContactInfo != null)
                {
                    UpdataFriendInfo(m_friendContactInfo.friends, msg);
                    UpdataFriendInfo(m_friendContactInfo.enemys, msg);
                    UpdataFriendInfo(m_friendContactInfo.blacks, msg);
                }

                if (m_RecentlyInfo != null)
                {
                    UpdataFriendInfo(m_RecentlyInfo.chats, msg);
                    UpdataFriendInfo(m_RecentlyInfo.teams, msg);
                }

                if (m_ApplyDataList != null)
                {
                    UpdataFriendInfo(m_ApplyDataList.applys, msg);
                }

                App.my.eventSet.FireEvent<FriendItemInfo>(EventID.Friend_FriendItemInfoUpdata, msg);
            }
            
        }
        */
        /*
        private void RemoveAllFriendBaseData()
        {
            m_AllFriendItemInfoPool.Clear();
        }
        */
    }


}
