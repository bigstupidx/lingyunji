using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace xys.IL
{
    class IntType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            ms.Save(info.Name, info.GetValue(parent));
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            if (ms.json.has(info.Name))
                info.SetValue(parent, ms.json.getInt(info.Name));
        }
    }

    class UIntType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            ms.Save(info.Name, info.GetValue(parent));
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            if (ms.json.has(info.Name))
                info.SetValue(parent, (uint)ms.json.getInt(info.Name));
        }
    }

    class ByteType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            ms.Save(info.Name, info.GetValue(parent));
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            if (ms.json.has(info.Name))
                info.SetValue(parent, (byte)ms.json.getInt(info.Name));
        }
    }

    class sByteType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            ms.Save(info.Name, info.GetValue(parent));
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            if (ms.json.has(info.Name))
                info.SetValue(parent, (sbyte)ms.json.getInt(info.Name));
        }
    }

    class ShortType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            ms.Save(info.Name, info.GetValue(parent));
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            if (ms.json.has(info.Name))
                info.SetValue(parent, (short)ms.json.getInt(info.Name));
        }
    }

    class UShortType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            ms.Save(info.Name, info.GetValue(parent));
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            if (ms.json.has(info.Name))
                info.SetValue(parent, (ushort)ms.json.getInt(info.Name));
        }
    }

    class FloatType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            ms.Save(info.Name, info.GetValue(parent));
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            if (ms.json.has(info.Name))
                info.SetValue(parent, (float)ms.json.getDouble(info.Name));
        }
    }

    class DoubleType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            ms.Save(info.Name, info.GetValue(parent));
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            if (ms.json.has(info.Name))
                info.SetValue(parent, ms.json.getDouble(info.Name));
        }
    }

    class LongType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            ms.Save(info.Name, info.GetValue(parent));
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            if (ms.json.has(info.Name))
                info.SetValue(parent, ms.json.getLong(info.Name));
        }
    }

    class ULongType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            ms.Save(info.Name, info.GetValue(parent));
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            if (ms.json.has(info.Name))
                info.SetValue(parent, (ulong)ms.json.getLong(info.Name));
        }
    }

    class StrType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            ms.Save(info.Name, info.GetValue(parent));
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            if (ms.json.has(info.Name))
                info.SetValue(parent, ms.json.getString(info.Name));
        }
    }

    class ObjectType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            ms.Save(info.Name, info.GetValue(parent));
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            if (ms.json.has(info.Name))
            {
                string value = ms.json.getString(info.Name);
                if (value == "(null)")
                    return;

                int pos = ms.json.getInt(info.Name);
                try
                {
                    info.SetValue(parent, ms.objs[pos]);
                }
                catch (System.Exception ex)
                {
                    Debuger.ErrorLog("type:{0} field:{1} error!", parent.GetType().Name, info.Name);
                    Debuger.LogException(ex);
                }
            }
        }
    }
}
