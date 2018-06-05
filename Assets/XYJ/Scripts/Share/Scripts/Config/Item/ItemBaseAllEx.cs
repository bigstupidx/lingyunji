using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    /// <summary>
    /// �������ͣ�ö��ֵ������idһ��
    /// </summary>
    public enum ItemIDType
    {
        SilverShell = 1,
        GoldShell = 2,
        JasperJade = 3,
        Exp = 4,
    }

    public partial class ItemBaseAll
    {
        static void OnLoadEnd()
        {
            foreach (var itor in GetAll())
            {
                if (itor.Value.stackNum <= 0)
                    itor.Value.stackNum = 1;
            }
        }

        /// <summary>
        /// ���ݵ�����������Ϣ
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static ItemBase Get(ItemIDType type)
        {
            return Get((int)type);
        }

        /// <summary>
        /// �������ͻ�ȡͼ��
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetIcon(ItemIDType type)
        {
            ItemBase item = Get((int)type);
            if (item != null)
                return item.icon;
            else
                return string.Empty;
        }
    }
}

