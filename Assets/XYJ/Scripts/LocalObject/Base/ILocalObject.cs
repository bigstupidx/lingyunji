namespace xys
{
    using UnityEngine;

    /// <summary>
    /// 本地前端角色对象，通过自己模块管理
    /// </summary>
    public interface ILocalObject
    {

        int uid { get; }// 角色唯一id，自动生成

        object initData { get; }// 可以通过该对象传位置，角度朝向，名字等信息

        // 初始化数据
        void InitData(int uid, object data);

        // 销毁自身
        void Destroy();

        // 逻辑更新
        void Update();

    }
}
