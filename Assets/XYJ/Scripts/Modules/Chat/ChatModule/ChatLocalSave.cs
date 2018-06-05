#if !USE_HOT
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Config;
using NetProto;
using NetProto.Hot;
using wProtobuf;

namespace xys.hot
{
    internal class ChatLocalSave
    {
        // TODO : 读写异常处理
        public static void SaveMsg(ChatMsgRspone msg)
        {
            //读取数据
            byte[] rBytes;
            try
            {
                rBytes = File.ReadAllBytes(string.Format("Chat/{0}", msg.fromUser.charid));
            }
            catch(Exception e)
            {
                Log.Error("chat local save error -> {0}", e.ToString());
                rBytes = null;
            }
            MessageStream msgStream;
            LocalChat data;
            // 有数据直接修改
            if(null != rBytes)
            {
                msgStream = new MessageStream(rBytes);
                data = new LocalChat();
                data.MergeFrom(msgStream);

                var info = new LocalInfo
                {
                    msg = msg.msg,
                    isMine = msg.fromUser.charid == App.my.localPlayer.charid,
                    itemDatas = msg.itemDatas,
                    itemIds = msg.itemIds,
                    time = msg.timeStamp
                };

                var list = data.infos;
                // 校验是否超过最大保存数量
                var num = ChatChannel.Get((int)msg.channel).saveNum;
                if(list.Count > num)
                {
                    for(int i = 0 ; i < list.Count - num ; i++)
                    {
                        list.RemoveAt(0);
                    }
                }
                list.Add(info);
                data.infos = list;
                data.fromUser = msg.fromUser;

                msgStream = new MessageStream(4096);
                data.WriteTo(msgStream);
                File.WriteAllBytes(string.Format("Chat/{0}", msg.fromUser.charid), msgStream.Buffer);
            }
            else
            {
                // 无记录，直接写入文件
                msgStream = new MessageStream(4096);
                data = new LocalChat();
                var info = new LocalInfo
                {
                    msg = msg.msg,
                    isMine = msg.fromUser.charid == xys.App.my.localPlayer.charid,
                    itemDatas = msg.itemDatas,
                    itemIds = msg.itemIds,
                    time = msg.timeStamp
                };
                var list = new List<LocalInfo> { info };
                data.infos = list;
                data.fromUser = msg.fromUser;
                data.WriteTo(msgStream);

                var path = string.Format("Chat/{0}", msg.fromUser.charid);
                if(Directory.Exists(path) == false)
                {
                    Directory.CreateDirectory(path);
                }
                File.WriteAllBytes(path, msgStream.Buffer);
            }
        }

        public static List<LocalInfo> GetMsg(long charId)
        {
            // 读取数据
            byte[] rBytes;
            try
            {
                rBytes = File.ReadAllBytes(string.Format("Chat/{0}", charId));
            }
            catch(Exception e)
            {
                Log.Error("chat local read error -> {0}", e.ToString());
                return null;
            }

            var rMsgStream = new MessageStream(rBytes);
            var data = new LocalChat();
            data.MergeFrom(rMsgStream);

            return data.infos;
        }

        public static string GetDateTimeFromTicks(long tick)
        {
            DateTime d = new DateTime(tick);
            string s = d.ToString("yyyy-MM-dd-HH-mm");
            var strArr = s.Split('-');
            var sb = new StringBuilder();
            sb.AppendFormat("{0}年{1}月{2}日  {3}时{4}分", strArr[0], strArr[1], strArr[2], strArr[3], strArr[4]);
            return sb.ToString();
        }
    }
}

#endif