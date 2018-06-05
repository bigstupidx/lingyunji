using System;
using System.Collections.Generic;


namespace xys.battle
{
    /// <summary>
    /// 场景对象管理
    /// </summary>
    interface IObjectManager
    {
        Dictionary<int, IObject> GetAllObject();
        IObject GetObject(int charSceneId);
    }
}
