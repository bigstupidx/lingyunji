namespace xys.IL
{
    using System.Reflection;
#if USE_HOT
    using Mono.Cecil;
    using ILRuntime.Reflection;
    using ILRuntime.CLR.TypeSystem;
    using ILRuntime.Runtime.Intepreter;
#endif
    using System.Collections.Generic;

    public static class Help
    {
        static void HotMM()
        {
            System.Type v = null;
            // List
            {
                v = typeof(List<object>);
                v = typeof(List<char>);
                v = typeof(List<short>);
                v = typeof(List<ushort>);
                v = typeof(List<int>);
                v = typeof(List<uint>);
                v = typeof(List<long>);
                v = typeof(List<ulong>);
                v = typeof(string);

                v = typeof(object[]);
                v = typeof(char[]);
                v = typeof(short[]);
                v = typeof(ushort[]);
                v = typeof(int[]);
                v = typeof(uint[]);
                v = typeof(long[]);
                v = typeof(ulong[]);
                v = typeof(string[]);
            }

            v = typeof(Dictionary<object, object>);

            // map<char,>
            {
                v = typeof(Dictionary<char, object>);
                v = typeof(Dictionary<char, char>);
                v = typeof(Dictionary<char, short>);
                v = typeof(Dictionary<char, ushort>);
                v = typeof(Dictionary<char, int>);
                v = typeof(Dictionary<char, uint>);
                v = typeof(Dictionary<char, long>);
                v = typeof(Dictionary<char, ulong>);
                v = typeof(Dictionary<char, string>);
            }
            // map<short,>
            {
                v = typeof(Dictionary<short, object>);
                v = typeof(Dictionary<short, char>);
                v = typeof(Dictionary<short, short>);
                v = typeof(Dictionary<short, ushort>);
                v = typeof(Dictionary<short, int>);
                v = typeof(Dictionary<short, uint>);
                v = typeof(Dictionary<short, long>);
                v = typeof(Dictionary<short, ulong>);
                v = typeof(Dictionary<short, string>);
            }
            // map<ushort,>
            {
                v = typeof(Dictionary<ushort, object>);
                v = typeof(Dictionary<ushort, char>);
                v = typeof(Dictionary<ushort, short>);
                v = typeof(Dictionary<ushort, ushort>);
                v = typeof(Dictionary<ushort, int>);
                v = typeof(Dictionary<ushort, uint>);
                v = typeof(Dictionary<ushort, long>);
                v = typeof(Dictionary<ushort, ulong>);
                v = typeof(Dictionary<ushort, string>);
            }
            // map<int,>
            {
                v = typeof(Dictionary<int, object>);
                v = typeof(Dictionary<int, char>);
                v = typeof(Dictionary<int, short>);
                v = typeof(Dictionary<int, ushort>);
                v = typeof(Dictionary<int, int>);
                v = typeof(Dictionary<int, uint>);
                v = typeof(Dictionary<int, long>);
                v = typeof(Dictionary<int, ulong>);
                v = typeof(Dictionary<int, string>);
            }
            // map<uint,>
            {
                v = typeof(Dictionary<uint, object>);
                v = typeof(Dictionary<uint, char>);
                v = typeof(Dictionary<uint, short>);
                v = typeof(Dictionary<uint, ushort>);
                v = typeof(Dictionary<uint, int>);
                v = typeof(Dictionary<uint, uint>);
                v = typeof(Dictionary<uint, long>);
                v = typeof(Dictionary<uint, ulong>);
                v = typeof(Dictionary<uint, string>);
            }
            // map<long,>
            {
                v = typeof(Dictionary<long, object>);
                v = typeof(Dictionary<long, char>);
                v = typeof(Dictionary<long, short>);
                v = typeof(Dictionary<long, ushort>);
                v = typeof(Dictionary<long, int>);
                v = typeof(Dictionary<long, uint>);
                v = typeof(Dictionary<long, long>);
                v = typeof(Dictionary<long, ulong>);
                v = typeof(Dictionary<long, string>);
            }
            // map<ulong,>
            {
                v = typeof(Dictionary<ulong, object>);
                v = typeof(Dictionary<ulong, char>);
                v = typeof(Dictionary<ulong, short>);
                v = typeof(Dictionary<ulong, ushort>);
                v = typeof(Dictionary<ulong, int>);
                v = typeof(Dictionary<ulong, uint>);
                v = typeof(Dictionary<ulong, long>);
                v = typeof(Dictionary<ulong, ulong>);
                v = typeof(Dictionary<ulong, string>);
            }
            // map<string,>
            {
                v = typeof(Dictionary<string, object>);
                v = typeof(Dictionary<string, char>);
                v = typeof(Dictionary<string, short>);
                v = typeof(Dictionary<string, ushort>);
                v = typeof(Dictionary<string, int>);
                v = typeof(Dictionary<string, uint>);
                v = typeof(Dictionary<string, long>);
                v = typeof(Dictionary<string, ulong>);
                v = typeof(Dictionary<string, string>);
            }
        }

        public static FieldInfo GetField(System.Type type, string name)
        {
            if (type == null)
                return null;

            var info = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (info != null)
                return info;

            return GetField(type.BaseType, name);
        }

        public static MethodInfo GetMethod(System.Type type, string name)
        {
            if (type == null)
                return null;

            var info = type.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (info != null)
                return info;

            return GetMethod(type.BaseType, name);
        }

        public static PropertyInfo GetProperty(System.Type type, string name)
        {
            if (type == null)
                return null;

            var info = type.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            if (info != null)
                return info;

            return GetProperty(type.BaseType, name);
        }

        static void Reg<T>()
        {
            var type = typeof(T);
            AllTypesByFullName[type.FullName] = type;
        }
        
        static ConstructorInfo GetConstructor(System.Type instanceType, System.Type param)
        {
            while (param != null)
            {
                var ctor = instanceType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new System.Type[] { param }, null);
                if (ctor != null)
                    return ctor;

                param = param.BaseType;
            }

            return null;
        }

        public static object CreateInstaince(System.Type instanceType, object parent)
        {
            object instance = null;
            ConstructorInfo ctor = null;

            if (parent != null)
            {
                var type = parent.GetType();

                // 创建实例出来
                ctor = GetConstructor(instanceType, type);
                if (ctor != null)
                {
                    try
                    {
                        instance = ctor.Invoke(new object[] { parent });
                    }
                    catch (System.Exception ex)
                    {
                        Debuger.ErrorLog("type:{0}", instanceType.FullName);
                        Debuger.LogException(ex);
                        throw ex;
                    }

                    return instance;
                }
            }

            ctor = instanceType.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new System.Type[] { }, null);
            if (ctor != null)
            {
                try
                {
                    instance = ctor.Invoke(new object[] { });
                }
                catch (System.Exception ex)
                {
                    Debuger.ErrorLog("type:{0}", instanceType.FullName);
                    Debuger.LogException(ex);
                    throw ex;
                }
            }
            else
            {
                Debuger.ErrorLog("{0}: not find Constructor!", instanceType.Name);
            }

            return instance;
        }

        static Help()
        {
#if USE_HOT
#if UNITY_EDITOR
            var iltypes = ILEditor.appdomain.LoadedTypes;
            foreach (var itor in iltypes)
#else
            foreach (var itor in xys.App.my.appdomain.LoadedTypes)
#endif
            {
                AllTypesByFullName.Add(itor.Key, itor.Value.ReflectionType);
            }

            Reg<int>();
            Reg<uint>();
            Reg<sbyte>();
            Reg<byte>();
            Reg<short>();
            Reg<ushort>();
            Reg<long>();
            Reg<ulong>();
            Reg<string>();
            Reg<float>();
            Reg<double>();
#endif
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            //UnityEngine.Debug.LogFormat("assemblies:{0}", assemblies.Length);
            foreach (var assembly in assemblies)
            {
                //UnityEngine.Debug.LogFormat("assembly:{0}", assembly.FullName);
                if (assembly.FullName.StartsWith("Assembly-CSharp")
                    //|| assembly.ManifestModule.Name == ("UnityEngine.UI.dll") 
                    //|| assembly.ManifestModule.Name == ("UnityEngine.dll")
                    )
                {
                    var types = assembly.GetTypes();
                    foreach (var type in types)
                    {
                        if (type.FullName.StartsWith("<"))
                            continue;

                        System.Type t = null;
                        if (AllTypesByFullName.TryGetValue(type.FullName, out t))
                        {
#if USE_HOT
                            if (t != type)
                            {
                                if ((t is ILRuntimeWrapperType) && ((ILRuntimeWrapperType)t).RealType == type)
                                    continue;

                                Debuger.ErrorLog("error:{0}", type.FullName);
                            }
#endif
                            continue;
                        }
                        AllTypesByFullName.Add(type.FullName, type);
                    }
                }
            }

            BaseTypes.Add(typeof(int));
            BaseTypes.Add(typeof(uint));
            BaseTypes.Add(typeof(sbyte));
            BaseTypes.Add(typeof(byte));
            BaseTypes.Add(typeof(short));
            BaseTypes.Add(typeof(ushort));
            BaseTypes.Add(typeof(long));
            BaseTypes.Add(typeof(ulong));
            BaseTypes.Add(typeof(string));
            BaseTypes.Add(typeof(float));
            BaseTypes.Add(typeof(double));

            AllTypesByFullName.Add("int", typeof(int));
            AllTypesByFullName.Add("uint", typeof(uint));
            AllTypesByFullName.Add("sbyte", typeof(sbyte));
            AllTypesByFullName.Add("byte", typeof(byte));
            AllTypesByFullName.Add("short", typeof(short));
            AllTypesByFullName.Add("ushort", typeof(ushort));
            AllTypesByFullName.Add("long", typeof(long));
            AllTypesByFullName.Add("ulong", typeof(ulong));
            AllTypesByFullName.Add("string", typeof(string));
            AllTypesByFullName.Add("float", typeof(float));
            AllTypesByFullName.Add("double", typeof(double));
        }

        static HashSet<System.Type> BaseTypes = new HashSet<System.Type>();

        public static bool IsBaseType(System.Type type)
        {
            return BaseTypes.Contains(type);
        }

        static Dictionary<string, System.Type> AllTypesByFullName = new Dictionary<string, System.Type>();

        public static System.Type GetTypeByFullName(string name)
        {
            System.Type t = null;
            if (AllTypesByFullName.TryGetValue(name, out t))
                return t;

            Debuger.ErrorLog("type:{0} not find!", name);
            return null;
        }

        public static System.Type GetType(string name)
        {
            return GetTypeByFullName(name);
        }

        public static List<System.Type> GetCustomAttributesType(System.Type type)
        {
            List<System.Type> types = new List<System.Type>();
            foreach (var itor in AllTypesByFullName)
            {
                if (itor.Value.IsAbstract)
                    continue;
                if (itor.Value.IsInterface)
                    continue;

                if (HasCustomAttributes(itor.Value, type))
                {
                    types.Add(itor.Value);
                }
            }

            return types;
        }

        // 得到继承type的类型
        public static List<System.Type> GetBaseType(string baseTypeFullName)
        {
            List<System.Type> types = new List<System.Type>();
            foreach (var itor in AllTypesByFullName)
            {
                if (itor.Value.IsAbstract)
                    continue;
                if (itor.Value.IsInterface)
                    continue;
                if (itor.Value.BaseType != null && itor.Value.BaseType.FullName == baseTypeFullName)
                    types.Add(itor.Value);
            }

            return types;
        }

        static bool HasCustomAttributes(System.Type type, System.Type customAtt)
        {
            if (type == null)
                return false;

            bool has = type.GetCustomAttributes(customAtt, true).Length == 0 ? false : true;
#if USE_HOT
            if (has)
                return true;

            ILRuntimeType rt = type as ILRuntimeType;
            if (rt == null)
                return false;

            try
            {
                return HasCustomAttributes(type.BaseType, customAtt);
            }
            catch (System.Exception ex)
            {
                return false;
            }
#else
            return has;
#endif
        }

        static void GetSerializeField(System.Type type, List<FieldInfo> infos)
        {
#if USE_HOT
            infos.AddRange(GetSerializeFieldImp(type));
            if (type.BaseType != null)
                GetSerializeField(type.BaseType, infos);
#else
            infos.AddRange(GetSerializeFieldImp(type));
#endif
        }

        static public List<FieldInfo> GetSerializeFieldImp(System.Type type)
        {
#if USE_HOT
            bool isILType = false;
            HashSet<string> noPubs = new HashSet<string>();
            HashSet<string> allfields = new HashSet<string>();
            if (type is ILRuntimeType)
            {
                isILType = true;
                type = ((ILRuntimeType)type).ILType.ReflectionType;
                TypeDefinition typeDef = ((ILRuntimeType)type).ILType.TypeDefinition;
                var fields = typeDef.Fields;
                for (int i = 0; i < fields.Count; ++i)
                {
                    var field = fields[i];
                    allfields.Add(field.Name);
                    if (!field.IsPublic)
                        noPubs.Add(field.Name);
                }
            }
#endif
            List<FieldInfo> fieldinfos = new List<FieldInfo>();
            {
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                for (int i = 0; i < fields.Length; ++i)
                {
                    var field = fields[i];
#if USE_HOT
                    if (isILType && !allfields.Contains(field.Name))
                        continue;
#endif
                    bool isPublic = field.IsPublic;
#if USE_HOT
                    if (isPublic)
                    {
                        if (isILType && noPubs.Contains(field.Name))
                            isPublic = false;
                    }
#endif
                    if (isPublic || (field.GetCustomAttributes(typeof(UnityEngine.SerializeField), false).Length != 0))
                    {
                        if (field.FieldType.IsArray)
                        {
                            if (!IsBaseType(field.FieldType.GetElementType()))
                                continue;
                        }

                        if (BaseTypes.Contains(field.FieldType))
                        {
                            // 基础类型
                            fieldinfos.Add(field);
                        }
                        else
                        {
                            if (
#if USE_HOT
                                (field.FieldType is ILRuntimeType && ((ILRuntimeType)field.FieldType).ILType.TypeDefinition.IsSerializable) ||
#endif
                                field.FieldType.IsSerializable || isType(field.FieldType, typeof(UnityEngine.Object)))
                                fieldinfos.Add(field);
                        }
                    }
                }
            }

            return fieldinfos;
        }

        static public List<FieldInfo> GetSerializeField(System.Type type)
        {
            List<FieldInfo> infos = new List<FieldInfo>();
            GetSerializeField(type, infos);
            return infos;
        }

        static public List<FieldInfo> GetSerializeField(object obj)
        {
#if USE_HOT
            if (obj is ILTypeInstance)
            {
                ILTypeInstance ili = (ILTypeInstance)obj;
                return GetSerializeField(ili.Type.ReflectionType);
            }
#endif
            return GetSerializeField(obj.GetType());
        }

        public static bool isType(System.Type src, System.Type baseType)
        {
            var bt = src.BaseType;
            if (bt == null)
                return false;

            if (bt == baseType)
                return true;

            return isType(bt, baseType);
        }

        public static object Create(System.Type type)
        {
            if (type.IsArray)
            {
                var elementType = GetType(type.FullName.Substring(0, type.FullName.Length - 2));
                return System.Array.CreateInstance(elementType, 0);
            }
            else if (type.Name == "String")
            {
                return string.Empty;
            }
            else
            {
                var constructor = type.GetConstructor(new System.Type[] { });
                if (constructor == null)
                {
                    UnityEngine.Debug.LogErrorFormat("type:{0} not GetConstructor", type.Name);
                    return null;
                }
                return constructor.Invoke(null);
            }
        }
    }
}

