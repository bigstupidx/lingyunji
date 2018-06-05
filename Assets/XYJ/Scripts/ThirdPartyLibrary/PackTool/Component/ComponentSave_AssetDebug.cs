using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public partial class ComponentSave
    {
#if ASSET_DEBUG
        public static TimeTrack LoadResources_timetrack = TimeTrackMgr.Instance.Get("ComponentSave.LoadResources");
#endif
    }
}
