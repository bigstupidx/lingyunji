#if USE_ABL || USE_RESOURCESEXPORT
using System.IO;
using UnityEngine;

namespace PackTool.New
{
    public partial class AutoExportAll
    {
        public static Versions LoadConfig()
        {
            string patchlist = Application.dataPath + "/../../VersionUpdate.xml";
            if (!File.Exists(patchlist))
            {
                patchlist = Application.dataPath + "/../VersionUpdate.xml";
            }

            FileStream stream = new FileStream(patchlist, FileMode.Open);
            VersionConfigHandler handler = new VersionConfigHandler();
            CommonBase.XMLParser.parseXMLFile(handler, stream);
            stream.Close();

            return handler.Current;
        }
    }
}
#endif