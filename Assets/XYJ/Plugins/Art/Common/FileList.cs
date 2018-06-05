#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace XTools
{
    public class FileList
    {
        Dictionary<string, List<string>> AllFileList = new Dictionary<string, List<string>>();

        public void Clear()
        {
            AllFileList.Clear();
        }

        public List<string> GetFiles(string path, string prefix = null, System.Func<string, List<string>> fun = null)
        {
            List<string> files;
            if (AllFileList.TryGetValue(path, out files))
                return files;
            
            if (fun == null)
                fun = Utility.GetAllFileList;

            files = fun(path);
            if (!string.IsNullOrEmpty(prefix))
            {
                for (int i = 0; i < files.Count; ++i)
                    files[i] = prefix + files[i];
            }
            AllFileList[path] = files;
            return files;
        }
    }
}
#endif