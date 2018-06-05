using NetProto;
using CommonBase;
using System.Collections.Generic;
using UnityEngine;
using Config;
using xys.battle;

namespace xys
{
    public partial class LocalPlayer : IPlayer, IModuleOwner
    {
        public LocalPlayer() : base(0)
        {
            cdMgr = new CDMgr();
        }

        public int token { get; private set; }
        public long charid { get; private set; }

        // 当前模块列表
        Dictionary<int, IModule> Modules = new Dictionary<int, IModule>();
        public BattleAttri uiShowBattleAttri { get; private set; } //ui显示的战斗属性,已经做了等级修正的

        public CDMgr cdMgr { get; private set; } // CD管理器

        public IModule GetModule(ModuleType type)
        {
            IModule m = null;
            if (Modules.TryGetValue((int)type, out m))
                return m;

            Debug.LogErrorFormat("module:{0} not find!", type);
            return m;
        }

        public IModule GetModuleByID(int id)
        {
            IModule m = null;
            if (Modules.TryGetValue(id, out m))
                return m;

            Debug.LogErrorFormat("module:{0} not find!", id);
            return m;
        }

        public T GetModule<T>() where T : IModule
        {
            foreach (var itor in Modules)
            {
                if (itor.Value == null)
                    continue;

                if (itor.Value.GetType() == typeof(T))
                    return (T)itor.Value;
            }

            return default(T);
        }


        public void BeginLogin(int token)
        {
            this.token = token;
        }

        //开始进入游戏
        public void BeginEnter(CharacterData cd)
        {
            name = cd.name;
            charid = cd.charid;
            InitCfg(RoleJob.GetRoleID(cd.career, cd.sex));
        }

        //开始切换场景,数据已经准备好了
        public void BeginChangeScene(ChangeSceneData data)
        {
            charSceneId = data.charSceneId;
            SetPosition(data.pos.ToVector3());
            SetRotate(data.angle);
            InitBattle();
        }

        public void Start()
        {
            //创建模块
            App.my.moduleMgr.ForEach((IModleFactory factory) =>
            {
                try
                {
                    IModule module = factory.Create(this);
                    Modules.Add(module.id, module);
                }
                catch (System.Exception ex)
                {
                    Debuger.ErrorLog("type:{0} Craete errro!", factory.type);
                    Debuger.LogException(ex);
                }
            });

            uiShowBattleAttri = new BattleAttri();
        }

        public void Reset()
        {
            foreach (var itor in Modules)
            {
                itor.Value.Release();
            }
        }

        public long GetMoney(AttType type)
        {
            return attributes.Get(type).longValue;
        }

        //public PackageMgr packageMgr
        //{
        //    get { return GetModule<PackageModule>().packageMgr; }
        //}


        public IObject GetUIChooseTarget()
        {
            return ((BattleManagerLocal)battle).m_autoChooseTarget.chooseTarget;
        }
        //public long GetMoney(MoneyType type)
        //{
        //    switch (type)
        //    {
        //        case MoneyType.FairyJade: return fairyJadeValue;
        //        case MoneyType.Gold: return goldShellValue;
        //        case MoneyType.Silver: return silverShellValue;
        //        case MoneyType.JasperJade: return jasperJadeValue;
        //        case MoneyType.Organization: return organizationValue;
        //        case MoneyType.Chivalrous: return chivalrousValue;
        //        case MoneyType.Family: return familyValue;
        //        case MoneyType.Energy: return energyValue;
        //    }

        //    throw new System.Exception(string.Format("Error:{0} MoneyType!", type));
        //}
    }
}