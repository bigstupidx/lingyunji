#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using PackTool;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace xys.UI
{
    public class DialogMgr : MonoBehaviour
    {
        [System.Serializable]
        class Data
        {
            public GameObject template; // 模版
            public List<DialogBase> shows = new List<DialogBase>(); // 当前显示的聊天
            public List<DialogBase> pools = new List<DialogBase>(); // 当前被回收的对象

            DialogBase GetByPool()
            {
                DialogBase db = null;
                for (int i = pools.Count - 1; i >= 0; --i)
                {
                    db = pools[i];
                    pools.RemoveAt(i);
                    if(db != null)
                        return db;
                }

                return db;
            }

            public DialogBase Get()
            {
                DialogBase db = GetByPool();
                if (db == null)
                {
                    db = Instantiate(template).GetComponent<DialogBase>();
                    db.name = template.name;
                    db.Init();
                }

                shows.Add(db);
                return db;
            }

            public void Free(DialogBase db)
            {
                shows.Remove(db);
                if (pools.Count >= 10)
                {
                    Object.Destroy(db.root);
                    db = null;
                }
                else
                {
                    pools.Add(db);
                }
            }

            public void Destroy()
            {
                if (template != null)
                    Object.Destroy(template);

                for (int i = 0; i < shows.Count; ++i)
                    shows[i].Hide(false);

                for (int i = 0; i < shows.Count; ++i)
                    Object.Destroy(shows[i].root);

                for (int i = 0; i < pools.Count; ++i)
                    Object.Destroy(pools[i].root);

                shows.Clear();
                pools.Clear();
                template = null;
            }
        }

        [SerializeField]
        Data[] Dialogs;

        //List<GameObject>[] Pools = new List<GameObject>[(int)DialogType.Total];

        EmptyGraphic m_ModalGraphic;
        EmptyGraphic modalGraphic
        {
            get
            {
                if (m_ModalGraphic == null)
                    m_ModalGraphic = GetComponent<EmptyGraphic>();

                return m_ModalGraphic;
            }
        }

        GraphicRaycaster m_GraphicRaycaster;

        // 是否开启模态
        public bool isModle
        {
            get { return modalGraphic.isActiveAndEnabled; }
            set { modalGraphic.enabled = value; }
        }

#if UNITY_EDITOR
        void Awake()
        {
            if (App.my == null)
            {
                gameObject.SetActive(false);
            }
        }
#endif

        public IEnumerator Init()
        {
            m_GraphicRaycaster = Helper.CheckRaycaster(gameObject) as GraphicRaycaster;
            m_GraphicRaycaster.enabled = false;

            rectTransform = GetComponent<RectTransform>();
            Dialogs = new Data[(int)DialogType.Total];
            int total = 0;
            for (int i = 1; i < (int)DialogType.Total; ++i)
            {
                ++total;
                string dtn = ((DialogType)i).ToString();
                RALoad.LoadPrefab(string.Format("Dialog/{0}", dtn), 
                    (GameObject go, object p)=> 
                    {
                        --total;
                        DialogType dt = (DialogType)p;
                        if (go == null)
                        {
                            Debug.LogErrorFormat("dt:{0} not find resources!", dt);
                            return;
                        }

                        Dialogs[(int)dt] = new Data();

#if UNITY_EDITOR
                        bool isactive = go.activeSelf;
#endif
                        go.SetActive(false);
                        Dialogs[(int)dt].template = Object.Instantiate(go);
                        Dialogs[(int)dt].template.name = go.name;
                        Dialogs[(int)dt].template.transform.SetParent(rectTransform);
#if UNITY_EDITOR
                        if (isactive)
                            go.SetActive(isactive);
#endif
                    }, 
                    (DialogType)i, 
                    false, true);
            }

            while (total != 0)
                yield return 0;

            isModle = false;
        }

        RectTransform rectTransform;

        int modal_total = 0; // 当前总的模态对话框总数
        int show_total = 0; // 显示的对话框总数 

        public T Show<T>(Dialog.Data data, bool isPlayAnim) where T : DialogBase
        {
            DialogType dt = Str2Enum.To(typeof(T).Name, DialogType.Null);
            if (dt == DialogType.Null)
                return null;

            Data d = Dialogs[(int)dt];
            DialogBase db = d.Get();
            db.rectRoot.SetParent(rectTransform);
            db.rectRoot.offsetMin = Vector2.zero;
            db.rectRoot.offsetMax = Vector2.zero;
            db.rectRoot.localEulerAngles = Vector3.zero;
            db.rectRoot.localScale = Vector3.one;
            StartCoroutine(db.Show(data, isPlayAnim));

            if (db.isModal)
            {
                ++modal_total;
                isModle = true;
            }

            ++show_total;
            if (show_total == 1)
                m_GraphicRaycaster.enabled = true;

            return db as T;
        }

        // 回收此对话框
        public void Free(DialogBase db)
        {
            if (db.isModal)
            {
                --modal_total;
                if (modal_total == 0)
                {
                    isModle = false;
                }
            }

            --show_total;
            if (show_total == 0)
            {
                m_GraphicRaycaster.enabled = false;
            }

            db.Release();
            Dialogs[(int)db.DialogType].Free(db);
        }

        //void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.A))
        //    {
        //        Dialog.TwoBtn.Show("这是测试", "这是测试", "确定", () => { return false; }, "取消", null, true);
        //    }
        //    else if (Input.GetKeyDown(KeyCode.B))
        //    {
        //        Dialog.OneBtn.Show("这是测试", "这是测试", "确定", () => { return false; }, true);
        //    }
        //}
    }
}
