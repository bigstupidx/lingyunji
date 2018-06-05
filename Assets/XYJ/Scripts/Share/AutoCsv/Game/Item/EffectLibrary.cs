// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class EffectLibrary 
    {
        static Dictionary<int, EffectLibrary> DataList = new Dictionary<int, EffectLibrary>();

        static public Dictionary<int, EffectLibrary> GetAll()
        {
            return DataList;
        }

        static public EffectLibrary Get(int key)
        {
            EffectLibrary value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("EffectLibrary.Get({0}) not find!", key);
            return null;
        }



        // ID
        public int id { get; set; }

        // 名称
        public string name { get; set; }

        // 锋锐
        public EffectTable sFengRui { get; set; }

        // 宗法
        public EffectTable sZongFa { get; set; }

        // 挫锋
        public EffectTable sCuoFeng { get; set; }

        // 御法
        public EffectTable sYuFa { get; set; }

        // 不动如山
        public EffectTable sBuDongRuShan { get; set; }

        // 妙手
        public EffectTable sMiaoShou { get; set; }

        // 易愈
        public EffectTable sYiYu { get; set; }

        // 攻心
        public EffectTable sGongXin { get; set; }

        // 御心
        public EffectTable sYuXin { get; set; }

        // 灵逸
        public EffectTable sLingYi { get; set; }

        // 万象由心
        public EffectTable sWanXiangYouXin { get; set; }

        // 刚身
        public EffectTable sGangShen { get; set; }

        // 灭魂
        public EffectTable sMieHun { get; set; }

        // 御灵
        public EffectTable sYuLing { get; set; }

        // 困兽
        public EffectTable sKunShou { get; set; }

        // 噬魂
        public EffectTable sSheHun { get; set; }

        // 不灭
        public EffectTable sBuMie { get; set; }

        // 杀戮
        public EffectTable sShaLu { get; set; }

        // 止戈
        public EffectTable sZhiGe { get; set; }

        // 长生
        public EffectTable sChangSheng { get; set; }

        // 剑魂
        public EffectTable sJianPo { get; set; }

        // 气宗
        public EffectTable sQiZong { get; set; }

        // 原力
        public EffectTable sYuanLi { get; set; }

        // 空灵
        public EffectTable sKongLing { get; set; }

        // 坚韧
        public EffectTable sJianRen { get; set; }

        // 健强
        public EffectTable sJianQiang { get; set; }

        // 迅捷
        public EffectTable sXunJie { get; set; }

        // 若危
        public EffectTable sRuoWei { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(EffectLibrary);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(EffectLibrary);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<EffectLibrary> allDatas = new List<EffectLibrary>();

            {
                string file = "Item/EffectLibrary.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int name_index = reader.GetIndex("name");
                int sFengRui_index = reader.GetIndex("sFengRui");
                int sZongFa_index = reader.GetIndex("sZongFa");
                int sCuoFeng_index = reader.GetIndex("sCuoFeng");
                int sYuFa_index = reader.GetIndex("sYuFa");
                int sBuDongRuShan_index = reader.GetIndex("sBuDongRuShan");
                int sMiaoShou_index = reader.GetIndex("sMiaoShou");
                int sYiYu_index = reader.GetIndex("sYiYu");
                int sGongXin_index = reader.GetIndex("sGongXin");
                int sYuXin_index = reader.GetIndex("sYuXin");
                int sLingYi_index = reader.GetIndex("sLingYi");
                int sWanXiangYouXin_index = reader.GetIndex("sWanXiangYouXin");
                int sGangShen_index = reader.GetIndex("sGangShen");
                int sMieHun_index = reader.GetIndex("sMieHun");
                int sYuLing_index = reader.GetIndex("sYuLing");
                int sKunShou_index = reader.GetIndex("sKunShou");
                int sSheHun_index = reader.GetIndex("sSheHun");
                int sBuMie_index = reader.GetIndex("sBuMie");
                int sShaLu_index = reader.GetIndex("sShaLu");
                int sZhiGe_index = reader.GetIndex("sZhiGe");
                int sChangSheng_index = reader.GetIndex("sChangSheng");
                int sJianPo_index = reader.GetIndex("sJianPo");
                int sQiZong_index = reader.GetIndex("sQiZong");
                int sYuanLi_index = reader.GetIndex("sYuanLi");
                int sKongLing_index = reader.GetIndex("sKongLing");
                int sJianRen_index = reader.GetIndex("sJianRen");
                int sJianQiang_index = reader.GetIndex("sJianQiang");
                int sXunJie_index = reader.GetIndex("sXunJie");
                int sRuoWei_index = reader.GetIndex("sRuoWei");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        EffectLibrary data = new EffectLibrary();
						data.id = reader.getInt(i, id_index, 0);         
						data.name = reader.getStr(i, name_index);         
						data.sFengRui = EffectTable.InitConfig(reader.getStr(i, sFengRui_index));         
						data.sZongFa = EffectTable.InitConfig(reader.getStr(i, sZongFa_index));         
						data.sCuoFeng = EffectTable.InitConfig(reader.getStr(i, sCuoFeng_index));         
						data.sYuFa = EffectTable.InitConfig(reader.getStr(i, sYuFa_index));         
						data.sBuDongRuShan = EffectTable.InitConfig(reader.getStr(i, sBuDongRuShan_index));         
						data.sMiaoShou = EffectTable.InitConfig(reader.getStr(i, sMiaoShou_index));         
						data.sYiYu = EffectTable.InitConfig(reader.getStr(i, sYiYu_index));         
						data.sGongXin = EffectTable.InitConfig(reader.getStr(i, sGongXin_index));         
						data.sYuXin = EffectTable.InitConfig(reader.getStr(i, sYuXin_index));         
						data.sLingYi = EffectTable.InitConfig(reader.getStr(i, sLingYi_index));         
						data.sWanXiangYouXin = EffectTable.InitConfig(reader.getStr(i, sWanXiangYouXin_index));         
						data.sGangShen = EffectTable.InitConfig(reader.getStr(i, sGangShen_index));         
						data.sMieHun = EffectTable.InitConfig(reader.getStr(i, sMieHun_index));         
						data.sYuLing = EffectTable.InitConfig(reader.getStr(i, sYuLing_index));         
						data.sKunShou = EffectTable.InitConfig(reader.getStr(i, sKunShou_index));         
						data.sSheHun = EffectTable.InitConfig(reader.getStr(i, sSheHun_index));         
						data.sBuMie = EffectTable.InitConfig(reader.getStr(i, sBuMie_index));         
						data.sShaLu = EffectTable.InitConfig(reader.getStr(i, sShaLu_index));         
						data.sZhiGe = EffectTable.InitConfig(reader.getStr(i, sZhiGe_index));         
						data.sChangSheng = EffectTable.InitConfig(reader.getStr(i, sChangSheng_index));         
						data.sJianPo = EffectTable.InitConfig(reader.getStr(i, sJianPo_index));         
						data.sQiZong = EffectTable.InitConfig(reader.getStr(i, sQiZong_index));         
						data.sYuanLi = EffectTable.InitConfig(reader.getStr(i, sYuanLi_index));         
						data.sKongLing = EffectTable.InitConfig(reader.getStr(i, sKongLing_index));         
						data.sJianRen = EffectTable.InitConfig(reader.getStr(i, sJianRen_index));         
						data.sJianQiang = EffectTable.InitConfig(reader.getStr(i, sJianQiang_index));         
						data.sXunJie = EffectTable.InitConfig(reader.getStr(i, sXunJie_index));         
						data.sRuoWei = EffectTable.InitConfig(reader.getStr(i, sRuoWei_index));         
                        if (lineParseMethod != null)
                            lineParseMethod.Invoke(null, new object[3] {data, reader, i });
                        allDatas.Add(data);
                    }
                    catch (System.Exception ex)
                    {
                        CsvCommon.Log.Error("file:{0} line:{1} error!", file, i);
                        CsvCommon.Log.Exception(ex);
                    }
                }
            }
            
            foreach(var data in allDatas)
            {
                if (DataList.ContainsKey(data.id))
                {
                    CsvCommon.Log.Error("EffectLibrary.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(EffectLibrary);
                    while (null != curType)
                    {
                        method = curType.GetMethod("OnLoadEnd", BindingFlags.Static | BindingFlags.NonPublic);
                        if (null != method)
                            break;
                        curType = curType.BaseType;
                    }
                }
                if (method != null)
                    method.Invoke(null, new object[0]);
            }
        }
    }
}


