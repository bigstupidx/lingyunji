using Config;
using UnityEngine;
using UnityEngine.UI;
using xys.UI.State;

namespace xys.UI
{
    namespace ItemObtainShow
    {
        public class WaveText
        {
            public string text;
            public int textState;

            public WaveText(string content, int colorState)
            {
                text = content;
                textState = colorState;
            }
        }

        public class UIAwardTipsText : MonoBehaviour
        {
            [SerializeField]
            Text desc;
            [SerializeField]
            StateRoot stateRoot;

            public void OnShow(WaveText waveText)
            {
                desc.text = waveText.text;
                stateRoot.CurrentState = waveText.textState;
            }
        }
    }
}