// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace Config
{
    public partial class JumpConfig 
    {
        static List<JumpConfig> DataList = new List<JumpConfig>();
        static public List<JumpConfig> GetAll()
        {
            return DataList;
        }

        static public JumpConfig Get(string key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return DataList[i];
            }
            CsvCommon.Log.Error("JumpConfig.Get({0}) not find!", key);
            return null;
        }

        static public int FindIndex(string key)
        {
            for (int i = 0; i < DataList.Count; ++i)
            {
                if (DataList[i].key == key)
                    return i;
            }
            return -1;
        }

        static Dictionary<string, List<JumpConfig>> DataList_key = new Dictionary<string, List<JumpConfig>>();

        static public Dictionary<string, List<JumpConfig>> GetAllGroupBykey()
        {
            return DataList_key;
        }

        static public List<JumpConfig> GetGroupBykey(string key)
        {
            List<JumpConfig> value = null;
            if (DataList_key.TryGetValue(key, out value))
                return value;
            CsvCommon.Log.Error("JumpConfig.GetGroupBykey({0}) not find!", key);
            return null;
        }


        // 编号
        public string key { get; set; }

        // 动作名称
        public string aniFly { get; set; }

        // 动作过程不着地
        public bool noFallingWhenAni { get; set; }

        // 后续下落动作
        public string fallingAni { get; set; }

        // 着地id
        public string[] landAnis { get; set; }

        // 着地进入疾跑
        public bool landToFastRun { get; set; }

        // 响应按键开始时间
        public float timeBetinInput { get; set; }

        // 响应帧
        public int responseFrame { get; set; }

        // 开始帧
        public int beginFrame { get; set; }

        // 结束帧
        public int endFrame { get; set; }

        // y轴初速度
        public float speedY { get; set; }

        // 前进初速度
        public float speedZ { get; set; }

        // 是否强制前进初速度
        public bool forceSpeedZ { get; set; }

        // 起跳后摇杆前进速度
        public float inputSpeedZ { get; set; }

        // 前进加速度
        public float accZ { get; set; }

        // 重力加速度
        public float accY { get; set; }

        // 开始FOV变化帧数
        public int fovStarFrame { get; set; }

        // 变化的帧数
        public int fovLastFrame { get; set; }

        // 变化后的FOV
        public int toFov { get; set; }

        // 开始FOV恢复帧数
        public int fovRecoverStarFrame { get; set; }

        // 恢复的帧数
        public int fovRecoverLastFrame { get; set; }

        // 响应操作的帧数
        public int responseOperationFrame { get; set; }

        // 拉高的相机高度
        public float changeCameraHeight { get; set; }

        // 俯冲旋转角速度
        public float glidingRotation { get; set; }


        public static void Load(CsvCommon.ICsvLoad reader)
        {
            if (DataList.Count != 0)
            {
                var type = typeof(JumpConfig);
                var method = type.GetMethod("OnReload", BindingFlags.Static | BindingFlags.NonPublic);
                if (method != null)
                    method.Invoke(null, new object[0]);
            }

            DataList.Clear();
            DataList_key.Clear();
            

            MethodInfo lineParseMethod = null;
            {
                var curType = typeof(JumpConfig);
                while (null != curType)
                {
                    lineParseMethod = curType.GetMethod("OnLoadEndLine");
                    if (null != lineParseMethod)
                        break;
                    curType = curType.BaseType;
                }
            }

            List<JumpConfig> allDatas = new List<JumpConfig>();

            {
                string file = "Skill/JumpConfig.txt"; 
                if (!reader.LoadFile(file, '	', false))
                    return;
                reader.generateKey(1); 
                int key_index = reader.TryIndex("key:group");
                int aniFly_index = reader.GetIndex("aniFly");
                int noFallingWhenAni_index = reader.GetIndex("noFallingWhenAni");
                int fallingAni_index = reader.GetIndex("fallingAni");
                int landAnis_index = reader.GetIndex("landAnis");
                int landToFastRun_index = reader.GetIndex("landToFastRun");
                int timeBetinInput_index = reader.GetIndex("timeBetinInput");
                int responseFrame_index = reader.GetIndex("responseFrame");
                int beginFrame_index = reader.GetIndex("beginFrame");
                int endFrame_index = reader.GetIndex("endFrame");
                int speedY_index = reader.GetIndex("speedY");
                int speedZ_index = reader.GetIndex("speedZ");
                int forceSpeedZ_index = reader.GetIndex("forceSpeedZ");
                int inputSpeedZ_index = reader.GetIndex("inputSpeedZ");
                int accZ_index = reader.GetIndex("accZ");
                int accY_index = reader.GetIndex("accY");
                int fovStarFrame_index = reader.GetIndex("fovStarFrame");
                int fovLastFrame_index = reader.GetIndex("fovLastFrame");
                int toFov_index = reader.GetIndex("toFov");
                int fovRecoverStarFrame_index = reader.GetIndex("fovRecoverStarFrame");
                int fovRecoverLastFrame_index = reader.GetIndex("fovRecoverLastFrame");
                int responseOperationFrame_index = reader.GetIndex("responseOperationFrame");
                int changeCameraHeight_index = reader.GetIndex("changeCameraHeight");
                int glidingRotation_index = reader.GetIndex("glidingRotation");
                for (int i = 3; i < reader.YCount; ++i)
                {
                    try
                    {
                        JumpConfig data = new JumpConfig();
						data.key = reader.getStr(i, key_index);         
						data.aniFly = reader.getStr(i, aniFly_index);         
						data.noFallingWhenAni = reader.getBool(i, noFallingWhenAni_index, false);         
						data.fallingAni = reader.getStr(i, fallingAni_index);         
						data.landAnis = reader.getStrs(i, landAnis_index, ';');         
						data.landToFastRun = reader.getBool(i, landToFastRun_index, false);         
						data.timeBetinInput = reader.getFloat(i, timeBetinInput_index, 0f);         
						data.responseFrame = reader.getInt(i, responseFrame_index, 0);         
						data.beginFrame = reader.getInt(i, beginFrame_index, 0);         
						data.endFrame = reader.getInt(i, endFrame_index, 0);         
						data.speedY = reader.getFloat(i, speedY_index, 0f);         
						data.speedZ = reader.getFloat(i, speedZ_index, 0f);         
						data.forceSpeedZ = reader.getBool(i, forceSpeedZ_index, false);         
						data.inputSpeedZ = reader.getFloat(i, inputSpeedZ_index, 0f);         
						data.accZ = reader.getFloat(i, accZ_index, 0f);         
						data.accY = reader.getFloat(i, accY_index, 0f);         
						data.fovStarFrame = reader.getInt(i, fovStarFrame_index, 0);         
						data.fovLastFrame = reader.getInt(i, fovLastFrame_index, 0);         
						data.toFov = reader.getInt(i, toFov_index, 0);         
						data.fovRecoverStarFrame = reader.getInt(i, fovRecoverStarFrame_index, 0);         
						data.fovRecoverLastFrame = reader.getInt(i, fovRecoverLastFrame_index, 0);         
						data.responseOperationFrame = reader.getInt(i, responseOperationFrame_index, 0);         
						data.changeCameraHeight = reader.getFloat(i, changeCameraHeight_index, 0f);         
						data.glidingRotation = reader.getFloat(i, glidingRotation_index, 0f);         
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
            
            DataList = allDatas;

            foreach (var data in allDatas)
            {
                {
                    List<JumpConfig> l = null;
                    if (!DataList_key.TryGetValue(data.key, out l))
                    {
                        l = new List<JumpConfig>();
                        DataList_key[data.key] = l;
                    }
                    l.Add(data);
                }
            }
            {
                MethodInfo method = null;
                {
                    var curType = typeof(JumpConfig);
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


