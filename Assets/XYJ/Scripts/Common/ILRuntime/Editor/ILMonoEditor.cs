using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
#if USE_HOT
using ILRuntime.Runtime.Intepreter;
#endif


namespace xys.IL.Editor
{
    public class ILMonoEditor
    {
        [UnityEditor.MenuItem("Assets/IL/xys.Hot空间添加宏")]
        static void AddMarco()
        {
            XTools.Utility.ForEachSelect(
                (AssetImporter ai) => 
                {
                    string assetPath = ai.assetPath;
                    var text = System.IO.File.ReadAllText(assetPath);
                    if (text.Contains("namespace xys.hot"))
                    {
                        if (text.StartsWith("#if !USE_HOT"))
                            return;

                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.AppendLine("#if !USE_HOT");
                        sb.AppendLine(text);
                        sb.Append("#endif");

                        System.IO.File.WriteAllText(assetPath, sb.ToString());
                    }
                },
                (string assetPath, string path) => { return assetPath.EndsWith(".cs", true, null); });
        }

        FieldInfo typeNameField;
        FieldInfo instanceField;
        FieldInfo objsField;
        FieldInfo jsonField;

        object target;

        List<System.Type> allTypes;

        static FieldInfo GetField(System.Type type, string name, BindingFlags flags)
        {
            var info = type.GetField(name, flags);
            if (info != null)
                return info;

            if (type.BaseType != null)
                return GetField(type.BaseType, name, flags);

            return null;
        }

        public void InitByBaseType(object target, string baseTypeFullName)
        {
            Init(target);

            allTypes = Help.GetBaseType(baseTypeFullName);
        }

        public void Init(object target)
        {
            this.target = target;
            System.Type type = target.GetType();
            typeNameField = GetField(type, "typeName", BindingFlags.Instance | BindingFlags.NonPublic);
            instanceField = GetField(type, "instance", BindingFlags.Instance | BindingFlags.NonPublic);
            objsField = GetField(type, "objs", BindingFlags.Instance | BindingFlags.NonPublic);
            jsonField = GetField(type, "json", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        public void InitByAttribute(object target, System.Type attributeType)
        {
            Init(target);

            allTypes = Help.GetCustomAttributesType(attributeType);
        }

        public void OnGUI(string newTypename)
        {
            string typeName = (string)typeNameField.GetValue(target);
            if (newTypename != typeName)
            {
                typeName = newTypename;
                typeNameField.SetValue(target, typeName);
                instanceField.SetValue(target, null);
                if (target is UnityEngine.Object)
                    EditorUtility.SetDirty((UnityEngine.Object)target);
            }

            if (string.IsNullOrEmpty(typeName))
                return;

            var type = Help.GetType(newTypename);
            if (type == null)
            {
                EditorGUILayout.LabelField(string.Format("类型:{0},查找失败!", newTypename));
                return;
            }

            object instance = instanceField.GetValue(target);
            if (instance == null
#if USE_HOT
                || (instance is ILTypeInstance && ((ILTypeInstance)instance).Type.FullName != newTypename) 
                || (!(instance is ILTypeInstance) && instance.GetType().FullName != newTypename)
#endif
                )
            {
                var ctor = Help.GetType(newTypename).GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, new System.Type[0] { } , null);
                if (ctor == null)
                {
                    Debuger.ErrorLog("type:{0} not find ctor!", newTypename);
                    instance = null;
                    return;
                }

                instance = ctor.Invoke(null);
                instanceField.SetValue(target, instance);
                if (target is UnityEngine.Object)
                    EditorUtility.SetDirty((UnityEngine.Object)target);

                JSONObject json = new JSONObject((string)jsonField.GetValue(target));
                var objs = (List<UnityEngine.Object>)objsField.GetValue(target);
                MonoSerialize.MergeFrom(instance, new MonoStream(json, objs));
            }

            if (xys.Editor.TypeEditor.OnGUI(instance))
            {
                var ms = IL.MonoSerialize.WriteTo(instance);
                objsField.SetValue(target, ms.objs);
                jsonField.SetValue(target, ms.json.toString());

                if (target is UnityEngine.Object)
                    EditorUtility.SetDirty((UnityEngine.Object)target);
            }
        }

        string m_searchName = string.Empty;

        public void OnGUI()
        {
            m_searchName = EditorGUILayout.TextField("输入搜索：", m_searchName);
            string typeName = (string)typeNameField.GetValue(target);
            string newTypename = xys.Editor.GUIHelp.StringPopupT("typeName", typeName, allTypes, (System.Type t) => { return t.FullName; }, m_searchName);

            OnGUI(newTypename);
        }
    }
}