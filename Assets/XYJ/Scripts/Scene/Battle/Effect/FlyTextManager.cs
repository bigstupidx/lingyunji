using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Config;
using NetProto;
using Battle;
using xys.UI;


//飘字管理
namespace xys.battle
{
    public static class FlyTextManager
    {
        //添加一条飘字
        public static void Play(Transform root, Battle_AttackAction damage, bool targetIsMe, bool sourceIsMe)
        {
            if (AttackFlg.IsFlg(damage.damageFlg, AttackFlg.Flg.Miss))
            {

            }

            //计算类型
            int num = (int)Mathf.Abs(damage.damageValue);
            string text = "";
            HudText.enType type = HudText.enType.otherDamage;

            //闪避
            if (AttackFlg.IsFlg(damage.damageFlg, AttackFlg.Flg.Miss))
            {
                type = targetIsMe ? HudText.enType.heroZengyi : HudText.enType.zengyi;
                text = "闪避";
            }
            //暴击
            else if (AttackFlg.IsFlg(damage.damageFlg, AttackFlg.Flg.Baoji))
            {
                type = targetIsMe ? HudText.enType.heroBaoji : HudText.enType.otherBaoji;
                text = string.Format(targetIsMe ? "暴击  -{0}" : "暴击  {0}", num);
            }
            //普通攻击
            else
            {
                type = targetIsMe ? HudText.enType.heroDamage : HudText.enType.otherDamage;
                text = string.Format(targetIsMe ? "-{0}" : "{0}", num);
            }

            Play(root, type, text, sourceIsMe);
        }

        //showInEdge表明飘字在屏幕外的是否需要挪到屏幕边缘
        static void Play(Transform target, HudText.enType type, string txt, bool showInEdge)
        {
            //屏幕外的情况，释放者不是主角不创建，释放者是主角由hud系统已到屏幕边缘
            if (!showInEdge)
            {
                Camera mainCamera = App.my.cameraMgr.m_mainCamera;
                if (mainCamera == null)
                    return;
                Vector3 screenPoint = mainCamera.WorldToScreenPoint(target.position);
                //飘字距离摄像机太远也不要票了
                if (screenPoint.z >= kvBattle.ActiiveNameDistance || screenPoint.z < 0 || screenPoint.x < 0 || screenPoint.x > Screen.width || screenPoint.y < 0 || screenPoint.y > Screen.height)
                    return;
            }

            XYJObjectPool.LoadPrefab("HudText", OnLoad, new object[] { target.position, type, txt, showInEdge }, false);
        }
        static void OnLoad(GameObject go, object param)
        {
            object[] pp = param as object[];
            App.my.uiSystem.hud2DSystem.Bind(go, (Vector3)pp[0], (bool)pp[3]);
            go.GetComponent<HudText>().Play((HudText.enType)pp[1], pp[2] as string);
        }
    }

}