using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

namespace xys.UI
{
    using State;

    public class HotTablePage : MonoBehaviour, ILSerialized
    {
        // 父窗口
        public HotTablePanel parent { get; set; }
        
        [SerializeField]
        StateRoot stateRoot;

        public StateRoot ToggleBtn
        {
            get { return stateRoot; }

#if UNITY_EDITOR
            [EditorField]
            set { stateRoot = value; }
#endif
        }

        [SerializeField]
        [HideInInspector]
        string typeName; // 对应的类型

        [SerializeField]
        [HideInInspector]
        List<Object> objs; // 对应的unit对象
        List<Object> ILSerialized.Objs { get { return objs; } }

        [SerializeField]
        [HideInInspector]
        string json; // 对应的json数据字段

        RefType refType;

#if UNITY_EDITOR
        object instance;
#endif

        public object GetObject() { return refType.Instance; }

        public string pageType
        {
            get
            {
                int pos = typeName.LastIndexOf('.');
                if (pos != -1)
                    return typeName.Substring(pos + 1);

                return typeName;
            }
        }

        public IEnumerator Init(HotTablePanel table)
        {
            parent = table;
            gameObject.SetActive(false);

            refType = new RefType(typeName, this);
            IL.MonoSerialize.MergeFrom(refType.Instance, new IL.MonoStream(new JSONObject(json), objs));
            objs = null;
            json = null;

            refType.InvokeMethod("Init", table.refType.Instance);
            var itor = (IEnumerator)refType.InvokeMethodReturn("InitSync");
            while (itor.MoveNext())
                yield return 0;
        }

        public void SetVisible(bool value)
        {
            int state = (value == true ? 1 : 0);
            if (state != stateRoot.CurrentState)
                stateRoot.CurrentState = state;
        }

        public bool IsVisible()
        {
            return stateRoot.CurrentState == 0 ? false : true;
        }

        public void Hide()
        {
            if (!gameObject.activeSelf)
                return;

            OnHide();
            gameObject.SetActive(false);
        }

        public IEnumerator Show(object args)
        {
            if (gameObject.activeSelf)
                yield break;

            gameObject.SetActive(true);
            try
            {
                OnShow(args);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }

            IEnumerator itor;
            try
            {
                itor = OnShowSync(args);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                yield break;
            }

            bool isNext = false;
            try
            {
                isNext = itor.MoveNext();
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
                yield break;
            }

            while (isNext)
            {
                yield return 0;
                try
                {
                    isNext = itor.MoveNext();
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                    yield break;
                }
            }
        }

        protected virtual void OnShow(object args)
        {
            refType.InvokeMethod("Show", args);
        }

        protected virtual IEnumerator OnShowSync(object args)
        {
            IEnumerator ator = (IEnumerator)refType.InvokeMethodReturn("ShowSync", args);
            while (ator.MoveNext())
                yield return 0;

            yield break;
        }

        protected void OnHide()
        {
            refType.InvokeMethod("Hide");
        }
    }
}