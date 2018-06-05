using System.Reflection;
using System.Collections.Generic;
#if USE_HOT
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Reflection;
#endif

namespace xys.Editor
{
    public interface ITypeGUI
    {
        bool OnGUI(object parent, FieldInfo info);
        object OnGUI(string label, object value, System.Type type, out bool isDirty);
    }

    class EmptyTypeGUI : ITypeGUI
    {
        public bool OnGUI(object parent, FieldInfo info)
        {
            return false;
        }

        public object OnGUI(string label, object value, System.Type type, out bool isDirty)
        {
            isDirty = false;
            return value;
        }
    }

    public static class TypeEditor 
    {
        static TypeEditor()
        {
            AllTypes.Add(typeof(int), new IntType());
            AllTypes.Add(typeof(uint), new UIntType());
            AllTypes.Add(typeof(sbyte), new sByteType());
            AllTypes.Add(typeof(byte), new ByteType());
            AllTypes.Add(typeof(short), new ShortType());
            AllTypes.Add(typeof(ushort), new UShortType());
            AllTypes.Add(typeof(long), new LongType());
            AllTypes.Add(typeof(ulong), new ULongType());
            AllTypes.Add(typeof(float), new FloatType());
            AllTypes.Add(typeof(double), new DoubleType());
            AllTypes.Add(typeof(string), new StrType());
            AllTypes.Add(typeof(UnityEngine.Object), new ObjectType());
        }
        
        // 基础类型
        static Dictionary<System.Type, ITypeGUI> AllTypes = new Dictionary<System.Type, ITypeGUI>();

        public static ITypeGUI Get(System.Type type)
        {
            ITypeGUI typeGUI = null;
            if (AllTypes.TryGetValue(type, out typeGUI))
                return typeGUI;

            if (IL.Help.isType(type, typeof(UnityEngine.Object)))
                return AllTypes[typeof(UnityEngine.Object)];

            if (type.IsArray)
            {
                string typeName = type.FullName;
                typeName = typeName.Substring(0, typeName.Length - 2);

                var elementType = IL.Help.GetType(typeName);
                if (IL.Help.IsBaseType(elementType))
                {
                    var arrayGUI = new ArrayObjectType(elementType, Get(elementType));
                    AllTypes.Add(type, arrayGUI);

                    return arrayGUI;
                }
                else
                {
                    return null;
                }
            }

#if USE_HOT
            if (type is ILRuntimeType && (type.Name.EndsWith("[]")))
            {
                return new EmptyTypeGUI();
            }
#endif
            if (!type.IsSerializable
#if USE_HOT
                && (type.GetType() != typeof(ILRuntimeType) && !((ILRuntimeType)type).ILType.TypeDefinition.IsSerializable)
#endif
                )
            {
                return new EmptyTypeGUI();
            }
            List<FieldInfo> fieldinfos = IL.Help.GetSerializeField(type);
            var gui = new AnyType(type, fieldinfos);
            AllTypes.Add(type, gui);
            return gui;
        }

        public static bool OnGUI(object parent)
        {
            bool isDirty = false;
            List<FieldInfo> fieldinfos = IL.Help.GetSerializeField(parent);
            for (int i = 0; i < fieldinfos.Count; ++i)
            {
                var field = fieldinfos[i];
                isDirty |= Get(field.FieldType).OnGUI(parent, field);
            }

            return isDirty;
        }
    }
}
