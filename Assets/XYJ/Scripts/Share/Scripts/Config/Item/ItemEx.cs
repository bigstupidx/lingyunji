// this file is gen by program, please do not modify it 
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System;
using xys;
#if !COM_SERVER
using xys.UI;
#endif

namespace Config
{
    public partial class Item : ItemBase
    {
        // 是否可使用
        public override bool IsCanUse { get { return isCanUse; } }
        public override bool IsCanSell { get { return isCanSell; } }
        public override bool IsCanAllUse { get { return allUse; } }
        public override bool IsCanLost { get { return unLost; } } // 是否可丢弃
        // 点集
        public class MapPoint
        {
            public int id;
            public string name;

            public static MapPoint InitConfig(string text)
            {
                var v = text.Split('|');
                MapPoint point = new MapPoint();
                point.id = int.Parse(v[0]);
                point.name = v[1];

                return point;
            }
        }

        public class MapPoints
        {
            public MapPoint[] points;

            public static MapPoints InitConfig(string text)
            {
                if (string.IsNullOrEmpty(text))
                    return null;

                var v = text.Split(';');
                MapPoints points = new MapPoints();
                points.points = new MapPoint[v.Length];
                for (int i = 0; i < v.Length; ++i)
                    points.points[i] = MapPoint.InitConfig(v[i]);

                return points;
            }
        }

#if COM_SERVER
        public override void OnCreate(NetProto.ItemData data)
        {
            if (enduLevel != 0)
                data.durable = enduLevel;
        }

        // 是否满足使用条件
        public override bool isCanUseCondition(GameServer.GameUser user)
        {
            if (!job.Has(user.job))
                return false;
            if (user.levelValue < useLevel)
                return false;
            if (cooling != 0)
            {
                if (!user.cdMgr.isEnd(xys.CDType.Item, (short)sonType))
                    return false;
            }
            if (sonType == (int)ItemChildType.bloodPool)
            {
                Config.kvCommon config = Config.kvCommon.Get("BloodPoolTrueLimit");
                if (config == null || user.bloodPoolValue >= uint.Parse(config.value))
                    return false;
            }

            //秘籍道具
            if (type == ItemType.consumables && sonType == (int)ItemChildType.playerSuperSkill)
            {
                //是否已经领悟
                GameServer.SkillModule skillMgr = user.GetModule(NetProto.ModuleType.MT_Skill) as GameServer.SkillModule;
                if (skillMgr != null)
                    return !skillMgr.IsItemComprehend(id);
            }
            return true;
        }

        // 开始使用道具
        public override bool Use(GameServer.GameUser user, int count)
        {
            if (rewardId != 0)
            {
                for (int i = 0; i < count; ++i)
                    GameServer.Reward.GiveReward(user, rewardId);
            }

            if (actPet != 0)
            {
                // 给宠物
                GameServer.PetsModule petsMoudle = user.GetModule<GameServer.PetsModule>();
                if (petsMoudle != null)
                    petsMoudle.OnItem2Pets(this.id);
            }

            if (actWeapon != 0)
            {
                // 给法宝
                GameServer.TrumpsModule trumpsModule = user.GetModule<GameServer.TrumpsModule>();
                if (trumpsModule != null)
                    trumpsModule.OnItem2Trump(this.id);
            }

            if (soulId != 0)
            {
                // 附魂
            }

            if (status != 0)
            {
                // 给状态
            }

            if (ActTitleId != 0)
            {
                // 给称号
                GameServer.TitleModule titleMgr = user.GetModule(NetProto.ModuleType.MT_Title) as GameServer.TitleModule;
                if (titleMgr != null)
                    titleMgr.UseItemActivation(ActTitleId);
            }

            if (type == ItemType.consumables && sonType == (int)ItemChildType.playerSuperSkill)
            {
                //给技能领悟
                GameServer.SkillModule skillMgr = user.GetModule(NetProto.ModuleType.MT_Skill) as GameServer.SkillModule;
                if (skillMgr != null)
                {
                    skillMgr.OnComprehendScheme(id);
                }
            }

            if (fashionId != 0)
            {
                // 给时装
            }

            if (addFashTimer != 0)
            {
                // 增加时装时长
            }

            if (mountId != 0)
            {
                // 给坐骑
            }

            if (addMouTimer != 0)
            {
                // 增加坐骑时长
            }

            if (cooling != 0)
            {
                user.cdMgr.StartCD(xys.CDType.Item, (short)sonType);
            }

            // 恢复血量
            if (addHp != 0)
            {
                if (sonType == (int)ItemChildType.bloodPool)
                    user.AddBloodPoolValue(addHp);
                else if (sonType == (int)ItemChildType.cure)
                    user.HpChange(addHp);
                else if (sonType == (int)ItemChildType.petDrug)
                    user.petBloodBottleValue = id;
            }

            return true;
        }

        // 是否售卖成功
        public override bool Sell(GameServer.GameUser user, int count)
        {
            if (priceSilver > 0)
            {
                user.AddSilverShell(priceSilver * count);
            }

            if (priceGold > 0)
                user.AddGoldShell(priceGold * count);

            if (priceBiyu > 0)
                user.AddJasperJade(priceBiyu * count);

            return true;
        }
#else
        // 是否满足使用条件
        public override bool isCanUseCondition()
        {
            var localPlayer = xys.App.my.localPlayer;
            if (!job.Has(localPlayer.job))
            {
                TipsContent tipsConfig = TipsContent.Get(3112);
                if (tipsConfig == null)
                    return false;
                xys.UI.SystemHintMgr.ShowHint(tipsConfig.des);
                return false;
            }

            if (localPlayer.levelValue < useLevel)
            {
                TipsContent tipsConfig = TipsContent.Get(3113);
                if (tipsConfig == null)
                    return false;
                xys.UI.SystemHintMgr.ShowHint(tipsConfig.des);
                return false;
            }

            if (cooling != 0)
            {
                if (!localPlayer.cdMgr.isEnd(xys.CDType.Item, (short)sonType))
                {
                    TipsContent tipsConfig = TipsContent.Get(3104);
                    if (tipsConfig == null)
                        return false;
                    xys.UI.SystemHintMgr.ShowHint(tipsConfig.des);
                    return false;
                }
            }

            if (sonType == (int)ItemChildType.cure)
            {
                if (App.my.localPlayer.hpValue == App.my.localPlayer.maxHpValue)
                {
                    Config.TipsContent config = Config.TipsContent.Get(3100);
                    if (config != null)
                    {
                        xys.UI.SystemHintMgr.ShowHint(config.des);
                        return false;
                    }
                }
            }

            return true;
        }
        public override bool Use() // 开始使用道具
        {
            var localPlayer = xys.App.my.localPlayer;
            // 英雄帖
            if (sonType == (int)ItemChildType.heroInvitation)
            {
                if (localPlayer.levelValue > useLevel)
                {
                    App.my.uiSystem.ShowPanel("UIChatHeroPostPanel");
                    App.my.uiSystem.HidePanel(PanelType.UIPackagePanel);
                }
                else
                {
                    // 等级不符
                    SystemHintMgr.ShowTipsHint(3113);
                }
                return false;
            }
            // 血池
            else if (sonType == (int)ItemChildType.bloodPool ||
                sonType == (int)ItemChildType.petDrug)
            {
                if (!App.my.uiSystem.IsShow(PanelType.UIBloodPoolPanel))
                {
                    App.my.uiSystem.ShowPanel(PanelType.UIBloodPoolPanel, true);
                    return false;
                }
                else
                {
                    Config.TipsContent tipsConfig = Config.TipsContent.Get(3101);
                    if (tipsConfig != null)
                    {
                        xys.UI.SystemHintMgr.ShowHint(string.Format(tipsConfig.des, addHp));
                    }
                }
            }

            return true;
        }
#endif
    }
}


