#if !USE_HOT
using Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WXB;
using xys.UI;
using xys.UI.State;

namespace xys.hot.UI
{
    class SoulGrids
    {
        public enum State
        {
            State_Enable,
            State_Attached,
            State_Disable,
        }
        Dictionary<int, StateRoot> m_UIData = new Dictionary<int, StateRoot>();
        public NetProto.SoulGrids data { get; private set; }
        public int gridCount { get; private set; }
        public SoulGrids(Transform trans)
        {
            gridCount = trans.childCount;
            for (int i = 0;i < gridCount; i++)
            {
                var grid = trans.GetChild(i).GetComponent<StateRoot>();
                if (grid != null)
                    m_UIData.Add(i, grid);
            }
        }

        public void SetData(NetProto.SoulGrids data)
        {
            this.data = data;
            var itr = data.soulData.GetEnumerator();
            while (itr.MoveNext())
            {
                if (itr.Current.Value.isActive)
                    SetState(itr.Current.Key, itr.Current.Value.soulID > 0 ? State.State_Attached : State.State_Enable);
                else
                    SetState(itr.Current.Key, State.State_Disable);
            }  
        }

        public void SetState(int index, SoulGrids.State state)
        {
            if (m_UIData.ContainsKey(index))
                m_UIData[index].SetCurrentState((int)state, false);
        }

        public SoulGrids.State GetState(int index)
        {
            return (SoulGrids.State)m_UIData[index].CurrentState;
        }
    }
}
#endif
