using UnityEngine;
using System.Collections;

namespace xys.UI
{
    [System.Serializable]
    class PlayAnimData
    {
        public PlayAnimData()
        {
            IsHideState = false;
        }

        public Animator stateSwitchAnim; // 状态切换动画
        public RuntimeAnimatorController hideState; // 隐藏状态动画
        public RuntimeAnimatorController restoreState; // 恢复状态动画

        PanelAnimator stateAnimator; // 状态动画

        // 当前是否隐藏状态下
        public bool IsHideState { get; private set; }

        public void Init(MonoBehaviour behaviour)
        {
            this.behaviour = behaviour;
            stateAnimator = new PanelAnimator(stateSwitchAnim);
        }

        MonoBehaviour behaviour;

        IEnumerator PlayState()
        {
            bool value = isHideState;
            IEnumerator ator = stateAnimator.PlayAnim(value ? hideState : restoreState, () => { return value ? 1 : 0; });
            while (ator.MoveNext())
                yield return 0;
        }

        public bool isHideState
        {
            get { return IsHideState; }
            set
            {
                if (IsHideState == value)
                    return;

                IsHideState = value;
                behaviour.StartCoroutine(PlayState());
            }
        }
    }

    public class UIMainPanel : UIHotPanel
    {
        [SerializeField]
        PlayAnimData showHideState;

        [SerializeField]
        PlayAnimData activtyState;

        protected override void OnInit()
        {
            base.OnInit();
            showHideState.Init(this);
            activtyState.Init(this);
        }

        public bool isHideState
        {
            get { return showHideState.isHideState; }
            set
            {
                showHideState.isHideState = value;
            }
        }

        public bool isActivtyHideState
        {
            get { return activtyState.isHideState; }
            set
            {
                activtyState.isHideState = value;
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                isHideState = !isHideState;
            }

            if (Input.GetKeyDown(KeyCode.B))
            {
                isActivtyHideState = !isActivtyHideState;
            }
        }
    }
}