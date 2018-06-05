using System;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace xys.UI
{
    namespace SystemHint
    {
        public class UISystemHint : MonoBehaviour
        {
            [SerializeField]
            Text content;
            [SerializeField]
            Image bg;
            [SerializeField]
            CanvasGroup group;

            [SerializeField]
            float startHeight = 120;        //起始位置
            [SerializeField]
            float endHeight = 180;          //结束位置
            [SerializeField]
            float offsetMoveY = 2f;         //运动偏移量
            [SerializeField]
            float durationWaitDis = 1.5f;   //等待消失时间（包括运行时间）
            [SerializeField]
            float durationDisappear = 0.5f; //消失时间
            [SerializeField]
            float offsetHeight = 35;        //UI直接偏移量
            [SerializeField]
            const float m_minWidth = 400f;  //UI最小宽度
            EventAgent events = new EventAgent();

            int m_posNum = 0;
            bool m_isMove = false;
            bool m_isAlphaChange = false;
            bool m_isFire = false;
            float m_from;
            float m_to;
            UGUITweenAlpha ta;

            public void OnShow(string str)
            {
                content.text = str;

                content.rectTransform.sizeDelta = new Vector2(content.preferredWidth + 100, content.rectTransform.sizeDelta.y);
                bg.rectTransform.sizeDelta = new Vector2(Mathf.Max(m_minWidth, content.rectTransform.sizeDelta.x), bg.rectTransform.sizeDelta.y);

                events.Subscribe(HintEventID.Level1Finish.ToString(), OnChangeLevel);
                if (offsetHeight == 0)
                    offsetHeight = bg.GetComponent<RectTransform>().sizeDelta.y;
                transform.localPosition = new Vector3(0, startHeight, 0);
                gameObject.SetActive(true);
                ShowHintMoveY(startHeight, endHeight);

            }

            void OnChangeLevel()
            {
                switch (m_posNum)
                {
                    case 0:
                        break;
                    case 1:
                        ShowHintMoveY(endHeight, endHeight + offsetHeight);
                        break;
                    case 2:
                        ShowHintMoveY(endHeight + offsetHeight, endHeight + offsetHeight * 2);
                        break;
                    case 3:
                        if (this != null)
                        {
                            if (ta != null)
                            {
                                ta.RemoveOnFinished(() =>
                                {
                                    Finish();
                                });
                            }
                            if (events != null)
                            {
                                events.Release();
                                events = null;
                            }
                            DestroyImmediate(gameObject);
                        }
                        break;
                    default:
                        break;
                }
                m_posNum++;
            }

            //提示条向上运动from < to
            public void ShowHintMoveY(float from, float to)
            {
                m_from = from;
                m_to = to;

                if (m_from < m_to)
                    m_isMove = true;
            }

            void FixedUpdate()
            {
                if (m_isMove)
                {
                    if (transform.localPosition.y >= endHeight - offsetHeight - offsetMoveY)
                    {
                        if (!m_isFire)
                        {
                            events.fireEvent(HintEventID.Level1Finish.ToString());
                            m_isFire = true;
                            m_posNum = 1;
                        }
                    }

                    if (transform.localPosition.y >= endHeight)
                    {
                        if (!m_isAlphaChange)
                            MoveEnd();
                    }

                    if (transform.localPosition.y >= m_to)
                    {
                        transform.localPosition = new Vector3(transform.localPosition.x, m_to, transform.localPosition.z);
                        m_isMove = false;
                    }
                    else
                    {
                        transform.localPosition += new Vector3(0, offsetMoveY, 0);
                    }
                }

                if (m_isAlphaChange)
                {
                    group.alpha = ta.value;
                }
            }

            void MoveEnd()
            {
                ta = transform.GetComponent<UGUITweenAlpha>();
                ta.AddOnFinished(() =>
                {
                    m_isAlphaChange = false;
                    Finish();
                });
                ta.delay = durationWaitDis;
                ta.duration = durationDisappear;

                m_isAlphaChange = true;
                ta.enabled = true;

            }

            void Finish()
            {
                if (gameObject != null)
                {
                    gameObject.SetActive(false);
                    DestroyImmediate(gameObject);
                }
            }

            void OnDestroy()
            {
                if (events != null)
                {
                    events.Release();
                    events = null;
                }
            }
        }
    }
}