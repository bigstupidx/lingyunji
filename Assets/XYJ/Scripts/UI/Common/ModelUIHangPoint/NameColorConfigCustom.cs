using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Config
{
    public partial class NameColorConfig
    {

        public enum ColorType : int
        {
            PlayerGreen = 1,            //玩家绿名
            PlayerYellow = 2,           //玩家黄名
            PlayerGray = 3,             //玩家灰名
            PlayerRed = 4,              //玩家红名

            Partner = 5,                //伙伴名称
            Pet = 6,                    //宠物名称
            NPC = 7,                    //NPC名称
            InitMon = 8,                //主动怪名
            PassMonBattle = 9,          //被动怪名-战斗时
            PassMonIdle = 10,           //被动怪名-脱战时

            PlayerTitleColor = 11,
            PlayerWhiteCall = 11,       //玩家称号白
            PlayerGreenCall = 12,       //玩家称号绿
            PlayerBlueCall = 13,        //玩家称号蓝
            PlayerPurpleCall = 14,      //玩家称号紫
            PlayerOrangeCall = 15,      //玩家称号橙
            PlayerRedCall = 16,         //玩家称号红

            NpcCall = 17,               //NPC称号
            InitMonCall = 18,           //主动怪称号
            PassMonBattleCall = 19,     //被动怪称号-战斗时
            PassMonIdleCall = 20,       //被动怪称号-脱战时

        }

        public static Color[] GetColors(ColorType type)
        {
            NameColorConfig cfg = Get((int)type);
            if (cfg == null)
                return new Color[] { Color.white, Color.white };
            else
                return new Color[] { ToColor(cfg.mainColor), ToColor(cfg.outlineColor) };
        }

        //十六进制转rgb
        public static Color ToColor(string colorName)
        {
            if (string.IsNullOrEmpty(colorName))
            {
                return Color.white;
            }
            if (colorName.StartsWith("#"))
                colorName = colorName.Replace("#", string.Empty);
            int v = 0;
            if (!int.TryParse(colorName, System.Globalization.NumberStyles.HexNumber, null, out v))
            {
                Debuger.LogError("ColorName Parse Error:" + colorName);
            }
            return new Color()
            {
                //a = Convert.ToByte((v >> 24) & 255) / 255f,
                a = 1,
                r = System.Convert.ToByte((v >> 16) & 255) / 255f,
                g = System.Convert.ToByte((v >> 8) & 255) / 255f,
                b = System.Convert.ToByte((v >> 0) & 255) / 255f
            };
        }

    }
}
