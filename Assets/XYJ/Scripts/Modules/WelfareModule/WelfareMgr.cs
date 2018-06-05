#if !USE_HOT
namespace xys.hot
{
    using NetProto;
    using Config;
    using UnityEngine;
    using System.Collections.Generic;
    using NetProto.Hot;

    class WelfareMgr
    {
        public WelfareDB m_WelfareDB
        {
            get;
            private set;
        }
        
        public WelfareMgr()
        {
            m_WelfareDB = new WelfareDB();
        }


        public void SetData(WelfareDB data)
        {
            m_WelfareDB = data;
        }
        public static bool CheckLVRwdAvailable()
        {
            WelfareMgr mgr = App.my.localPlayer.GetModule<WelfareModule>().welfareData as WelfareMgr;
            var welfareData = mgr.m_WelfareDB;

            int playerLv = App.my.localPlayer.levelValue;
            int rwdNum = Config.UpgradeRewardDefine.GetAll().Count;
            //finalindex表示0到finalindex都可以领取,先默认移动到数组尾部
            int finalIndex = GetRwdUpLimit();
            //int finalIndex = rwdNum - 1;
            bool ret = false;
            IntBit rwdStatus = new IntBit(welfareData.lvRwdStatus);
            for (int i = 0; i < rwdNum; i++)
            {
                if (!rwdStatus.Get(i))
                {
                    if (i <= finalIndex)
                    {
                        ret = true;
                    }
                }
            }
            return ret;
        }
        public static int GetRwdUpLimit()
        {
            int playerLv = App.my.localPlayer.levelValue;
            int rwdNum = Config.UpgradeRewardDefine.GetAll().Count;
            int ret = rwdNum - 1;
            //通过当前等级判断前几个奖励可以领取
            for (int i = 0; i < rwdNum; i++)
            {
                if (playerLv < Config.UpgradeRewardDefine.Get(i + 1).level)
                {
                    ret = i - 1;
                    break;
                }
            }

            return ret;
        }
    }
}
#endif