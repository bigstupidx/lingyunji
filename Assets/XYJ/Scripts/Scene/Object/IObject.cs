namespace xys
{
    using NetProto;
    using CommonBase;
    using UnityEngine;
    using xys.battle;

    /// <summary>
    /// 客户端对象
    /// </summary>
    public partial interface IObject : IShareObject
    {
        Transform root { get; }

        string name { get; }

        ObjectEventSet eventSet { get; }
        AttributeSet<AttType> attributes { get; }
        //战斗相关
        BattleManagerBase battle { get; }
        void InitDataByAOI(SceneObjectSyncData data);
        void Update();
        // 销毁自身
        void Destroy();
        void EnterScene();
        void ExitScene();
    }  
}
