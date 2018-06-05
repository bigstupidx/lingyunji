using NetProto;
using CommonBase;
using System.Collections.Generic;
using xys.battle;
using UnityEngine;
using Config;

namespace xys
{
    //class ObjectAttributeFactory : IAttributeFactory<AttType>
    //{
    //    public IAttribute<AttType> Create(AttType attid, ValueType vt)
    //    {
    //        return Help.Create(attid, vt, null);
    //    }
    //}

    abstract public partial class ObjectBase : IObject, IAttributeFactory<AttType>
    {
        public string name { get; protected set; }
        //坐标
        public Vector3 position { get; protected set; }
        //角度
        public float rotateAngle { get; protected set; }


        // 属性集(外部一般不直接访问)
        public AttributeSet<AttType> attributes { get; private set; }
        public ObjectEventSet eventSet { get; protected set; }

        //上次ui选中的时间
        public float m_lastUIChooseTime;

        IAttribute<AttType> IAttributeFactory<AttType>.Create(AttType attid, ValueType vt)
        {
            return Help.Create(attid, vt, null);
        }

        public ObjectBase(ObjectType type, int charSceneid)
        {
            this.type = type;
            charSceneId = charSceneid;
            attributes = new AttributeSet<AttType>(this);
            eventSet = new ObjectEventSet(this);
        }

        //public bool isAlive { get { return m_isAlive && battle.actor!=null && battle.actor.m_rootTrans != null; } }

        public bool isAlive { get { return m_isAlive; } }


        public Transform root { get { return battle==null?null:battle.m_root; } }

        bool m_isAlive;

        protected List<AttConfig> syncList
        {
            get
            {
                switch (type)
                {
                case ObjectType.Player: return AttConfig.remoteList;
                case ObjectType.Npc:
                case ObjectType.Pet:
                    return AttConfig.npcList;
                }

                return null;
            }
        }

        public virtual void InitDataByAOI(SceneObjectSyncData data )
        {
            position = data.pos.ToVector3();
            rotateAngle = data.angle;
            this.charSceneId = data.charSceneId;
            int cfgid = data.sid;
            var output = wProtobuf.MessageStream.ReaderCreate(data.atts.buffer);
            foreach (var config in syncList)
            {
                var att = attributes.factory.Create(config.id, config.type);
                att.MergeFrom(output);
                attributes.Set(config.id, att);
            }
            if (!string.IsNullOrEmpty(data.name))
                name = data.name;
            InitCfg(cfgid);
            if (data.battleCamp != (int)BattleCamp.SameAsCfg)
                battleCamp = (BattleCamp)data.battleCamp;
            InitBattle();
        }

        protected void InitBattle()
        {
            //只初始化一次
            if(battle==null)
            {
                battle = ManagerCreate.CreateBattleManager(this);
                battle.Awake();
                m_isAlive = true;
            }
        }


        public void SetPosition(Vector3 pos)
        {
            if(pos.x>-1000 && pos.x<1000)
            {

            }
            else
            {
                int i = 0;
                i++;
            }
            this.position = pos;
            if (root != null)
                root.transform.position = pos;
        }

        public void SetPositionExceptRoot(Vector3 pos)
        {
            this.position = pos;
        }

        public void SetRotate(float angle)
        {
            this.rotateAngle = angle;
            if(root!=null)
            root.transform.localEulerAngles = new Vector3(0,angle,0);
        }

        public void SetDead()
        {
            if (!m_isAlive)
                return;
            m_isAlive = false;
            battle.m_stateMgr.ChangeState(StateType.Dead);
        }

        //进入场景
        public void EnterScene()
        {
            battle.EnterScene();
        }

        public void ExitScene()
        {
            battle.ExitScene();
        }

        public void Update()
        {
            battle.Update();
        }
         

        public virtual void Destroy()
        {
            ExitScene();
            battle.Destroy();
            m_isAlive = false;
            battle = null;
            eventSet.RemoveAllEvents();
        }

        //GM重新加载
        public void GM_Reload()
        {
            cfgInfo = RoleDefine.Get(cfgInfo.id);
            battle.m_skillMgr.OnExitScene();
            battle.m_skillMgr.OnDestroy();
            battle.m_skillMgr.OnAwake(this);
            battle.m_skillMgr.OnStart();
            battle.m_skillMgr.OnEnterScene();

        }
    }
}
