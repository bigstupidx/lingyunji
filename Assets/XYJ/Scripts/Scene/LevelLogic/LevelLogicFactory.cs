#if !USE_HOT
namespace xys.hot
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using Config;

    public class LevelLogicFactory
    {
        public static LevelLogicBase Create(LevelType levelType)
        {
            switch (levelType)
            {
                case LevelType.MainCity:
                    return new LevelLogicCity();
                case LevelType.NormalDuplicate:
                    return new LevelLogicDungeon();
                case LevelType.StroyDuplicate:
                    return new LevelLogicDungeon();
                default:
                    return new LevelLogicBase();
            }
        }
    }
}
#endif