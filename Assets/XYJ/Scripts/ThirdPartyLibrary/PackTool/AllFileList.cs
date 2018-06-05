using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PackTool
{
//     public class AllFileList
//     {
//         public static void init()
//         {
// #if !SCENE_DEBUG
//             ResourcesGroup.Clear();
//             Csv.CsvReader csv = new Csv.CsvReader();
//             System.IO.Stream stream = ResourcesPack.FindBaseStream((ResourcesPath.ConfigPath + "FilesMD5.csv"));
//             csv.LoadStream(stream, ',');
// 
//             string file;
//             for (int i = 0; i < csv.getYCount(); ++i)
//             {
//                 file = csv.getStr(i, 0);
//                 if (string.IsNullOrEmpty(file))
//                     continue;
// 
//                 //file = file.Replace('\\', '/');
// 
//                 ResourcesGroup.AddFile(file);
//             }
// #endif
//         }
//     }
}