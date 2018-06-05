namespace XTools
{
    public class TimeCheck
    {
        long time;
        long lastTime;

        public TimeCheck(bool b = false)
        {
            time = 0;
            if (b == true)
                begin();
        }

        public void begin()
        {
            time = System.DateTime.Now.Ticks;
            lastTime = System.DateTime.Now.Ticks;
        }

        public float delay
        {
            get
            {
                lastTime = System.DateTime.Now.Ticks;
                return (lastTime - time) * 0.0000001f;
            }
        }

        public float delayMS
        {
            get
            {
                lastTime = System.DateTime.Now.Ticks;
                return (lastTime - time) * 0.0001f;
            }
        }

        public float delayLast
        {
            get
            {
                float old =lastTime;
                lastTime = System.DateTime.Now.Ticks;
                return (lastTime - old) * 0.0000001f;
            }
        }

        public float delayLastMS
        {
            get
            {
                float old = lastTime;
                lastTime = System.DateTime.Now.Ticks;
                return (lastTime - old) * 0.0001f;
            }
        }

        // 获取上次时间，并重新开始计时
        public float renew
        {
            get
            {
                float t = delay;
                begin();
                return t;
            }
        }
    }
}