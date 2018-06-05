using FMODUnity;
using System.IO;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace xys.UI
{
    public class FmodAduioEvent : MonoBehaviour
    {
        [FMODUnity.EventRef]
        public string eventName;
    }
}