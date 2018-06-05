using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace xys.GameStory
{
    public class StoryEventDataPointsFx : IStoryEventData
    {
        public string fxName = string.Empty;
        public string pointsId;
        public float destroyTime = -1;// 默认时间


        /// <summary>
        /// 把结构转为json字符串
        /// </summary>
        /// <returns></returns>
        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        /// <summary>
        /// 转结构设置数据
        /// </summary>
        /// <param name="cxt"></param>
        public void ParseJson(string cxt)
        {
            if (string.IsNullOrEmpty(cxt))
                return;

            // 一个参数
            StoryEventDataPointsFx data = JsonUtility.FromJson<StoryEventDataPointsFx>(cxt);
            if (data != null)
            {
                fxName = data.fxName;
                pointsId = data.pointsId;
                destroyTime = data.destroyTime;
            }
        }

    }

    public class StoryEventPlayFx : StoryEventBase
    {
        // 点集合
        // 特效名
        // 持续时间：-1表示默认时间
        StoryEventDataPointsFx m_config;

        /// <summary>
        /// 执行事件
        /// </summary>
        public override void OnFire()
        {
            Debug.Log("StoryEvent.Fire : " + type);
            m_config = eventData as StoryEventDataPointsFx;
            if (m_config == null)
                return;

            if (string.IsNullOrEmpty(m_config.fxName))
            {
                return;
            }
            // 获取点位置
            Points points = GetPoints(m_config.pointsId);
            XYJObjectPool.LoadPrefab(m_config.fxName, (GameObject go, object p) =>
            {
                if (go != null)
                {
                    if (points==null || points.positions.Count == 0)
                    {
                        go.SetActive(true);

                        // Set destory time here!
                        if (m_config.destroyTime > 0.0f)
                        {
                            EffectDestroy destroy = go.GetComponent<EffectDestroy>();
                            if (destroy == null)
                                destroy = go.AddComponent<EffectDestroy>();
                            destroy.m_destroyTime = m_config.destroyTime;
                        }
                    }
                    else
                    {
                        go.SetActive(false);
                        GameObject clone = null;
                        foreach (Vector3 point in points.positions)
                        {
                            // 在每个位置点上设置特效
                            clone = GameObject.Instantiate(go);
                            clone.SetActive(true);
                            clone.transform.position = point;

                            // Set destory time here!
                            if (m_config.destroyTime > 0.0f)
                            {
                                EffectDestroy destroy = clone.GetComponent<EffectDestroy>();
                                if (destroy == null)
                                    destroy = clone.AddComponent<EffectDestroy>();
                                destroy.m_destroyTime = m_config.destroyTime;
                            }
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 退出事件
        /// </summary>
        public override void OnExit()
        {
            Debug.Log("StoryEvent.Exit : " + type);
        }

        /// <summary>
        /// 更新事件
        /// </summary>
        /// <param name="timePass"></param>
        public override void Update(float timePass)
        {
            //Debug.Log("StoryEvent.Update : " + type);
        }

        /// <summary>
        /// 暂停事件
        /// </summary>
        public override void OnPause()
        {
            Debug.Log("StoryEvent.OnPause : " + type);
        }

        /// <summary>
        /// 恢复事件
        /// </summary>
        public override void OnResume()
        {
            Debug.Log("StoryEvent.OnResume : " + type);
        }

        /// <summary>
        /// 停止事件
        /// </summary>
        public override void OnStop()
        {
            Debug.Log("StoryEvent.OnStop : " + type);
        }

    }
}
