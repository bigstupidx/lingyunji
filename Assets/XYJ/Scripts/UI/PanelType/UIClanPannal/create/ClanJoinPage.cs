#if !USE_HOT
using Config;
using NetProto;
using NetProto.Hot;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class ClanJoinPage : HotTablePageBase
    {
    
       

        private UIClanCreatePanel m_parent = null;

        ClanJoinPage() : base(null) { }
        public ClanJoinPage(HotTablePage page) : base(page)
        {

        }

        protected override void OnInit()
        {
           
           
        }

        protected override void OnShow(object args)
        {
           
           // this.ResetUI();
        }

        protected override void OnHide()
        {

        }

       
 
    }
}
#endif