using FMODUnity;
using System.IO;
using FMOD.Studio;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace xys
{
    public class SoundMgr
    {
        static SoundMgr()
        {
            isInit = false;
        }

        static Bank stringBank = null;

        public static bool isInit { get; private set; }

        // 初始化fmod插件
        static public void Init()
        {
            if (isInit)
                return;

            isInit = true;
            try
            {
                system = RuntimeManager.StudioSystem;
            }
            catch (System.Exception ex)
            {
                system = null;
                Debug.LogException(ex);
                return;
            }

#if USE_RESOURCESEXPORT
        string root = FmodInit.Root;
#else

#if UNITY_EDITOR
            string root = ResourcesPath.dataPath + "/../Data/Sounds/";
#else
        string root = ResourcesPath.streamingAssetsPath + "Data/Sounds/";
#endif

#endif
            root = root + XTools.Utility.FmodPlatformKey;
            string string_bank_file = string.Format("{0}/masterBank.strings.bank", root);
            if (!File.Exists(string_bank_file))
            {
                XYJLogger.LogError("string_bank_file:{0} not find!", string_bank_file);
                return;
            }

            Debuger.DebugLog("root:{0} string_bank_file:{1}", root, string_bank_file);
            List<string> bankFileNames = new List<string>();
            {
                try
                {
                    stringBank = RuntimeManager.LoadBankFullPath(string_bank_file, false);
                }
                catch (System.Exception ex)
                {
                    stringBank = null;
                    XYJLogger.LogException(ex);
                    return;
                }
                if (stringBank != null)
                {
                    int stringCount;
                    stringBank.getStringCount(out stringCount);
                    const string BankPrefix = "bank:/";
                    int BankPrefixLength = BankPrefix.Length;

                    for (int stringIndex = 0; stringIndex < stringCount; stringIndex++)
                    {
                        string currentString;
                        System.Guid currentGuid;
                        stringBank.getStringInfo(stringIndex, out currentGuid, out currentString);
                        if (currentString.StartsWith(BankPrefix))
                        {
                            string bankFileName = currentString.Substring(BankPrefixLength) + ".bank";
                            if (!bankFileName.Contains("strings.bank")) // filter out the strings bank
                                bankFileNames.Add(bankFileName);
                        }
                    }
                }
            }

            foreach (string bankfile in bankFileNames)
            {
                //string bankName = Path.GetFileNameWithoutExtension(bankfile);
                Bank bank = null;
                try
                {
                    bank = RuntimeManager.LoadBankFullPath(string.Format("{0}/{1}", root, bankfile));
                }
                catch (System.Exception ex)
                {
                    XYJLogger.LogException(ex);
                    continue;
                }

                EventDescription[] eventList;
                var result = bank.getEventList(out eventList);
                if (result != FMOD.RESULT.OK)
                {
                    Debug.LogErrorFormat("load:{0} error:{1}!", bankfile, result);
                    continue;
                }

                foreach (var eventDesc in eventList)
                {
                    string path;
                    result = eventDesc.getPath(out path);
                    System.Guid guid;
                    result = eventDesc.getID(out guid);
                    if (result != FMOD.RESULT.OK)
                    {
                        Debug.LogErrorFormat("getPath error:{0} file:{1}!", result, bankfile);
                        continue;
                    }

                    EventList.Add(path, bankfile);
                    EventNameToGuid.Add(Path.GetFileNameWithoutExtension(path).ToLower(), guid);
                }
            }

            listener = new StudioListenerAuto(0);

            xys.UI.EventHandler.pointerClickHandler.Add(OnClickBtnSound, -2);
        }

        static StudioListenerAuto listener = null;

        static public void Update()
        {
            if (listener != null)
                listener.Update();
        }

        static FMOD.Studio.System system;

        // 当前的事件列表
        static Dictionary<string, string> EventList = new Dictionary<string, string>();

        // 事件名对应完成的事件guid
        static Dictionary<string, System.Guid> EventNameToGuid = new Dictionary<string, System.Guid>();

        public static EventInstance PlayOneShot(string eventname, Vector3 position = new Vector3())
        {
            if (system == null)
                return null;

            System.Guid guid;
            if (!EventNameToGuid.TryGetValue(eventname.ToLower(), out guid))
            {
                Debug.LogErrorFormat("event:{0} not find!", eventname);
                return null;
            }

            return RuntimeManager.PlayOneShot(guid, position);
        }

        public static EventInstance PlayOneShotAttached(string eventname, System.Func<bool> isDestory, System.Func<FMOD.ATTRIBUTES_3D> To3DAttributes)
        {
            if (system == null)
                return null;

            System.Guid guid;
            if (!EventNameToGuid.TryGetValue(eventname.ToLower(), out guid))
            {
                Debug.LogErrorFormat("event:{0} not find!", eventname);
                return null;
            }

            return RuntimeManager.PlayOneShotAttached(guid, isDestory, To3DAttributes);
        }

        public static void SetPuase(bool pauseStatus)
        {
            RuntimeManager.SetPause(pauseStatus);
        }

        public static EventInstance PlayOneShotAttached(string eventname, GameObject gameObject)
        {
            if (system == null)
                return null;

            System.Guid guid;
            if (!EventNameToGuid.TryGetValue(eventname.ToLower(), out guid))
            {
                Debug.LogErrorFormat("event:{0} not find!", eventname);
                return null;
            }

            return RuntimeManager.PlayOneShotAttached(guid, gameObject);
        }

        public static void CheckSoundEvent(ref string name, string newname)
        {
            if (string.IsNullOrEmpty(name))
                name = newname;
            else
            {
                int pos = name.LastIndexOf('/');
                if (pos != -1)
                    name = name.Substring(pos + 1);
            }
        }

        public static void PlaySound(string name)
        {
            if (name == "nil")
                return;

            int pos = name.LastIndexOf('/');
            if (pos != -1)
                name = name.Substring(pos + 1);

            PlayOneShot(name);
        }

        static bool OnClickBtnSound(GameObject arg1, BaseEventData arg2)
        {
            if (arg1 == null)
                return true;

            if (arg1.GetComponent<UnityEngine.UI.Selectable>() == null)
                return true;

            string name = "ui_button";
            UI.FmodAduioEvent fae = arg1.GetComponent<UI.FmodAduioEvent>();
            if (fae != null)
                name = fae.name;

            PlaySound(name);
            return true;
        }

#if UNITY_EDITOR
        public static void OnGUI()
        {
            if (GUILayout.Button("Test Play"))
            {
                int rand = Random.Range(0, EventNameToGuid.Count);
                var values = EventNameToGuid.Values;
                var ator = values.GetEnumerator();
                while (rand-- != 0)
                    ator.MoveNext();

                RuntimeManager.PlayOneShot(ator.Current, Vector3.zero);
            }
        }
#endif
    }
}