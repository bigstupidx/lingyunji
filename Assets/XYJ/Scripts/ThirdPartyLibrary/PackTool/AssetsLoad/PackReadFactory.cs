#if USE_RESOURCESEXPORT || USE_ABL
using System.Collections.Generic;

namespace PackTool
{
    // 资源包的读取
    internal class PackReadFactory
    {
        public static IPackRead CreateResUnitePackRead(List<Archive> zips)
        {
            return new ResUnite(zips);
        }

        public static IPackRead CreateExterDirPackRead(string root)
        {
            return new DirectoryPackRead(root);
        }
    }
}
#endif