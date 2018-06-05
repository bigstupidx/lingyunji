using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{ 
    public class CurrentUsedResources : ResourcesFind
    {
        static CurrentUsedResources()
        {
            instance = new CurrentUsedResources();
            instance.FindAllResources();
        }

        static public CurrentUsedResources instance { get; protected set; }
    }
}