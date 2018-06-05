// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: ClientProto/Friend/Friend.proto

using wProtobuf;
using System.Collections;
using System.Collections.Generic;
using wProtobufRPC = wProtobuf.RPC;
using RPCILocalCall = wProtobuf.RPC.ILocalCall;
using RPCIRemoteCall = wProtobuf.RPC.IRemoteCall;

namespace NetProto
{
    public enum FriendListType
    {
        FD_None = 0,
        FD_Friend = 1,
        FD_Enemy = 2,
        FD_Black = 4,
        FD_Apply = 8,
        FD_Chat = 16,
        FD_Team = 32,
    }
    public class FriendItemInfo : IMessage
    {
        public long charid = 0L;
        public string name = null;
        public int level = 0;
        public int sex = 0;
        public int job = 0;
        public bool isOnline = false;
        public long friendLiness = 0L;
        public long lastChatTime = 0L;
        public long lastTeamTime = 0L;
        public long lastOnLineTime = 0L;
        public long lastKillTime = 0L;
        public FriendListType itemType = FriendListType.FD_None;
        public long applyTime = 0L;
        public int isRead = 0;
        public int CalculateSize()
        {
            int _total_size_ = 0;
            if (charid != 0L)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt64Size(charid);
            }
            if (!string.IsNullOrEmpty(name))
            {
                _total_size_ += 1 + ComputeSize.ComputeStringSize(name);
            }
            if (level != 0)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt32Size(level);
            }
            if (sex != 0)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt32Size(sex);
            }
            if (job != 0)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt32Size(job);
            }
            if (isOnline != false)
            {
                _total_size_ += (1 + 1);
            }
            if (friendLiness != 0L)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt64Size(friendLiness);
            }
            if (lastChatTime != 0L)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt64Size(lastChatTime);
            }
            if (lastTeamTime != 0L)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt64Size(lastTeamTime);
            }
            if (lastOnLineTime != 0L)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt64Size(lastOnLineTime);
            }
            if (lastKillTime != 0L)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt64Size(lastKillTime);
            }
            if (itemType != FriendListType.FD_None)
            {
                _total_size_ += 1 + ComputeSize.ComputeEnumSize((int)itemType);
            }
            if (applyTime != 0L)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt64Size(applyTime);
            }
            if (isRead != 0)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt32Size(isRead);
            }
            return _total_size_;
        }
        public void WriteTo(IWriteStream output)
        {
            if (charid != 0L)
            {
                output.WriteRawTag(8);
                output.WriteInt64(charid);
            }
            if (!string.IsNullOrEmpty(name))
            {
                output.WriteRawTag(18);
                output.WriteString(name);
            }
            if (level != 0)
            {
                output.WriteRawTag(24);
                output.WriteInt32(level);
            }
            if (sex != 0)
            {
                output.WriteRawTag(32);
                output.WriteInt32(sex);
            }
            if (job != 0)
            {
                output.WriteRawTag(40);
                output.WriteInt32(job);
            }
            if (isOnline != false)
            {
                output.WriteRawTag(48);
                output.WriteBool(isOnline);
            }
            if (friendLiness != 0L)
            {
                output.WriteRawTag(56);
                output.WriteInt64(friendLiness);
            }
            if (lastChatTime != 0L)
            {
                output.WriteRawTag(64);
                output.WriteInt64(lastChatTime);
            }
            if (lastTeamTime != 0L)
            {
                output.WriteRawTag(72);
                output.WriteInt64(lastTeamTime);
            }
            if (lastOnLineTime != 0L)
            {
                output.WriteRawTag(80);
                output.WriteInt64(lastOnLineTime);
            }
            if (lastKillTime != 0L)
            {
                output.WriteRawTag(88);
                output.WriteInt64(lastKillTime);
            }
            if (itemType != FriendListType.FD_None)
            {
                output.WriteRawTag(96);
                output.WriteEnum((int)itemType);
            }
            if (applyTime != 0L)
            {
                output.WriteRawTag(104);
                output.WriteInt64(applyTime);
            }
            if (isRead != 0)
            {
                output.WriteRawTag(112);
                output.WriteInt32(isRead);
            }
        }
        public void MergeFrom(IReadStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                switch(tag)
                {
                case 8:
                    {
                        charid = input.ReadInt64();
                    }
                    break;
                case 18:
                    {
                        name = input.ReadString();
                    }
                    break;
                case 24:
                    {
                        level = input.ReadInt32();
                    }
                    break;
                case 32:
                    {
                        sex = input.ReadInt32();
                    }
                    break;
                case 40:
                    {
                        job = input.ReadInt32();
                    }
                    break;
                case 48:
                    {
                        isOnline = input.ReadBool();
                    }
                    break;
                case 56:
                    {
                        friendLiness = input.ReadInt64();
                    }
                    break;
                case 64:
                    {
                        lastChatTime = input.ReadInt64();
                    }
                    break;
                case 72:
                    {
                        lastTeamTime = input.ReadInt64();
                    }
                    break;
                case 80:
                    {
                        lastOnLineTime = input.ReadInt64();
                    }
                    break;
                case 88:
                    {
                        lastKillTime = input.ReadInt64();
                    }
                    break;
                case 96:
                    {
                        itemType = (FriendListType)input.ReadEnum();
                    }
                    break;
                case 104:
                    {
                        applyTime = input.ReadInt64();
                    }
                    break;
                case 112:
                    {
                        isRead = input.ReadInt32();
                    }
                    break;
                default:
                    input.SkipLastField(tag);
                    break;
                }
            }
        }
    }

    public class FriendDbData : IMessage
    {
        public class FriendsEntry : IMessage
        {
            public long key = 0L;
            public FriendItemInfo value = new FriendItemInfo();
            public int CalculateSize()
            {
                int _total_size_ = 0;
                if (key != 0L)
                {
                    _total_size_ += 1 + ComputeSize.ComputeInt64Size(key);
                }
                if (value != null)
                {
                    _total_size_ += 1 + ComputeSize.ComputeMessageSize(value);
                }
                return _total_size_;
            }
            public void WriteTo(IWriteStream output)
            {
                if (key != 0L)
                {
                    output.WriteRawTag(8);
                    output.WriteInt64(key);
                }
                if (value != null)
                {
                    output.WriteRawTag(18);
                    output.WriteMessage(value);
                }
            }
            public void MergeFrom(IReadStream input)
            {
                uint tag;
                while ((tag = input.ReadTag()) != 0)
                {
                    switch(tag)
                    {
                    case 8:
                        {
                            key = input.ReadInt64();
                        }
                        break;
                    case 18:
                        {
                            if (value == null)
                                value = new FriendItemInfo();
                            input.ReadMessage(value);
                        }
                        break;
                    default:
                        input.SkipLastField(tag);
                        break;
                    }
                }
            }
        }
        public Dictionary<long , FriendItemInfo> friends = new Dictionary<long , FriendItemInfo>();
        public int CalculateSize()
        {
            int _total_size_ = 0;
            if (friends != null && friends.Count != 0)
            {
                var entry = new FriendsEntry();
                foreach (var itor in friends)
                {
                    _total_size_ += 1;
                    entry.key = itor.Key;
                    entry.value = itor.Value;
                    _total_size_ += ComputeSize.ComputeMessageSize(entry);
                }
            }
            return _total_size_;
        }
        public void WriteTo(IWriteStream output)
        {
            if (friends != null && friends.Count != 0)
            {
                var entry = new FriendsEntry();
                foreach (var itor in friends)
                {
                    entry.key = itor.Key;
                    entry.value = itor.Value;
                    output.WriteRawTag(10);
                    output.WriteMessage(entry);
                }
            }
        }
        public void MergeFrom(IReadStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                switch(tag)
                {
                case 10:
                    {
                        if (friends == null)
                            friends = new Dictionary<long, FriendItemInfo>();
                        var entry = new FriendsEntry();
                        input.ReadMessage(entry);
                        friends[entry.key] = entry.value;
                    }
                    break;
                default:
                    input.SkipLastField(tag);
                    break;
                }
            }
        }
    }

    public class FriendDeleteMsg : IMessage
    {
        public long charid = 0L;
        public FriendListType msgtype = FriendListType.FD_None;
        public ReturnCode code = ReturnCode.ReturnCode_OK;
        public int CalculateSize()
        {
            int _total_size_ = 0;
            if (charid != 0L)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt64Size(charid);
            }
            if (msgtype != FriendListType.FD_None)
            {
                _total_size_ += 1 + ComputeSize.ComputeEnumSize((int)msgtype);
            }
            if (code != ReturnCode.ReturnCode_OK)
            {
                _total_size_ += 1 + ComputeSize.ComputeEnumSize((int)code);
            }
            return _total_size_;
        }
        public void WriteTo(IWriteStream output)
        {
            if (charid != 0L)
            {
                output.WriteRawTag(8);
                output.WriteInt64(charid);
            }
            if (msgtype != FriendListType.FD_None)
            {
                output.WriteRawTag(16);
                output.WriteEnum((int)msgtype);
            }
            if (code != ReturnCode.ReturnCode_OK)
            {
                output.WriteRawTag(32);
                output.WriteEnum((int)code);
            }
        }
        public void MergeFrom(IReadStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                switch(tag)
                {
                case 8:
                    {
                        charid = input.ReadInt64();
                    }
                    break;
                case 16:
                    {
                        msgtype = (FriendListType)input.ReadEnum();
                    }
                    break;
                case 32:
                    {
                        code = (ReturnCode)input.ReadEnum();
                    }
                    break;
                default:
                    input.SkipLastField(tag);
                    break;
                }
            }
        }
    }

    public class FriendSearchInfo : IMessage
    {
        public class SearchMapEntry : IMessage
        {
            public long key = 0L;
            public FriendItemInfo value = new FriendItemInfo();
            public int CalculateSize()
            {
                int _total_size_ = 0;
                if (key != 0L)
                {
                    _total_size_ += 1 + ComputeSize.ComputeInt64Size(key);
                }
                if (value != null)
                {
                    _total_size_ += 1 + ComputeSize.ComputeMessageSize(value);
                }
                return _total_size_;
            }
            public void WriteTo(IWriteStream output)
            {
                if (key != 0L)
                {
                    output.WriteRawTag(8);
                    output.WriteInt64(key);
                }
                if (value != null)
                {
                    output.WriteRawTag(18);
                    output.WriteMessage(value);
                }
            }
            public void MergeFrom(IReadStream input)
            {
                uint tag;
                while ((tag = input.ReadTag()) != 0)
                {
                    switch(tag)
                    {
                    case 8:
                        {
                            key = input.ReadInt64();
                        }
                        break;
                    case 18:
                        {
                            if (value == null)
                                value = new FriendItemInfo();
                            input.ReadMessage(value);
                        }
                        break;
                    default:
                        input.SkipLastField(tag);
                        break;
                    }
                }
            }
        }
        public Dictionary<long , FriendItemInfo> searchMap = new Dictionary<long , FriendItemInfo>();
        public ReturnCode code = ReturnCode.ReturnCode_OK;
        public int CalculateSize()
        {
            int _total_size_ = 0;
            if (searchMap != null && searchMap.Count != 0)
            {
                var entry = new SearchMapEntry();
                foreach (var itor in searchMap)
                {
                    _total_size_ += 1;
                    entry.key = itor.Key;
                    entry.value = itor.Value;
                    _total_size_ += ComputeSize.ComputeMessageSize(entry);
                }
            }
            if (code != ReturnCode.ReturnCode_OK)
            {
                _total_size_ += 1 + ComputeSize.ComputeEnumSize((int)code);
            }
            return _total_size_;
        }
        public void WriteTo(IWriteStream output)
        {
            if (searchMap != null && searchMap.Count != 0)
            {
                var entry = new SearchMapEntry();
                foreach (var itor in searchMap)
                {
                    entry.key = itor.Key;
                    entry.value = itor.Value;
                    output.WriteRawTag(10);
                    output.WriteMessage(entry);
                }
            }
            if (code != ReturnCode.ReturnCode_OK)
            {
                output.WriteRawTag(40);
                output.WriteEnum((int)code);
            }
        }
        public void MergeFrom(IReadStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                switch(tag)
                {
                case 10:
                    {
                        if (searchMap == null)
                            searchMap = new Dictionary<long, FriendItemInfo>();
                        var entry = new SearchMapEntry();
                        input.ReadMessage(entry);
                        searchMap[entry.key] = entry.value;
                    }
                    break;
                case 40:
                    {
                        code = (ReturnCode)input.ReadEnum();
                    }
                    break;
                default:
                    input.SkipLastField(tag);
                    break;
                }
            }
        }
    }

    public class FriendDetailUpdata : IMessage
    {
        public FriendItemInfo info = new FriendItemInfo();
        public ReturnCode code = ReturnCode.ReturnCode_OK;
        public int CalculateSize()
        {
            int _total_size_ = 0;
            if (info != null)
            {
                _total_size_ += 1 + ComputeSize.ComputeMessageSize(info);
            }
            if (code != ReturnCode.ReturnCode_OK)
            {
                _total_size_ += 1 + ComputeSize.ComputeEnumSize((int)code);
            }
            return _total_size_;
        }
        public void WriteTo(IWriteStream output)
        {
            if (info != null)
            {
                output.WriteRawTag(10);
                output.WriteMessage(info);
            }
            if (code != ReturnCode.ReturnCode_OK)
            {
                output.WriteRawTag(16);
                output.WriteEnum((int)code);
            }
        }
        public void MergeFrom(IReadStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                switch(tag)
                {
                case 10:
                    {
                        if (info == null)
                            info = new FriendItemInfo();
                        input.ReadMessage(info);
                    }
                    break;
                case 16:
                    {
                        code = (ReturnCode)input.ReadEnum();
                    }
                    break;
                default:
                    input.SkipLastField(tag);
                    break;
                }
            }
        }
    }

    public class FriendRetunrError : IMessage
    {
        public ReturnCode code = ReturnCode.ReturnCode_OK;
        public int CalculateSize()
        {
            int _total_size_ = 0;
            if (code != ReturnCode.ReturnCode_OK)
            {
                _total_size_ += 1 + ComputeSize.ComputeEnumSize((int)code);
            }
            return _total_size_;
        }
        public void WriteTo(IWriteStream output)
        {
            if (code != ReturnCode.ReturnCode_OK)
            {
                output.WriteRawTag(8);
                output.WriteEnum((int)code);
            }
        }
        public void MergeFrom(IReadStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                switch(tag)
                {
                case 8:
                    {
                        code = (ReturnCode)input.ReadEnum();
                    }
                    break;
                default:
                    input.SkipLastField(tag);
                    break;
                }
            }
        }
    }

    public class FriendTips : IMessage
    {
        public string name = null;
        public int CalculateSize()
        {
            int _total_size_ = 0;
            if (!string.IsNullOrEmpty(name))
            {
                _total_size_ += 1 + ComputeSize.ComputeStringSize(name);
            }
            return _total_size_;
        }
        public void WriteTo(IWriteStream output)
        {
            if (!string.IsNullOrEmpty(name))
            {
                output.WriteRawTag(10);
                output.WriteString(name);
            }
        }
        public void MergeFrom(IReadStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                switch(tag)
                {
                case 10:
                    {
                        name = input.ReadString();
                    }
                    break;
                default:
                    input.SkipLastField(tag);
                    break;
                }
            }
        }
    }

    public class FriendUpdataMsg : IMessage
    {
        public long charid = 0L;
        public FriendListType type = FriendListType.FD_None;
        public int CalculateSize()
        {
            int _total_size_ = 0;
            if (charid != 0L)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt64Size(charid);
            }
            if (type != FriendListType.FD_None)
            {
                _total_size_ += 1 + ComputeSize.ComputeEnumSize((int)type);
            }
            return _total_size_;
        }
        public void WriteTo(IWriteStream output)
        {
            if (charid != 0L)
            {
                output.WriteRawTag(8);
                output.WriteInt64(charid);
            }
            if (type != FriendListType.FD_None)
            {
                output.WriteRawTag(16);
                output.WriteEnum((int)type);
            }
        }
        public void MergeFrom(IReadStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                switch(tag)
                {
                case 8:
                    {
                        charid = input.ReadInt64();
                    }
                    break;
                case 16:
                    {
                        type = (FriendListType)input.ReadEnum();
                    }
                    break;
                default:
                    input.SkipLastField(tag);
                    break;
                }
            }
        }
    }
    public class C2WFriendRequest
    {
        public C2WFriendRequest(RPCILocalCall l)
        {
            local = l;
        }
        RPCILocalCall local;
        public void SearchFriend(Str input, System.Action<wProtobuf.RPC.Error, FriendSearchInfo> onEnd)
        {
            local.Call("C2WFriend.SearchFriend", input, onEnd);
        }
        public wProtobufRPC.IYieldResult<FriendSearchInfo> SearchFriendYield(Str input)
        {
            wProtobufRPC.IYieldResult<FriendSearchInfo> info = wProtobufRPC.YieldFactory.Create<FriendSearchInfo>();
            SearchFriend(input, info.OnEnd);
            return info;
        }
        public void ApplyFriend(Int64 input, System.Action<wProtobuf.RPC.Error, FriendDetailUpdata> onEnd)
        {
            local.Call("C2WFriend.ApplyFriend", input, onEnd);
        }
        public wProtobufRPC.IYieldResult<FriendDetailUpdata> ApplyFriendYield(Int64 input)
        {
            wProtobufRPC.IYieldResult<FriendDetailUpdata> info = wProtobufRPC.YieldFactory.Create<FriendDetailUpdata>();
            ApplyFriend(input, info.OnEnd);
            return info;
        }
        public void AgreeApply(Int64 input, System.Action<wProtobuf.RPC.Error, FriendDetailUpdata> onEnd)
        {
            local.Call("C2WFriend.AgreeApply", input, onEnd);
        }
        public wProtobufRPC.IYieldResult<FriendDetailUpdata> AgreeApplyYield(Int64 input)
        {
            wProtobufRPC.IYieldResult<FriendDetailUpdata> info = wProtobufRPC.YieldFactory.Create<FriendDetailUpdata>();
            AgreeApply(input, info.OnEnd);
            return info;
        }
        public void QueryApplyList(System.Action<wProtobuf.RPC.Error, FriendDetailUpdata> onEnd)
        {
            local.Call("C2WFriend.QueryApplyList", onEnd);
        }
        public wProtobufRPC.IYieldResult<FriendDetailUpdata> QueryApplyListYield()
        {
            wProtobufRPC.IYieldResult<FriendDetailUpdata> info = wProtobufRPC.YieldFactory.Create<FriendDetailUpdata>();
            QueryApplyList(info.OnEnd);
            return info;
        }
        public void DeleteRecondFromData(FriendDeleteMsg input, System.Action<wProtobuf.RPC.Error, FriendDetailUpdata> onEnd)
        {
            local.Call("C2WFriend.DeleteRecondFromData", input, onEnd);
        }
        public wProtobufRPC.IYieldResult<FriendDetailUpdata> DeleteRecondFromDataYield(FriendDeleteMsg input)
        {
            wProtobufRPC.IYieldResult<FriendDetailUpdata> info = wProtobufRPC.YieldFactory.Create<FriendDetailUpdata>();
            DeleteRecondFromData(input, info.OnEnd);
            return info;
        }
        public void QueryFriendData(System.Action<wProtobuf.RPC.Error, FriendDetailUpdata> onEnd)
        {
            local.Call("C2WFriend.QueryFriendData", onEnd);
        }
        public wProtobufRPC.IYieldResult<FriendDetailUpdata> QueryFriendDataYield()
        {
            wProtobufRPC.IYieldResult<FriendDetailUpdata> info = wProtobufRPC.YieldFactory.Create<FriendDetailUpdata>();
            QueryFriendData(info.OnEnd);
            return info;
        }
        public void BlakSomeOne(Int64 input, System.Action<wProtobuf.RPC.Error, FriendDetailUpdata> onEnd)
        {
            local.Call("C2WFriend.BlakSomeOne", input, onEnd);
        }
        public wProtobufRPC.IYieldResult<FriendDetailUpdata> BlakSomeOneYield(Int64 input)
        {
            wProtobufRPC.IYieldResult<FriendDetailUpdata> info = wProtobufRPC.YieldFactory.Create<FriendDetailUpdata>();
            BlakSomeOne(input, info.OnEnd);
            return info;
        }
        public void QueryRecentlyList(System.Action<wProtobuf.RPC.Error, FriendDetailUpdata> onEnd)
        {
            local.Call("C2WFriend.QueryRecentlyList", onEnd);
        }
        public wProtobufRPC.IYieldResult<FriendDetailUpdata> QueryRecentlyListYield()
        {
            wProtobufRPC.IYieldResult<FriendDetailUpdata> info = wProtobufRPC.YieldFactory.Create<FriendDetailUpdata>();
            QueryRecentlyList(info.OnEnd);
            return info;
        }
        public void QueryFriendDetal(FriendUpdataMsg input, System.Action<wProtobuf.RPC.Error, FriendDetailUpdata> onEnd)
        {
            local.Call("C2WFriend.QueryFriendDetal", input, onEnd);
        }
        public wProtobufRPC.IYieldResult<FriendDetailUpdata> QueryFriendDetalYield(FriendUpdataMsg input)
        {
            wProtobufRPC.IYieldResult<FriendDetailUpdata> info = wProtobufRPC.YieldFactory.Create<FriendDetailUpdata>();
            QueryFriendDetal(input, info.OnEnd);
            return info;
        }
        public void ClearApplyList(System.Action<wProtobuf.RPC.Error, FriendRetunrError> onEnd)
        {
            local.Call("C2WFriend.ClearApplyList", onEnd);
        }
        public wProtobufRPC.IYieldResult<FriendRetunrError> ClearApplyListYield()
        {
            wProtobufRPC.IYieldResult<FriendRetunrError> info = wProtobufRPC.YieldFactory.Create<FriendRetunrError>();
            ClearApplyList(info.OnEnd);
            return info;
        }
    }
    public abstract class C2WFriendRespone
    {
        public C2WFriendRespone(RPCIRemoteCall r)
        {
            r.RegAsync<Str, FriendSearchInfo>("C2WFriend.SearchFriend", OnSearchFriend);
            r.Reg<Int64, FriendDetailUpdata>("C2WFriend.ApplyFriend", OnApplyFriend);
            r.Reg<Int64, FriendDetailUpdata>("C2WFriend.AgreeApply", OnAgreeApply);
            r.Reg<FriendDetailUpdata>("C2WFriend.QueryApplyList", OnQueryApplyList);
            r.Reg<FriendDeleteMsg, FriendDetailUpdata>("C2WFriend.DeleteRecondFromData", OnDeleteRecondFromData);
            r.Reg<FriendDetailUpdata>("C2WFriend.QueryFriendData", OnQueryFriendData);
            r.Reg<Int64, FriendDetailUpdata>("C2WFriend.BlakSomeOne", OnBlakSomeOne);
            r.Reg<FriendDetailUpdata>("C2WFriend.QueryRecentlyList", OnQueryRecentlyList);
            r.Reg<FriendUpdataMsg, FriendDetailUpdata>("C2WFriend.QueryFriendDetal", OnQueryFriendDetal);
            r.Reg<FriendRetunrError>("C2WFriend.ClearApplyList", OnClearApplyList);
        }

        protected abstract IEnumerator OnSearchFriend(Str input, wProtobufRPC.OutValue<FriendSearchInfo> outV);
        protected abstract FriendDetailUpdata OnApplyFriend(Int64 input);
        protected abstract FriendDetailUpdata OnAgreeApply(Int64 input);
        protected abstract FriendDetailUpdata OnQueryApplyList();
        protected abstract FriendDetailUpdata OnDeleteRecondFromData(FriendDeleteMsg input);
        protected abstract FriendDetailUpdata OnQueryFriendData();
        protected abstract FriendDetailUpdata OnBlakSomeOne(Int64 input);
        protected abstract FriendDetailUpdata OnQueryRecentlyList();
        protected abstract FriendDetailUpdata OnQueryFriendDetal(FriendUpdataMsg input);
        protected abstract FriendRetunrError OnClearApplyList();
        public static void Reg(RPCIRemoteCall r, IC2WFriendRespone respone)
        {
            r.RegAsync<Str, FriendSearchInfo>("C2WFriend.SearchFriend", respone.OnSearchFriend);
            r.Reg<Int64, FriendDetailUpdata>("C2WFriend.ApplyFriend", respone.OnApplyFriend);
            r.Reg<Int64, FriendDetailUpdata>("C2WFriend.AgreeApply", respone.OnAgreeApply);
            r.Reg<FriendDetailUpdata>("C2WFriend.QueryApplyList", respone.OnQueryApplyList);
            r.Reg<FriendDeleteMsg, FriendDetailUpdata>("C2WFriend.DeleteRecondFromData", respone.OnDeleteRecondFromData);
            r.Reg<FriendDetailUpdata>("C2WFriend.QueryFriendData", respone.OnQueryFriendData);
            r.Reg<Int64, FriendDetailUpdata>("C2WFriend.BlakSomeOne", respone.OnBlakSomeOne);
            r.Reg<FriendDetailUpdata>("C2WFriend.QueryRecentlyList", respone.OnQueryRecentlyList);
            r.Reg<FriendUpdataMsg, FriendDetailUpdata>("C2WFriend.QueryFriendDetal", respone.OnQueryFriendDetal);
            r.Reg<FriendRetunrError>("C2WFriend.ClearApplyList", respone.OnClearApplyList);
        }
    }
    public interface IC2WFriendRespone
    {
        IEnumerator OnSearchFriend(Str input, wProtobufRPC.OutValue<FriendSearchInfo> outV);
        FriendDetailUpdata OnApplyFriend(Int64 input);
        FriendDetailUpdata OnAgreeApply(Int64 input);
        FriendDetailUpdata OnQueryApplyList();
        FriendDetailUpdata OnDeleteRecondFromData(FriendDeleteMsg input);
        FriendDetailUpdata OnQueryFriendData();
        FriendDetailUpdata OnBlakSomeOne(Int64 input);
        FriendDetailUpdata OnQueryRecentlyList();
        FriendDetailUpdata OnQueryFriendDetal(FriendUpdataMsg input);
        FriendRetunrError OnClearApplyList();
    }
    public class W2CFriendRequest
    {
        public W2CFriendRequest(RPCILocalCall l)
        {
            local = l;
        }
        RPCILocalCall local;
        public void W2CRpcTest(Int64 input, System.Action<wProtobuf.RPC.Error, None> onEnd)
        {
            local.Call("W2CFriend.W2CRpcTest", input, onEnd);
        }
        public wProtobufRPC.IYieldResult<None> W2CRpcTestYield(Int64 input)
        {
            wProtobufRPC.IYieldResult<None> info = wProtobufRPC.YieldFactory.Create<None>();
            W2CRpcTest(input, info.OnEnd);
            return info;
        }
    }
    public abstract class W2CFriendRespone
    {
        public W2CFriendRespone(RPCIRemoteCall r)
        {
            r.Reg<Int64, None>("W2CFriend.W2CRpcTest", OnW2CRpcTest);
        }

        protected abstract None OnW2CRpcTest(Int64 input);
        public static void Reg(RPCIRemoteCall r, IW2CFriendRespone respone)
        {
            r.Reg<Int64, None>("W2CFriend.W2CRpcTest", respone.OnW2CRpcTest);
        }
    }
    public interface IW2CFriendRespone
    {
        None OnW2CRpcTest(Int64 input);
    }
}