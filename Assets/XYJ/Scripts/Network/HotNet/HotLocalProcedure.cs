#if !USE_HOT
namespace wProtobuf
{
    public static class StaticWriteStream
    {
        public static void WriteMessage(this IWriteStream stream, xys.hot.RPC.IMessage value)
        {
            stream.WriteInt32(value.CalculateSize());
            value.WriteTo(stream);
        }

        public static void ReadMessage(this IReadStream stream, xys.hot.RPC.IMessage value)
        {
            stream.ReadMessage(() => 
            {
                value.MergeFrom(stream);
            });
        }
    }
}

namespace xys.hot
{
    static class RPCHelp
    {
        public static Result wMessageMergeFrom<Result>(wProtobuf.RealBytes bytes) where Result : wProtobuf.IMessage, new()
        {
            if (bytes == null)
                return default(Result);

            var result = new Result();
            if (bytes.bytes != null && bytes.bytes.Length > 0)
            {
                wProtobuf.MessageStream stream = wProtobuf.MessageStream.ReaderCreate(bytes.bytes);
                try
                {
                    result.MergeFrom(stream);
                }
                catch (System.Exception ex)
                {
                    Debuger.ErrorLog("type:{0} MergeFrom Error!", typeof(Result).Name);
                    Debuger.LogException(ex);
                }
            }

            return result;
        }

        public static Result hMessageMergeFrom<Result>(wProtobuf.RealBytes bytes) where Result : RPC.IMessage, new()
        {
            if (bytes == null)
                return default(Result);

            var result = new Result();
            if (bytes.bytes != null && bytes.bytes.Length > 0)
            {
                wProtobuf.MessageStream stream = wProtobuf.MessageStream.ReaderCreate(bytes.bytes);
                try
                {
                    result.MergeFrom(stream);
                }
                catch (System.Exception ex)
                {
                    Debuger.ErrorLog("type:{0} MergeFrom Error!", typeof(Result).Name);
                    Debuger.LogException(ex);
                }
            }

            return result;
        }

        public static Result MergeFrom<Result>(wProtobuf.RealBytes bytes) where Result : new()
        {
            if (bytes == null)
                return default(Result);

            var result = new Result();
            if (bytes.bytes != null && bytes.bytes.Length > 0)
            {
                wProtobuf.MessageStream stream = wProtobuf.MessageStream.ReaderCreate(bytes.bytes);
                try
                {
                    if (result is wProtobuf.IMessage)
                        ((wProtobuf.IMessage)result).MergeFrom(stream);
                    else if (result is RPC.IMessage)
                        ((RPC.IMessage)result).MergeFrom(stream);
                    else
                    {
                        Debuger.ErrorLog("MergeFrom obj:{0} is not IMessage!", typeof(Result).FullName);
                        return default(Result);
                    }
                }
                catch (System.Exception ex)
                {
                    Debuger.ErrorLog("type:{0} MergeFrom Error!", typeof(Result).Name);
                    Debuger.LogException(ex);
                }
            }

            return result;
        }

        public static byte[] WriteTo(wProtobuf.IMessage obj)
        {
            var wi = obj;
            byte[] bytes = new byte[wi.CalculateSize()];
            wi.WriteTo(new wProtobuf.MessageStream(bytes));
            return bytes;
        }

        public static byte[] WriteTo(RPC.IMessage obj)
        {
            var wi = obj;
            byte[] bytes = new byte[wi.CalculateSize()];
            wi.WriteTo(new wProtobuf.MessageStream(bytes));
            return bytes;
        }

        public static byte[] WriteTo(object obj)
        {
            if (obj is wProtobuf.IMessage)
            {
                return WriteTo((wProtobuf.IMessage)obj);
            }
            else if (obj is RPC.IMessage)
            {
                return WriteTo((RPC.IMessage)obj);
            }

            Debuger.ErrorLog("WriteTo obj:{0} is not IMessage!", obj == null ? "null" : obj.GetType().FullName);
            return null;
        }
    }

    internal class HotLocalProcedure : RPC.ILocalCall
    {
        public HotLocalProcedure(wRPC.LocalProcedure parent)
        {
            this.parent = parent;
        }

        wRPC.LocalProcedure parent;
        Network.BitStream stream = new Network.BitStream(1024);

        public void Call<Param, Result>(string key, Param param, System.Action<wProtobuf.RPC.Error, Result> onEnd) where Result : new()
        {
            OnCallEnd<Result> end = new OnCallEnd<Result>(onEnd);
            parent.Call<HotMessage, wProtobuf.RealBytes>(key, new HotMessage(param), end.OnEnd);
        }

        class OnCallEnd<Result> where Result : new()
        {
            public OnCallEnd(System.Action<wProtobuf.RPC.Error, Result> onEnd)
            {
                this.onEnd = onEnd;
            }

            System.Action<wProtobuf.RPC.Error, Result> onEnd;

            public void OnEnd(wProtobuf.RPC.Error error, wProtobuf.RealBytes bytes)
            {
                OnResultEnd(error, bytes, onEnd);
            }
        }

        static void OnResultEnd<Result>(wProtobuf.RPC.Error error, wProtobuf.RealBytes bytes, System.Action<wProtobuf.RPC.Error, Result> onEnd) where Result : new()
        {
            if (bytes == null)
            {
                if (onEnd != null)
                {
                    onEnd(error, default(Result));
                }

                return;
            }

            if (onEnd != null)
            {
                var result = RPCHelp.MergeFrom<Result>(bytes);
                onEnd(error, result);
            }
        }

        public void Call(string key, System.Action<wProtobuf.RPC.Error> fun)
        {
            parent.Call(key, fun);
        }

        public void Call<Result>(string key, System.Action<wProtobuf.RPC.Error, Result> fun) where Result : new()
        {
            OnCallEnd<Result> call = new OnCallEnd<Result>(fun);
            parent.Call<wProtobuf.RealBytes>(key, call.OnEnd);
        }

        public void Call<Param>(string key, Param param, System.Action<wProtobuf.RPC.Error> fun)
        {
            parent.Call(key, new HotMessage(param), fun);
        }
    }
}
#endif