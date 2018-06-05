using System.Reflection;
using System.Collections.Generic;

namespace xys.IL
{
    class AnyType : ITypeSerialize
    {
        public AnyType(System.Type type, List<FieldInfo> fieldInfos)
        {
            this.type = type;
            this.fieldInfos = fieldInfos;
        }

        System.Type type;
        List<FieldInfo> fieldInfos;

        public void WriteTo(object parent, FieldInfo info, MonoStream ms)
        {
            object current = info.GetValue(parent);
            if (current == null)
                return;

            JSONObject j = new JSONObject();
            ms.json.put(info.Name, j);
            ms = new MonoStream(j, ms.objs);

            for (int i = 0; i < fieldInfos.Count; ++i)
            {
                var field = fieldInfos[i];
                MonoSerialize.Get(field.FieldType).WriteTo(current, field, ms);
            }
        }

        public void MergeFrom(object parent, FieldInfo info, MonoStream ms)
        {
            if (ms.json.has(info.Name))
            {
                JSONObject json = ms.json.getJSONObject(info.Name);
                object current = info.GetValue(parent);
                if (current == null)
                {
                    current = Help.Create(info.FieldType);
                    info.SetValue(parent, current);
                }

                for (int i = 0; i < fieldInfos.Count; ++i)
                {
                    var field = fieldInfos[i];
                    MonoSerialize.Get(field.FieldType).MergeFrom(current, field, new MonoStream(json, ms.objs));
                }
            }
        }
    }
}