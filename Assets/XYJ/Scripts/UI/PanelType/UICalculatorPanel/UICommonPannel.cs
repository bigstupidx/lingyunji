#if !USE_HOT
using behaviac;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using xys;
using xys.UI;

namespace xys.hot.UI
{
    /// <summary>
    /// 一些公共面板接口都放在这里
    /// </summary>
    public class UICommon
    {
        //二次确认框
        static public void ShowConfirmPannel(string title, System.Action okFun, System.Action cancelFun)
        {

        }
        
        //显示属性对比tips
        static public void ShowAttributeTips(xys.battle.BattleAttri oldAttri,xys.battle.BattleAttri newAttri,string titleName = "",int titleOldValue = 0, int titleNewValue = 0)
        {
            UIAttributeTipsParam param = new UIAttributeTipsParam();
            param.title = titleName;
            param.titleOldValue = titleOldValue;
            param.titleNewValue = titleNewValue;
            param.oldAttri = oldAttri;
            param.newAttri = newAttri;
            App.my.uiSystem.HidePanel(PanelType.UIAttributeTipsPanel, false);
            App.my.uiSystem.ShowPanel(PanelType.UIAttributeTipsPanel, param);
        }
        //宠物数据
        static public void ShowPetTips(NetProto.PetsAttribute attribute)
        {
            if (attribute == null)
                return;
            App.my.uiSystem.ShowPanel(PanelType.UIPetsTipsPanel, attribute);
        }
        //法宝tips
        static public void ShowTrumpTips(int trumpId,Vector2 pos)
        {
            UITrumpTipsParam param = new UITrumpTipsParam();
            param.trumpId = trumpId;
            param.pos = pos;
            App.my.uiSystem.ShowPanel(PanelType.UITrumpTipsPanel, param);
        }

        static public void ShowTrumpSkillTips(int skillId,int skillLv,Vector2 pos)
        {
            UITrumpTipsParam param = new UITrumpTipsParam();
            param.skillId = skillId;
            param.skillLv = skillLv;
            param.pos = pos;
            App.my.uiSystem.ShowPanel(PanelType.UITrumpTipsPanel, param);
        }

        //道具tips
        static public void ShowItemTips(int itemId)
        {
            ItemTipsPanel.Param param = new ItemTipsPanel.Param();
            param.itemId = itemId;
            App.my.uiSystem.ShowPanel(PanelType.UIItemTipsPanel, param, true);
        }

        //通用提示框
        static public void ShowTipsWithItem(string des, int itemid, int count, System.Action<bool> action, string leftbtnstr = "", string rightbtnstr = "")
        {
            UICommonTipsPanel.Param param = new UICommonTipsPanel.Param();
            param.des = des;
            param.itemId = itemid;
            param.itemNum = count;
            param.btnEvent = action;
            if (leftbtnstr != string.Empty)
                param.leftBtnTxt = leftbtnstr;
            if (rightbtnstr != string.Empty)
                param.rightBtnTxt = rightbtnstr;
            App.my.uiSystem.ShowPanel(PanelType.UICommonTipsPanel, param);
        }

        static public void ShowTipsWithPromptKey(string des, string promptKey, string leftbtnstr = "", string rightbtnstr = "")
        {
            UICommonTipsPanel.Param param = new UICommonTipsPanel.Param();
            param.des = des;
            param.promptKey = promptKey;
            if (leftbtnstr != string.Empty)
                param.leftBtnTxt = leftbtnstr;
            if (rightbtnstr != string.Empty)
                param.rightBtnTxt = rightbtnstr;
            App.my.uiSystem.ShowPanel(PanelType.UICommonTipsPanel, param);
        }

        //通用小键盘
        static public void ShowCalculator(Vector3 pos, int maxValue, int minValue = 1, int defaultValue = 1, System.Action<int> action = null)
        {
            UICalculatorPanel.Param param = new UICalculatorPanel.Param();
            param.defaultValue = defaultValue;
            param.maxValue = maxValue;
            param.minValue = minValue;
            param.pos = pos;
            param.valueChange = action;
            App.my.uiSystem.ShowPanel(PanelType.UICalculatorPanel, param);
        }

        /// <summary>
        /// 通用屏蔽字提示
        /// </summary>
        static public bool CheckSensitiveWord(string text)
        {
            //字库筛选
            if (TextRegexParser.ContainsSensitiveWord(text))
            {
                xys.UI.Utility.TipContentUtil.Show("systemHint_sensitiveWord");
                return false;
            }
            return true;
        }
    }
}
#endif