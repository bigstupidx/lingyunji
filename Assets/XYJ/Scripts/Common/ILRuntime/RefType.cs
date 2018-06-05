namespace xys
{
    using UnityEngine;
    using System.Reflection;
    using System.Collections.Generic;

    public class RefType
    {
        public RefType(string fullType, object param)
        {
            this.fullType = fullType;
            type = IL.Help.GetTypeByFullName(fullType);

            if (type != null)
                instance = IL.Help.CreateInstaince(type, param);
        }

        public RefType(string fullType)
        {
            this.fullType = fullType;
            type = IL.Help.GetTypeByFullName(fullType);

            if (type != null)
                instance = IL.Help.CreateInstaince(type, null);
        }

        public RefType(object instance)
        {
            this.instance = instance;

#if USE_HOT
            if (instance is ILRuntime.Runtime.Intepreter.ILTypeInstance)
            {
                var realType = (ILRuntime.Runtime.Intepreter.ILTypeInstance)instance;
                type = realType.Type.ReflectionType;
            }
            else
#endif
            {
                type = instance.GetType();
            }

            fullType = type.FullName;
        }

        object instance;
        string fullType;
        System.Type type;
       
        public object Instance { get { return instance; } }
        public System.Type Type { get { return type; } }

        public T GetField<T>(string name) { return (T)GetField(name); }
        public void SetField(string name, object value)
        {
            var field = IL.Help.GetField(type, name);
            if (field == null)
            {
                Debuger.ErrorLog("type:{0} field:{1} not find!", fullType, name);
                return ;
            }

            field.SetValue(instance, value);
        }

        // 得到某个字段的值
        public object GetField(string name)
        {
            var field = IL.Help.GetField(type, name);
            if (field == null)
            {
                Debuger.ErrorLog("type:{0} field:{1} not find!", fullType, name);
                return null;
            }

            return field.GetValue(instance);
        }

        public T TryGetField<T>(string name)
        {
            return (T)TryGetField(name);
        }

        public object TryGetField(string name)
        {
            var field = IL.Help.GetField(type, name);
            if (field == null)
                return null;

            return field.GetValue(instance);
        }

        public void TrySetField(string name, object value)
        {
            var field = IL.Help.GetField(type, name);
            if (field == null)
                return;

            field.SetValue(instance, value);
        }

        public void InvokeMethod(string name, params object[] param)
        {
            InvokeMethodReturn(name, param);
        }

        public object InvokeMethodReturn(string name, params object[] param)
        {
            var methodInfo = IL.Help.GetMethod(type, name);
            if (methodInfo == null)
            {
                Debuger.ErrorLog("type:{0} field:{1} not find!", fullType, name);
                return null;
            }

            return methodInfo.Invoke(instance, param);
        }

        public void TryInvokeMethod(string name, params object[] param)
        {
            TryInvokeMethodReturn(name, param);
        }

        public object TryInvokeMethodReturn(string name, params object[] param)
        {
            var methodInfo = IL.Help.GetMethod(type, name);
            if (methodInfo == null)
                return null;
            return methodInfo.Invoke(instance, param);
        }

        public object GetProperty(string name)
        {
            var propertyInfo = IL.Help.GetProperty(type, name);
            if (propertyInfo == null)
            {
                Debuger.ErrorLog("type:{0} Property:{1} not find!", fullType, name);
                return null;
            }

            return propertyInfo.GetValue(instance, null);
        }

        public T GetProperty<T>(string name)
        {
            return (T)GetProperty(name);
        }

        public void SetProperty(string name, object value)
        {
            var propertyInfo = IL.Help.GetProperty(type, name);
            if (propertyInfo == null)
            {
                Debuger.ErrorLog("type:{0} Property:{1} not find!", fullType, name);
                return;
            }

            propertyInfo.SetValue(instance, value, null);
        }

        public object TryGetProperty(string name)
        {
            var propertyInfo = IL.Help.GetProperty(type, name);
            if (propertyInfo == null)
                return null;
            return propertyInfo.GetValue(instance, null);
        }

        public T TryGetProperty<T>(string name)
        {
            return (T)TryGetProperty(name);
        }

        public void TrySetProperty(string name, object value)
        {
            var propertyInfo = IL.Help.GetProperty(type, name);
            if (propertyInfo == null)
                return;

            propertyInfo.SetValue(instance, value, null);
        }
    }
}