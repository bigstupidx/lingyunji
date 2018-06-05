using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xys
{
    class CsvLog : CsvCommon.Log.I
    {
        public void Debug(string format, params object[] objs)
        {
            Debuger.DebugLog(format, objs);
        }

        public void Error(string format, params object[] objs)
        {
            Debuger.ErrorLog(format, objs);
        }

        public void Exception(System.Exception ex)
        {
            Debuger.LogException(ex);
        }

        public void Warning(string format, params object[] objs)
        {
            Debuger.WarningLog(format, objs);
        }
    }

    class CommonLog : Log.I
    {
        public void Debug(string format, params object[] objs)
        {
            Debuger.DebugLog(format, objs);
        }

        public void Error(string format, params object[] objs)
        {
            Debuger.ErrorLog(format, objs);
        }

        public void Exception(System.Exception ex)
        {
            Debuger.LogException(ex);
        }

        public void Warning(string format, params object[] objs)
        {
            Debuger.WarningLog(format, objs);
        }
    }
}
