using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace xys.UI
{
    public enum PanelType
    {
        // 面板类型
        Null,                                   //无效面板
        UIMainPanel,
        UIPackagePanel,                         //包裹面板
        UIItemTipsPanel,                        //tips面板
        UITempPackagePanel,                     //临时背包面板
        UIRoleAttriPanel,                       //角色属性面板
        UIWelfarePanel,                         //福利面板
        UIPetsPanel,                            //宠物面板
        UIPetsSkillPanel,                       //宠物技能面板
        UIPetsAssginPanel,                      //宠物加点面板
        UIPetsTipsPanel,                        //宠物道具属性面板
        UIPetsQualificationsPanel,              //宠物属性加点

        UIFriendPanel,                          //好友面板  
        UIRoleOperationPanel,                   //好友角色信息面板
        UIEquipPanel,                           //装备面板 
        UIRoleTitlePanel,                       //称号界面

        UIAccountPanel,                         //关卡结算界面
        UICalculatorPanel,                      //小键盘面板
        UICommonTipsPanel,                      //提示面板
        UIMapPanel,                             //地图界面
        UIFaceMakePanel,                        //捏脸界面
        UIAppearancePanel,                      //外观界面
        UISkillPanel,                           //技能界面
        UIConvenientPanel,                      //快捷进入关卡界面
        UIDecomposePanel,                       //装备分解界面
        UITrumpsPanel,//法宝面板
        UITrumpTipsPanel,//法宝tip面板
        UICompoundPanel,                         //合成界面
        UIBloodTipsPanel,                       //血量tips面板
        UIBloodPoolPanel,                      //血池面板

        #region 主界面右上角图标
        UIExchangeStorePanel,                   //积分商城
        UIActivityOpenPanel,                    //活动开启界面
        UIActivityUnlockedPanel,                //新活动解锁界面
        UIActivityIntroductionPanel,            //活动详细界面
        UIStorePanel,                           //商店界面
        UIStrongerPanel,                        //变强界面
        UIAuctionPanel,                         //拍卖行界面
        UIFirstChargePanel,                     //首冲界面
        UIActivityPanel,                        //活动日程界面
        UIImprovePanel,                         //提升界面
        UIMoneyTreePanel,                       //摇钱树
        UIFoodPanel,                            //抢食
        UIAdventurePanel,                       //奇遇
        UIQuestionPanel,                        //答题
        UIDungeonScorePanel,                    //副本战绩
        UIDungeonExitPanel,                     //副本退出
        #endregion

        UIClanPanel,                            //氏族面板
        UIClanCreatePanel,                      //创建氏族面板

        UITradingMarketTablePanel,              //易市面板
        UIQuickSellPanel,                       //快捷出售面板

        UIAttributeTipsPanel,//属性对比面板
        // 面板类型总个数
        UIPanelTotal,
    }

    public enum PanelCameraType
    {
        Default,    // 默认相机
        Top,        // 最顶层
    }
}