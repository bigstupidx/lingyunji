#if !COM_SERVER
using System.Collections.Generic;

namespace Config
{
    public partial class ColorConfig
    {
        //根据颜色名字获取颜色码
        public static string GetIndexByName(string name)
        {
            string code;
            if (s_nameToCodes.TryGetValue(name, out code))
                return code;
            else
                return "";
        }

        static Dictionary<string, string> s_nameToCodes;

        static void OnLoadEnd()
        {
            s_nameToCodes = new Dictionary<string, string>();

            foreach (var p in DataList)
            {
                UnityEngine.Color c;
                if (WXB.Tools.ParseColor(p.Value.colorCode, 0, out c))
                {
                    WXB.ColorConst.Set(p.Value.colorName, c);
                }

                s_nameToCodes.Add(p.Value.colorName, p.Value.colorCode);
            }
        }
    }
}
#endif