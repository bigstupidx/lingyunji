using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using PackTool;
using WXB;
using System.Text.RegularExpressions;
using Config;
using UnityEngine.Events;

namespace xys.UI
{
    public static class Helper
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T : Component
        {
            T mono = go.GetComponent<T>();
            if (mono == null)
                mono = go.AddComponent<T>();
            return mono;
        }

        public static void SetClick(Button btn, UnityAction onClick)
        {
            //不管有没有，先删掉,lua传进来的必定不同，所以这里只能全部删掉
            btn.onClick.RemoveAllListeners(); //btn.onClick.RemoveListener(onClick);
            btn.onClick.AddListener(onClick);
        }
        
        public static void SetSprite(this Image image, string name)
        {
            if (image == null)
                return;

            if (string.IsNullOrEmpty(name))
            {
                image.sprite = null;
                return;
            }

#if USE_RESOURCESEXPORT || USE_ABL
            Sprite s = null;
            if (SpritesLoad.Get(name, out s))
            {
                if (s == null)
                {
                    // 再去重新加载下
                    SpritesLoad.Load(name, (Sprite sprite, object p) => 
                    {
                        if (image != null)
                            image.sprite = sprite;
                    }, null);
                }
                else
                {
                    image.sprite = s;
                }
            }
            else
            {
                image.sprite = null;
                Debug.LogErrorFormat("Sprite:{0} not find!", name);
            }
#else
            image.sprite = SpritesLoad.Get(name);
#endif
        }

        //static Dictionary<string, Sprite> TextureToSprite = new Dictionary<string,Sprite>();

        static Sprite Create(Texture2D t)
        {
            int width = t.width;
            int height = t.height;

            return Sprite.Create(t, new Rect(0f, 0f, width, height), new Vector2(width / 2, height / 2));
        }

        static public void SetLayer(Component component, int layer)
        {
            SetLayer(component.gameObject, layer);
        }

        static public void SetLayer(GameObject go, int layer)
        {
            go.layer = layer;

            Transform t = go.transform;

            for (int i = 0, imax = t.childCount; i < imax; ++i)
            {
                Transform child = t.GetChild(i);
                SetLayer(child.gameObject, layer);
            }
        }

        // 创建一个UI特效
        static public void CreateEffect(GameObject parent, GameObject effect, Canvas sortLayer = null, int relativeSortingOrder = 1)
        {
            GameObject go = Object.Instantiate(effect);
            SetLayer(go, parent.layer);

            Transform child = go.transform;
            child.parent = parent.transform;
            child.localPosition = Vector3.zero;
            child.localScale = Vector2.one;

            UIFxRQGraphic fxRQ = go.GetOrAddComponent<UIFxRQGraphic>();
            fxRQ.canvas = sortLayer;
            fxRQ.relativeSortingOrder = relativeSortingOrder;
        }

        //返回字数限制
        static public bool TextLimit(ref string text, int maxLength)
        {
            //byte[] byteStr = System.Text.Encoding.Default.GetBytes(text);
            //计算总长度
            int length = GetTextLength(text);

            if (maxLength > 0 && length > maxLength)
            {
                text = text.Substring(0, text.Length - 1);
                while (GetTextLength(text) > maxLength)
                {
                    text = text.Substring(0, text.Length - 1);
                }
                return true;
            }
            return false;
        }

        //取得字节长度
        static int GetTextLength(string text)
        {
            char[] charList = text.ToCharArray();
            int length = 0;
            for (int i = 0; i < charList.Length; ++i)
            {
                if (CheckStringChineseUN(charList[i]))
                {
                    length += 2;
                }
                else
                {
                    length += 1;
                }
            }

            return length;
        }

        //ASCII码判断
        static bool CheckStringChinese(char text)
        {
            return (int)text > 127;
        }

        //UNICODE判断
        static bool CheckStringChineseUN(char text)
        {
            return text >= 0x4e00 && text <= 0x9fbb;
        }

        //判断是否是中文
        static Regex regChina = new Regex("^[^\x00-\xFF]");
        public static bool CheckStringChineseReg(char text)
        {
            return regChina.IsMatch(text.ToString());
        }

        //判断是否是数字
        static Regex regNum = new Regex("^[0-9]");
        public static bool CheckStringNumReg(char text)
        {
            return regNum.IsMatch(text.ToString());
        }

        //判断是否是字母
        static Regex regCharacter = new Regex(@"[a-zA-Z]+");
        public static bool CheckStringCharacterReg(char text)
        {
            return regCharacter.IsMatch(text.ToString());
        }

        /// <summary>
        /// 设置RectTransform的父节点
        /// </summary>
        /// <param name="target"></param>
        /// <param name="parentObject"></param>
        public static void SetRectTransformParent(Transform target, Transform parentObject)
        {
            if (parentObject == null)
                return;

            target.SetParent(parentObject);
            target.localScale = Vector3.one;
            if (target is RectTransform)
            {
                RectTransform rt = (RectTransform)target;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.anchoredPosition3D = Vector3.zero;
                rt.sizeDelta = Vector2.zero;
            }
            else
            {
                target.localPosition = Vector3.zero;
                target.localEulerAngles = Vector3.zero;
            }
        }

        public static UnityEngine.EventSystems.BaseRaycaster CheckRaycaster(GameObject go)
        {
            var br = go.GetComponent<UnityEngine.EventSystems.BaseRaycaster>();
            if (br != null)
            {
                if (!(br is UI.GraphicRaycaster))
                {
                    Object.Destroy(br);
                    return go.AddComponent<UI.GraphicRaycaster>();
                }
                else
                {
                    return br;
                }
            }

            return go.AddComponent<UI.GraphicRaycaster>();
        }

        public static void SetParent(this GameObject go, Transform parent )
        {
            go.transform.SetParent(parent);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;

            RectTransform rectTrans = go.transform  as RectTransform;
            if (rectTrans!=null)
            {
                rectTrans.anchorMin = Vector2.zero;
                rectTrans.anchorMax = Vector2.one;
                rectTrans.offsetMin = Vector2.zero;
                rectTrans.offsetMax = Vector2.zero;
            }

        }

        //查找组件
        public static T FindComnpnent<T>(Transform root, string path,bool logError=true) where T : Component
        {
            Transform trans = root.Find(path);
            if (trans != null)
                return trans.GetComponent<T>();
            else if (logError)
                Debug.LogError(string.Format("找不到组件{0} path={1}",typeof(T),path));
            return null;
        }

        public static string GetItemColorByQuality(ItemQuality quality)
        {
            return string.Format("#[{0}]", QualitySourceConfig.Get(quality).colorname);
        }
    }
}