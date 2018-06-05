using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace XTools
{
    public class TimerFrame
    {
        // 返回值为true，不移除，返回falsh从其中移除，即下桢不更新
        public delegate bool UPDATE(System.Object p);

        // 桢更新回调
        public class Frame
        {
            public Frame()
            {

            }

            public void Reset(UPDATE f, System.Object p)
            {
                fun = f;
                param = p;
                bCannel = false;
            }

            public UPDATE fun;
            public System.Object param;
            public bool bCannel;

#if COM_DEBUG
            public float delayTime = 0.0f;
#endif
            public void Release()
            {
                fun = null;
                param = null;
                bCannel = false;
#if COM_DEBUG
                delayTime = 0.0f;
#endif
            }
        }

        List<Frame> m_frameList = new List<Frame>();
        List<Frame> m_frameList1 = new List<Frame>();

        static void ReleaseFree(Frame frame)
        {
            frame.Release();
            Buff<Frame>.Free(frame);
        }

        static Frame GetEmpty(UPDATE f, System.Object p)
        {
            Frame frame = Buff<Frame>.Get();

            frame.Reset(f, p);

            return frame;
        }

        public static int frameCount { get; protected set; }

        // 添加一个桢更新回调
        public void addFrameUpdate(UPDATE f, System.Object p)
        {
            Frame frame = GetEmpty(f, p);
            m_frameList.Add(frame);
        }

        // 注意Frame用对象池管理，在update返回为false之后，会被回收重新利用!!!!
        public Frame Add(UPDATE f, System.Object p)
        {
            Frame frame = GetEmpty(f, p);
            m_frameList.Add(frame);
            return frame;
        }

        static void SwapFrameList(ref List<Frame> t1, ref List<Frame> t2)
        {
            List<Frame> t3 = t1;
            t1 = t2;
            t2 = t3;
        }

        Frame u = null;

        public void checkFrameUpdate()
        {
#if COM_DEBUG
            float deltaTime = Time.unscaledDeltaTime;
#endif
            SwapFrameList(ref m_frameList1, ref m_frameList);

            for (int i = 0; i < m_frameList1.Count; ++i)
            {
                u = m_frameList1[i];
                if (u.bCannel == false)
                {
#if COM_DEBUG
                    u.delayTime -= deltaTime;
                    if (u.delayTime > 0)
                    {
                        m_frameList.Add(u);
                        continue;
                    }
#endif
                    try
                    {
                        if (u.fun(u.param) == true)
                        {
                            m_frameList.Add(u);
                        }
                        else
                        {
                            ReleaseFree(u);
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Debuger.LogException(ex);
                    }
                }
                else
                {
                    u.fun = null;
                    u.param = null;
                    ReleaseFree(u);
                }
            }

            m_frameList1.Clear();
        }
    }
}