// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class BulletActionConfig 
    {
        static Dictionary<string, BulletActionConfig> DataList = new Dictionary<string, BulletActionConfig>();

        static public Dictionary<string, BulletActionConfig> GetAll()
        {
            return DataList;
        }

        static public BulletActionConfig Get(string key)
        {
            BulletActionConfig value = null;
            if (DataList.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("BulletActionConfig.Get({0}) not find!", key);
            return null;
        }



        // 编号
        public string id { get; set; }

        // 目标点
        public TargetPos targetPos { get; set; }

        // 是否追踪
        public int follow { get; set; }

        // 轨迹参数
        public float[] pathParas { get; set; }

        // 飞行速度
        public float speed { get; set; }

        // 子弹模型
        public string fxName { get; set; }

        // 发射位置
        public FirePos firePos { get; set; }

        // 发射点
        public string FirePosBone { get; set; }

        // 发射偏移
        public float[] firePosOff { get; set; }

        // 发射延迟
        public float fireDelay { get; set; }

        // 飞行角度
        public float flyAngle { get; set; }

        // 生命周期
        public float lifeTime { get; set; }

        // 创建action
        public string[] createActions { get; set; }

        // 间隔时间
        public float interval { get; set; }

        // 间隔action
        public string[] intervalActions { get; set; }

        // 消失action
        public string[] destroyActions { get; set; }

        // 消失特效
        public string destroyEffect { get; set; }

        // 子弹结束不销毁模型
        public bool notDestroyModel { get; set; }

        public xys.battle.BattleAttri battleAttri;

        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(BulletActionConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(BulletActionConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<BulletActionConfig> allDatas = new List<BulletActionConfig>();

            {
                string file = "Skill/BulletActionConfig#monster.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int targetPos_index = reader.GetIndex("targetPos");
                int follow_index = reader.GetIndex("follow");
                int pathParas_index = reader.GetIndex("pathParas");
                int speed_index = reader.GetIndex("speed");
                int fxName_index = reader.GetIndex("fxName");
                int firePos_index = reader.GetIndex("firePos");
                int FirePosBone_index = reader.GetIndex("FirePosBone");
                int firePosOff_index = reader.GetIndex("firePosOff");
                int fireDelay_index = reader.GetIndex("fireDelay");
                int flyAngle_index = reader.GetIndex("flyAngle");
                int lifeTime_index = reader.GetIndex("lifeTime");
                int createActions_index = reader.GetIndex("createActions");
                int interval_index = reader.GetIndex("interval");
                int intervalActions_index = reader.GetIndex("intervalActions");
                int destroyActions_index = reader.GetIndex("destroyActions");
                int destroyEffect_index = reader.GetIndex("destroyEffect");
                int notDestroyModel_index = reader.GetIndex("notDestroyModel");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        BulletActionConfig data = new BulletActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.targetPos = ((TargetPos)reader.getInt(i, targetPos_index, 0));         
						data.follow = reader.getInt(i, follow_index, 0);         
						data.pathParas = reader.getFloats(i, pathParas_index, ';');         
						data.speed = reader.getFloat(i, speed_index, 0f);         
						data.fxName = reader.getStr(i, fxName_index);         
						data.firePos = ((FirePos)reader.getInt(i, firePos_index, 0));         
						data.FirePosBone = reader.getStr(i, FirePosBone_index);         
						data.firePosOff = reader.getFloats(i, firePosOff_index, ';');         
						data.fireDelay = reader.getFloat(i, fireDelay_index, 0f);         
						data.flyAngle = reader.getFloat(i, flyAngle_index, 0f);         
						data.lifeTime = reader.getFloat(i, lifeTime_index, 0f);         
						data.createActions = reader.getStrs(i, createActions_index, ';');         
						data.interval = reader.getFloat(i, interval_index, 0f);         
						data.intervalActions = reader.getStrs(i, intervalActions_index, ';');         
						data.destroyActions = reader.getStrs(i, destroyActions_index, ';');         
						data.destroyEffect = reader.getStr(i, destroyEffect_index);         
						data.notDestroyModel = reader.getBool(i, notDestroyModel_index, false);         
                        data.battleAttri = xys.battle.CsvLineAttri.GenBattleAttri(reader, i);
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
				xys.battle.CsvLineAttri.ClearCache();
            }
            {
                string file = "Skill/BulletActionConfig#pet.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int targetPos_index = reader.GetIndex("targetPos");
                int follow_index = reader.GetIndex("follow");
                int pathParas_index = reader.GetIndex("pathParas");
                int speed_index = reader.GetIndex("speed");
                int fxName_index = reader.GetIndex("fxName");
                int firePos_index = reader.GetIndex("firePos");
                int FirePosBone_index = reader.GetIndex("FirePosBone");
                int firePosOff_index = reader.GetIndex("firePosOff");
                int fireDelay_index = reader.GetIndex("fireDelay");
                int flyAngle_index = reader.TryIndex("flyAngle");
                int lifeTime_index = reader.GetIndex("lifeTime");
                int createActions_index = reader.GetIndex("createActions");
                int interval_index = reader.GetIndex("interval");
                int intervalActions_index = reader.GetIndex("intervalActions");
                int destroyActions_index = reader.GetIndex("destroyActions");
                int destroyEffect_index = reader.GetIndex("destroyEffect");
                int notDestroyModel_index = reader.GetIndex("notDestroyModel");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        BulletActionConfig data = new BulletActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.targetPos = ((TargetPos)reader.getInt(i, targetPos_index, 0));         
						data.follow = reader.getInt(i, follow_index, 0);         
						data.pathParas = reader.getFloats(i, pathParas_index, ';');         
						data.speed = reader.getFloat(i, speed_index, 0f);         
						data.fxName = reader.getStr(i, fxName_index);         
						data.firePos = ((FirePos)reader.getInt(i, firePos_index, 0));         
						data.FirePosBone = reader.getStr(i, FirePosBone_index);         
						data.firePosOff = reader.getFloats(i, firePosOff_index, ';');         
						data.fireDelay = reader.getFloat(i, fireDelay_index, 0f);         
						data.flyAngle = reader.getFloat(i, flyAngle_index, 0f);         
						data.lifeTime = reader.getFloat(i, lifeTime_index, 0f);         
						data.createActions = reader.getStrs(i, createActions_index, ';');         
						data.interval = reader.getFloat(i, interval_index, 0f);         
						data.intervalActions = reader.getStrs(i, intervalActions_index, ';');         
						data.destroyActions = reader.getStrs(i, destroyActions_index, ';');         
						data.destroyEffect = reader.getStr(i, destroyEffect_index);         
						data.notDestroyModel = reader.getBool(i, notDestroyModel_index, false);         
                        data.battleAttri = xys.battle.CsvLineAttri.GenBattleAttri(reader, i);
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
				xys.battle.CsvLineAttri.ClearCache();
            }
            {
                string file = "Skill/BulletActionConfig#player.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int id_index = reader.GetIndex("id");
                int targetPos_index = reader.GetIndex("targetPos");
                int follow_index = reader.GetIndex("follow");
                int pathParas_index = reader.GetIndex("pathParas");
                int speed_index = reader.GetIndex("speed");
                int fxName_index = reader.GetIndex("fxName");
                int firePos_index = reader.GetIndex("firePos");
                int FirePosBone_index = reader.GetIndex("FirePosBone");
                int firePosOff_index = reader.GetIndex("firePosOff");
                int fireDelay_index = reader.GetIndex("fireDelay");
                int flyAngle_index = reader.GetIndex("flyAngle");
                int lifeTime_index = reader.GetIndex("lifeTime");
                int createActions_index = reader.GetIndex("createActions");
                int interval_index = reader.GetIndex("interval");
                int intervalActions_index = reader.GetIndex("intervalActions");
                int destroyActions_index = reader.GetIndex("destroyActions");
                int destroyEffect_index = reader.GetIndex("destroyEffect");
                int notDestroyModel_index = reader.GetIndex("notDestroyModel");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        BulletActionConfig data = new BulletActionConfig();
						data.id = reader.getStr(i, id_index);         
						data.targetPos = ((TargetPos)reader.getInt(i, targetPos_index, 0));         
						data.follow = reader.getInt(i, follow_index, 0);         
						data.pathParas = reader.getFloats(i, pathParas_index, ';');         
						data.speed = reader.getFloat(i, speed_index, 0f);         
						data.fxName = reader.getStr(i, fxName_index);         
						data.firePos = ((FirePos)reader.getInt(i, firePos_index, 0));         
						data.FirePosBone = reader.getStr(i, FirePosBone_index);         
						data.firePosOff = reader.getFloats(i, firePosOff_index, ';');         
						data.fireDelay = reader.getFloat(i, fireDelay_index, 0f);         
						data.flyAngle = reader.getFloat(i, flyAngle_index, 0f);         
						data.lifeTime = reader.getFloat(i, lifeTime_index, 0f);         
						data.createActions = reader.getStrs(i, createActions_index, ';');         
						data.interval = reader.getFloat(i, interval_index, 0f);         
						data.intervalActions = reader.getStrs(i, intervalActions_index, ';');         
						data.destroyActions = reader.getStrs(i, destroyActions_index, ';');         
						data.destroyEffect = reader.getStr(i, destroyEffect_index);         
						data.notDestroyModel = reader.getBool(i, notDestroyModel_index, false);         
                        data.battleAttri = xys.battle.CsvLineAttri.GenBattleAttri(reader, i);
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
				xys.battle.CsvLineAttri.ClearCache();
            }
            
            foreach(var data in allDatas)
            {
                if (DataList.ContainsKey(data.id))
                {
                    CsvCommon.Log.Error("BulletActionConfig.id :{0} is repeated", data.id);
                }
                else
                {
                    DataList[data.id] = data;
                }
            }

            {
                MethodInfo method = null;
                {
                    var curType = typeof(BulletActionConfig);
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


