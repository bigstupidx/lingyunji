#if (UNITY_EDITOR || COM_DEBUG) && USE_RESOURCESEXPORT
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace PackTool
{
    public class ShaderCheckSupport : ASyncOperation
    {
        const string SupportKey = "ShaderCheckSupport";
        const string RecordKey = "ShaderCheckRecord";
        static readonly string RecordFile = ResourcesPath.LocalTempPath + "ShaderCheckSupport.ini";

        public string current { get; protected set; }

        public int total { get; protected set; }

        public int index { get; protected set; }

        public IEnumerator BeginCheck()
        {
            IniFile iniFile = new IniFile();
            iniFile.load(RecordFile);

            isDone = false;
            List<string> shaderList = new List<string>();

            StreamingAssetLoad.EachAllFile(
                (string file) => 
                {
                    if (file.EndsWith(".us"))
                    {
                        shaderList.Add(file);
                    }
                });

            byte[] bytes = new byte[1024*1024*2];
            total = shaderList.Count;
            index = 0;
            for (int i = 0; i < shaderList.Count; ++i)
            {
                index = i;
                string shader = shaderList[i];
                current = shader;
                Debuger.WarningLog("ShaderLoad.Load:" + shader);

                if (iniFile.HasKey(shader))
                    continue;

                iniFile.Set(shader, "不支持");
                iniFile.Save(RecordFile);
                Debuger.WarningLog("检测shader:" + shader);
                int lenght = 0;
                if (!StreamingAssetLoad.GetFileBytes(shader, bytes, out lenght))
                    continue; // 没有这个文件

                AssetBundleCreateRequest acr = AssetBundle.LoadFromMemoryAsync(bytes);
                while (!acr.isDone)
                    yield return 0;

                GameObject shader_obj = acr.assetBundle.mainAsset as GameObject;
                acr.assetBundle.Unload(false);
                Debuger.WarningLog("支持shader:" + shader_obj.name);

                iniFile.Remove(shader);
                iniFile.Save(RecordFile);
                progress = 1.0f * (i + 1) / shaderList.Count;
            }

            isDone = true;
            yield break;
        }
    }
}
#endif