using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif

namespace xys.UI
{
    using State;
    public abstract class TablePage<T> : MonoBehaviour
    {
        // ¸¸´°¿Ú
        public TablePanel<T> parent { get; set; }
        
        [SerializeField]
        StateRoot stateRoot;

        public StateRoot ToggleBtn
        {
            get { return stateRoot; }

#if UNITY_EDITOR
            set { stateRoot = value; }
#endif
        }

        public abstract T pageType
        {
            get;
        }

        public IEnumerator Init(TablePanel<T> table)
        {
            parent = table;

            IEnumerator itor = OnInit();
            while (itor.MoveNext())
                yield return 0;
        }

        protected virtual IEnumerator OnInit()
        {
            gameObject.SetActive(false);
            yield break;
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

        public virtual void Hide()
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
            OnShow(args);
            IEnumerator itor = OnShowSync(args);
            while (itor.MoveNext())
                yield return 0;
        }

        protected virtual void OnShow(object args) { }
        protected virtual IEnumerator OnShowSync(object args)
        {
            yield break;
        }

        protected virtual void OnHide()
        {

        }
    }
}