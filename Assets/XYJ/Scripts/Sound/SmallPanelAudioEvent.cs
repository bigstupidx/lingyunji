using UnityEngine;

namespace xys.UI
{
    public class SmallPanelAudioEvent : MonoBehaviour
    {
        [FMODUnity.EventRef]
        public string openSound;

        [FMODUnity.EventRef]
        public string closeSound;

        void Awake()
        {
            SoundMgr.CheckSoundEvent(ref openSound, "ui_open_popup");
            SoundMgr.CheckSoundEvent(ref closeSound, "ui_close_popup");
        }

        void OnEnable()
        {
            if (gameObject.activeSelf)
                SoundMgr.PlaySound(openSound);
        }

        void OnDisable()
        {
            if (!gameObject.activeSelf)
                SoundMgr.PlaySound(closeSound);
        }
    }
}