namespace xys
{

    using UnityEngine;
    using FMOD.Studio;

    public class BackgroundMusic
    {
        static EventInstance eventInstance;
        static bool isPause = false; // 当前是否暂停播放

        // 播放背景音
        public static void Play(string name)
        {
            Release();

            if (string.IsNullOrEmpty(name))
                return;

            eventInstance = SoundMgr.PlayOneShotAttached(name, () => { return false; },
                () =>
                {
                    var trans = Camera.main;
                    if (trans != null)
                        return FMODUnity.RuntimeUtils.To3DAttributes(trans.transform);

                    return FMODUnity.RuntimeUtils.To3DAttributes(Vector3.zero);
                });

            if (isPause)
            {
                if (eventInstance != null)
                    eventInstance.setPaused(isPause);
            }
        }

        public static void Release()
        {
            if (eventInstance != null)
            {
                eventInstance.stop(STOP_MODE.IMMEDIATE);
                eventInstance.release();
                eventInstance = null;
            }
        }

        // 暂停背景声播放 
        public static bool pause
        {
            get { return pause; }
            set
            {
                if (isPause == value)
                    return;

                isPause = value;
                if (eventInstance != null)
                    eventInstance.setPaused(value);
            }
        }
    }

}