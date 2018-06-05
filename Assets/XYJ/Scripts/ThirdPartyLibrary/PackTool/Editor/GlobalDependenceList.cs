using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace PackTool
{
    public class GlobalDependenceList : DependenceList
    {
        static GlobalDependenceList()
        {
            instance = new GlobalDependenceList();
        }

        static public GlobalDependenceList instance { get; protected set; }
    }
}