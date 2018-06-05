using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
#if USE_HOT
using ILRuntime.Reflection;
using ILRuntime.Runtime.Intepreter;
#endif

namespace xys.IL
{
    public class MonoStream
    {
        public MonoStream(JSONObject json, List<Object> objs)
        {
            this.json = json;
            this.objs = objs;
        }

        public JSONObject json { get; private set; }
        public List<Object> objs { get; private set; }

        public void Save(string key, object p)
        {
            if (p == null)
                return;

            if (p.GetType().IsArray)
            {
                System.Array va = p as System.Array;
                // 数组
                JSONArray array = new JSONArray();
                json.put(key, array);
                for (int i = 0; i < va.Length; ++i)
                {
                    object v = va.GetValue(i);
                    if (v == null)
                        array.put("(null)");
                    else if (v is Object)
                    {
                        array.put(i, Add(v as Object));
                    }
                    else if (Help.IsBaseType(v.GetType()))
                    {
                        // 基础类型
                        array.put(i, v);
                    }
#if USE_HOT
                    else if (v is ILTypeInstance)
                    {

                    }
#endif
                    else
                    {
                        JSONObject j = new JSONObject();
                        MonoStream ms = new MonoStream(j, objs);
                        array.put(i, j);
                        MonoSerialize.WriteTo(v, ms);
                    }
                }
            }
            else
            {
                if (p is Object)
                {
                    json.putOpt(key, Add(p as Object));
                }
                else
                {
                    json.putOpt(key, p);
                }
            }
        }

        int Add(Object o)
        {
            int pos = objs.IndexOf(o);
            if (pos == -1)
            {
                objs.Add(o);
                pos = objs.Count - 1;
            }

            return pos;
        }
    }

    public interface ITypeSerialize
    {
        void WriteTo(object parent, FieldInfo info, MonoStream ms);
        void MergeFrom(object parent, FieldInfo info, MonoStream ms);
    }

    public static class MonoSerialize
    {
        static Dictionary<System.Type, ITypeSerialize> AllTypes = new Dictionary<System.Type, ITypeSerialize>();

        static MonoSerialize()
        {
            AllTypes.Add(typeof(int), new IntType());
            AllTypes.Add(typeof(uint), new UIntType());
            AllTypes.Add(typeof(sbyte), new sByteType());
            AllTypes.Add(typeof(byte), new ByteType());
            AllTypes.Add(typeof(short), new ShortType());
            AllTypes.Add(typeof(ushort), new UShortType());
            AllTypes.Add(typeof(long), new LongType());
            AllTypes.Add(typeof(ulong), new ULongType());
            AllTypes.Add(typeof(string), new StrType());
            AllTypes.Add(typeof(float), new FloatType());
            AllTypes.Add(typeof(double), new DoubleType());
            AllTypes.Add(typeof(Object), new ObjectType());

            AllTypes.Add(typeof(int[]), new ArrayIntType());
            AllTypes.Add(typeof(uint[]), new ArrayUIntType());
            AllTypes.Add(typeof(sbyte[]), new ArraySByteType());
            AllTypes.Add(typeof(byte[]), new ArrayByteType());
            AllTypes.Add(typeof(short[]), new ArrayShortType());
            AllTypes.Add(typeof(ushort[]), new ArrayUShortType());
            AllTypes.Add(typeof(long[]), new ArrayLongType());
            AllTypes.Add(typeof(ulong[]), new ArrayULongType());
            AllTypes.Add(typeof(string[]), new ArrayStrType());
            AllTypes.Add(typeof(float[]), new ArrayFloatType());
            AllTypes.Add(typeof(double[]), new ArrayDoubleType());
            AllTypes.Add(typeof(Object[]), new ArrayObjectType());
        }

        public static ITypeSerialize Get(System.Type type)
        {
            ITypeSerialize ts = null;
            if (AllTypes.TryGetValue(type, out ts))
                return ts;

            if (Help.isType(type, typeof(Object)))
                return AllTypes[typeof(Object)];

            List<FieldInfo> fieldinfos = Help.GetSerializeField(type);
            ts = new AnyType(type, fieldinfos);
            AllTypes.Add(type, ts);

            return ts;
        }

        public static void WriteTo(object obj, MonoStream ms)
        {
            List<FieldInfo> infos = Help.GetSerializeField(obj);
            for (int i = 0; i < infos.Count; ++i)
            {
                var field = infos[i];
                Get(field.FieldType).WriteTo(obj, field, ms);
            }
        }

        public static MonoStream WriteTo(object obj)
        {
            MonoStream ms = new MonoStream(new JSONObject(), new List<Object>());
            List<FieldInfo> infos = Help.GetSerializeField(obj);
            for (int i = 0; i < infos.Count; ++i)
            {
                var field = infos[i];
                Get(field.FieldType).WriteTo(obj, field, ms);
            }

            return ms;
        }

        public static void MergeFrom(object obj, MonoStream ms)
        {
            List<FieldInfo> infos = Help.GetSerializeField(obj);
            for (int i = 0; i < infos.Count; ++i)
            {
                var field = infos[i];
                Get(field.FieldType).MergeFrom(obj, field, ms);
            }
        }
    }
}
