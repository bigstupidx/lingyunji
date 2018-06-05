#if MEMORY_CHECK
using MonoBehaviour = MemoryMonoBehaviour;
#endif
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using WXB;

namespace xys.UI
{
    public abstract class DialogBase : MonoBehaviour
    {
        [SerializeField]
        protected SymbolText mMessage; // 消息

        [SerializeField]
        protected Text mTitle; // 标题

        protected DialogType dialogType;

        protected Animator openOrCloseAnimator; // 打开或关闭面板动画

        public DialogType DialogType { get { return dialogType; } }

        // 是否模态对话框,
        // 模态对话框当前消息只会在此对话框内响应
        // 非模态不会阻挡其他控件的消息响应
        public bool isModal { get; protected set; }

        protected System.Action m_hideFun = null;

        [FMODUnity.EventRef]
        public string openSound;

        [FMODUnity.EventRef]
        public string closeSound;

        // 面板当前状态
        public enum State
        {
            ShowAnim, // 开始播放界面打开的动画
            Show, // 显示状态中
            Hiding, // 开始播放界面关闭的动画
            Hide, // 隐藏了,面板都成未激活状态了
        }

        public State state { get; protected set; }

        public RectTransform rectRoot { get; protected set; }

        public GameObject root { get; protected set; }

        public virtual void Init()
        {
            string tn = GetType().Name;
            dialogType = Str2Enum.To(tn, DialogType.Null);

#if UNITY_EDITOR
            if (dialogType.ToString().CompareTo(tn) != 0)
            {
                Debug.LogErrorFormat("dialog type:{0} class:{1} not same!", dialogType, tn);
                Destroy(gameObject);
                return;
            }
#endif
            root = gameObject;
            rectRoot = GetComponent<RectTransform>();
            openOrCloseAnimator = GetComponent<Animator>();
            if (openOrCloseAnimator != null)
                openOrCloseAnimator.enabled = false;
            state = State.Hide;

            SoundMgr.CheckSoundEvent(ref openSound, "ui_open_interface");
            SoundMgr.CheckSoundEvent(ref closeSound, "ui_close_interface");

            OnInit();
        }

        public bool isVisible
        {
            get
            {
                switch (state)
                {
                case State.Show:
                case State.ShowAnim:
                case State.Hiding:
                    return true;
                }

                return false;
            }
        }

        public virtual void OnInit() { }

        protected abstract void OnShow(Dialog.Data data);

        IEnumerator PlayAnim(bool forward)
        {
            if (openOrCloseAnimator.runtimeAnimatorController != null)
            {
                State current = state;

                openOrCloseAnimator.enabled = false;
                openOrCloseAnimator.Rebind();
                openOrCloseAnimator.updateMode = AnimatorUpdateMode.UnscaledTime;

                openOrCloseAnimator.Play(forward ? "open" : "close");
                openOrCloseAnimator.Update(0f);
                AnimatorStateInfo info = openOrCloseAnimator.GetCurrentAnimatorStateInfo(0);
                float lenght = info.length;
                yield return 0;

                if (current != state)
                    yield break;

                float total = 0f;
                while (true)
                {
                    if (current != state)
                        yield break;

                    float delta = Time.unscaledDeltaTime;
                    if (total + delta >= lenght)
                    {
                        // 结束了
                        openOrCloseAnimator.Update(lenght - total);
                        break;
                    }
                    else
                    {
                        openOrCloseAnimator.Update(delta);
                        total += delta;
                        yield return 0;
                    }
                }

                if (!forward)
                {
                    openOrCloseAnimator.Rebind();
                    openOrCloseAnimator.Play("close");
                    openOrCloseAnimator.Update(0f);
                }
            }
        }

        UIBlurRoot cacheBlurRoot;

        public bool isBlurExcep
        {
            get
            {
                if (cacheBlurRoot == null)
                    return false;

                return cacheBlurRoot.enabled;
            }

            set
            {
                if (isBlurExcep == value)
                    return;

                if (cacheBlurRoot == null)
                {
                    cacheBlurRoot = UIBlurExcepRoot.Get<UIBlurRoot>(root);
                }
                cacheBlurRoot.enabled = value;
            }
        }

        public virtual IEnumerator Show(Dialog.Data data, bool isPlayAnim)
        {
            m_hideFun = data.hideFun;
            isModal = data.isModle;
            mMessage.text = data.message;
            mTitle.text = data.title;

            switch (state)
            {
            case State.Hide:
                {
                    isBlurExcep = data.isBlur;
                    root.SetActive(true);
                    rectRoot.SetAsLastSibling();

                    SoundMgr.PlaySound(openSound);

                    if (openOrCloseAnimator != null)
                    {
                        if (isPlayAnim)
                        {
                            // 最后一桢
                            openOrCloseAnimator.Rebind();
                            openOrCloseAnimator.Play("open");
                            openOrCloseAnimator.Update(openOrCloseAnimator.GetCurrentAnimatorStateInfo(0).length);
                        }
                        else
                        {
                            // 第一桢
                            openOrCloseAnimator.Rebind();
                            openOrCloseAnimator.Play("open");
                            openOrCloseAnimator.Update(0);
                        }
                    }

                    state = State.ShowAnim;
                    OnShow(data);

                    if (!isVisible)
                        yield break;

                    // 先初始化面板数据
                    if (isPlayAnim && openOrCloseAnimator != null)
                    {
                        yield return PlayAnim(true);

                        if (state != State.ShowAnim)
                            yield break;
                    }

                    state = State.Show;
                }
                break;
            case State.Show: // 当前已经处于显示当中
                {
                    OnShow(data);
                }
                break;
            default:
                Debuger.ErrorLog("Show state:{0} error!", state);
                break;
            }
        }

        public virtual void Hide(bool isPlayAnim)
        {
            if (isVisible)
                StartCoroutine(HideImp(isPlayAnim));
        }

        protected virtual IEnumerator HideImp(bool isPlayAnim)
        {
            switch (state)
            {
            case State.Hide:
                yield break;

            case State.Hiding:
                break;

            case State.Show:
            case State.ShowAnim:
                {
                    SoundMgr.PlaySound(closeSound);

                    // 先初始化面板数据
                    if (isPlayAnim && openOrCloseAnimator != null)
                    {
                        state = State.Hiding;
                        yield return PlayAnim(false);
                        if (state != State.Hiding)
                            yield break;
                    }
                }
                break;
            }

            OnHide();

            state = State.Hide;

            if (m_hideFun != null)
            {
                m_hideFun();
            }

            root.SetActive(false);

            xys.App.my.uiSystem.dialogMgr.Free(this);
        }

        protected virtual void OnHide() { }

        protected virtual void OnDisable()
        {
            if (state != State.Hide)
            {
#if UNITY_EDITOR
                if (xys.App.my.uiSystem != null)
                    return;
#endif
                xys.App.my.uiSystem.StartCoroutine(HideImp(false));
            }
        }

        public virtual void Release()
        {
            m_hideFun = null;
            isModal = false;
            mTitle.text = null;
            mMessage.text = null;
            OnRelease();
        }

        protected virtual void OnRelease()
        {

        }

        protected void OnClickBtn(System.Func<bool> fun)
        {
            if (fun != null)
            {
                if (!fun())
                {
                    fun = null;
                    Hide(true);
                }
            }
            else
            {
                Hide(true);
            }
        }
    }
}