using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace xys.Editor
{
    public class AnyType : ITypeGUI
    {
        public AnyType(System.Type type, List<FieldInfo> infos)
        {
            this.type = type;
            this.infos = infos;

            foreach (var d in infos)
            {
                if (d.Name == "root" && d.FieldType == typeof(GameObject))
                {
                    copyRoot = d;
                    break;
                }
            }

            bool isR = false;
            if (copyRoot != null)
            {
                foreach (var d in infos)
                {
                    if (d == copyRoot)
                        continue;

                    if (d.FieldType == typeof(GameObject) ||
                        XTools.Utility.isInherited(d.FieldType, typeof(Component)))
                    {
                        isR = true;
                    }
                }
            }

            if (!isR)
                copyRoot = null;
        }

        FieldInfo copyRoot;
        System.Type type;
        List<FieldInfo> infos;

        public object OnGUI(string label, object value, System.Type type, out bool isDirty)
        {
            isDirty = false;
            for (int i = 0; i < infos.Count; ++i)
            {
                var field = infos[i];
                object v = field.GetValue(value);
                if (v == null && (!IL.Help.isType(field.FieldType, typeof(Object))))
                {
                    v = IL.Help.Create(field.FieldType);
                    field.SetValue(value, v);
                    isDirty = true;
                }

                isDirty |= TypeEditor.Get(field.FieldType).OnGUI(value, field);
            }

            return value;
        }

        Dictionary<int, bool> isFoldouts = new Dictionary<int, bool>();

        public bool OnGUI(object parent, FieldInfo info)
        {
            using (new IndentLevel())
            {
                bool isDirty = false;

                object current = info.GetValue(parent);
                if (current == null)
                {
                    current = IL.Help.Create(info.FieldType);
                    info.SetValue(parent, current);
                    isDirty = true;
                }

                if (current != null)
                {
                    int hashcode = current.GetHashCode();
                    var isFoldout = false;
                    if (!isFoldouts.TryGetValue(hashcode, out isFoldout))
                        isFoldouts.Add(hashcode, isFoldout);

                    UnityEditor.EditorGUILayout.BeginHorizontal();
                    isFoldout = UnityEditor.EditorGUILayout.Foldout(isFoldout, info.Name);                    
                    if (copyRoot != null && GUILayout.Button("复制初始化"))
                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        GameObject root = copyRoot.GetValue(current) as GameObject;
                        if (root != null)
                        {
                            sb.AppendLine("var rt = root.transform;");
                            sb.AppendLine("Transform rtm = null;");
                            foreach (var d in infos)
                            {
                                if (d.Name == "root")
                                    continue;

                                if (XTools.Utility.isInherited(d.FieldType, typeof(Component)))
                                {
                                    Component v = (Component)d.GetValue(current);
                                    if (v != null)
                                    {
                                        if (d.FieldType == typeof(Transform))
                                        {
                                            sb.AppendFormat("{0} = (rtm = rt.Find(\"{0}\")) == null ? null : rtm;", d.Name, XTools.Utility.GetPath(root, v.gameObject));
                                        }
                                        else if (d.FieldType == typeof(RectTransform))
                                        {
                                            sb.AppendFormat("{0} = (rtm = rt.Find(\"{0}\")) == null ? null : (RectTransform)rtm;", d.Name, XTools.Utility.GetPath(root, v.gameObject));
                                        }
                                        else
                                        {
                                            sb.AppendFormat("{0} = (rtm = rt.Find(\"{1}\")) == null ? null : rtm.GetComponent<{2}>();", d.Name, XTools.Utility.GetPath(root, v.gameObject), d.FieldType.Name);
                                        }

                                        sb.AppendLine();
                                    }
                                }
                                else if (d.FieldType == typeof(GameObject))
                                {
                                    GameObject v = (GameObject)d.GetValue(current);
                                    if (v != null)
                                    {
                                        sb.AppendFormat("{0} = (rtm = rt.Find(\"{1}\")) == null ? null : rtm.gameObject;", d.Name, XTools.Utility.GetPath(root, v.gameObject), d.FieldType.Name);
                                        sb.AppendLine();
                                    }
                                }
                            }

                            GUIUtility.systemCopyBuffer = sb.ToString();
                            Debug.Log(sb.ToString());
                        }
                    }
                    UnityEditor.EditorGUILayout.EndHorizontal();
                    isFoldouts[hashcode] = isFoldout;
                    if (!isFoldout)
                        return false;
                }

                bool isd = false;
                OnGUI(info.Name, current, info.FieldType, out isd);
                return isDirty | isd;
            }
        }
    }
}
