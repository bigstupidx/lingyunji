namespace xys
{
    using System.Reflection;

    public static class NetReturnCode
    {
        // �Ƿ������
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

            // �������ͳһ�ĳ������������֮���

            return true; // ������
        }

        public static bool isErrorYield(wProtobuf.RPC.IYieldResult obj)
        {
            if (obj.code != wProtobuf.RPC.Error.Success)
                return false; // ����������ͳһ�ĳ�����������ʾ֮���

            return false;
        }

        public static bool isErrorYield<T>(wProtobuf.RPC.IYieldResult<T> obj)
        {
            if (obj.code != wProtobuf.RPC.Error.Success)
                return true; // RPC�ص��Ĵ���

            // ����������ͳһ�ĳ�����������ʾ֮���
            if (obj.result == null)
            {
                Debuger.ErrorLog("type:{0} obj.result == null", typeof(T));
                return false; // ����ֵΪ�յģ�������
            }

            return isError(obj.result);
        }
    }
}