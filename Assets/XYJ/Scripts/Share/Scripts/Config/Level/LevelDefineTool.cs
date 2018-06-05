#if !COM_SERVER
#define Client
#endif
using System.Collections.Generic;
using System.IO;
#if Client
using xys;
#else
using GameServer;
#endif

namespace Config
{
    public class EventMonitorVo
    {
        public string eventId;
        public bool overAll;
        public List<int> conditionIndexList;
        public List<int> actionIndexList;
    }

    public partial class LevelDefine
    {
        LevelDesignConfig jsonConfig_;
        public LevelDesignConfig jsonConfig
        {
            get
            {
                if (jsonConfig_ == null)
                    jsonConfig_ = LoadJsonFile();

                return jsonConfig_;
            }
        }

#if Client
        /// <summary>
        /// 判断当前是否能够进入关卡，客户端用
        /// </summary>
        /// <returns></returns>
        public bool IsCanEnter()
        {
            LocalPlayer player = App.my.localPlayer;
            //LevelData levelData = App.my.localPlayer.GetModule<LevelModule>().levelData;

            if (player != null && player.isAlive)
            {
                //判断等级
                int roleLevel = player.levelValue;
                if (roleLevel < gradeLimit)
                {
                    Debuger.Log("进入关卡等级不足");
                    return false;
                }
                //判断次数
                if(!App.my.localPlayer.GetModule<LevelModule>().CheckLevelCount(id))
                {
                    Debuger.Log("超过最大挑战次数");
                    return false;
                }
                //if(levelData.count.ContainsKey(id))
                //{
                //    int count = levelData.count[id];
                //    if (count >= maxChallengeTimes)
                //    {
                //    }
                //}

                //首先判断关卡的进入方式
                if (teamCondition == TeamCondition.NeedNoTeam)
                {
                    //必须单人进
                    if(player.GetModule<TeamModule>().InTeam())
                    {
                        Debuger.Log("当前有队伍，进不了单人副本");
                        return false;
                    }
                }
                else if(teamCondition == TeamCondition.NeedTeam)
                {
                   //必须组队
                    if(!player.GetModule<TeamModule>().InTeam())
                    {
                        Debuger.Log("必须组队才能进");
                        return false;
                    }
                    else
                    {   
                        //判断组队人数
                        if(player.GetModule<TeamModule>().TeamNum() < teamNum)
                        {
                            Debuger.Log("队伍人数不足");
                            return false;
                        }
                    }
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
#else
        /// <summary>
        /// 判断当前是否能够进入关卡，服务器用
        /// </summary>
        /// <returns></returns>
        public bool isCanEnter(GameServer.GameUser user)
        {
            if(user.levelValue < gradeLimit)
            {
                //等级条件不满足
                Log.Debug("等级不足, 无法进入关卡");
                return false;
            }

            GameLevelMgr mgr = user.GetModule<GameLevelModule>().m_gameLevelMgr;
            if (null == mgr)
                return false;

            //超过最大次数
            if (!mgr.CheckLevelCount(id, maxChallengeTimes))
                return false;

            GameTeamMgr.Team team = GameApp.my.teamMgr.GetTeamByCharid(user.charid);
            //首先判断关卡的进入方式
            if (teamCondition == TeamCondition.NeedNoTeam)
            {
                //必须单人进
                if (team != null)
                {
                    Log.Debug("只有单人才能进， 现在组队中无法进入");
                    return false;
                }
            }
            else if (teamCondition == TeamCondition.NeedTeam)
            {
                //必须组队
                if (null == team)
                {
                    Log.Debug("组队才能进入");
                    return false;
                }
                else
                {
                    //已有队伍，判断组队人数
                    if (team.members.Count < teamNum)
                    {
                        Log.Debug("队伍人数不足");
                        return false;
                    }
                    else
                    {
                        //判断是否在同一个场景
                        if(teamEnterType == TeamEnterType.Call && !GameApp.my.teamMgr.CheckTeamScene(team))
                        {
                            Log.Debug("队员不在同一场景");
                            return false;
                        }
                    }
                }
            }
            else if(teamCondition == TeamCondition.All)
            {
                //是否组队都能进入，如果组队了需要判断组队进入的方式
                if(team != null)
                {
                    //判断是否在同一个场景
                    if (teamEnterType == TeamEnterType.Call && !GameApp.my.teamMgr.CheckTeamScene(team))
                    {
                        Log.Debug("队员不在同一场景");
                        return false;
                    }
                }
            }
            return true;
        }
#endif

        LevelDesignConfig LoadJsonFile()
        {
#if Client
            string text = PackTool.TextLoad.GetString(string.Format("Data/Config/Edit/Level/LevelDesignConfig/{0}.json", configId));
            if (string.IsNullOrEmpty(text))
            {
                Debuger.ErrorLog("config:{0} is empty!", configId);
                return null;
            }

            try
            {
                return UnityEngine.JsonUtility.FromJson<LevelDesignConfig>(text);
            }
            catch (System.Exception ex)
            {
                Debuger.LogException(ex);
                Debuger.ErrorLog("load json file:{0} error!", configId);
            }

            return null;
#else
            string path = string.Format("../ServerConfig/Game/Config/Edit/Level/LevelDesignConfig/{0}.json", configId);
            if (File.Exists(path))
            {
                try
                {
                    return Common.Json.JsonUtility.FromJson<LevelDesignConfig>(File.ReadAllText(path));
                }
                catch (System.Exception ex)
                {
                    Log.Exception(ex);
                    Log.Error("load Json File:{0} error!", configId);
                }
            }
            else
            {
                Log.Error("file:{0} not find!", path);
            }
            
            return null;
#endif
        }
    }
}