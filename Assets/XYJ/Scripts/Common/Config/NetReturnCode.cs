namespace xys
{
    using System.Reflection;

    public static class NetReturnCode
    {
        // 是否出错了
        public static bool isError<T>(T obj)
        {
            if (obj == null)
                return false;

            var field = obj.GetType().GetField("code", BindingFlags.Instance | BindingFlags.Public);
            if (field == null)
                return false;

            if (field.FieldType != typeof(NetProto.ReturnCode))
                return false;

            NetProto.ReturnCode code = (NetProto.ReturnCode)field.GetValue(obj);
            if (code == NetProto.ReturnCode.ReturnCode_OK)
                return false;

            // 这里加下统一的出错处理，比如读表之类的

            return true; // 出错了
        }

        public static bool isErrorYield(wProtobuf.RPC.IYieldResult obj)
        {
            if (obj.code != wProtobuf.RPC.Error.Success)
                return false; // 这里后面加下统一的出错处理，包括提示之类的

            return false;
        }

        public static bool isErrorYield<T>(wProtobuf.RPC.IYieldResult<T> obj)
        {
            if (obj.code != wProtobuf.RPC.Error.Success)
                return true; // RPC回调的错误

            // 这里后面加下统一的出错处理，包括提示之类的
            if (obj.result == null)
            {
                Debuger.ErrorLog("type:{0} obj.result == null", typeof(T));
                return false; // 返回值为空的，有问题
            }

            return isError(obj.result);
        }
    }
}