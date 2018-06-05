#if !USE_HOT
namespace xys.hot
{
    using System;
    using NetProto;
    using UnityEngine;
    partial class HotAppearanceModule : HotModuleBase
    {
        xys.hot.AppearanceMgr appearanceMgr = new xys.hot.AppearanceMgr();

        public HotAppearanceModule(xys.AppearanceModule m) : base(m)
        {

        }

        #region HotModule Call Methods

        protected override void OnDeserialize(wProtobuf.IReadStream output)
        {
            if (output == null)
                return;

            if (this.appearanceMgr.m_AppearanceData == null)
                this.appearanceMgr.m_AppearanceData = new AppearanceData();
            this.appearanceMgr.m_AppearanceData.MergeFrom(output);
        }

        protected override void OnAwake()
        {
            Init();           
        }

        #endregion

        public AppearanceData GetAppearanceData()
        {
            return appearanceMgr.m_AppearanceData;
        }

        public AppearanceMgr GetMgr()
        {
            return appearanceMgr;
        }

        
    }
}
#endif