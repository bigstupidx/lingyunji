using System.Text;
using Config;

public class GlobalSymbol
{
    static ParserSymbol instance = new ParserSymbol();

    public static string To(string text, object obj = null)
    {
        return instance.To(text, obj);
    }

    // 只支持*通配符
    static GlobalSymbol()
    {
        instance.AddKey("[att_*]", ParserAttribute);
        //主角昵称
        instance.AddKey("$name", ParserRoleName);
        //npc对主角称谓解析
        instance.AddKey("[sex*]", ParserNpcCall);
        // 主角加入的氏族名称
        instance.AddKey("[guild]", ParsePlayerGuildName);
    }

    //解释属性 obj为AttributeTableElement
    static string ParserAttribute(string symbol, object obj)
    {
        return null;
        //int beginIndex = 5;
        //string subSymbol = symbol.Substring(beginIndex, symbol.Length - beginIndex - 1);
        //int attId = AttributeDefinePrototypeManage.instance.GetIdByName(subSymbol);
        //if (attId == -1)
        //{
        //    Debuger.LogError("GlobalSymbol 属性解释出错");
        //    return "";
        //}
        //AttributeTableElement data = obj as AttributeTableElement;
        //if (data == null)
        //    return "";
        //CheckTableDataManage check = CheckTableDataManage.GetCheckType(typeof (AttributeTable));
        //BaseTable.DataType dataType = check.GetDataType(attId);
        //if (dataType == BaseTable.DataType.Float)
        //{
        //    return EquipManage.Instance.GetValueText(attId, data.GetValueById(attId)*100);
        //}
        //else
        //{
        //    return EquipManage.Instance.GetValueText(attId, data.GetValueById(attId));
        //}
    }

    //解释主角名字
    static string ParserRoleName(string symbol, object obj)
    {
        //if(null != Battle.ObjectManage.Instance.mainPlayer && null != Battle.ObjectManage.Instance.mainPlayer.m_objectData && 
        //    null != Battle.ObjectManage.Instance.mainPlayer.m_objectData.GetTableElement(TableManage.Player))
        //{
        //    string name = Battle.ObjectManage.Instance.mainPlayer.m_objectData.GetTableElement(TableManage.Player).GetStr(PlayerTable.sName);
        //    return name;
        //}
        return "";
    }

    //解析npc对主角的称谓
    static string ParserNpcCall(string symbol, object obj)
    {
        //string rtnString = symbol.Replace("[sex", "");
        //rtnString = rtnString.Replace("]", "");
        //string[] appellations = rtnString.Split(':');
        //if (null != Battle.ObjectManage.Instance.mainPlayer && null != Battle.ObjectManage.Instance.mainPlayer.m_objectData &&
        //    null != Battle.ObjectManage.Instance.mainPlayer.m_objectData.GetTableElement(TableManage.Player) && 2 == appellations.Length)
        //{
        //    int gender = Battle.ObjectManage.Instance.mainPlayer.m_objectData.JobSex;
        //    string name = (1 == gender) ? appellations[0] : appellations[1];
        //    return name;
        //}
        return "";
    }

    // 主角加入的氏族名称
    static string ParsePlayerGuildName(string symbol, object obj)
    {
        //if(null == Battle.ObjectManage.Instance.mainPlayer)
        return string.Empty;
        //return TableManage.GetMeElement(TableManage.Player).GetStr(PlayerTable.sGuildName);
    }

    // =========================================

    static StringBuilder s_sb = new StringBuilder();

    static string GetColour(char c)
    {
        switch (c)
        {
            case 'R': return "FF0000";
            case 'G': return "00FF00";
            case 'B': return "0000FF";
            case 'K': return "000000";
            case 'Y': return "FFEA04";
            case 'W': return "FFFFFF";
        }

        return "";
    }

    static public string ToUT(string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        s_sb.Length = 0;
        bool isfunchar = false; // 是否遇到功能字符
        bool ishascolor = false; // 前面是否有颜色解析标识
        int startpos = 0;
        for (; startpos < text.Length;)
        {
            char c = text[startpos];
            if (!isfunchar)
            {
                if (c == '#')
                {
                    isfunchar = true;
                    ++startpos;
                }
                else
                {
                    s_sb.Append(c);
                    ++startpos;
                }
            }
            else
            {
                isfunchar = false;
                switch (c)
                {
                    case 'R':
                    case 'G':
                    case 'B':
                    case 'K':
                    case 'W':
                        {
                            if (ishascolor)
                            {
                                s_sb.Append("</color>");
                            }

                            s_sb.AppendFormat("<color=#{0}>", GetColour(c));
                            ishascolor = true;
                            ++startpos;
                        }
                        break;
                    case '[':
                        {
                            // 颜色
                            int endpos = text.IndexOf(']', startpos);
                            if (endpos == -1)
                            {
                                // 错误的颜色格式，忽略
                                s_sb.Append(c);
                                ++startpos;
                            }
                            else
                            {
                                string colorname = text.Substring(startpos + 1, endpos - startpos - 1);
                                if (ishascolor)
                                {
                                    s_sb.Append("</color>");
                                }

                                s_sb.AppendFormat("<color=#{0}>", ColorConfig.GetIndexByName(colorname));
                                ishascolor = true;
                                startpos = endpos + 1;
                            }
                        }
                        break;
                    case 'n':
                        {
                            if (ishascolor)
                            {
                                s_sb.Append("</color>");
                            }

                            ishascolor = false;
                            ++startpos;
                        }
                        break;
                    case 'r':
                        {
                            s_sb.Append('\n');
                            ++startpos;
                        }
                        break;
                    default:
                        {
                            s_sb.Append(c);
                            ++startpos;
                        }
                        break;
                }
            }
        }

        if (ishascolor)
        {
            s_sb.Append("</color>");
        }

        text = s_sb.ToString();
        s_sb.Length = 0;
        return text;
    }

    /// <summary>
    /// 这叫科学计数？？？？
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    static public string ToBef(double value)
    {
        string valueStr = value.ToString("C", System.Globalization.CultureInfo.CreateSpecificCulture("en-US"));
        return valueStr.Replace("$", string.Empty).Replace(".00", string.Empty);
    }

    static public string ToNum(int index)
    {
        switch(index)
        {
            case 1:
                return "壹";
            case 2:
                return "贰";
            case 3:
                return "叁";
            case 4:
                return "肆";
            case 5:
                return "伍";
            case 6:
                return "陆";
            case 7:
                return "柒";
            case 8:
                return "捌";
            case 9:
                return "镹";
            case 0:
                return "零";
        }
        return string.Empty;
    }
    //#if UNITY_EDITOR
    //    [UnityEditor.MenuItem("Assets/Test/Symbol")]
    //    static void TestSymbol()
    //    {
    //        UnityEditor.AssetImporter ai = UnityEditor.AssetImporter.GetAtPath(UnityEditor.AssetDatabase.GetAssetPath(UnityEditor.Selection.activeObject));
    //        Debug.LogFormat("type:{0}", ai.GetType().Name);
    //    }
    //#endif
}