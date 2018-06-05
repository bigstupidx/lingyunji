using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace xys.IL
{
    class ArrayIntType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            var array = (System.Array)info.GetValue(parent);
            if (array != null)
            {
                ms.Save(info.Name, array);
            }
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            JSONArray array = null;
            if (ms.json.TryGetValueJSONArray(info.Name, out array))
            {
                int[] values = new int[array.length()];
                for (int i = 0; i < array.length(); ++i)
                    values[i] = array.getInt(i);

                info.SetValue(parent, values);
            }
        }
    }

    class ArrayUIntType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            var array = (System.Array)info.GetValue(parent);
            if (array != null)
            {
                ms.Save(info.Name, array);
            }
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            JSONArray array = null;
            if (ms.json.TryGetValueJSONArray(info.Name, out array))
            {
                uint[] values = new uint[array.length()];
                for (int i = 0; i < array.length(); ++i)
                    values[i] = (uint)array.getInt(i);

                info.SetValue(parent, values);
            }
        }
    }

    class ArraySByteType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            var array = (System.Array)info.GetValue(parent);
            if (array != null)
            {
                ms.Save(info.Name, array);
            }
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            JSONArray array = null;
            if (ms.json.TryGetValueJSONArray(info.Name, out array))
            {
                sbyte[] values = new sbyte[array.length()];
                for (int i = 0; i < array.length(); ++i)
                    values[i] = (sbyte)array.getInt(i);

                info.SetValue(parent, values);
            }
        }
    }

    class ArrayByteType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            var array = (System.Array)info.GetValue(parent);
            if (array != null)
            {
                ms.Save(info.Name, array);
            }
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            JSONArray array = null;
            if (ms.json.TryGetValueJSONArray(info.Name, out array))
            {
                byte[] values = new byte[array.length()];
                for (int i = 0; i < array.length(); ++i)
                    values[i] = (byte)array.getInt(i);

                info.SetValue(parent, values);
            }
        }
    }

    class ArrayShortType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            var array = (System.Array)info.GetValue(parent);
            if (array != null)
            {
                ms.Save(info.Name, array);
            }
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            JSONArray array = null;
            if (ms.json.TryGetValueJSONArray(info.Name, out array))
            {
                short[] values = new short[array.length()];
                for (int i = 0; i < array.length(); ++i)
                    values[i] = (short)array.getInt(i);

                info.SetValue(parent, values);
            }
        }
    }

    class ArrayUShortType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            var array = (System.Array)info.GetValue(parent);
            if (array != null)
            {
                ms.Save(info.Name, array);
            }
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            JSONArray array = null;
            if (ms.json.TryGetValueJSONArray(info.Name, out array))
            {
                ushort[] values = new ushort[array.length()];
                for (int i = 0; i < array.length(); ++i)
                    values[i] = (ushort)array.getInt(i);

                info.SetValue(parent, values);
            }
        }
    }

    class ArrayFloatType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            var array = (System.Array)info.GetValue(parent);
            if (array != null)
            {
                ms.Save(info.Name, array);
            }
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            JSONArray array = null;
            if (ms.json.TryGetValueJSONArray(info.Name, out array))
            {
                float[] values = new float[array.length()];
                for (int i = 0; i < array.length(); ++i)
                    values[i] = (float)array.getDouble(i);

                info.SetValue(parent, values);
            }
        }
    }

    class ArrayDoubleType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            var array = (System.Array)info.GetValue(parent);
            if (array != null)
            {
                ms.Save(info.Name, array);
            }
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            JSONArray array = null;
            if (ms.json.TryGetValueJSONArray(info.Name, out array))
            {
                double[] values = new double[array.length()];
                for (int i = 0; i < array.length(); ++i)
                    values[i] = array.getDouble(i);

                info.SetValue(parent, values);
            }
        }
    }

    class ArrayLongType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            var array = (System.Array)info.GetValue(parent);
            if (array != null)
            {
                ms.Save(info.Name, array);
            }
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            JSONArray array = null;
            if (ms.json.TryGetValueJSONArray(info.Name, out array))
            {
                long[] values = new long[array.length()];
                for (int i = 0; i < array.length(); ++i)
                    values[i] = array.getLong(i);

                info.SetValue(parent, values);
            }
        }
    }

    class ArrayULongType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            var array = (System.Array)info.GetValue(parent);
            if (array != null)
            {
                ms.Save(info.Name, array);
            }
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            JSONArray array = null;
            if (ms.json.TryGetValueJSONArray(info.Name, out array))
            {
                ulong[] values = new ulong[array.length()];
                for (int i = 0; i < array.length(); ++i)
                    values[i] = (ulong)array.getLong(i);

                info.SetValue(parent, values);
            }
        }
    }

    class ArrayStrType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            var array = (System.Array)info.GetValue(parent);
            if (array != null)
            {
                ms.Save(info.Name, array);
            }
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            JSONArray array = null;
            if (ms.json.TryGetValueJSONArray(info.Name, out array))
            {
                string[] values = new string[array.length()];
                for (int i = 0; i < array.length(); ++i)
                    values[i] = array.getString(i);

                info.SetValue(parent, values);
            }
        }
    }

    class ArrayObjectType : ITypeSerialize
    {
        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            var array = (System.Array)info.GetValue(parent);
            if (array != null)
            {
                ms.Save(info.Name, array);
            }
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            JSONArray array = null;
            if (ms.json.TryGetValueJSONArray(info.Name, out array))
            {
                string[] values = new string[array.length()];
                for (int i = 0; i < array.length(); ++i)
                    values[i] = array.getString(i);

                info.SetValue(parent, values);
            }
        }
    }
}
