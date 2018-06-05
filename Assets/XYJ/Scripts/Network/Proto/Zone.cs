// Generated by the protocol buffer compiler.  DO NOT EDIT!
// source: ClientProto/Zone.proto

using wProtobuf;
using System.Collections;
using System.Collections.Generic;
using wProtobufRPC = wProtobuf.RPC;
using RPCILocalCall = wProtobuf.RPC.ILocalCall;
using RPCIRemoteCall = wProtobuf.RPC.IRemoteCall;

namespace NetProto
{
    public enum ZoneType
    {
        Server = 0,
        User = 1,
        Team = 2,
        Guild = 3,
        Max = 4,
    }
    public enum ObjectType
    {
        Player = 0,
        Npc = 1,
        Pet = 2,
    }
    public class SceneObjectSyncData : IMessage
    {
        public ObjectType type = ObjectType.Player;
        public int charSceneId = 0;
        public int sid = 0;
        public Point3 pos = new Point3();
        public ByteString atts = null;
        public ByteString extraAtts = null;
        public int refreshId = 0;
        public int angle = 0;
        public string name = null;
        public int materid = 0;
        public int battleCamp = 0;
        public int CalculateSize()
        {
            int _total_size_ = 0;
            if (type != ObjectType.Player)
            {
                _total_size_ += 1 + ComputeSize.ComputeEnumSize((int)type);
            }
            if (charSceneId != 0)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt32Size(charSceneId);
            }
            if (sid != 0)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt32Size(sid);
            }
            if (pos != null)
            {
                _total_size_ += 1 + ComputeSize.ComputeMessageSize(pos);
            }
            if (atts != null && atts.Length != 0)
            {
                _total_size_ += 1 + ComputeSize.ComputeBytesSize(atts);
            }
            if (extraAtts != null && extraAtts.Length != 0)
            {
                _total_size_ += 1 + ComputeSize.ComputeBytesSize(extraAtts);
            }
            if (refreshId != 0)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt32Size(refreshId);
            }
            if (angle != 0)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt32Size(angle);
            }
            if (!string.IsNullOrEmpty(name))
            {
                _total_size_ += 1 + ComputeSize.ComputeStringSize(name);
            }
            if (materid != 0)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt32Size(materid);
            }
            if (battleCamp != 0)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt32Size(battleCamp);
            }
            return _total_size_;
        }
        public void WriteTo(IWriteStream output)
        {
            if (type != ObjectType.Player)
            {
                output.WriteRawTag(8);
                output.WriteEnum((int)type);
            }
            if (charSceneId != 0)
            {
                output.WriteRawTag(16);
                output.WriteInt32(charSceneId);
            }
            if (sid != 0)
            {
                output.WriteRawTag(24);
                output.WriteInt32(sid);
            }
            if (pos != null)
            {
                output.WriteRawTag(34);
                output.WriteMessage(pos);
            }
            if (atts != null && atts.Length != 0)
            {
                output.WriteRawTag(58);
                output.WriteBytes(atts);
            }
            if (extraAtts != null && extraAtts.Length != 0)
            {
                output.WriteRawTag(66);
                output.WriteBytes(extraAtts);
            }
            if (refreshId != 0)
            {
                output.WriteRawTag(72);
                output.WriteInt32(refreshId);
            }
            if (angle != 0)
            {
                output.WriteRawTag(80);
                output.WriteInt32(angle);
            }
            if (!string.IsNullOrEmpty(name))
            {
                output.WriteRawTag(90);
                output.WriteString(name);
            }
            if (materid != 0)
            {
                output.WriteRawTag(96);
                output.WriteInt32(materid);
            }
            if (battleCamp != 0)
            {
                output.WriteRawTag(104);
                output.WriteInt32(battleCamp);
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
                        type = (ObjectType)input.ReadEnum();
                    }
                    break;
                case 16:
                    {
                        charSceneId = input.ReadInt32();
                    }
                    break;
                case 24:
                    {
                        sid = input.ReadInt32();
                    }
                    break;
                case 34:
                    {
                        if (pos == null)
                            pos = new Point3();
                        input.ReadMessage(pos);
                    }
                    break;
                case 58:
                    {
                        atts = input.ReadBytes();
                    }
                    break;
                case 66:
                    {
                        extraAtts = input.ReadBytes();
                    }
                    break;
                case 72:
                    {
                        refreshId = input.ReadInt32();
                    }
                    break;
                case 80:
                    {
                        angle = input.ReadInt32();
                    }
                    break;
                case 90:
                    {
                        name = input.ReadString();
                    }
                    break;
                case 96:
                    {
                        materid = input.ReadInt32();
                    }
                    break;
                case 104:
                    {
                        battleCamp = input.ReadInt32();
                    }
                    break;
                default:
                    input.SkipLastField(tag);
                    break;
                }
            }
        }
    }

    public class SOSyncs : IMessage
    {
        public List<SceneObjectSyncData> objs = new List<SceneObjectSyncData>();
        public int CalculateSize()
        {
            int _total_size_ = 0;
            if (objs != null && objs.Count != 0)
            {
                _total_size_ += 1 * objs.Count;
                for (int i = 0; i < objs.Count; ++i)
                    _total_size_ += ComputeSize.ComputeMessageSize(objs[i]);
            }
            return _total_size_;
        }
        public void WriteTo(IWriteStream output)
        {
            if (objs != null && objs.Count != 0)
            {
                for (int i = 0; i < objs.Count; ++i)
                {
                    output.WriteRawTag(10);
                    output.WriteMessage(objs[i]);
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
                        if (objs == null)
                            objs = new List<SceneObjectSyncData>();
                        var local_temp = new SceneObjectSyncData();
                        input.ReadMessage(local_temp);
                        objs.Add(local_temp);
                    }
                    break;
                default:
                    input.SkipLastField(tag);
                    break;
                }
            }
        }
    }

    public class ChangeSceneData : IMessage
    {
        public long zoneId = 0L;
        public int charSceneId = 0;
        public string logic = null;
        public Point3 pos = new Point3();
        public int angle = 0;
        public int CalculateSize()
        {
            int _total_size_ = 0;
            if (zoneId != 0L)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt64Size(zoneId);
            }
            if (charSceneId != 0)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt32Size(charSceneId);
            }
            if (!string.IsNullOrEmpty(logic))
            {
                _total_size_ += 1 + ComputeSize.ComputeStringSize(logic);
            }
            if (pos != null)
            {
                _total_size_ += 1 + ComputeSize.ComputeMessageSize(pos);
            }
            if (angle != 0)
            {
                _total_size_ += 1 + ComputeSize.ComputeInt32Size(angle);
            }
            return _total_size_;
        }
        public void WriteTo(IWriteStream output)
        {
            if (zoneId != 0L)
            {
                output.WriteRawTag(8);
                output.WriteInt64(zoneId);
            }
            if (charSceneId != 0)
            {
                output.WriteRawTag(16);
                output.WriteInt32(charSceneId);
            }
            if (!string.IsNullOrEmpty(logic))
            {
                output.WriteRawTag(26);
                output.WriteString(logic);
            }
            if (pos != null)
            {
                output.WriteRawTag(34);
                output.WriteMessage(pos);
            }
            if (angle != 0)
            {
                output.WriteRawTag(40);
                output.WriteInt32(angle);
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
                        zoneId = input.ReadInt64();
                    }
                    break;
                case 16:
                    {
                        charSceneId = input.ReadInt32();
                    }
                    break;
                case 26:
                    {
                        logic = input.ReadString();
                    }
                    break;
                case 34:
                    {
                        if (pos == null)
                            pos = new Point3();
                        input.ReadMessage(pos);
                    }
                    break;
                case 40:
                    {
                        angle = input.ReadInt32();
                    }
                    break;
                default:
                    input.SkipLastField(tag);
                    break;
                }
            }
        }
    }
}