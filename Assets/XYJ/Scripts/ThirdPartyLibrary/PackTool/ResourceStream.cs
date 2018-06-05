using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    // 资源类型
    public enum ResType
    {
        Builtin, // 系统级
        Resources, // Resources目录下
        Asset, // 普通资源
        Empty, // 无资源
    }

    public class ResourceStream
    {
        public ResType type;
        public int hashcode;
        public string path;

        public string Key
        {
            get
            {
                switch (type)
                {
                case ResType.Asset: return StringHashCode.Get(hashcode);
                case ResType.Builtin: return StringHashCode.Get(hashcode);
                case ResType.Resources: return string.Format(":{0}", path);
                case ResType.Empty: return string.Empty;
                }

                Debuger.ErrorLog("ResourceStream Key error! type:{0}", type);
                return string.Empty;
            }
        }

        public void Reader(BinaryReader reader)
        {
            type = (ResType)reader.ReadByte();
            switch(type)
            {
            case ResType.Asset:
            case ResType.Builtin:
                hashcode = reader.ReadInt32();
                break;
            case ResType.Resources:
                path = reader.ReadString();
                break;
            case ResType.Empty:
                break;
            default:
                Debuger.ErrorLog("ResourceStream Reader error! type:{0}", type);
                break;
            }
        }

        public bool GetRes()
        {
            return type == ResType.Empty ? false : true;
        }

#if UNITY_EDITOR
        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)type);
            switch (type)
            {
            case ResType.Empty:
                break;
            case ResType.Asset:
            case ResType.Builtin:
                writer.Write(hashcode);
                break;
            case ResType.Resources:
                writer.Write(path);
                break;
            default:
                Debuger.ErrorLog("ResourceStream Write error! type:{0}", type);
                break;
            }
        }

        // 是否内部资源
        public static bool IsInteriorResources(string path)
        {
            path.CopyTo(0, temp_path, 0, path.Length);
            ToLower(temp_path, 0, path.Length);
            Relpace(temp_path, 0, path.Length, '\\', '/');

            int max = path.Length - resources_path.Length;
            if (max > 0)
            {
                for (int i = 0; i < max; ++i)
                {
                    if (IsSame(temp_path, i, resources_path))
                        return true;
                }
            }

            if (!IsSame(temp_path, 0, assets_path))
                return true;

            return false;
        }

        static char[] temp_path = new char[1024];
        const string resources_path = "/resources/";
        const string assets_path = "assets/";

        public static ResourceStream GetResType(Object obj)
        {
            ResourceStream rs = new ResourceStream();
            if (obj == null)
            {
                rs.type = ResType.Empty;
                return rs;
            }

            string path = UnityEditor.AssetDatabase.GetAssetPath(obj).Replace("__copy__/", "");
            if (string.IsNullOrEmpty(path))
            {
                rs.type = ResType.Empty;
                return rs;
            }

            path.CopyTo(0, temp_path, 0, path.Length);
            ToLower(temp_path, 0, path.Length);
            Relpace(temp_path, 0, path.Length, '\\', '/');

            if (path.Length > assets_path.Length && IsSame(temp_path, 0, assets_path))
            {
                int max = path.Length - resources_path.Length;
                if (max < 0)
                {
                    rs.type = ResType.Asset;
                    rs.path = path;
                    rs.hashcode = path.Substring(7).GetHashCode();
                    return rs;
                }

                for (int i = 0; i < max; ++i)
                {
                    if (IsSame(temp_path, i, resources_path))
                    {
                        rs.type = ResType.Resources;
                        rs.path = path.Substring(i + resources_path.Length, path.Length - i - resources_path.Length);
                        return rs;
                    }
                }

                rs.type = ResType.Asset;
                rs.path = path;
                rs.hashcode = path.Substring(7).GetHashCode();
                return rs;
            }
            else
            {
                rs.type = ResType.Builtin;
                rs.path = obj.name;
                rs.hashcode = rs.path.GetHashCode();
                return rs; // 系统级的资源
            }
        }

        static bool IsSame(char[] c, int start, string str)
        {
            for (int i = 0; i < str.Length; ++i)
            {
                if (str[i] != c[start + i])
                    return false;
            }

            return true;
        }

        static void Relpace(char[] c, int start, int lenght, char s, char r)
        {
            for (int i = start; i < lenght; ++i)
            {
                if (temp_path[i] == s)
                    temp_path[i] = 'r';
            }
        }

        static void ToLower(char[] c, int start, int lenght)
        {
            for (int i = start; i < lenght; ++i)
                temp_path[i] = char.ToLower(temp_path[i]);
        }
#endif
    }
}
