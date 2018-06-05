using System;
using Network;
using NetProto;
using System.Collections.Generic;

namespace xys
{
    public interface IModleFactory
    {
        string type { get; }
        int id { get; }
        IModule Create(IModuleOwner owner);
    }

    class ModuleFactory<T> : IModleFactory where T : IModule, new()
    {
        public ModuleFactory(ModuleType t)
        {
            type = t.ToString();
            id = (int)t;
        }

        public string type { get; private set; }
        public int id { get; private set; }

        public IModule Create(IModuleOwner owner)
        {
            try
            {
                T t = new T();
                t.SetModuleType(type, id);
                t.Awake(owner);
                return t;
            }
            catch (System.Exception ex)
            {
                Debuger.ErrorLog("type:{0} new Error!", type);
                Debuger.LogException(ex);
                return null;
            }
        }
    }

    public class HotModuleFactory : IModleFactory
    {
        public HotModuleFactory(string type, int id, string fullname)
        {
            this.type = type;
            this.id = id;
            this.fullname = fullname;
        }

        public string type { get; private set; }
        public int id { get; private set; }

        public string fullname { get; private set; }

        public IModule Create(IModuleOwner owner)
        {
            try
            {
                var t = new HotModule(fullname);
                ((IModule)(t)).SetModuleType(type, id);
                t.Awake(owner);
                return t;
            }
            catch (System.Exception ex)
            {
                Debuger.ErrorLog("type:{0} new Error!", type);
                Debuger.LogException(ex);
                return null;
            }
        }
    }

    public class ModuleFactoryMgr 
    {
        public ModuleFactoryMgr()
        {
            App.my.handler.Reg<ModuleData>(Protoid.A2C_SyncModuleData, OnModuleSyncData);
            App.my.handler.Reg<ModuleData>(Protoid.W2C_SyncModuleData, W2COnModuleSyncData);
        }

        Dictionary<string, IModleFactory> ModuleFactorys = new Dictionary<string, IModleFactory>();

        // 注册一个玩家身上的模块
        public void Reg<T>(ModuleType type) where T : IModule, new()
        {
            Debuger.DebugLog("RegModule type:{0} id:{1} type:{2}", type, (int)type, typeof(T).FullName);
            ModuleFactorys.Add(type.ToString(), new ModuleFactory<T>(type));
        }

        public void RegHot(string type, int id, string fullname)
        {
            Debuger.DebugLog("RegHot type:{0} id:{1} type:{2}", type, id, fullname);
            ModuleFactorys.Add(type, new HotModuleFactory(type, id, fullname));
        }

        public void ForEach(System.Action<IModleFactory> action)
        {
            foreach (var itor in ModuleFactorys)
            {
                action(itor.Value);
            }
        }

        void W2COnModuleSyncData(Network.IPacket packet, ModuleData md)
        {
            OnModuleSyncData(packet, md);
        }

        // 下发本地玩家的模块数据
        void OnModuleSyncData(IPacket packet, ModuleData md)
        {
            LocalPlayer player = App.my.localPlayer;
            BitStream ms = new BitStream(md.data == null ? new byte[0] : md.data.buffer);
            ms.WritePos = md.data == null ? 0 : md.data.buffer.Length;

            var module = player.GetModule(md.type);
            if (module == null)
            {
                Debuger.ErrorLog("module:{0} not find!", md.type);
                return;
            }
            module.Deserialize(ms);
        }
    }
}