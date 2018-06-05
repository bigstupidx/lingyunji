#if !USE_HOT
using System;
using System.Collections.Generic;
using Config;
using NetProto;
using UI;
using UnityEngine;
using UnityEngine.UI;
using WXB;

namespace xys.hot
{
    // 表情面板由哪个面板开启
    public enum EmotionOpenType
    {
        None,
        Chat,
        Hero,
        Friend,
        Count,
    }
    public class ChatDefins
    {
        // 是否已扩展
        public const string Expand = "MainChatExpand";
        // 频道显示
        public const string ChannelShowCondition = "{0}_Show_Condition";
        // 语言播放
        public const string ChannelVoiceCondition = "{0}_Voice_Condition";

        // 语音最短时长
        public const int MinVoiceDuration = 1;
        // 语音最长时长
        public const int MaxVoiceDuratiuon = 30;

        // 语音文本敏感转化词
        public const string VoiceTextTranslation = "做一个有担当有素质的人";

        // chatPanel
        // 文本Text最低行高
        public const int TextMinHeight = 28;
        // 文本Layout的最低行高
        public const int TextLayoutMinHeight = 48;
        // chatMessageCommon最低高度
        public const int ChatMessageCommonMinHeight = 48;
        // chatInfo最低高度
        public const int ChatInfoMinHeight = 98;
        // 历史输入数量
        public const int ChatHistoryNum = 20;
        // 快捷输入数量
        public const int ChatInputSimpleNum = 20;
        // 主界面长按语音按钮事件
        public const float ChatVoiceLongPressInterval = 0.5f;
        // 主界面文本最大宽度
        public const float ChatSimpleWidthMax = 292f;
        // 主界面文本最小宽度
        public const float ChatSimpleWidthMin = 60f;
        // 主界面与文本信息的间隔
        public const float ChatSimpleSpacing = 2f;
        // 文本背景最低宽度
        public const float ChatCommonBgWidthMin = 60f;
        // 文本背景最大宽度
        public const float ChatCommonBgWidthMax = 402f;
        // 文本背景宽度修正系数
        public const float ChatCommonBgWidthFix = 34f;
        // 文本的最大宽度
        public const float ChatCommonTextWidthMax = 367f;
        // 点击时传递的数据结构
        public struct CellData
        {
            public enum Type
            {
                Item,
                Equip,
                Pet,
            }
            //-------------背包
            public int pos;// 背包索引
            public int count;// 物品数量
            public int subType;// 装备部件
            public bool isEquiped;
            public Type type;
            public ItemData itemData;
            //-------------宠物
            public int index;// 宠物槽位
            public bool isVariation;
            public PetsAttribute petsData;
        }

        // 点击传递时的历史数据
        public class History
        {
            public string message;
            public List<CellData> datas;
            public List<DataNode> nodes;
        }

        public class DataNode
        {
            public int startPos;
            public bool isValid;// 此节点是否还有效
            public string text;
            private bool init = false;
            public void AddChar(int anchorPos, int caretPos, int length, int limitPos)
            {
                var endPos = startPos + text.Length;
                // 超过限制字符数
                if(endPos >= limitPos || startPos >= limitPos)
                {
                    isValid = false;
                    text = text.Substring(0, text.Length - ( endPos - limitPos + 1 ));
                    return;
                }
                // 是否是区间黏连操作
                bool section = anchorPos != caretPos;
                if(section)
                {
                    // 起点在区间内
                    if(anchorPos > startPos && anchorPos < endPos
                    // 落点在区间内
                    || caretPos > startPos && caretPos < endPos
                    // 起点落点均在区间外
                    || caretPos >= endPos && anchorPos <= startPos
                    || caretPos <= startPos && anchorPos >= endPos)
                    {
                        isValid = false;
                    }
                    else
                    {
                        isValid = true;
                    }
                }
                else// 非区间操作以当前的caretPosition为准
                {
                    if(caretPos > startPos && caretPos < endPos)
                    {
                        isValid = false;
                    }
                    else
                    {
                        isValid = true;
                    }
                }
                // 未被编辑
                if(isValid
                // 受到影响
                && caretPos <= startPos && anchorPos <= startPos)
                {
                    if(section)
                    {
                        if(!init)
                        {
                            init = true;
                            startPos = caretPos;
                        }
                        else
                        {
                            startPos -= Mathf.Abs(anchorPos - caretPos) - length; 
                        }
                    }
                    else
                    {
                        if (!init)
                        {
                            init = true;
                        }
                        else
                        {
                            startPos++;
                        }
                    }
                }
            }

            public void Delete(int anchorPos, int caretPos, int limitPos)
            {
                var endPos = startPos + text.Length;
                // 超过限制字符数
                if(endPos >= limitPos || startPos >= limitPos)
                {
                    isValid = false;
                    text = text.Substring(0, text.Length - ( endPos - limitPos + 1 ));
                    return;
                }
                // 是否是区间黏连操作
                bool section = anchorPos != caretPos;
                if(section)
                {
                    // 起点在区间内
                    if(anchorPos > startPos && anchorPos <= endPos
                        // 落点在区间内
                        || caretPos > startPos && caretPos <= endPos
                        // 起落点均在区间外
                        || caretPos >= endPos && anchorPos <= startPos
                        || caretPos <= endPos && caretPos >= startPos)
                    {
                        isValid = false;
                    }
                    else
                    {
                        isValid = true;
                    }
                }
                else
                {
                    if(caretPos > startPos && caretPos <= endPos)
                    {
                        isValid = false;
                    }
                    else
                    {
                        isValid = true;
                    }
                }
                // 未被编辑
                if(isValid
                // 受到影响
                && caretPos <= startPos && anchorPos <= startPos)
                {
                    if(section)
                    {
                        startPos -= Mathf.Abs(anchorPos - caretPos);
                    }
                    else
                    {
                        startPos--;
                    }
                }
            }
        }
    }
}

#endif