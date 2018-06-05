using Config;
using NetProto;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.battle
{
    public class SearchAction : IAction<SearchActionConfig>
    {
        struct MultiRectInfo
        {
            public Vector3 searchPoint;
            public float searchAngle;
        }


        public override RunType GetRunType()
        {
            return RunType.ServerOnly;
        }

        static public Vector3 GetSearchPos(Vector3 searchPoint, float searchAngle, SearchActionConfig cfg)
        {
            //偏移角度
            if (cfg.posOff.Length == 2)
            {
                Vector3 off;
                off.z = cfg.posOff[0];
                off.x = cfg.posOff[1];
                off.y = 0;
                off = BattleHelp.RotateAngle(off, searchAngle);
                searchPoint += off;
            }
            return searchPoint;
        }


        public override bool OnExcute(ActionInfo info)
        {

            List<IObject> objList = GetSearchObject(info.source, info.target, info.firePos, info.fireAngle, info.skill, info);
            return objList != null && objList.Count > 0;
        }

        //技能施放前搜索目标
        public List<int> GetSearchObjectid(IObject source, IObject target)
        {
            List<IObject> objs = GetSearchObject(source, target, source.position, source.rotateAngle, null);
            if (objs == null)
                return null;
            List<int> objids = new List<int>();
            foreach (var p in objs)
                objids.Add(p.charSceneId);
            return objids;
        }

        List<IObject> GetSearchObject(IObject source, IObject target, Vector3 firePos, float fireAngle, SkillLogic skill, ActionInfo info = null)
        {
            List<IObject> objList = null;
            Dictionary<int, IObject> allObjs = BattleHelp.GetAOIObjByPos(source, firePos);

            if (allObjs == null)
                return null;

            Vector3 searchPoint;
            float searchAngle;
            switch (cfg.searchPosition)
            {
                case SearchActionConfig.SearchPoint.Source:
                    searchPoint = firePos;
                    searchAngle = fireAngle;
                    break;
                case SearchActionConfig.SearchPoint.SkillPoint:
                    if (skill == null)
                        return null;
                    searchPoint = skill.m_skillPoint;
                    searchAngle = fireAngle;
                    break;
                case SearchActionConfig.SearchPoint.Target:
                    if (target == null)
                        return null;
                    searchPoint = target.position;
                    searchAngle = fireAngle;
                    break;
                default:
                    searchPoint = Vector3.zero;
                    searchAngle = 0;
                    XYJLogger.LogError("搜索原点出错 " + cfg.searchPosition);
                    break;
            }

            //多个矩形
            if (cfg.searchType == SearchActionConfig.SearchSharp.MultiRect)
            {
                List<MultiRectInfo> temList = new List<MultiRectInfo>();
                for (int i = 0; i < cfg.searchPara.Length; i += 3)
                {
                    MultiRectInfo rectinfo;
                    rectinfo.searchAngle = searchAngle + cfg.searchPara[i + 2];
                    rectinfo.searchPoint = GetSearchPos(searchPoint, rectinfo.searchAngle, cfg);
                    temList.Add(rectinfo);
                }
                return GetRect(Vector3.zero, 0, source, allObjs, temList, info);
            }

            //偏移角度
            if (!cfg.isCircle)
                searchPoint = GetSearchPos(searchPoint, searchAngle, cfg);
            //计算搜索目标
            switch (cfg.searchType)
            {
                case SearchActionConfig.SearchSharp.Angle: objList = GetAngle(searchPoint, searchAngle, source, allObjs, info); break;
                case SearchActionConfig.SearchSharp.Rect: objList = GetRect(searchPoint, searchAngle, source, allObjs, null, info); break;
            }
            return objList;
        }

        //扇形
        List<IObject> GetAngle(Vector3 pos, float angle, IObject source, Dictionary<int, IObject> allObjs, ActionInfo info = null)
        {
            List<IObject> findList = new List<IObject>();
            foreach (var p in allObjs)
            {
                IObject obj = p.Value;
                //阵营不对
                if (BattleHelp.GetRelation(source, obj) != cfg.battleRelation)
                    continue;

                float dis = BattleHelp.GetAttackDistance(pos, obj);
                //距离不对
                if (dis > cfg.searchPara[0])
                    continue;
                //可以配置最小距离
                if (cfg.searchPara.Length == 3 && dis < cfg.searchPara[2])
                    continue;
                Vector3 targetDir = obj.position - pos;
                targetDir.y = pos.y;
                //不是圆需要判断角度
                if (!cfg.isCircle)
                {
                    if (BattleHelp.GetForwardAngle(angle, targetDir) * 2 > cfg.searchPara[1])
                        continue;
                }

                if (info == null || ActionManager.HandleActionList(info.skill, info.source, obj, info.actionRecord, cfg.actionList, info.firePos, info.fireAngle))
                    findList.Add(obj);
                if (findList.Count > cfg.searchMaxCnt)
                    break;
            }
            return findList;
        }

        //矩形
        List<IObject> GetRect(Vector3 pos, float angle, IObject source, Dictionary<int, IObject> allObjs, List<MultiRectInfo> rectinfo = null, ActionInfo info = null)
        {
            List<IObject> findList = new List<IObject>();
            foreach (var p in allObjs)
            {
                //阵营不对
                if (BattleHelp.GetRelation(source, p.Value) != cfg.battleRelation)
                    continue;
                IObject obj = null;
                //一个矩形
                if (info == null)
                {
                    Vector3 posOff = p.Value.position - pos;
                    if (BattleHelp.PosInRect(cfg.searchPara[0] / 2, cfg.searchPara[1], posOff, angle))
                        obj = p.Value;
                }
                //多个矩形
                else
                {
                    for (int i = 0; i < rectinfo.Count; i++)
                    {
                        Vector3 posOff = p.Value.position - rectinfo[i].searchPoint;
                        if (BattleHelp.PosInRect(cfg.searchPara[0] / 2, cfg.searchPara[1], posOff, rectinfo[i].searchAngle))
                        {
                            obj = p.Value;
                            break;
                        }
                    }
                }

                if (obj != null)
                {
                    if (info == null || ActionManager.HandleActionList(info.skill, info.source, obj, info.actionRecord, cfg.actionList, info.firePos, info.fireAngle))
                        findList.Add(obj);
                    if (findList.Count > cfg.searchMaxCnt)
                        break;
                }
            }
            return findList;
        }

    }
}
