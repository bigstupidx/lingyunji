using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace xys.UI.State
{
    // 元素的类型,可以随意增删，调整顺序，但是不可改变名称
    public enum Type
    {
        Go,

        UImage,
        UText,
        UAlpha,
        UWidth,
        UHeight,
        UColor,
        UMaterial,
        UGradient,

        Pos,
        Rotate,
        Scale,
        StateRoot,
        PlayAnim,

        //
        Max,
    }

    [System.Serializable]
    public class Element
    {
        [SerializeField]
        string type_key;

        public Type type
        {
            get { return Str2Enum.To<Type>(type_key, default(Type)); }
            set { type_key = value.ToString(); }
        }
        
        public Object target; // 目标对象
        public ElementStateData[] stateData; // 状态数据

        public T GetTarget<T>() where T : Object
        {
            return target as T;
        }

        public ElementAgent Agent { get { return Factory.GetAgent(this); } }

#if UNITY_EDITOR
        public void AddState()
        {
            int lenght = stateData == null ? 0 : stateData.Length;
            System.Array.Resize<ElementStateData>(ref stateData, lenght + 1);
            stateData[lenght] = new ElementStateData();

            Agent.Init(this, stateData[lenght]);
        }

        public void RemoveState(int index)
        {
            XTools.Utility.ArrayRemove(ref stateData, index);
        }

        public string Name
        {
            get { return Factory.Names[(int)type];}
        }
#endif
    }

    // 注意，派生出来的子类不允许有任何的成员变量，只能有函数，因为这个东西，只会有一份实例
    public abstract class ElementAgent
    {
        public abstract void Init(Element element, ElementStateData sd);

        public abstract void Set(Element element, int index);

        // 是否允许渐变
        public virtual bool isSmooth { get { return false; } }

#if UNITY_EDITOR
        public abstract bool ShowState(Element element, ElementStateData sc, int index);

        public abstract void ShowElementTarget(Element element);

        public abstract void ShowElementState(Element element, int stateid, bool iscanset);

        public string Name { get; set; }

        public static System.Func<StateRoot> GetCurrentStateRoot = null;

        protected static StateRoot GetStateRoot()
        {
            if (GetCurrentStateRoot == null)
                return null;

            return GetCurrentStateRoot();
        }

        static public void RegisterUndo(System.Action action, params Object[] objs)
        {
            object o = null;
            RegisterUndo<object>((ref object vv) => 
            {
                action();
            },
            ref o, objs);
        }

        public delegate void ActionRef<T>(ref T obj);

        static public void RegisterUndo<T>(ActionRef<T> action, ref T t, params Object[] objs)
        {
            List<Object> rs = new List<Object>();
            StateRoot sr = GetStateRoot();
            if (sr != null)
            {
                rs.Add(sr);
            }

            if (objs != null)
            {
                rs.AddRange(objs);
            }

            UnityEditor.Undo.RecordObjects(rs.ToArray(), "State Root Change");
            for (int i = 0; i < rs.Count; ++i)
                UnityEditor.EditorUtility.SetDirty(rs[i]);
            
            action(ref t);

            for (int i = 0; i < rs.Count; ++i)
                UnityEditor.EditorUtility.SetDirty(rs[i]);
        }

        static public bool ShowVector3(ref Vector3 v3, string name)
        {
            Vector3 v = UnityEditor.EditorGUILayout.Vector3Field(name, v3);
            if (v3 == v)
            {
                return false;
            }

            RegisterUndo<Vector3>((ref Vector3 vv) => 
            {
                vv = v;
            }, 
            ref v3);

            return true;
        }

        static public bool ShowColor32(ref Color32 c32, string name)
        {
            Color32 v = UnityEditor.EditorGUILayout.ColorField(name, c32);
            if (XTools.Utility.Color32Equal(ref v, ref c32))
            {
                return false;
            }

            RegisterUndo<Color32>((ref Color32 vv) =>
            {
                vv = v;
            },
            ref c32);

            return true;
        }

        static public bool ShowString(ref string str)
        {
            string v = UnityEditor.EditorGUILayout.TextField(str);
            if (v == str)
            {
                return false;
            }

            RegisterUndo<string>((ref string vv) =>
            {
                vv = v;
            },
            ref str);

            return true;
        }

        static public bool ShowSliderFloat(ref Vector3 v3, string name, float min, float max)
        {
            float x = UnityEditor.EditorGUILayout.Slider(name, v3.x, min, max);
            if (x == v3.x)
                return false;

            RegisterUndo<Vector3>((ref Vector3 vv) => 
            {
                vv.x = x;
            },
            ref v3);

            return true;
        }

        static public bool ShowFloat(ref Vector3 v3, string name)
        {
            float x = UnityEditor.EditorGUILayout.FloatField(v3.x);
            if (x == v3.x)
                return false;

            RegisterUndo<Vector3>((ref Vector3 vv) =>
            {
                vv.x = x;
            },
            ref v3);

            return true;
        }

        static public bool ShowIntField(ref int intv, string name)
        {
            int x = UnityEditor.EditorGUILayout.IntField(name, intv);
            if (x == intv)
                return false;

            RegisterUndo<int>((ref int vv) =>
            {
                vv = x;
            },
            ref intv);

            return true;
        }

        static public bool ShowToggle(ref bool bv, string name)
        {
            bool x = UnityEditor.EditorGUILayout.Toggle(name, bv);
            if (x == bv)
                return false;

            RegisterUndo<bool>((ref bool vv) =>
            {
                vv = x;
            },
            ref bv);

            return true;
        }
#endif
    }

    public class Factory
    {
#if UNITY_EDITOR
        public static string[] Names = new string[(int)Type.Max];
#endif
        static ElementAgent[] factorys = new ElementAgent[(int)Type.Max];

        static T Create<T>(string name) where T : new()
        {
            T t = new T();
#if UNITY_EDITOR
            ElementAgent tt = t as ElementAgent;
            tt.Name = name;
#endif
            return t;
        }

        static Factory()
        {
            factorys[(int)Type.Go] = Create<GoEA>("对象");

            // UGUI相关的
            {
                factorys[(int)Type.UImage] = Create<UImageEA>("U精灵");
                factorys[(int)Type.UText] = Create<UTextEA>("U文本");
                factorys[(int)Type.UAlpha] = Create<UAlphaEA>("U透明");
                factorys[(int)Type.UWidth] = Create<UWidthEA>("U宽度");
                factorys[(int)Type.UHeight] = Create<UHeightEA>("U高度");
                factorys[(int)Type.UColor] = Create<UColorEA>("U颜色");
                factorys[(int)Type.UMaterial] = Create<UMaterialEA>("U材质");
                factorys[(int)Type.UGradient] = Create<UGradientEA>("U颜色渐变");
            }

            factorys[(int)Type.Pos] = Create<PositionEA>("位置");
            factorys[(int)Type.Rotate] = Create<RotateEA>("旋转");
            factorys[(int)Type.Scale] = Create<ScaleEA>("缩放");
            factorys[(int)Type.StateRoot] = Create<StateRootEA>("状态");
            factorys[(int)Type.PlayAnim] = Create<PlayAnim>("动画");
            
#if UNITY_EDITOR
            for (int i = 0; i < (int)Type.Max; ++i)
            {
                Names[i] = (factorys[i] == null ? ((Type)i).ToString() : factorys[i].Name);
            }
#endif
        }

        static public ElementAgent GetAgent(Element element)
        {
            return factorys[(int)element.type];
        }
    }
}