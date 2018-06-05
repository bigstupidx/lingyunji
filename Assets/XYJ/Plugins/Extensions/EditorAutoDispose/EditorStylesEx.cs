#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace EditorExtensions
{
    /// <summary>
    /// 主要用来实现自定义编辑器UIStyles
    /// </summary>
    public class EditorStylesEx
    {
        // 字体
        static Font _font;
        public static Font DefFont
        {
            get
            {
                if (_font == null)
                {
                    _font = Font.CreateDynamicFontFromOSFont("SimSun", 13);//宋体，unity编辑器下中文显示太丑了，想要效果好一点的话考虑用这个
                    if (_font == null)
                        _font = GUI.skin.font;
                }
                return _font;
            }
        }

        // GUI skin
        static GUISkin _skin;
        public static GUISkin DefSkin
        {
            get
            {
                if (_skin == null)
                {
                    _skin = GUI.skin;
                }
                return _skin;
            }
        }

        static GUIStyle _preferencesSectionBox;
        public static GUIStyle PreferencesSectionBox
        {
            get
            {
                if (_preferencesSectionBox == null)
                    _preferencesSectionBox = new GUIStyle("PreferencesSectionBox");
                return _preferencesSectionBox;
            }
        }

        private static GUIStyle _labelWordWrap;
        public static GUIStyle LabelWordWrap
        {
            get
            {
                if (_labelWordWrap == null)
                {
#if UNITY_EDITOR
                    _labelWordWrap = new GUIStyle(EditorStyles.label);
#else
                _labelWordWrap = new GUIStyle(GUI.skin.label);
#endif
                    _labelWordWrap.wordWrap = true;
                    _labelWordWrap.font = DefFont;
                }
                return _labelWordWrap;
            }
        }

        private static GUIStyle _textAreaWordWrap;
        public static GUIStyle TextAreaWordWrap
        {
            get
            {

                if (_textAreaWordWrap == null)
                {
#if UNITY_EDITOR
                    _textAreaWordWrap = new GUIStyle(EditorStyles.textArea);
#else
                _textAreaWordWrap = new GUIStyle(GUI.skin.textArea);
#endif
                    _textAreaWordWrap.wordWrap = true;
                    _textAreaWordWrap.font = DefFont;
                }
                return _textAreaWordWrap;
            }
        }

        static GUIStyle _boxArea;
        public static GUIStyle BoxArea
        {
            get
            {
                if (_boxArea == null)
                {
                    GUIStyle style = new GUIStyle(GUI.skin.box);

                    style.normal.textColor = Color.white;
                    style.active.textColor = Color.white;
                    style.hover.textColor = Color.white;
                    style.focused.textColor = Color.white;
                    style.border = new RectOffset(4, 4, 4, 4);//九宫格
                    style.margin = new RectOffset(0, 0, 6, 6);//边缘，只影响layout的控件
                    style.padding = new RectOffset(6, 6, 0, 4);//内部边缘
                    style.overflow = new RectOffset(1, 1, 1, 2);//内部的控件的范围
                    style.stretchWidth = true;
                    _boxArea = style;
                }
                return _boxArea;
            }
        }

        static GUIStyle _frameArea;
        public static GUIStyle FrameArea
        {
            get
            {
                if (_frameArea == null)
                {
                    GUIStyle style = new GUIStyle(GUI.skin.box);

                    style.normal.textColor = Color.white;
                    style.active.textColor = Color.white;
                    style.hover.textColor = Color.white;
                    style.focused.textColor = Color.white;
                    style.border = new RectOffset(2, 2, 2, 2);//九宫格
                    style.margin = new RectOffset(0, 0, 2, 2);//边缘，只影响layout的控件
                    style.padding = new RectOffset(0, 0, 0, 2);//内部边缘
                    style.overflow = new RectOffset(1, 1, 1, 2);//内部的控件的范围
                    style.stretchWidth = true;
                    _frameArea = style;
                }
                return _frameArea;
            }
        }

        static GUIStyle _labelAreaHeader;
        public static GUIStyle LabelAreaHeader
        {
            get
            {
                if (_labelAreaHeader == null)
                {
                    GUIStyle style = new GUIStyle(GUI.skin.label);

                    style.normal.textColor = new Color32(200, 200, 200, 255);
                    style.active.textColor = style.normal.textColor;
                    style.hover.textColor = style.normal.textColor;
                    style.focused.textColor = style.normal.textColor;
                    style.border = new RectOffset(4, 4, 4, 4);//九宫格
                    style.margin = new RectOffset(0, 0, 1, 1);//边缘，只影响layout的控件
                    style.padding = new RectOffset(6, 6, 2, 2);//内部边缘
                    style.overflow = new RectOffset(2, 0, 0, -2);
                    style.fontStyle = FontStyle.Bold;
                    style.font = DefFont;
                    style.alignment = TextAnchor.UpperLeft;
                    style.imagePosition = ImagePosition.TextOnly;
                    style.fixedHeight = 20;
                    style.stretchWidth = true;
                    _labelAreaHeader = style;
                }

                return _labelAreaHeader;
            }
        }

        static GUIStyle _tipButton;
        public static GUIStyle TipButton
        {
            get
            {
                if (_tipButton == null)
                {
                    GUIStyle style = new GUIStyle(GUI.skin.button);

                    Texture2D normalTex = EditorIconTexture.GetCustom("infoButton_normal");
                    Texture2D hoverTex = EditorIconTexture.GetCustom("infoButton_over");
                    if (normalTex != null && hoverTex != null)
                    {
                        style.normal.background = normalTex;
                        style.active.background = hoverTex;
                        style.hover.background = normalTex;
                        style.focused.background = normalTex;
                        style.onNormal.background = hoverTex;
                        style.onActive.background = normalTex;
                        style.onHover.background = hoverTex;
                        style.onFocused.background = hoverTex;
                        style.fixedHeight = 16;
                        style.fixedWidth = 16;
                    }

                    style.margin = new RectOffset(0, 0, 0, 0);//边缘，只影响layout的控件
                    style.padding = new RectOffset(1, 1, 1, 1);//内部边缘

                    style.normal.textColor = Color.white;
                    style.active.textColor = Color.white;
                    style.hover.textColor = Color.white;
                    style.focused.textColor = Color.white;
                    _tipButton = style;
                }
                return _tipButton;
            }

        }

    }
}
#endif