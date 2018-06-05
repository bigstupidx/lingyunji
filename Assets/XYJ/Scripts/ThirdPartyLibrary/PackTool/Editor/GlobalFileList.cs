using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using XTools;

namespace PackTool
{
    public class GlobalFileList : FileList
    {
        static GlobalFileList()
        {
            instance = new GlobalFileList();
        }

        static public GlobalFileList instance { get; protected set; }
    }
}
