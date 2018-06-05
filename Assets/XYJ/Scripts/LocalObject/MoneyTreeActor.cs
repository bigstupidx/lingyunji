// Author : PanYuHuan
// Create Date : 2017/7/27

namespace xys
{
    using hot;

    public class MoneyTreeActor : LocalActorBase
    {
        public int treeUId;
        private MoneyTreeMgr moneyTreeMgr { get { return hotApp.my.GetModule<HotMoneyTreeModule>().moneyTreeMgr; } }
        protected override void OnUpdate()
        {
            
        }

        // 点击摇钱树模型
        protected override void OnPlayerClick()
        {
            if(MoneyTreeDef.ThisTreeBelongToMe(treeUId))
            {
                App.my.uiSystem.ShowPanel("UIPlantTreePanel", moneyTreeMgr.m_MoneyTreeDbData.listPlayerTrees[treeUId], true);
            }
            else
            {
                xys.UI.Utility.TipContentUtil.Show("moneyTree_find_tip");
            }
        }

        public void OnClickOwnTree()
        {
            OnPlayerClick();
        }
    }
}
