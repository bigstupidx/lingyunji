namespace xys
{
    using NetProto;
    using CommonBase;
    using UnityEngine;
    using System.Collections.Generic;

    public abstract class INpc : ObjectBase
    {
        public INpc(ObjectType type, int charSceneid) : base(type, charSceneid)
        {

        }
    }

    public partial class NpcBase : INpc
    {
        public UnityEngine.Vector3 bornRot { get; private set; }
        public bool canClick { get; set; }
        public NpcPatrolData m_patrolData { get; private set; }

        public NpcBase(ObjectType type,int csi) : base(type,csi)
        {

        }

        public override void InitDataByAOI(SceneObjectSyncData data)
        {
            base.InitDataByAOI(data);
            if (data.refreshId!=null)
                SetRefreshId(data.refreshId, App.my.localPlayer.GetModule<LevelModule>().GetSpawnData(data.refreshId));
            bornRot = new UnityEngine.Vector3(0, data.angle, 0);
            canClick = true;
        }

        /// <summary>
        /// hardcode  暂时实现点击npc的功能，用来进入关卡，后面会删除
        /// </summary>
        public void OnClickNpc()
        {
            if(cfgInfo.id == 424)
            {
                App.my.uiSystem.ShowPanel(UI.PanelType.UIConvenientPanel, null);
            }
        }

        /// <summary>
        /// 设置巡逻数据
        /// </summary>
        /// <param name="patrolId"></param>
        public bool SetPatrolData(string patrolId, bool loopType)
        {
            List<PathPointVo> voList = App.my.localPlayer.GetModule<LevelModule>().GetPathData(patrolId);
            if(null != voList && voList.Count > 0)
            {
                m_patrolData = new NpcPatrolData();
                m_patrolData.patrolId = patrolId;
                m_patrolData.loopType = loopType;
                m_patrolData.pathList = voList;
                return true;
            }
            return false;
        }
    }

    public class NpcPatrolData
    {
        public string patrolId;
        public bool loopType;
        public List<PathPointVo> pathList = new List<PathPointVo>();
        public List<PathPointVo> tempList = new List<PathPointVo>();
    }

    public class PathPointVo
    {
        public Vector3 pos;
        public float stayTime;
        public float speed;
        public string eventId;
    }
}
