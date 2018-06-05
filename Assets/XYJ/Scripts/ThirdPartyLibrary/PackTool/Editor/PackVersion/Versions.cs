using System;
using System.Collections;
using System.Collections.Generic;

namespace PackTool.New
{
    public class Versions
    {
        public Versions(long svnoffset)
        {
            svn = svnversion.GetVersion() + svnoffset;
        }

        public long svn { get; protected set; }

        public Version startVersion; // 当前要打包的版本号
        public Version resVersion; // 资源版本，只用在打单独的程序包时

        public bool isSimplify = true; // 是否精简打包

        public List<VersionConfig> configs = new List<VersionConfig>();
    }
}