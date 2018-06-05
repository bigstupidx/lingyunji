#if USE_RESOURCESEXPORT || USE_ABL
using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace PackTool
{
    // 资源包
    public partial class ResourcesPack
    {
        public ResourcesPack()
        {
            packRead = null;
            config = null;
        }

        IPackRead packRead { get; set; } // 资源包

        public ResourceConfig config { get; protected set; } // 当前资源的版本信息

        public bool isVaild { get { return packRead == null ? false : true; } }

        void Clear()
        {
            if (packRead != null)
                packRead.Release();

            packRead = null;
            config = null;
        }

        void InitExterDirectory(string filepath)
        {
            Clear();

            packRead = PackReadFactory.CreateExterDirPackRead(filepath);
            config = LoadResourceConfig(packRead);
        }

        void InitPatchUnite(List<Archive> patchs)
        {
            Clear();

            packRead = PackReadFactory.CreateResUnitePackRead(patchs);
            config = LoadResourceConfig(packRead);
        }

        void InitEmptyFile()
        {
            Clear();
            packRead = new EmptyPackRead();
            config = new ResourceConfig();
        }
    }
}
#endif