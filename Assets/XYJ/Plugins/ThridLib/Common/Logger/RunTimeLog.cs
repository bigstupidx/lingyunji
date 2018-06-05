using System;
using UnityEngine;

public partial class XYJLogger
{
    static public ILog CreateLog()
    {
#if COM_DEBUG
        return new RunTime();
#else
        return new NoDebugLog();
#endif
    }

    public class RunTime : ILog
    {
        public void Log(LogType type, string format, params object[] objs)
        {
            XYJLogger.LogInfo(type, format, objs);
        }

        public void Log(object message)
        {
            XYJLogger.LogDebug(message.ToString());
        }

        public void Log(object message, UnityEngine.Object context)
        {
            XYJLogger.LogDebug("message:{0} context:{1}", message, context);
        }

        public void LogError(object message)
        {
            XYJLogger.LogError(message.ToString());
        }

        public void LogError(object message, UnityEngine.Object context)
        {
            XYJLogger.LogError("message:{0} context:{1}", message, context);
        }

        public void LogException(Exception exception)
        {
            XYJLogger.LogError("exception:{0} StackTrace:{1} Source:{2}", exception.Message, exception.StackTrace, exception.Source);
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            XYJLogger.LogError("exception:{0} StackTrace:{1} Source:{2} context:{3}", exception.Message, exception.StackTrace, exception.Source, context);
        }

        public void LogWarning(object message)
        {
            XYJLogger.LogWarning(message.ToString());
        }

        public void LogWarning(object message, UnityEngine.Object context)
        {
            XYJLogger.LogWarning("message:{0} context:{1}", message, context);
        }
    }

    public class NoDebugLog : ILog
    {
        public void Log(LogType type, string format, params object[] objs)
        {
            
        }

        public void Log(object message)
        {
           
        }

        public void Log(object message, UnityEngine.Object context)
        {
            
        }

        public void LogError(object message)
        {
            XYJLogger.LogDebug(message.ToString());
        }

        public void LogError(object message, UnityEngine.Object context)
        {
            XYJLogger.LogError("message:{0} context:{1}", message, context);
        }

        public void LogException(Exception exception)
        {
            XYJLogger.LogError("exception:{0} StackTrace:{1} Source:{2}", exception.Message, exception.StackTrace, exception.Source);
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            XYJLogger.LogError("exception:{0} StackTrace:{1} Source:{2} context:{3}", exception.Message, exception.StackTrace, exception.Source, context);
        }

        public void LogWarning(object message)
        {
            
        }

        public void LogWarning(object message, UnityEngine.Object context)
        {
            
        }
    }
}
