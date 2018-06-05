using System;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace xys.UI
{
    public static class EventHandler
    {
        public static bool Execute<T>(GameObject target, BaseEventData eventData, Handler<T> functor) where T : IEventSystemHandler
        {
            if (functor.Execute(target, eventData))
            {
                bool v = ExecuteEvents.Execute(target, eventData, functor.handler);
                functor.ExecuteEnd(target, eventData);
                return v;
            }

            return true;
        }

        private static readonly List<Transform> s_InternalTransformList = new List<Transform>(30);

        private static void GetEventChain(GameObject root, IList<Transform> eventChain)
        {
            eventChain.Clear();
            if (root == null)
                return;

            var t = root.transform;
            while (t != null)
            {
                eventChain.Add(t);
                t = t.parent;
            }
        }

        public static GameObject ExecuteHierarchy<T>(GameObject root, BaseEventData eventData, Handler<T> callbackFunction) where T : IEventSystemHandler
        {
            GetEventChain(root, s_InternalTransformList);

            for (var i = 0; i < s_InternalTransformList.Count; i++)
            {
                var transform = s_InternalTransformList[i];
                if (Execute(transform.gameObject, eventData, callbackFunction))
                    return transform.gameObject;
            }
            return null;
        }

        public class Handler<T1> where T1 : IEventSystemHandler
        {
            public Handler(ExecuteEvents.EventFunction<T1> h)
            {
                handler = h;
            }

            internal ExecuteEvents.EventFunction<T1> handler;

            class Call
            {
                public int onlyid;
                public IH h;
                public int priority = -1; // 优先级，越高的越先调用
                public bool isCannel = false; // 是否取消监听
            }

            Dictionary<int, Call> Calls = new Dictionary<int, Call>();
            List<Call> Lists = new List<Call>();
            int total = 0;

            List<Call> EndLists = new List<Call>();

            static void Sort(List<Call> list)
            {
                list.Sort((Call x, Call y) =>
                {
                    if (x == null)
                        return 1;

                    if (y == null)
                        return -1;

                    return y.priority.CompareTo(x.priority);
                });
            }

            interface IH
            {
                bool Call(GameObject target, BaseEventData eventData, ref bool isfire);
            }

            class HC : IH
            {
                public HC(Func<GameObject, BaseEventData, bool> func)
                {
                    this.func = func;
                }

                Func<GameObject, BaseEventData, bool> func;

                bool IH.Call(GameObject target, BaseEventData eventData, ref bool isfire)
                {
                    return func(target, eventData);
                }
            }

            class HCF : IH
            {
                public HCF(Func<Data, bool> func)
                {
                    this.func = func;
                }

                Func<Data, bool> func;

                bool IH.Call(GameObject target, BaseEventData eventData, ref bool isfire)
                {
                    Data d = new Data() { go = target, eventData = eventData, isOutValue = isfire };
                    bool v = func(d);
                    isfire = d.isOutValue;
                    return v;
                }
            }

            public class Data
            {
                public GameObject go;
                public BaseEventData eventData;
                public bool isOutValue; // false事件分发至此为止
            }

            // 注意返回值表示是否继续监听此事件
            public int Add(Func<Data, bool> fun, int priority = -1)
            {
                if (fun == null)
                    return 0;

                Call c = new Call();
                c.h = new HCF(fun);
                c.onlyid = ++total;
                c.priority = priority;

                Calls.Add(c.onlyid, c);
                Lists.Add(c);
                Sort(Lists);
                return c.onlyid;
            }

            // 注意返回值表示是否继续监听此事件
            public int Add(Func<GameObject, BaseEventData, bool> fun, int priority = -1)
            {
                if (fun == null)
                    return 0;

                Call c = new Call();
                c.h = new HC(fun);
                c.onlyid = ++total;
                c.priority = priority;

                Calls.Add(c.onlyid, c);
                Lists.Add(c);
                Sort(Lists);
                return c.onlyid;
            }

            // 注意返回值表示是否继续监听此事件
            public int AddEnd(Func<GameObject, BaseEventData, bool> fun, int priority = -1)
            {
                if (fun == null)
                    return 0;

                Call c = new Call();
                c.h = new HC(fun);
                c.onlyid = ++total;
                c.priority = priority;

                Calls.Add(c.onlyid, c);
                EndLists.Add(c);
                Sort(EndLists);
                return c.onlyid;
            }

            public bool Remove(int id)
            {
                Call c = null;
                if (Calls.TryGetValue(id, out c))
                {
                    c.isCannel = true;
                    Calls.Remove(id);
                    return true;
                }

                return false;
            }

            // 是否拦截事件的分发
            public bool Execute(GameObject target, BaseEventData eventData)
            {
                return Execute(Lists, Calls, target, eventData);
            }

            static bool Execute(List<Call> Lists, Dictionary<int, Call> Calls, GameObject target, BaseEventData eventData)
            {
                bool isfire = true; // 事件是否继续分发
                if (Lists.Count != 0)
                {
                    Call c = null;
                    int count = Lists.Count;
                    for (int i = 0; i < count; ++i)
                    {
                        c = Lists[i];
                        if (c == null || c.isCannel)
                            continue;

                        try
                        {
                            if (c.h.Call(target, eventData, ref isfire))
                            {
                                if (c.isCannel)
                                    Lists[i] = null;
                            }
                            else
                            {
                                Calls.Remove(c.onlyid);
                                Lists[i] = null;
                            }

                            if (!isfire)
                                break;
                        }
                        catch (Exception ex)
                        {
                            Debug.LogException(ex);
                        }
                    }

                    Lists.RemoveAll((Call x) => { return (x == null || x.isCannel) ? true : false; });
                }

                return isfire;
            }

            public bool ExecuteEnd(GameObject target, BaseEventData eventData)
            {
                return Execute(EndLists, Calls, target, eventData);
            }
        }

        public static Handler<IBeginDragHandler> beginDragHandler { get { return s_beginDragHandler; } }
        public static Handler<ICancelHandler> cancelHandler { get { return s_cancelHandler; } }
        public static Handler<IDeselectHandler> deselectHandler { get { return s_deselectHandler; } }
        public static Handler<IDragHandler> dragHandler { get { return s_dragHandler; } }
        public static Handler<IDropHandler> dropHandler { get { return s_dropHandler; } }
        public static Handler<IEndDragHandler> endDragHandler { get { return s_endDragHandler; } }
        public static Handler<IInitializePotentialDragHandler> initializePotentialDrag { get { return s_initializePotentialDrag; } }
        public static Handler<IMoveHandler> moveHandler { get { return s_moveHandler; } }
        public static Handler<IPointerClickHandler> pointerClickHandler { get { return s_pointerClickHandler; } }
        public static Handler<IPointerDownHandler> pointerDownHandler { get { return s_pointerDownHandler; } }
        public static Handler<IPointerEnterHandler> pointerEnterHandler { get { return s_pointerEnterHandler; } }
        public static Handler<IPointerExitHandler> pointerExitHandler { get { return s_pointerExitHandler; } }
        public static Handler<IPointerUpHandler> pointerUpHandler { get { return s_pointerUpHandler; } }
        public static Handler<IScrollHandler> scrollHandler { get { return s_scrollHandler; } }
        public static Handler<ISelectHandler> selectHandler { get { return s_selectHandler; } }
        public static Handler<ISubmitHandler> submitHandler { get { return s_submitHandler; } }
        public static Handler<IUpdateSelectedHandler> updateSelectedHandler { get { return s_updateSelectedHandler; } }

        static Handler<IBeginDragHandler> s_beginDragHandler = new Handler<IBeginDragHandler>(ExecuteEvents.beginDragHandler);
        static Handler<ICancelHandler> s_cancelHandler = new Handler<ICancelHandler>(ExecuteEvents.cancelHandler);
        static Handler<IDeselectHandler> s_deselectHandler = new Handler<IDeselectHandler>(ExecuteEvents.deselectHandler);
        static Handler<IDragHandler> s_dragHandler = new Handler<IDragHandler>(ExecuteEvents.dragHandler);
        static Handler<IDropHandler> s_dropHandler = new Handler<IDropHandler>(ExecuteEvents.dropHandler);
        static Handler<IEndDragHandler> s_endDragHandler = new Handler<IEndDragHandler>(ExecuteEvents.endDragHandler);
        static Handler<IInitializePotentialDragHandler> s_initializePotentialDrag = new Handler<IInitializePotentialDragHandler>(ExecuteEvents.initializePotentialDrag);
        static Handler<IMoveHandler> s_moveHandler = new Handler<IMoveHandler>(ExecuteEvents.moveHandler);
        static Handler<IPointerClickHandler> s_pointerClickHandler = new Handler<IPointerClickHandler>(ExecuteEvents.pointerClickHandler);
        static Handler<IPointerDownHandler> s_pointerDownHandler = new Handler<IPointerDownHandler>(ExecuteEvents.pointerDownHandler);
        static Handler<IPointerEnterHandler> s_pointerEnterHandler = new Handler<IPointerEnterHandler>(ExecuteEvents.pointerEnterHandler);
        static Handler<IPointerExitHandler> s_pointerExitHandler = new Handler<IPointerExitHandler>(ExecuteEvents.pointerExitHandler);
        static Handler<IPointerUpHandler> s_pointerUpHandler = new Handler<IPointerUpHandler>(ExecuteEvents.pointerUpHandler);
        static Handler<IScrollHandler> s_scrollHandler= new Handler<IScrollHandler>(ExecuteEvents.scrollHandler);
        static Handler<ISelectHandler> s_selectHandler= new Handler<ISelectHandler>(ExecuteEvents.selectHandler);
        static Handler<ISubmitHandler> s_submitHandler = new Handler<ISubmitHandler>(ExecuteEvents.submitHandler);
        static Handler<IUpdateSelectedHandler> s_updateSelectedHandler = new Handler<IUpdateSelectedHandler>(ExecuteEvents.updateSelectedHandler);
    }
}