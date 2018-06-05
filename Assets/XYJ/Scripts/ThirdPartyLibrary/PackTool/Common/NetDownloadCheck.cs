using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UI;

namespace PackTool
{
    public class NetDownloadCheck
    {
        public bool isDone = false;

        public IEnumerator BeginCheckGUI(GUITextShowList gsl)
        {
            bool isRun = true;
            while (true)
            {
                yield return 0;
                if (isRun)
                {
                    switch (Application.internetReachability)
                    {
                    case NetworkReachability.NotReachable:
                        {
                            isRun = false;
                            gsl.AddButton("当前环境无网络！(重新检测)", (object p) => { isRun = true; }, null, 1);
                        }
                        break;

                    case NetworkReachability.ReachableViaCarrierDataNetwork: // 3G环境
                    case NetworkReachability.ReachableViaLocalAreaNetwork: // wifi环境
                        isDone = true;
                        yield break;
                    }

                    isRun = false;
                }
            }
        }
    }
}