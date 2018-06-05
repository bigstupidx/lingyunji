namespace xys
{
    using NetProto;
    using CommonBase;
    using UnityEngine;
    using System.Collections.Generic;
    public class Pet : NpcBase
    {
        public int m_masterId { get; private set; }
        public Pet(int csi)
            : base(ObjectType.Pet,csi)
        {

        }

        public override void InitDataByAOI(SceneObjectSyncData data)
        {
            base.InitDataByAOI(data);
            m_masterId = data.materid;
        }
    }
}
