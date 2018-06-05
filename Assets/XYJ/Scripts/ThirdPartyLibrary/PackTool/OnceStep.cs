#if COM_DEBUG || UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using XTools;

namespace PackTool
{
    public class OnceStep : ASyncOperation
    {
        public void Reset()
        {
            isDone = false;
        }

        public IEnumerator Next(string text, float totaltime = -1)
        {
            return NextImp(text, totaltime);
        }

        // 下一步
        IEnumerator NextImp(string text, float totaltime = -1)
        {
            isDone = false;
            GUITextShow.TextInfo textinfo = GUITextShow.AddButton(text, (object p) => { isDone = true; }, null);
            TimeCheck tc = new TimeCheck(true);
            while (!isDone)
            {
                yield return 0;
                if (totaltime != -1 && tc.delay >= totaltime)
                    isDone = true;
            }

            textinfo.isCannel = true;
        }
    }
}
#endif