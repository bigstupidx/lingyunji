using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    //public class FileFirstLoad : ASyncOperation
    //{
    //    public FileFirstLoad()
    //    {
    //        total = 0;
    //        current = 0;
    //        MagicThread.Instance.StartCoroutine(Begin());
    //    }

    //    public int total { get; protected set; }
    //    public int current { get; protected set; }

    //    IEnumerator Begin()
    //    {
    //        List<AsyncFile> afs = new List<AsyncFile>();
    //        ResourcesPack.EachAllFile((string file) => 
    //        {
    //            if (XTools.Utility.isLzma(file))
    //            {
    //                 加载文件
    //                afs.Add(ResourcesPack.PreLoadFile(file));
    //            }
    //        });

    //        total = afs.Count;
    //        progress = 0.2f;
    //        for (current = 0; current < total; ++current)
    //        {
    //            if (afs[current] == null)
    //                continue;

    //            while (!afs[current].isDone)
    //                yield return 0;

    //            progress = 0.2f + 0.8f * (current + 1) / afs.Count;
    //        }
            
    //        progress = 1.0f;
    //        isDone = true;
    //    }
    //}
}