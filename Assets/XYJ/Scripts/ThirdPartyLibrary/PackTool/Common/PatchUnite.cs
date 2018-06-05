#if USE_RESOURCESEXPORT
using System;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace PackTool
{
    // 补丁包合并，多个补丁包合并为一个
    public class PatchUnite : ASyncOperation
    {
        // 补丁包
        class Patch
        {
            public Patch(string f, string p)
            {
                file = f;
                pass = p;
            }

            public string file; // 补丁文件
            public string pass; // 密码
            public Archive archive;

            public void Init(Dictionary<string, Stream> streams)
            {
                archive = new Archive(file, 0);
                archive.ReadInfo(
                    (Archive.FileItem afi) => 
                    {
                        streams[afi.name] = afi.GetStream(archive);
                    });
            }
        }

        // 要更新的补丁列表
        List<Patch> mPatchList = new List<Patch>();

        public void AddPatch(string file, string pass)
        {
            mPatchList.Add(new Patch(file, pass));
            total = mPatchList.Count;
        }

        public int total { get; protected set; }

        public int current { get; protected set; }

        void BeginUnite(string sourcefile)
        {
            bool hasExitSource = File.Exists(sourcefile);
            total = mPatchList.Count;

            if (!hasExitSource && total == 1)
            {
                // 原补丁文件不存在，并且只有一个补丁
                File.Move(mPatchList[0].file, sourcefile);
                return;
            }

            current = 1;
            progress = 0.1f;
            Dictionary<string, Stream> saveFileList = new Dictionary<string, Stream>();
            byte[] bytes = new byte[1024 * 1024];
            progress = 0.2f;

            Archive source_archive = null;
            if (File.Exists(sourcefile))
            {
                source_archive = new Archive(sourcefile, 0);
                source_archive.ReadInfo((Archive.FileItem afi) => { saveFileList.Add(afi.name, afi.GetStream(source_archive)); });
            }

            for (int i = 0; i < mPatchList.Count; ++i)
            {
                current = i + 1;
                mPatchList[i].Init(saveFileList);
            }

            progress = 0.5f;

            // 开始更新
            if (source_archive != null)
            {
                Archive.Generate(sourcefile + ".temp", saveFileList, bytes);
                source_archive.Close();
                File.Delete(sourcefile);
                File.Move(sourcefile + ".temp", sourcefile);
            }
            else
            {
                Archive.Generate(sourcefile, saveFileList, bytes);
            }

            bytes = null;

            foreach (Patch patch in mPatchList)
            {
                if (patch.archive != null)
                    patch.archive.Close();
            }
 
            mPatchList.Clear();
            progress = 1.0f;
            isDone = true;
        }

        // 开始安装
        public void Begin(string sourcefile)
        {
            try
            {
                BeginUnite(sourcefile);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                error_message("存储空间不足!");
            }
        }
    }
}
#endif