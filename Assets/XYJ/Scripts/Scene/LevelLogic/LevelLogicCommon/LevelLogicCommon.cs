#if !USE_HOT
namespace xys.hot
{
    /// <summary>
    /// 关卡公用的组件
    /// </summary>
    public class LevelLogicCommon
    {
        //倒计时
        public LevelLogicCountdown m_countdown { get; private set; }


        public LevelLogicCommon()
        {
            m_countdown = new LevelLogicCountdown();
        }
    }
}
#endif