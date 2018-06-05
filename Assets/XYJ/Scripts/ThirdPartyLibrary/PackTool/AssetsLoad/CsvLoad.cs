using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace PackTool
{
    public class CsvLoad : CsvCommon.CsvLoadKey
    {
        public CsvLoad(string workPath) : base(workPath)
        {

        }

        public override bool LoadFile(string file, char compart, bool bAdd = false)
        {
            Stream stream = TextLoad.GetStream(workPath + file);
            if (stream == null)
            {
                Debuger.ErrorLog("file:{0} not find!", file);
                return false;
            }

            StreamReader reader = new StreamReader(stream);
            Load(reader, compart, bAdd);
            stream.Close();
            reader.Close();
            return true;
        }
    }
}